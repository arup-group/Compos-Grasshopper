using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using System.Linq;
using Grasshopper.Kernel.Parameters;
using ComposAPI;
using static ComposAPI.CodeOptionsEN;

namespace ComposGH.Components
{
  public class CreateDesignCode : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f89b420e-a35e-4197-9c64-87504fe02b59");
    public CreateDesignCode()
      : base("Design Code", "DC", "Create Compos Design Code",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateDesignCode;
    #endregion

    #region Custom UI
    // This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropdownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Code",
      "National Annex",
      "Cement Type",
      "Settings"
    });
    List<string> DesignCodePretty = new List<string>(new string[]
    {
      "BS5950-3.1:1990 (superseded)",
      "BS5950-3.1:1990+A1:2010",
      "EN1994-1-1:2004",
      "HKSUOS:2005",
      "HKSUOS:2011",
      "AS/NZS2327:2017"
    });
    List<bool> Checkboxes = new List<bool>();
    List<string> CheckboxNames = new List<string>(new string[]
    {
      "Beam propped during construction",
      "Include steel beam weight",
      "Include thin-flange sections",
      "Include concrete slab weight",
      "Consider shear deflection",
      "Consider shrinkage deflection",
      "Ignore shrinkage def. if L/d < 20",
      "Use approx. modular ratios"
    });

    private bool First = true;
    private Code Code = Code.EN1994_1_1_2004;
    private NationalAnnex NA = NationalAnnex.Generic;

    private DesignOption DesignOptions = new DesignOption();
    private CodeOptionsASNZ CodeOptions = new CodeOptionsASNZ();
    private CodeOptionsEN EC4CodeOptions = new CodeOptionsEN();

    public override void CreateAttributes()
    {
      if (this.First)
      {
        this.DropdownItems = new List<List<string>>();
        this.SelectedItems = new List<string>();

        // code
        this.DropdownItems.Add(this.DesignCodePretty);
        this.SelectedItems.Add(this.DesignCodePretty[2]); //EC4 default

        // national annex
        this.DropdownItems.Add(Enum.GetValues(typeof(NationalAnnex)).Cast<NationalAnnex>()
            .Select(x => x.ToString().Replace("_", " ")).ToList());
        this.SelectedItems.Add(this.DropdownItems[1][0]); // Generic default

        // cement type
        this.DropdownItems.Add(Enum.GetValues(typeof(CementClass)).Cast<CementClass>()
            .Select(x => "Cement class " + x.ToString()).ToList());
        this.SelectedItems.Add("Cement class " + this.EC4CodeOptions.CementType.ToString()); // Class N default

        this.Checkboxes = new List<bool>();
        this.Checkboxes.Add(this.DesignOptions.ProppedDuringConstruction);
        this.Checkboxes.Add(this.DesignOptions.InclSteelBeamWeight);
        this.Checkboxes.Add(this.DesignOptions.InclThinFlangeSections);
        this.Checkboxes.Add(this.DesignOptions.InclConcreteSlabWeight);
        this.Checkboxes.Add(this.DesignOptions.ConsiderShearDeflection);
        this.Checkboxes.Add(this.EC4CodeOptions.ConsiderShrinkageDeflection);
        this.Checkboxes.Add(this.EC4CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
        this.Checkboxes.Add(this.EC4CodeOptions.ApproxModularRatios);

        First = false;
      }
      m_attributes = new UI.MultiDropDownCheckBoxesComponentUI(this, SetSelected, this.DropdownItems, this.SelectedItems, CheckBoxToggles, this.Checkboxes, this.CheckboxNames, this.SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropdownItems[i][j];

      if (i == 0)
      {
        for (int k = 0; k < this.DesignCodePretty.Count; k++)
        {
          if (this.SelectedItems[i] == this.DesignCodePretty[k])
          {
            if (this.Code == (Code)k)
              return;

            this.Code = (Code)k;
          }
        }
        switch (this.Code)
        {
          case Code.BS5950_3_1_1990_Superseded:
          case Code.BS5950_3_1_1990_A1_2010:
          case Code.HKSUOS_2005:
          case Code.HKSUOS_2011:
            // change dropdown content
            while (DropdownItems.Count > 1)
              DropdownItems.RemoveAt(1);
            while (SelectedItems.Count > 1)
              SelectedItems.RemoveAt(1);
            while (Checkboxes.Count > 5)
              Checkboxes.RemoveAt(5);
            while (CheckboxNames.Count > 5)
              CheckboxNames.RemoveAt(5);

            break;

          case Code.EN1994_1_1_2004:
            // change dropdown content
            while (this.DropdownItems.Count > 1)
              this.DropdownItems.RemoveAt(1);
            while (SelectedItems.Count > 1)
              this.SelectedItems.RemoveAt(1);
            // national annex
            this.DropdownItems.Add(Enum.GetValues(typeof(NationalAnnex)).Cast<NationalAnnex>()
                .Select(x => x.ToString().Replace("_", " ")).ToList());
            this.SelectedItems.Add(NA.ToString().Replace("_", " "));
            // cement type
            this.DropdownItems.Add(Enum.GetValues(typeof(CementClass)).Cast<CementClass>()
                .Select(x => "Cement class " + x.ToString()).ToList());
            this.SelectedItems.Add("Cement class " + this.EC4CodeOptions.CementType.ToString());

            while (this.Checkboxes.Count > 5)
              this.Checkboxes.RemoveAt(5);
            while (this.CheckboxNames.Count > 5)
              this.CheckboxNames.RemoveAt(5);
            this.Checkboxes.Add(this.EC4CodeOptions.ConsiderShrinkageDeflection);
            this.Checkboxes.Add(this.EC4CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
            this.Checkboxes.Add(this.EC4CodeOptions.ApproxModularRatios);
            this.CheckboxNames.Add("Consider shrinkage deflection");
            this.CheckboxNames.Add("Ignore shrinkage def. if L/d < 20");
            this.CheckboxNames.Add("Use approx. modular ratios");

            break;

          case Code.AS_NZS2327_2017:
            // change dropdown content
            while (this.DropdownItems.Count > 1)
              this.DropdownItems.RemoveAt(1);
            while (this.SelectedItems.Count > 1)
              this.SelectedItems.RemoveAt(1);
            while (this.Checkboxes.Count > 5)
              this.Checkboxes.RemoveAt(5);
            while (this.CheckboxNames.Count > 5)
              this.CheckboxNames.RemoveAt(5);

            // removed due to Compos ignoring that setting
            //this.Checkboxes.Add(this.CodeOptions.ConsiderShrinkageDeflection);
            //this.CheckboxNames.Add("Consider shrinkage deflection");

            break;

          default:
            break;
        }
      }
      if (i == 1)
        this.NA = (NationalAnnex)Enum.Parse(typeof(NationalAnnex), this.SelectedItems[i].Replace(" ", "_"));
      if (i == 2)
        this.EC4CodeOptions.CementType = (CementClass)Enum.Parse(typeof(CementClass), this.SelectedItems[i].Last().ToString());

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      this.Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void CheckBoxToggles(List<bool> newcheckboxes)
    {
      for (int i = 0; i < this.Checkboxes.Count; i++)
        this.Checkboxes[i] = newcheckboxes[i];
      this.DesignOptions.ProppedDuringConstruction = this.Checkboxes[0];
      this.DesignOptions.InclSteelBeamWeight = this.Checkboxes[1];
      this.DesignOptions.InclThinFlangeSections = this.Checkboxes[2];
      this.DesignOptions.InclConcreteSlabWeight = this.Checkboxes[3];
      this.DesignOptions.ConsiderShearDeflection = this.Checkboxes[4];
      // removed due to Compos ignoring that setting
      //if (this.Checkboxes.Count == 6)
      //  this.CodeOptions.ConsiderShrinkageDeflection = this.Checkboxes[5];
      if (this.Checkboxes.Count == 8)
      {
        this.EC4CodeOptions.ConsiderShrinkageDeflection = this.Checkboxes[5];
        this.EC4CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios = this.Checkboxes[6];
        this.EC4CodeOptions.ApproxModularRatios = this.Checkboxes[7];
      }
    }

    private void UpdateUIFromSelectedItems()
    {
      for (int i = 0; i < this.DesignCodePretty.Count; i++)
      {
        if (this.SelectedItems[0] == this.DesignCodePretty[i])
          this.Code = (Code)i;
      }
      if (this.Code == Code.EN1994_1_1_2004)
      {
        this.NA = (NationalAnnex)Enum.Parse(typeof(NationalAnnex), this.SelectedItems[1].Replace(" ", "_"));
        this.EC4CodeOptions.CementType = (CementClass)Enum.Parse(typeof(CementClass), this.SelectedItems[2].Last().ToString());
      }

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      this.Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("EC4 Safety Factors", "SF", "(Optional) EC4 Safety Factors", GH_ParamAccess.item);
      pManager.AddGenericParameter("Creep&Shrinkage Shrinkage", "csp", "(Optional) Creep and Shrinkage parameters for Shrinkage situation. If no input default code values will be used", GH_ParamAccess.item);
      pManager.AddGenericParameter("Creep&Shrinkage Long Term", "CSP", "(Optional) Creep and Shrinkage parameters Long term situation. If no input default code values will be used", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Design Code", "Co", "Compos Design Code", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      SafetyFactorsGoo safetyFactorsGoo = null;
      ISafetyFactors safetyFactors = null;
      SafetyFactorsENGoo ec4safetyFactorsGoo = null;
      ISafetyFactorsEN ec4safetyFactors = null;
      if (this.Code == Code.EN1994_1_1_2004)
      {
        ec4safetyFactorsGoo = (SafetyFactorsENGoo)GetInput.GenericGoo<SafetyFactorsENGoo>(this, DA, 0);
        ec4safetyFactors = (ec4safetyFactorsGoo == null) ? null : ec4safetyFactorsGoo.Value;
      }
      else
      {
        safetyFactorsGoo = (SafetyFactorsGoo)GetInput.GenericGoo<SafetyFactorsGoo>(this, DA, 0);
        safetyFactors = (safetyFactorsGoo == null) ? null : safetyFactorsGoo.Value;
      }

      switch (this.Code)
      {
        case Code.BS5950_3_1_1990_Superseded:
        case Code.BS5950_3_1_1990_A1_2010:
        case Code.HKSUOS_2005:
        case Code.HKSUOS_2011:
          DesignCode otherCodes = new DesignCode();
          otherCodes.Code = this.Code;
          otherCodes.DesignOption = this.DesignOptions;

          if (safetyFactors != null)
            otherCodes.SafetyFactors = safetyFactors;

          DA.SetData(0, new DesignCodeGoo(otherCodes));
          break;

        case Code.EN1994_1_1_2004:
          EN1994 ec4 = new EN1994();
          ec4.NationalAnnex = this.NA;
          ec4.DesignOption = this.DesignOptions;
          ec4.CodeOptions = this.EC4CodeOptions;

          CreepShrinkageParametersEN shrink = (CreepShrinkageParametersEN)GetInput.GenericGoo<CreepShrinkageEuroCodeParametersGoo>(this, DA, 1);
          if (shrink != null)
            ec4.CodeOptions.ShortTerm = shrink;
          CreepShrinkageParametersEN longt = (CreepShrinkageParametersEN)GetInput.GenericGoo<CreepShrinkageEuroCodeParametersGoo>(this, DA, 2);
          if (longt != null)
            ec4.CodeOptions.LongTerm = longt;

          if (ec4safetyFactors != null)
            ec4.SafetyFactors = ec4safetyFactors;

          DA.SetData(0, new DesignCodeGoo(ec4));
          break;

        case Code.AS_NZS2327_2017:
          ASNZS2327 asnz = new ASNZS2327();
          asnz.DesignOption = this.DesignOptions;
          asnz.CodeOptions = this.CodeOptions;
          double shrinkageparam = 0;
          if (DA.GetData(1, ref shrinkageparam))
            asnz.CodeOptions.ShortTerm.CreepCoefficient = shrinkageparam;
          double longtermparam = 0;
          if (DA.GetData(2, ref longtermparam))
            asnz.CodeOptions.LongTerm.CreepCoefficient = longtermparam;

          if (safetyFactors != null)
            asnz.SafetyFactors = safetyFactors;

          DA.SetData(0, new DesignCodeGoo(asnz));
          break;

        default:
          break;
      }
    }

    #region update input params
    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");
      switch (this.Code)
      {
        case Code.BS5950_3_1_1990_Superseded:
        case Code.BS5950_3_1_1990_A1_2010:
        case Code.HKSUOS_2005:
        case Code.HKSUOS_2011:
          // remove input parameters 
          while (this.Params.Input.Count > 1)
            this.Params.UnregisterInputParameter(Params.Input[1], true);
          break;

        case Code.EN1994_1_1_2004:
          // remove input parameters
          while (this.Params.Input.Count > 1)
            this.Params.UnregisterInputParameter(Params.Input[1], true);

          // add input parameters of generic type
          this.Params.RegisterInputParam(new Param_GenericObject());
          this.Params.RegisterInputParam(new Param_GenericObject());
          break;

        case Code.AS_NZS2327_2017:
          //remove input parameters
          while (this.Params.Input.Count > 1)
            this.Params.UnregisterInputParameter(Params.Input[1], true);

          //add input parameters of number type
          this.Params.RegisterInputParam(new Param_Number());
          this.Params.RegisterInputParam(new Param_Number());
          break;

        default:
          break;
      }
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, this.DropdownItems, this.SelectedItems, this.SpacerDescriptions);

      // checkbox bool list
      writer.SetInt32("checkboxCount", this.Checkboxes.Count);
      for (int i = 0; i < this.Checkboxes.Count; i++)
        writer.SetBoolean("checkbox" + i, this.Checkboxes[i]);

      // checkbox names
      writer.SetInt32("checkboxnamesCount", this.CheckboxNames.Count);
      for (int i = 0; i < this.CheckboxNames.Count; i++)
        writer.SetString("checkboxname" + i, this.CheckboxNames[i]);

      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref this.DropdownItems, ref this.SelectedItems, ref this.SpacerDescriptions);

      // bool list
      int checkboxCount = reader.GetInt32("checkboxCount");
      this.Checkboxes = new List<bool>();
      for (int i = 0; i < checkboxCount; i++)
        this.Checkboxes.Add(reader.GetBoolean("checkbox" + i));

      // checkbox names
      int namesCount = reader.GetInt32("checkboxnamesCount");
      this.CheckboxNames = new List<string>();
      for (int i = 0; i < namesCount; i++)
        this.CheckboxNames.Add(reader.GetString("checkboxname" + i));

      UpdateUIFromSelectedItems();

      this.First = false;

      return base.Read(reader);
    }
    #endregion

    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }

    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      switch (this.Code)
      {
        case Code.EN1994_1_1_2004:
          Params.Input[1].Name = "Creep&Shrinkage Shrinkage";
          Params.Input[1].NickName = "csp";
          Params.Input[1].Description = "(Optional) Creep and Shrinkage parameters for Shrinkage situation. If no input default code values will be used";
          Params.Input[1].Optional = true;
          Params.Input[2].Name = "Creep&Shrinkage Long Term";
          Params.Input[2].NickName = "CSP";
          Params.Input[2].Description = "(Optional) Creep and Shrinkage parameters for Long Term situation. If no input default code values will be used";
          Params.Input[2].Optional = true;
          break;

        case Code.AS_NZS2327_2017:
          Params.Input[1].Name = "Creep coefficient Shrinkage";
          Params.Input[1].NickName = "crp";
          Params.Input[1].Description = "(Optional) Creep coefficient for Shrinkage situation. If no input default code values will be used";
          Params.Input[1].Optional = true;
          Params.Input[2].Name = "Creep coefficient Long Term";
          Params.Input[2].NickName = "CRP";
          Params.Input[2].Description = "(Optional) Creep coefficient for Long Term situation. If no input default code values will be used";
          Params.Input[2].Optional = true;
          break;

        default:
          break;
      }
    }
    #endregion
  }
}
