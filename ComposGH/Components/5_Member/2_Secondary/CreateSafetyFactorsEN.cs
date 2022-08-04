using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateSafetyFactorsEN : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("842633ae-4a9c-4483-a606-02f1099fed0f");
    public CreateSafetyFactorsEN()
      : base("Create" + SafetyFactorsENGoo.Name.Replace(" ", string.Empty),
          SafetyFactorsENGoo.Name.Replace(" ", string.Empty),
          "Create a " + SafetyFactorsENGoo.Description + " for a " + DesignCodeGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.EC4SafetyFactors;
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
      "Load Combination",
    });

    private bool First = true;
    private LoadCombination LoadCombinationType = LoadCombination.Equation6_10;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // spacing
        DropdownItems.Add(Enum.GetValues(typeof(LoadCombination)).Cast<LoadCombination>()
            .Select(x => x.ToString().Replace("__", " or ").Replace("_", ".")).ToList());
        DropdownItems[0].RemoveAt(2); // remove 'Custom'
        SelectedItems.Add(LoadCombination.Equation6_10.ToString().Replace("__", " or ").Replace("_", "."));

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropdownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

      if (LoadCombinationType.ToString().Replace("__", " or ").Replace("_", ".") == SelectedItems[i])
        return;

      LoadCombinationType = (LoadCombination)Enum.Parse(typeof(LoadCombination), SelectedItems[i].Replace(" or ", "__").Replace(".", "_"));

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      LoadCombinationType = (LoadCombination)Enum.Parse(typeof(StudSpacingType), SelectedItems[0].Replace(" or ", "__").Replace(".", "_"));

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
      pManager.AddNumberParameter("Const. ξ-factor", "ξ", "EC0 reduction factor at construction stage (dead/permenant load)", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Const. Combination factor", "Ψ0", "Factor for construction stage combination value of a variable action", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Const. Permanent load factor", "γG", "Partial factor for permanent loads at construction stage", GH_ParamAccess.item, 1.35);
      pManager.AddNumberParameter("Const. Variable load factor", "γQ", "Partial factor for variable loads at construction stage", GH_ParamAccess.item, 1.5);
      pManager.AddNumberParameter("Final ξ-factor", "ξ", "EC0 reduction factor at final stage (dead/permenant load)", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Final Combination factor", "Ψ0", "Factor for final stage combination value of a variable action", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Final Permanent load factor", "γG", "Partial factor for permanent loads at final stage", GH_ParamAccess.item, 1.35);
      pManager.AddNumberParameter("Final Variable load factor", "γQ", "Partial factor for variable loads at final stage", GH_ParamAccess.item, 1.5);
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
      pManager.AddGenericParameter(SafetyFactorsENGoo.Name, SafetyFactorsENGoo.NickName, SafetyFactorsENGoo.Description + " for a " + DesignCodeGoo.Description + " (EN)", GH_ParamAccess.item);
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
        combinationFactors = null;
        SelectedItems[0] = LoadCombinationType.ToString().Replace("__", " or ").Replace("_", ".");
      }
      else
      {
        SelectedItems[0] = "Custom";
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
        mf = null;
      }

      SafetyFactorsEN safetyFactors = new SafetyFactorsEN();
      if (combinationFactors == null)
        safetyFactors.LoadCombinationFactors.LoadCombination = this.LoadCombinationType;
      else
        safetyFactors.LoadCombinationFactors.LoadCombination = LoadCombination.Custom;

      safetyFactors.LoadCombinationFactors = combinationFactors;

      if (mf != null)
        safetyFactors.MaterialFactors = mf;

      DA.SetData(0, new SafetyFactorsENGoo(safetyFactors));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropdownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropdownItems, ref SelectedItems, ref SpacerDescriptions);

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
      
    }
    #endregion
  }
}
