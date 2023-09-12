using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components {
  public class CreateSafetyFactorsEN : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("842633ae-4a9c-4483-a606-02f1099fed0f");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.EC4SafetyFactors;
    private LoadCombination LoadCombinationType = LoadCombination.Equation6_10;

    public CreateSafetyFactorsEN() : base("Create" + SafetyFactorsENGoo.Name.Replace(" ", string.Empty),
      SafetyFactorsENGoo.Name.Replace(" ", string.Empty),
      "Create a " + SafetyFactorsENGoo.Description + " for a " + DesignCodeGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];

      if (LoadCombinationType.ToString().Replace("__", " or ").Replace("_", ".") == _selectedItems[i]) {
        return;
      }

      LoadCombinationType = (LoadCombination)Enum.Parse(typeof(LoadCombination), _selectedItems[i].Replace(" or ", "__").Replace(".", "_"));

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Load Combination" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // spacing
      _dropDownItems.Add(Enum.GetValues(typeof(LoadCombination)).Cast<LoadCombination>()
          .Select(x => x.ToString().Replace("__", " or ").Replace("_", ".")).ToList());
      _dropDownItems[0].RemoveAt(2); // remove 'Custom'
      _selectedItems.Add(LoadCombination.Equation6_10.ToString().Replace("__", " or ").Replace("_", "."));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddNumberParameter("Const. ξ-factor", "Cξ", "EC0 reduction factor at construction stage (dead/permanent load)", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Const. Combination factor", "CΨ0", "Factor for construction stage combination value of a variable action", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Const. Permanent load factor", "CγG", "Partial factor for permanent loads at construction stage", GH_ParamAccess.item, 1.35);
      pManager.AddNumberParameter("Const. Variable load factor", "CγQ", "Partial factor for variable loads at construction stage", GH_ParamAccess.item, 1.5);
      pManager.AddNumberParameter("Final ξ-factor", "Fξ", "EC0 reduction factor at final stage (dead/permanent load)", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Final Combination factor", "FΨ0", "Factor for final stage combination value of a variable action", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Final Permanent load factor", "FγG", "Partial factor for permanent loads at final stage", GH_ParamAccess.item, 1.35);
      pManager.AddNumberParameter("Final Variable load factor", "FγQ", "Partial factor for variable loads at final stage", GH_ParamAccess.item, 1.5);
      pManager.AddNumberParameter("Steel γM0 factor", "γM0", "Steel beam partial factor for resistance of cross-sections whatever the class is", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Steel γM1 factor", "γM1", "Steel beam partial factor for resistance of members to instability assessed by member checks", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Steel γM2 factor", "γM2", "Steel beam partial factor for resistance of cross-sections in tension to fracture", GH_ParamAccess.item, 1.25);
      pManager.AddNumberParameter("Concrete γC factor", "γC", "Concrete material partial factor", GH_ParamAccess.item, 1.5);
      pManager.AddNumberParameter("Decking γ-factor", "γDeck", "Material Partial Safety Factor for Metal Decking", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Shear Stud safety factor", "γvs", "Material Partial Safety Factor for Shear Studs", GH_ParamAccess.item, 1.25);
      pManager.AddNumberParameter("Shear Stud safety factor", "γS", "Material Partial Safety Factor for Reinforcement", GH_ParamAccess.item, 1.15);
      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new SafetyFactorENParam());
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      var combinationFactors = new LoadCombinationFactors();
      double cxi = 0;
      double cpsi0 = 0;
      double cgammaG = 0;
      double cgammaQ = 0;
      double fxi = 0;
      double fpsi0 = 0;
      double fgammaG = 0;
      double fgammaQ = 0;
      if (DA.GetData(0, ref cxi)) {
        combinationFactors.ConstantXi = cxi;
      }
      if (DA.GetData(1, ref cpsi0)) {
        combinationFactors.ConstantPsi = cpsi0;
      }
      if (DA.GetData(2, ref cgammaG)) {
        combinationFactors.Constantgamma_G = cgammaG;
      }
      if (DA.GetData(3, ref cgammaQ)) {
        combinationFactors.Constantgamma_Q = cgammaQ;
      }
      if (DA.GetData(4, ref fxi)) {
        combinationFactors.FinalXi = fxi;
      }
      if (DA.GetData(5, ref fpsi0)) {
        combinationFactors.FinalPsi = fpsi0;
      }
      if (DA.GetData(6, ref fgammaG)) {
        combinationFactors.Finalgamma_G = fgammaG;
      }
      if (DA.GetData(7, ref fgammaQ)) {
        combinationFactors.Finalgamma_Q = fgammaQ;
      }
      if (Params.Input[0].Sources.Count == 0
        & Params.Input[1].Sources.Count == 0
        & Params.Input[2].Sources.Count == 0
        & Params.Input[3].Sources.Count == 0
        & Params.Input[4].Sources.Count == 0
        & Params.Input[5].Sources.Count == 0
        & Params.Input[6].Sources.Count == 0
        & Params.Input[7].Sources.Count == 0) {
        string remark = (LoadCombinationType == LoadCombination.Equation6_10) ?
          "Load combination factors following Equation 6.10 will be used" :
          "Load combination factors for the worse of Equation 6.10a and 6.10b will be used (not applicable for storage structures)";
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, remark);
        _selectedItems[0] = LoadCombinationType.ToString().Replace("__", " or ").Replace("_", ".");
      } else {
        _selectedItems[0] = "Custom";
      }

      var mf = new MaterialPartialFactors();
      double gM0 = 0;
      double gM1 = 0;
      double gM2 = 0;
      double gC = 0;
      double gDeck = 0;
      double gvs = 0;
      double gS = 0;
      if (DA.GetData(8, ref gM0)) {
        mf.Gamma_M0 = gM0;
      }
      if (DA.GetData(9, ref gM1)) {
        mf.Gamma_M1 = gM1;
      }
      if (DA.GetData(10, ref gM2)) {
        mf.Gamma_M2 = gM2;
      }
      if (DA.GetData(11, ref gC)) {
        mf.Gamma_C = gC;
      }
      if (DA.GetData(12, ref gDeck)) {
        mf.Gamma_Deck = gDeck;
      }
      if (DA.GetData(13, ref gvs)) {
        mf.Gamma_vs = gvs;
      }
      if (DA.GetData(14, ref gS)) {
        mf.Gamma_S = gS;
      }

      if (Params.Input[8].Sources.Count == 0
        & Params.Input[9].Sources.Count == 0
        & Params.Input[10].Sources.Count == 0
        & Params.Input[11].Sources.Count == 0
        & Params.Input[12].Sources.Count == 0
        & Params.Input[13].Sources.Count == 0
        & Params.Input[14].Sources.Count == 0) {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Default Material Partial Safety Factor values from EN1994-1-1 will be used");
      }

      var safetyFactors = new SafetyFactorsEN();
      if (combinationFactors == null) {
        safetyFactors.LoadCombinationFactors.LoadCombination = LoadCombinationType;
      } else {
        safetyFactors.LoadCombinationFactors.LoadCombination = LoadCombination.Custom;
      }

      safetyFactors.LoadCombinationFactors = combinationFactors;

      if (mf != null) {
        safetyFactors.MaterialFactors = mf;
      }

      DA.SetData(0, new SafetyFactorsENGoo(safetyFactors));
    }

    protected override void UpdateUIFromSelectedItems() {
      LoadCombinationType = (LoadCombination)Enum.Parse(typeof(LoadCombination), _selectedItems[0].Replace(" or ", "__").Replace(".", "_"));

      base.UpdateUIFromSelectedItems();
    }
  }
}
