﻿using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateSafetyFactorsEN : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("842633ae-4a9c-4483-a606-02f1099fed0f");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.EC4SafetyFactors;
    public CreateSafetyFactorsEN()
      : base("Create" + SafetyFactorsENGoo.Name.Replace(" ", string.Empty),
          SafetyFactorsENGoo.Name.Replace(" ", string.Empty),
          "Create a " + SafetyFactorsENGoo.Description + " for a " + DesignCodeGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
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
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new SafetyFactorENParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      LoadCombinationFactors combinationFactors = new LoadCombinationFactors();
      double cxi = 0;
      double cpsi0 = 0;
      double cgammaG = 0;
      double cgammaQ = 0;
      double fxi = 0;
      double fpsi0 = 0;
      double fgammaG = 0;
      double fgammaQ = 0;
      if (DA.GetData(0, ref cxi))
        combinationFactors.ConstantXi = cxi;
      if (DA.GetData(1, ref cpsi0))
        combinationFactors.ConstantPsi = cpsi0;
      if (DA.GetData(2, ref cgammaG))
        combinationFactors.Constantgamma_G = cgammaG;
      if (DA.GetData(3, ref cgammaQ))
        combinationFactors.Constantgamma_Q = cgammaQ;
      if (DA.GetData(4, ref fxi))
        combinationFactors.FinalXi = fxi;
      if (DA.GetData(5, ref fpsi0))
        combinationFactors.FinalPsi = fpsi0;
      if (DA.GetData(6, ref fgammaG))
        combinationFactors.Finalgamma_G = fgammaG;
      if (DA.GetData(7, ref fgammaQ))
        combinationFactors.Finalgamma_Q = fgammaQ;
      if (this.Params.Input[0].Sources.Count == 0
        & this.Params.Input[1].Sources.Count == 0
        & this.Params.Input[2].Sources.Count == 0
        & this.Params.Input[3].Sources.Count == 0
        & this.Params.Input[4].Sources.Count == 0
        & this.Params.Input[5].Sources.Count == 0
        & this.Params.Input[6].Sources.Count == 0
        & this.Params.Input[7].Sources.Count == 0)
      {
        string remark = (LoadCombinationType == LoadCombination.Equation6_10) ?
          "Load combination factors following Equation 6.10 will be used" :
          "Load combination factors for the worse of Equation 6.10a and 6.10b will be used (not applicable for storage structures)";
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, remark);
        this._selectedItems[0] = LoadCombinationType.ToString().Replace("__", " or ").Replace("_", ".");
      }
      else
      {
        this._selectedItems[0] = "Custom";
      }

      MaterialPartialFactors mf = new MaterialPartialFactors();
      double gM0 = 0;
      double gM1 = 0;
      double gM2 = 0;
      double gC = 0;
      double gDeck = 0;
      double gvs = 0;
      double gS = 0;
      if (DA.GetData(8, ref gM0))
        mf.gamma_M0 = gM0;
      if (DA.GetData(9, ref gM1))
        mf.gamma_M1 = gM1;
      if (DA.GetData(10, ref gM2))
        mf.gamma_M2 = gM2;
      if (DA.GetData(11, ref gC))
        mf.gamma_C = gC;
      if (DA.GetData(12, ref gDeck))
        mf.gamma_Deck = gDeck;
      if (DA.GetData(13, ref gvs))
        mf.gamma_vs = gvs;
      if (DA.GetData(14, ref gS))
        mf.gamma_S = gS;


      if (this.Params.Input[8].Sources.Count == 0
        & this.Params.Input[9].Sources.Count == 0
        & this.Params.Input[10].Sources.Count == 0
        & this.Params.Input[11].Sources.Count == 0
        & this.Params.Input[12].Sources.Count == 0
        & this.Params.Input[13].Sources.Count == 0
        & this.Params.Input[14].Sources.Count == 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Default Material Partial Safety Factor values from EN1994-1-1 will be used");
      }

      SafetyFactorsEN safetyFactors = new SafetyFactorsEN();
      if (combinationFactors == null)
        safetyFactors.LoadCombinationFactors.LoadCombination = this.LoadCombinationType;
      else
        safetyFactors.LoadCombinationFactors.LoadCombination = LoadCombination.Custom;

      safetyFactors.LoadCombinationFactors = combinationFactors;

      if (mf != null)
        safetyFactors.MaterialFactors = mf;

      Output.SetItem(this, DA, 0, new SafetyFactorsENGoo(safetyFactors));
    }

    #region Custom UI
    private LoadCombination LoadCombinationType = LoadCombination.Equation6_10;

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] { "Load Combination" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // spacing
      this._dropDownItems.Add(Enum.GetValues(typeof(LoadCombination)).Cast<LoadCombination>()
          .Select(x => x.ToString().Replace("__", " or ").Replace("_", ".")).ToList());
      this._dropDownItems[0].RemoveAt(2); // remove 'Custom'
      this._selectedItems.Add(LoadCombination.Equation6_10.ToString().Replace("__", " or ").Replace("_", "."));

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (this.LoadCombinationType.ToString().Replace("__", " or ").Replace("_", ".") == this._selectedItems[i])
        return;

      this.LoadCombinationType = (LoadCombination)Enum.Parse(typeof(LoadCombination), this._selectedItems[i].Replace(" or ", "__").Replace(".", "_"));

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      this.LoadCombinationType = (LoadCombination)Enum.Parse(typeof(LoadCombination), this._selectedItems[0].Replace(" or ", "__").Replace(".", "_"));

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
