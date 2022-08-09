using System;

using ComposGH.Parameters;
using Grasshopper.Kernel;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateCreepShrinkageParams : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("cbc950b0-0a13-40a1-be96-0fb8fac21101");
    public CreateCreepShrinkageParams()
      : base("Creep & Shrinkage Params", "CSP", "Create Compos Creep and Shrinkage parameters for EN1994-1-1 Design Code",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreepShrinkageParams;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddNumberParameter("Creep Coefficient", "CC", "Creep multiplier used for calculating E ratio for long term and shrinkage (see clause 5.4.2.2 of EN 1994-1-1:2004)", GH_ParamAccess.item, 1.4);
      pManager.AddIntegerParameter("Concrete Age at Load [Days]", "CAL", "Age of concrete in days when load applied, used to calculate the creep coefficient ", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Final Concrete Age [Days]", "CAF", "(Optional) Final age of concrete in days, used to calculate the creep coefficient (default = 36500)", GH_ParamAccess.item, 36500);
      pManager.AddNumberParameter("Relative Humidity", "RH", "(Optional) Relative humidity as decimal fraction (0.5 => 50%), used to calculate the creep coefficient (default = 0.5)", GH_ParamAccess.item, 0.5);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Creep & Shrinkage Params", "CSP", "Create Compos Creep and Shrinkage parameters for EN1994-1-1 Design Code", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      CreepShrinkageParametersEN csparams = new CreepShrinkageParametersEN();
      csparams.ConcreteAgeAtLoad = 28;

      
      double creepmultiplier = 0;
      int ageLoad = 0;
      int ageFinal = 0;
      if (DA.GetData(0, ref creepmultiplier))
        csparams.CreepCoefficient = creepmultiplier;
      if (DA.GetData(1, ref ageLoad))
        csparams.ConcreteAgeAtLoad = ageLoad;
      if (DA.GetData(2, ref ageFinal))
        csparams.FinalConcreteAgeCreep = ageFinal;
      if (this.Params.Input[3].Sources.Count > 0)
        csparams.RelativeHumidity = GetInput.Ratio(this, DA, 3, UnitsNet.Units.RatioUnit.DecimalFraction);
      
      DA.SetData(0, new CreepShrinkageEuroCodeParametersGoo(csparams));
    }
  }
}
