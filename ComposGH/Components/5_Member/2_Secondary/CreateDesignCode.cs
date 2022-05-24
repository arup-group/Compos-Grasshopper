using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using System.Linq;
using Grasshopper.Kernel.Parameters;
using ComposAPI;
using static ComposAPI.EC4Options;

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
    List<string> DesigncodesPretty = new List<string>(new string[]
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
      "Include concrete slabe weight",
      "Consider shear deflection",
      "Consider shrikage declection",
      "Ignore shrinkage def. if L/d < 20",
      "Use approx. modular ratios"
    });

    private bool First = true;
    private Code Code = Code.EN1994_1_1_2004;
    private NationalAnnex NA = NationalAnnex.Generic;
    private CementClass CementClass = CementClass.N;

    private DesignOption DesignOptions = new DesignOption();
    private CodeOptions CodeOptions = new CodeOptions();
    private EC4Options EC4codeOptions = new EC4Options();


    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // code
        DropdownItems.Add(DesigncodesPretty);
        SelectedItems.Add(DesigncodesPretty[2]); //EC4 default

        // national annex
        DropdownItems.Add(Enum.GetValues(typeof(NationalAnnex)).Cast<NationalAnnex>()
            .Select(x => x.ToString().Replace("_", " ")).ToList());
        SelectedItems.Add(DropdownItems[1][0]); // Generic default

        // cement type
        DropdownItems.Add(Enum.GetValues(typeof(CementClass)).Cast<CementClass>()
            .Select(x => "Cement class " + x.ToString()).ToList());
        SelectedItems.Add("Cement class " + EC4codeOptions.CementType.ToString()); // Class N default

        Checkboxes = new List<bool>();
        Checkboxes.Add(DesignOptions.ProppedDuringConstruction);
        Checkboxes.Add(DesignOptions.InclSteelBeamWeight);
        Checkboxes.Add(DesignOptions.InclThinFlangeSections);
        Checkboxes.Add(DesignOptions.InclConcreteSlabWeight);
        Checkboxes.Add(DesignOptions.ConsiderShearDeflection);
        Checkboxes.Add(EC4codeOptions.ConsiderShrinkageDeflection);
        Checkboxes.Add(EC4codeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
        Checkboxes.Add(EC4codeOptions.ApproxModularRatios);

        First = false;
      }
      m_attributes = new UI.MultiDropDownCheckBoxesComponentUI(this, SetSelected, DropdownItems, SelectedItems, CheckBoxToggles, Checkboxes, CheckboxNames, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

      if (i == 0)
      {
        for (int k = 0; k < DesigncodesPretty.Count; k++)
        {
          if (SelectedItems[i] == DesigncodesPretty[k])
          {
            if (Code == (Code)k)
              return;

            Code = (Code)k;
          }
        }
        switch (Code)
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
            while (DropdownItems.Count > 1)
              DropdownItems.RemoveAt(1);
            while (SelectedItems.Count > 1)
              SelectedItems.RemoveAt(1);
            // national annex
            DropdownItems.Add(Enum.GetValues(typeof(NationalAnnex)).Cast<NationalAnnex>()
                .Select(x => x.ToString().Replace("_", " ")).ToList());
            SelectedItems.Add(NA.ToString().Replace("_", " "));
            // cement type
            DropdownItems.Add(Enum.GetValues(typeof(CementClass)).Cast<CementClass>()
                .Select(x => "Cement class " + x.ToString()).ToList());
            SelectedItems.Add("Cement class " + CementClass.ToString());

            while (Checkboxes.Count > 5)
              Checkboxes.RemoveAt(5);
            while (CheckboxNames.Count > 5)
              CheckboxNames.RemoveAt(5);
            Checkboxes.Add(EC4codeOptions.ConsiderShrinkageDeflection);
            Checkboxes.Add(EC4codeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
            Checkboxes.Add(EC4codeOptions.ApproxModularRatios);
            CheckboxNames.Add("Consider shrikage declection");
            CheckboxNames.Add("Ignore shrinkage def. if L/d < 20");
            CheckboxNames.Add("Use approx. modular ratios");

            break;

          case Code.AS_NZS2327_2017:
            // change dropdown content
            while (DropdownItems.Count > 1)
              DropdownItems.RemoveAt(1);
            while (SelectedItems.Count > 1)
              SelectedItems.RemoveAt(1);
            while (Checkboxes.Count > 5)
              Checkboxes.RemoveAt(5);
            while (CheckboxNames.Count > 5)
              CheckboxNames.RemoveAt(5);

            Checkboxes.Add(CodeOptions.ConsiderShrinkageDeflection);
            CheckboxNames.Add("Consider shrikage declection");

            break;

          default:
            break;
        }
      }
      if (i == 1)
        NA = (NationalAnnex)Enum.Parse(typeof(NationalAnnex), SelectedItems[i].Replace(" ", "_"));
      if (i == 2)
        CementClass = (CementClass)Enum.Parse(typeof(CementClass), SelectedItems[i].Last().ToString());


      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void CheckBoxToggles(List<bool> newcheckboxes)
    {
      for (int i = 0; i < Checkboxes.Count; i++)
        Checkboxes[i] = newcheckboxes[i];
    }

    private void UpdateUIFromSelectedItems()
    {
      for (int i = 0; i < DesigncodesPretty.Count; i++)
      {
        if (SelectedItems[0] == DesigncodesPretty[i])
          Code = (Code)i;
      }
      if (Code == Code.EN1994_1_1_2004)
      {
        NA = (NationalAnnex)Enum.Parse(typeof(NationalAnnex), SelectedItems[1].Replace(" ", "_"));
        CementClass = (CementClass)Enum.Parse(typeof(CementClass), SelectedItems[2].Last().ToString());
      }

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
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
      pManager.AddGenericParameter("Design Code", "DC", "Compos Design Code", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      SafetyFactorsGoo safetyFactorsGoo = null;
      ISafetyFactors safetyFactors = null;
      EC4SafetyFactorsGoo ec4safetyFactorsGoo = null;
      IEC4SafetyFactors ec4safetyFactors = null;
      if (Code == Code.EN1994_1_1_2004)
      {
        ec4safetyFactorsGoo = (EC4SafetyFactorsGoo)GetInput.GenericGoo<EC4SafetyFactorsGoo>(this, DA, 0);
        ec4safetyFactors = (ec4safetyFactorsGoo == null) ? null : ec4safetyFactorsGoo.Value;
      }
      else
      {
        safetyFactorsGoo = (SafetyFactorsGoo)GetInput.GenericGoo<SafetyFactorsGoo>(this, DA, 0);
        safetyFactors = (safetyFactorsGoo == null) ? null : safetyFactorsGoo.Value;
      }

      switch (Code)
      {
        case Code.BS5950_3_1_1990_Superseded:
        case Code.BS5950_3_1_1990_A1_2010:
        case Code.HKSUOS_2005:
        case Code.HKSUOS_2011:
          DesignCode otherCodes = new DesignCode();
          otherCodes.Code = Code;
          otherCodes.DesignOption = DesignOptions;

          if (safetyFactors != null)
            otherCodes.SafetyFactors = safetyFactors;

          DA.SetData(0, new DesignCodeGoo(otherCodes));
          break;

        case Code.EN1994_1_1_2004:
          EN1994 ec4 = new EN1994();
          ec4.NationalAnnex = NA;
          ec4.DesignOption = DesignOptions;
          ec4.CodeOptions = EC4codeOptions;

          CreepShrinkageEuroCodeParameters shrink = (CreepShrinkageEuroCodeParameters)GetInput.GenericGoo<CreepShrinkageEuroCodeParametersGoo>(this, DA, 1);
          if (shrink != null)
            ec4.CodeOptions.ShortTerm = shrink;
          CreepShrinkageEuroCodeParameters longt = (CreepShrinkageEuroCodeParameters)GetInput.GenericGoo<CreepShrinkageEuroCodeParametersGoo>(this, DA, 2);
          if (longt != null)
            ec4.CodeOptions.LongTerm = longt;

          if (ec4safetyFactors != null)
            ec4.SafetyFactors = ec4safetyFactors;

          DA.SetData(0, new DesignCodeGoo(ec4));
          break;

        case Code.AS_NZS2327_2017:
          ASNZS2327 asnz = new ASNZS2327();
          asnz.DesignOption = DesignOptions;
          asnz.CodeOptions = CodeOptions;
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
      switch (Code)
      {
        case Code.BS5950_3_1_1990_Superseded:
        case Code.BS5950_3_1_1990_A1_2010:
        case Code.HKSUOS_2005:
        case Code.HKSUOS_2011:
          //remove input parameters 
          while (Params.Input.Count > 1)
            Params.UnregisterInputParameter(Params.Input[1], true);
          break;

        case Code.EN1994_1_1_2004:
          //remove input parameters
          while (Params.Input.Count > 1)
            Params.UnregisterInputParameter(Params.Input[1], true);

          //add input parameters of generic type
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_GenericObject());
          break;

        case Code.AS_NZS2327_2017:
          //remove input parameters
          while (Params.Input.Count > 1)
            Params.UnregisterInputParameter(Params.Input[1], true);

          //add input parameters of number type
          Params.RegisterInputParam(new Param_Number());
          Params.RegisterInputParam(new Param_Number());
          break;

        default:
          break;
      }
    }
    #endregion
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropdownItems, SelectedItems, SpacerDescriptions);

      // checkbox bool list
      writer.SetInt32("checkboxCount", Checkboxes.Count);
      for (int i = 0; i < Checkboxes.Count; i++)
        writer.SetBoolean("checkbox" + i, Checkboxes[i]);

      // checkbox names
      writer.SetInt32("checkboxnamesCount", CheckboxNames.Count);
      for (int i = 0; i < CheckboxNames.Count; i++)
        writer.SetString("checkboxname" + i, CheckboxNames[i]);

      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropdownItems, ref SelectedItems, ref SpacerDescriptions);

      // bool list
      int checkboxCount = reader.GetInt32("checkboxCount");
      Checkboxes = new List<bool>();
      for (int i = 0; i < checkboxCount; i++)
        Checkboxes.Add(reader.GetBoolean("checkbox" + i));

      // checkbox names
      int namesCount = reader.GetInt32("checkboxnamesCount");
      CheckboxNames = new List<string>();
      for (int i = 0; i < namesCount; i++)
        CheckboxNames.Add(reader.GetString("checkboxname" + i));

      UpdateUIFromSelectedItems();

      First = false;

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
      switch (Code)
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
