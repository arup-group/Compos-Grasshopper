using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComposGH.Components {
  public class CreateDesignCode : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f89b420e-a35e-4197-9c64-87504fe02b59");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateDesignCode;
    private List<bool> Checkboxes = new List<bool>();

    private List<string> CheckboxNames = new List<string>(new string[]
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

    private CodeOptionsASNZ CodeOptions = new CodeOptionsASNZ();

    private List<string> DesignCodePretty = new List<string>(new string[]
{
      "BS5950-3.1:1990 (superseded)",
      "BS5950-3.1:1990+A1:2010",
      "EN1994-1-1:2004",
      "HKSUOS:2005",
      "HKSUOS:2011",
      "AS/NZS2327:2017"
});

    private DesignOption DesignOptions = new DesignOption();

    private CodeOptionsEN EC4CodeOptions = new CodeOptionsEN();

    private NationalAnnex NA = NationalAnnex.Generic;

    public CreateDesignCode()
                                                                  : base("Create" + DesignCodeGoo.Name.Replace(" ", string.Empty),
      DesignCodeGoo.Name.Replace(" ", string.Empty),
      "Create a " + DesignCodeGoo.Description + " for a " + MemberGoo.Description,
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat5()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void CreateAttributes() {
      if (!_isInitialised)
        InitialiseDropdowns();

      m_attributes = new OasysGH.UI.DropDownCheckBoxesComponentAttributes(this, SetSelected, _dropDownItems, _selectedItems, CheckBoxToggles, Checkboxes, CheckboxNames, _spacerDescriptions);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      // bool list
      int checkboxCount = reader.GetInt32("checkboxCount");
      List<bool> newcheckboxes = new List<bool>();
      for (int i = 0; i < checkboxCount; i++)
        newcheckboxes.Add(reader.GetBoolean("checkbox" + i));

      // checkbox names
      int namesCount = reader.GetInt32("checkboxnamesCount");
      CheckboxNames = new List<string>();
      for (int i = 0; i < namesCount; i++)
        CheckboxNames.Add(reader.GetString("checkboxname" + i));

      CheckBoxToggles(newcheckboxes);

      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        for (int k = 0; k < DesignCodePretty.Count; k++) {
          if (_selectedItems[i] == DesignCodePretty[k]) {
            if (Code == (Code)k)
              return;

            Code = (Code)k;
          }
        }
        switch (Code) {
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
            while (_dropDownItems.Count > 1)
              _dropDownItems.RemoveAt(1);
            while (_selectedItems.Count > 1)
              _selectedItems.RemoveAt(1);
            // national annex
            _dropDownItems.Add(Enum.GetValues(typeof(NationalAnnex)).Cast<NationalAnnex>()
                .Select(x => x.ToString().Replace("_", " ")).ToList());
            _selectedItems.Add(NA.ToString().Replace("_", " "));
            // cement type
            _dropDownItems.Add(Enum.GetValues(typeof(CementClass)).Cast<CementClass>()
                .Select(x => "Cement class " + x.ToString()).ToList());
            _selectedItems.Add("Cement class " + EC4CodeOptions.CementType.ToString());

            while (Checkboxes.Count > 5)
              Checkboxes.RemoveAt(5);
            while (CheckboxNames.Count > 5)
              CheckboxNames.RemoveAt(5);
            Checkboxes.Add(EC4CodeOptions.ConsiderShrinkageDeflection);
            Checkboxes.Add(EC4CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
            Checkboxes.Add(EC4CodeOptions.ApproxModularRatios);
            CheckboxNames.Add("Consider shrinkage deflection");
            CheckboxNames.Add("Ignore shrinkage def. if L/d < 20");
            CheckboxNames.Add("Use approx. modular ratios");

            break;

          case Code.AS_NZS2327_2017:
            // change dropdown content
            while (_dropDownItems.Count > 1)
              _dropDownItems.RemoveAt(1);
            while (_selectedItems.Count > 1)
              _selectedItems.RemoveAt(1);
            while (Checkboxes.Count > 5)
              Checkboxes.RemoveAt(5);
            while (CheckboxNames.Count > 5)
              CheckboxNames.RemoveAt(5);

            // removed due to Compos ignoring that setting
            //Checkboxes.Add(CodeOptions.ConsiderShrinkageDeflection);
            //CheckboxNames.Add("Consider shrinkage deflection");
            break;

          default:
            break;
        }
      }
      if (i == 1)
        NA = (NationalAnnex)Enum.Parse(typeof(NationalAnnex), _selectedItems[i].Replace(" ", "_"));
      if (i == 2)
        EC4CodeOptions.CementType = (CementClass)Enum.Parse(typeof(CementClass), _selectedItems[i].Last().ToString());

      ModeChangeClicked();

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      Params.Input[0].Optional = true;
      switch (Code) {
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

    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
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

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] {
        "Code",
        "National Annex",
        "Cement Type",
        "Settings" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // code
      _dropDownItems.Add(DesignCodePretty);
      _selectedItems.Add(DesignCodePretty[2]); //EC4 default

      // national annex
      _dropDownItems.Add(Enum.GetValues(typeof(NationalAnnex)).Cast<NationalAnnex>()
          .Select(x => x.ToString().Replace("_", " ")).ToList());
      _selectedItems.Add(_dropDownItems[1][0]); // Generic default

      // cement type
      _dropDownItems.Add(Enum.GetValues(typeof(CementClass)).Cast<CementClass>()
          .Select(x => "Cement class " + x.ToString()).ToList());
      _selectedItems.Add("Cement class " + EC4CodeOptions.CementType.ToString()); // Class N default

      Checkboxes = new List<bool>();
      Checkboxes.Add(DesignOptions.ProppedDuringConstruction);
      Checkboxes.Add(DesignOptions.InclSteelBeamWeight);
      Checkboxes.Add(DesignOptions.InclThinFlangeSections);
      Checkboxes.Add(DesignOptions.InclConcreteSlabWeight);
      Checkboxes.Add(DesignOptions.ConsiderShearDeflection);
      Checkboxes.Add(EC4CodeOptions.ConsiderShrinkageDeflection);
      Checkboxes.Add(EC4CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Checkboxes.Add(EC4CodeOptions.ApproxModularRatios);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new SafetyFactorENParam(), SafetyFactorsENGoo.Name, SafetyFactorsENGoo.NickName, "(Optional) " + SafetyFactorsENGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(CreepShrinkageParametersGoo.Name + " Short Term", CreepShrinkageParametersGoo.NickName.ToLower(), "(Optional) Short Term " + CreepShrinkageParametersGoo.Description + ". If no input, the default code values will be used", GH_ParamAccess.item);
      pManager.AddGenericParameter(CreepShrinkageParametersGoo.Name + " Long Term", CreepShrinkageParametersGoo.NickName.ToUpper(), "(Optional) Long Term " + CreepShrinkageParametersGoo.Description + ". If no input, the default code values will be used", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new DesignCodeParam());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      SafetyFactorsGoo safetyFactorsGoo = null;
      ISafetyFactors safetyFactors = null;
      SafetyFactorsENGoo ec4safetyFactorsGoo = null;
      ISafetyFactorsEN ec4safetyFactors = null;
      if (Code == Code.EN1994_1_1_2004) {
        ec4safetyFactorsGoo = (SafetyFactorsENGoo)Input.GenericGoo<SafetyFactorsENGoo>(this, DA, 0);
        ec4safetyFactors = (ec4safetyFactorsGoo == null) ? null : ec4safetyFactorsGoo.Value;
      }
      else {
        safetyFactorsGoo = (SafetyFactorsGoo)Input.GenericGoo<SafetyFactorsGoo>(this, DA, 0);
        safetyFactors = (safetyFactorsGoo == null) ? null : safetyFactorsGoo.Value;
      }

      switch (Code) {
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
          if (Code == Code.BS5950_3_1_1990_Superseded || Code == Code.BS5950_3_1_1990_A1_2010)
            if (DesignOptions.ProppedDuringConstruction == true)
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Propped construction is defined that is not covered by BS5950: Part 3.1 - 2010, see Clause 5.5.2 of BS5950: Part 3.1 - 2010 for details.");
          break;

        case Code.EN1994_1_1_2004:
          EN1994 ec4 = new EN1994();
          ec4.NationalAnnex = NA;
          ec4.DesignOption = DesignOptions;
          ec4.CodeOptions = EC4CodeOptions;

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

          Output.SetItem(this, DA, 0, new DesignCodeGoo(asnz));
          break;

        default:
          break;
      }
    }

    protected override void UpdateUIFromSelectedItems() {
      for (int i = 0; i < DesignCodePretty.Count; i++) {
        if (_selectedItems[0] == DesignCodePretty[i])
          Code = (Code)i;
      }
      if (Code == Code.EN1994_1_1_2004) {
        NA = (NationalAnnex)Enum.Parse(typeof(NationalAnnex), _selectedItems[1].Replace(" ", "_"));
        EC4CodeOptions.CementType = (CementClass)Enum.Parse(typeof(CementClass), _selectedItems[2].Last().ToString());
      }

      base.UpdateUIFromSelectedItems();
    }

    private void CheckBoxToggles(List<bool> newcheckboxes) {
      for (int i = 0; i < newcheckboxes.Count; i++)
        Checkboxes[i] = newcheckboxes[i];
      DesignOptions.ProppedDuringConstruction = Checkboxes[0];
      DesignOptions.InclSteelBeamWeight = Checkboxes[1];
      DesignOptions.InclThinFlangeSections = Checkboxes[2];
      DesignOptions.InclConcreteSlabWeight = Checkboxes[3];
      DesignOptions.ConsiderShearDeflection = Checkboxes[4];
      // removed due to Compos ignoring that setting
      //if (Checkboxes.Count == 6)
      //  CodeOptions.ConsiderShrinkageDeflection = Checkboxes[5];
      if (Checkboxes.Count == 8) {
        EC4CodeOptions.ConsiderShrinkageDeflection = Checkboxes[5];
        EC4CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios = Checkboxes[6];
        EC4CodeOptions.ApproxModularRatios = Checkboxes[7];
      }
    }

    private void ModeChangeClicked() {
      RecordUndoEvent("Changed Parameters");
      switch (Code) {
        case Code.BS5950_3_1_1990_Superseded:
        case Code.BS5950_3_1_1990_A1_2010:
        case Code.HKSUOS_2005:
        case Code.HKSUOS_2011:
          // remove input parameters
          while (Params.Input.Count > 0)
            Params.UnregisterInputParameter(Params.Input[0], true);
          Params.RegisterInputParam(new SafetyFactorParam());
          break;

        case Code.EN1994_1_1_2004:
          // remove input parameters
          while (Params.Input.Count > 0)
            Params.UnregisterInputParameter(Params.Input[0], true);

          // add input parameters of generic type
          Params.RegisterInputParam(new SafetyFactorENParam());
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_GenericObject());
          break;

        case Code.AS_NZS2327_2017:
          //remove input parameters
          while (Params.Input.Count > 0)
            Params.UnregisterInputParameter(Params.Input[0], true);

          //add input parameters of number type
          Params.RegisterInputParam(new SafetyFactorParam());
          Params.RegisterInputParam(new Param_Number());
          Params.RegisterInputParam(new Param_Number());
          break;

        default:
          break;
      }
    }
  }
}
