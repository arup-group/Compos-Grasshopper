using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using OasysGH.Components;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Components
{
  public class CreateCreepShrinkageParams : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("cbc950b0-0a13-40a1-be96-0fb8fac21101");
    public CreateCreepShrinkageParams()
      : base("Create" + CreepShrinkageParametersGoo.Name.Replace(" ", string.Empty),
          CreepShrinkageParametersGoo.Name.Replace(" ", string.Empty),
          "Create a " + CreepShrinkageParametersGoo.Description + " for a (EN) " + DesignCodeGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreepShrinkageParams;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddNumberParameter("Creep Coefficient", "CC", "Creep multiplier used for calculating E ratio for long term and shrinkage (see clause 5.4.2.2 of EN 1994-1-1:2004)", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Concrete Age at Load [Days]", "CAL", "Age of concrete in days when load applied, used to calculate the creep coefficient", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Final Concrete Age [Days]", "CAF", "(Optional) Final age of concrete in days, used to calculate the creep coefficient (default = 36500)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Relative Humidity", "RH", "(Optional) Relative humidity as decimal fraction (0.5 => 50%), used to calculate the creep coefficient (default = 0.5)", GH_ParamAccess.item);
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(CreepShrinkageParametersGoo.Name, CreepShrinkageParametersGoo.NickName, CreepShrinkageParametersGoo.Description + " for a (EN) " + DesignCodeGoo.Description , GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      CreepShrinkageParametersEN csparams = new CreepShrinkageParametersEN();
      
      double creepmultiplier = 0;
      if (DA.GetData(0, ref creepmultiplier))
        csparams.CreepCoefficient = creepmultiplier;
      
      int ageLoad = 0;
      if (DA.GetData(1, ref ageLoad))
        csparams.ConcreteAgeAtLoad = ageLoad;
      
      int ageFinal = 36500;
      if (DA.GetData(2, ref ageFinal))
        csparams.FinalConcreteAgeCreep = ageFinal;
      
      if (this.Params.Input[3].Sources.Count > 0)
        csparams.RelativeHumidity = GetInput.Ratio(this, DA, 3, UnitsNet.Units.RatioUnit.DecimalFraction);
      
      DA.SetData(0, new CreepShrinkageParametersGoo(csparams));
    }
  }
}
