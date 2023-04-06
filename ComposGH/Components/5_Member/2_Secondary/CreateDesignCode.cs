using System;
using System.Linq;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateDesignCode : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f89b420e-a35e-4197-9c64-87504fe02b59");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateDesignCode;
    public CreateDesignCode()
      : base("Create" + DesignCodeGoo.Name.Replace(" ", string.Empty),
          DesignCodeGoo.Name.Replace(" ", string.Empty),
          "Create a " + DesignCodeGoo.Description + " for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new SafetyFactorENParam(), SafetyFactorsENGoo.Name, SafetyFactorsENGoo.NickName, "(Optional) " + SafetyFactorsENGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(CreepShrinkageParametersGoo.Name + " Short Term", CreepShrinkageParametersGoo.NickName.ToLower(), "(Optional) Short Term " + CreepShrinkageParametersGoo.Description + ". If no input, the default code values will be used", GH_ParamAccess.item);
      pManager.AddGenericParameter(CreepShrinkageParametersGoo.Name + " Long Term", CreepShrinkageParametersGoo.NickName.ToUpper(), "(Optional) Long Term " + CreepShrinkageParametersGoo.Description + ". If no input, the default code values will be used", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new DesignCodeParam());
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
        ec4safetyFactorsGoo = (SafetyFactorsENGoo)Input.GenericGoo<SafetyFactorsENGoo>(this, DA, 0);
        ec4safetyFactors = (ec4safetyFactorsGoo == null) ? null : ec4safetyFactorsGoo.Value;
      }
      else
      {
        safetyFactorsGoo = (SafetyFactorsGoo)Input.GenericGoo<SafetyFactorsGoo>(this, DA, 0);
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
          if (this.Code == Code.BS5950_3_1_1990_Superseded || this.Code == Code.BS5950_3_1_1990_A1_2010)
            if (this.DesignOptions.ProppedDuringConstruction == true)
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Propped construction is defined that is not covered by BS5950: Part 3.1 - 2010, see Clause 5.5.2 of BS5950: Part 3.1 - 2010 for details.");
          break;

        case Code.EN1994_1_1_2004:
          EN1994 ec4 = new EN1994();
          ec4.NationalAnnex = this.NA;
          ec4.DesignOption = this.DesignOptions;
          ec4.CodeOptions = this.EC4CodeOptions;

          CreepShrinkageParametersGoo shrink = (CreepShrinkageParametersGoo)Input.GenericGoo<CreepShrinkageParametersGoo>(this, DA, 1);
          if (shrink != null)
            ec4.CodeOptions.ShortTerm = shrink.Value;
          CreepShrinkageParametersGoo longt = (CreepShrinkageParametersGoo)Input.GenericGoo<CreepShrinkageParametersGoo>(this, DA, 2);
          if (longt != null)
            ec4.CodeOptions.LongTerm = longt.Value;

          if (ec4safetyFactors != null)
            ec4.SafetyFactors = ec4safetyFactors;

          Output.SetItem(this, DA, 0, new DesignCodeGoo(ec4));
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

          Output.SetItem(this, DA, 0, new DesignCodeGoo(asnz));
          break;

        default:
          break;
      }
    }

    #region Custom UI
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
    private Code Code = Code.EN1994_1_1_2004;
    private NationalAnnex NA = NationalAnnex.Generic;
    private DesignOption DesignOptions = new DesignOption();
    private CodeOptionsASNZ CodeOptions = new CodeOptionsASNZ();
    private CodeOptionsEN EC4CodeOptions = new CodeOptionsEN();

    public override void CreateAttributes()
    {
      if (!this._isInitialised)
        this.InitialiseDropdowns();

      m_attributes = new OasysGH.UI.DropDownCheckBoxesComponentAttributes(this, SetSelected, this._dropDownItems, this._selectedItems, CheckBoxToggles, this.Checkboxes, this.CheckboxNames, this._spacerDescriptions);
    }

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] {
        "Code",
        "National Annex",
        "Cement Type",
        "Settings" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // code
      this._dropDownItems.Add(this.DesignCodePretty);
      this._selectedItems.Add(this.DesignCodePretty[2]); //EC4 default

      // national annex
      this._dropDownItems.Add(Enum.GetValues(typeof(NationalAnnex)).Cast<NationalAnnex>()
          .Select(x => x.ToString().Replace("_", " ")).ToList());
      this._selectedItems.Add(this._dropDownItems[1][0]); // Generic default

      // cement type
      this._dropDownItems.Add(Enum.GetValues(typeof(CementClass)).Cast<CementClass>()
          .Select(x => "Cement class " + x.ToString()).ToList());
      this._selectedItems.Add("Cement class " + this.EC4CodeOptions.CementType.ToString()); // Class N default

      this.Checkboxes = new List<bool>();
      this.Checkboxes.Add(this.DesignOptions.ProppedDuringConstruction);
      this.Checkboxes.Add(this.DesignOptions.InclSteelBeamWeight);
      this.Checkboxes.Add(this.DesignOptions.InclThinFlangeSections);
      this.Checkboxes.Add(this.DesignOptions.InclConcreteSlabWeight);
      this.Checkboxes.Add(this.DesignOptions.ConsiderShearDeflection);
      this.Checkboxes.Add(this.EC4CodeOptions.ConsiderShrinkageDeflection);
      this.Checkboxes.Add(this.EC4CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      this.Checkboxes.Add(this.EC4CodeOptions.ApproxModularRatios);

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0)
      {
        for (int k = 0; k < this.DesignCodePretty.Count; k++)
        {
          if (this._selectedItems[i] == this.DesignCodePretty[k])
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
            while (_dropDownItems.Count > 1)
              _dropDownItems.RemoveAt(1);
            while (_selectedItems.Count > 1)
              _selectedItems.RemoveAt(1);
            while (Checkboxes.Count > 5)
              Checkboxes.RemoveAt(5);
            while (CheckboxNames.Count > 5)
              CheckboxNames.RemoveAt(5);
            break;

          case Code.EN1994_1_1_2004:
            // change dropdown content
            while (this._dropDownItems.Count > 1)
              this._dropDownItems.RemoveAt(1);
            while (_selectedItems.Count > 1)
              this._selectedItems.RemoveAt(1);
            // national annex
            this._dropDownItems.Add(Enum.GetValues(typeof(NationalAnnex)).Cast<NationalAnnex>()
                .Select(x => x.ToString().Replace("_", " ")).ToList());
            this._selectedItems.Add(NA.ToString().Replace("_", " "));
            // cement type
            this._dropDownItems.Add(Enum.GetValues(typeof(CementClass)).Cast<CementClass>()
                .Select(x => "Cement class " + x.ToString()).ToList());
            this._selectedItems.Add("Cement class " + this.EC4CodeOptions.CementType.ToString());

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
            while (this._dropDownItems.Count > 1)
              this._dropDownItems.RemoveAt(1);
            while (this._selectedItems.Count > 1)
              this._selectedItems.RemoveAt(1);
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
        this.NA = (NationalAnnex)Enum.Parse(typeof(NationalAnnex), this._selectedItems[i].Replace(" ", "_"));
      if (i == 2)
        this.EC4CodeOptions.CementType = (CementClass)Enum.Parse(typeof(CementClass), this._selectedItems[i].Last().ToString());

      ModeChangeClicked();

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      for (int i = 0; i < this.DesignCodePretty.Count; i++)
      {
        if (this._selectedItems[0] == this.DesignCodePretty[i])
          this.Code = (Code)i;
      }
      if (this.Code == Code.EN1994_1_1_2004)
      {
        this.NA = (NationalAnnex)Enum.Parse(typeof(NationalAnnex), this._selectedItems[1].Replace(" ", "_"));
        this.EC4CodeOptions.CementType = (CementClass)Enum.Parse(typeof(CementClass), this._selectedItems[2].Last().ToString());
      }

      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      Params.Input[0].Optional = true;
      switch (this.Code)
      {
        case Code.EN1994_1_1_2004:
          Params.Input[1].Name = CreepShrinkageParametersGoo.Name + " Short Term";
          Params.Input[1].NickName = CreepShrinkageParametersGoo.NickName.ToLower();
          Params.Input[1].Description = "(Optional) Short Term " + CreepShrinkageParametersGoo.Description + ". If no input, the default code values will be used";
          Params.Input[1].Optional = true;
          Params.Input[2].Name = CreepShrinkageParametersGoo.Name + " Long Term";
          Params.Input[2].NickName = CreepShrinkageParametersGoo.NickName.ToUpper();
          Params.Input[2].Description = "(Optional) Long Term " + CreepShrinkageParametersGoo.Description + ". If no input, the default code values will be used";
          Params.Input[2].Optional = true;
          break;
        case Code.AS_NZS2327_2017:
          Params.Input[1].Name = "Shrinkage";
          Params.Input[1].NickName = "Shr";
          Params.Input[1].Description = "(Optional) Shrinkage multiplier. If no input, the default ASNZ code values will be used";
          Params.Input[1].Optional = true;
          Params.Input[2].Name = "Long Creep Coeff.";
          Params.Input[2].NickName = "CC";
          Params.Input[2].Description = "(Optional) Long Term Creep Coefficient. If no input, the default ASNZ code values will be used";
          Params.Input[2].Optional = true;
          break;

        default:
          break;
      }
    }

    private void CheckBoxToggles(List<bool> newcheckboxes)
    {
      for (int i = 0; i < newcheckboxes.Count; i++)
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
          while (this.Params.Input.Count > 0)
            this.Params.UnregisterInputParameter(Params.Input[0], true);
          this.Params.RegisterInputParam(new SafetyFactorParam());
          break;

        case Code.EN1994_1_1_2004:
          // remove input parameters
          while (this.Params.Input.Count > 0)
            this.Params.UnregisterInputParameter(Params.Input[0], true);

          // add input parameters of generic type
          this.Params.RegisterInputParam(new SafetyFactorENParam());
          this.Params.RegisterInputParam(new Param_GenericObject());
          this.Params.RegisterInputParam(new Param_GenericObject());
          break;

        case Code.AS_NZS2327_2017:
          //remove input parameters
          while (this.Params.Input.Count > 0)
            this.Params.UnregisterInputParameter(Params.Input[0], true);

          //add input parameters of number type
          this.Params.RegisterInputParam(new SafetyFactorParam());
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
      // bool list
      int checkboxCount = reader.GetInt32("checkboxCount");
      List<bool> newcheckboxes = new List<bool>();
      for (int i = 0; i < checkboxCount; i++)
        newcheckboxes.Add(reader.GetBoolean("checkbox" + i));

      // checkbox names
      int namesCount = reader.GetInt32("checkboxnamesCount");
      this.CheckboxNames = new List<string>();
      for (int i = 0; i < namesCount; i++)
        this.CheckboxNames.Add(reader.GetString("checkboxname" + i));

      CheckBoxToggles(newcheckboxes);

      return base.Read(reader);
    }
    #endregion
  }
}
