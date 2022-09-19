using System;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateSafetyFactors : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c0df8c23-4aa1-439b-83a1-9b59078284c2");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.SafetyFactors;
    public CreateSafetyFactors()
      : base("Create" + SafetyFactorsGoo.Name.Replace(" ", string.Empty),
          SafetyFactorsGoo.Name.Replace(" ", string.Empty),
          "Create a " + SafetyFactorsGoo.Description + " for a " + DesignCodeGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddNumberParameter("Const. Dead", "dlF", "Load combination factor (γf) for Dead Load during Construction Stage", GH_ParamAccess.item, 1.4);
      pManager.AddNumberParameter("Final Dead", "DLF", "Load combination factor (γf) for Final Dead Load", GH_ParamAccess.item, 1.4);
      pManager.AddNumberParameter("Const. Live", "llF", "Load combination factor (γf) for Live Load during Construction Stage", GH_ParamAccess.item, 1.6);
      pManager.AddNumberParameter("Final Live", "LLF", "Load combination factor (γf) for Final Live Load", GH_ParamAccess.item, 1.6);
      pManager.AddNumberParameter("Steel beam", "SBF", "Steel material safety factor", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Concrete compression", "CcF", "Concrete material safety factor in compression", GH_ParamAccess.item, 1.5);
      pManager.AddNumberParameter("Concrete shear", "CvF", "Concrete material safety factor in shear", GH_ParamAccess.item, 1.25);
      pManager.AddNumberParameter("Metal decking", "MdF", "Decking material safety factor", GH_ParamAccess.item, 1.0);
      pManager.AddNumberParameter("Shear stud", "SsF", "Shear stud safety factor", GH_ParamAccess.item, 1.25);
      pManager.AddNumberParameter("Reinforcement", "RbF", "Reinforcement material safety factor", GH_ParamAccess.item, 1.15);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new SafetyFactorParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      LoadFactors lf = new LoadFactors();
      double dl = 0;
      double DL = 0;
      double ll = 0;
      double LL = 0;
      if (DA.GetData(0, ref dl))
        lf.ConstantDead = dl;
      if (DA.GetData(1, ref DL))
        lf.FinalDead = DL;
      if (DA.GetData(2, ref ll))
        lf.ConstantLive = ll;
      if (DA.GetData(3, ref LL))
        lf.FinalLive = LL;
      if (this.Params.Input[0].Sources.Count == 0
        & this.Params.Input[1].Sources.Count == 0
        & this.Params.Input[2].Sources.Count == 0
        & this.Params.Input[3].Sources.Count == 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Default Load Factor values from BS5950-1.1:1990+A1:2010");
      }

      MaterialFactors mf = new MaterialFactors();
      double steel = 0;
      double conc_comp = 0;
      double conc_shear = 0;
      double deck = 0;
      double stud = 0;
      double reb = 0;
      if (DA.GetData(4, ref steel))
        mf.SteelBeam = steel;
      if (DA.GetData(5, ref conc_comp))
        mf.ConcreteCompression = conc_comp;
      if (DA.GetData(6, ref conc_shear))
        mf.ConcreteShear = conc_shear;
      if (DA.GetData(7, ref deck))
        mf.MetalDecking = deck;
      if (DA.GetData(8, ref stud))
        mf.ShearStud = stud;
      if (DA.GetData(9, ref reb))
        mf.Reinforcement = reb;


      if (this.Params.Input[4].Sources.Count == 0
        & this.Params.Input[5].Sources.Count == 0
        & this.Params.Input[6].Sources.Count == 0
        & this.Params.Input[7].Sources.Count == 0
        & this.Params.Input[8].Sources.Count == 0
        & this.Params.Input[9].Sources.Count == 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Default Material Partial Safety Factor values from BS5950-1.1:1990+A1:2010");
      }
      SafetyFactors sf = new SafetyFactors() { LoadFactors = lf, MaterialFactors = mf };
      DA.SetData(0, new SafetyFactorsGoo(sf));
    }
  }
}
