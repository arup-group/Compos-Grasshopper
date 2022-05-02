﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using Grasshopper.Kernel.Parameters;
using ComposAPI;

namespace ComposGH.Components
{
  public class EC4SafetyFactors : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("842633ae-4a9c-4483-a606-02f1099fed0f");
    public EC4SafetyFactors()
      : base("EC4 Safety Factors", "EC4SF", "Create Compos EC4 Safety Factors",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    //protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateStud;
    #endregion

    #region Custom UI
    // This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // spacing
        dropdownitems.Add(Enum.GetValues(typeof(LoadCombination)).Cast<LoadCombination>()
            .Select(x => x.ToString().Replace("__", " or ").Replace("_", ".")).ToList());
        dropdownitems[0].RemoveAt(2); // remove 'Custom'
        selecteditems.Add(LoadCombination.Equation6_10.ToString().Replace("__", " or ").Replace("_", "."));

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (loadcombinationType.ToString().Replace("__", " or ").Replace("_", ".") == selecteditems[i])
        return;

      loadcombinationType = (LoadCombination)Enum.Parse(typeof(LoadCombination), selecteditems[i].Replace(" or ", "__").Replace(".", "_"));

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      loadcombinationType = (LoadCombination)Enum.Parse(typeof(StudGroupSpacing.StudSpacingType), selecteditems[0].Replace(" or ", "__").Replace(".", "_"));

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Load Combination",
    });

    private bool first = true;
    private LoadCombination loadcombinationType = LoadCombination.Equation6_10;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddNumberParameter("Const. Dead", "ξ", "EC0 reduction factor at consturction stage (dead/permenant load)", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Combination factor", "Ψ0", "Factor for combination value of a variable action", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Permanent load factor", "γG", "Partial factor for permanent loads", GH_ParamAccess.item, 1.35);
      pManager.AddNumberParameter("Variable load factor", "γQ", "Partial factor for variable loads", GH_ParamAccess.item, 1.5);
      pManager.AddNumberParameter("Steel γM0 factor", "γM0", "Steel beam partial factor for resistance of cross-sections whatever the class is", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Steel γM1 factor", "γM1", "Steel beam partial factor for resistance of members to instability assessed by member checks", GH_ParamAccess.item, 1.5);
      pManager.AddNumberParameter("Steel γM2 factor", "γM2", "Steel beam partial factor for resistance of cross-sections in tension to fracture", GH_ParamAccess.item, 1.25);
      pManager.AddNumberParameter("Concrete γC factor", "γC", "Concrete material partial factor", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Decking γ-factor", "γDeck", "Material Partial Safety Factor for Metal Decking", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Shear Stud safety factor", "γvs", "Material Partial Safety Factor for Shear Studs", GH_ParamAccess.item, 1.25);
      pManager.AddNumberParameter("Shear Stud safety factor", "γS", "Material Partial Safety Factor for Reinforcement", GH_ParamAccess.item, 1.15);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Safety Factors", "SF", "Compos Safety Factors", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      LoadCombinationFactors lf = new LoadCombinationFactors();
      double xi = 0;
      double psi0 = 0;
      double gammaG = 0;
      double gammaQ = 0;
      if (DA.GetData(0, ref xi))
        lf.xi = xi;
      if (DA.GetData(1, ref psi0))
        lf.psi_0 = psi0;
      if (DA.GetData(2, ref gammaG))
        lf.gamma_G = gammaG;
      if (DA.GetData(3, ref gammaQ))
        lf.gamma_Q = gammaQ;
      if (this.Params.Input[0].Sources.Count == 0
        & this.Params.Input[1].Sources.Count == 0
        & this.Params.Input[2].Sources.Count == 0
        & this.Params.Input[3].Sources.Count == 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Default Load Factor values from BS5950-1.1:1990+A1:2010");
        lf = null;
        selecteditems[0] = loadcombinationType.ToString().Replace("__", " or ").Replace("_", ".");
      }
      else
      {
        selecteditems[0] = "Custom";
      }

      EC4MaterialPartialFactors mf = new EC4MaterialPartialFactors();
      double gM0 = 0;
      double gM1 = 0;
      double gM2 = 0;
      double gC = 0;
      double gDeck = 0;
      double gvs = 0;
      double gS = 0;
      if (DA.GetData(4, ref gM0))
        mf.gamma_M0 = gM0;
      if (DA.GetData(5, ref gM1))
        mf.gamma_M1 = gM1;
      if (DA.GetData(6, ref gM2))
        mf.gamma_M2 = gM2;
      if (DA.GetData(7, ref gC))
        mf.gamma_C = gC;
      if (DA.GetData(8, ref gDeck))
        mf.gamma_Deck = gDeck;
      if (DA.GetData(9, ref gvs))
        mf.gamma_vs = gvs;
      if (DA.GetData(10, ref gS))
        mf.gamma_S = gS;


      if (this.Params.Input[4].Sources.Count == 0
        & this.Params.Input[5].Sources.Count == 0
        & this.Params.Input[6].Sources.Count == 0
        & this.Params.Input[7].Sources.Count == 0
        & this.Params.Input[8].Sources.Count == 0
        & this.Params.Input[9].Sources.Count == 0
        & this.Params.Input[10].Sources.Count == 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Default Material Partial Safety Factor values from EN1994-1-1");
        mf = null;
      }

      ComposAPI.EC4SafetyFactors sf = new ComposAPI.EC4SafetyFactors();
      if (lf == null)
        sf.LoadCombination = this.loadcombinationType;
      else
        sf.LoadCombination = LoadCombination.Custom;
      
      if (mf != null)
        sf.MaterialFactors = mf;

      DA.SetData(0, new SafetyFactorsGoo(sf));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

      UpdateUIFromSelectedItems();

      first = false;

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
      
    }
    #endregion
  }
}
