﻿using System;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components {
  public class CreateCreepShrinkageParams : GH_OasysComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("cbc950b0-0a13-40a1-be96-0fb8fac21101");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreepShrinkageParams;

    public CreateCreepShrinkageParams() : base("Create" + CreepShrinkageParametersGoo.Name.Replace(" ", string.Empty),
      CreepShrinkageParametersGoo.Name.Replace(" ", string.Empty),
      "Create a " + CreepShrinkageParametersGoo.Description + " for a (EN) " + DesignCodeGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5()) { Hidden = true; } // sets the initial state of the component to hidden

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddNumberParameter("Creep Coefficient", "CC", "Creep multiplier used for calculating E ratio for long term and shrinkage (see clause 5.4.2.2 of EN 1994-1-1:2004)", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Concrete Age at Load [Days]", "CAL", "Age of concrete in days when load applied, used to calculate the creep coefficient", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Final Concrete Age [Days]", "CAF", "(Optional) Final age of concrete in days, used to calculate the creep coefficient (default = 36500)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Relative Humidity", "RH", "(Optional) Relative humidity as decimal fraction (0.5 => 50%), used to calculate the creep coefficient (default = 0.5)", GH_ParamAccess.item);
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter(CreepShrinkageParametersGoo.Name, CreepShrinkageParametersGoo.NickName, CreepShrinkageParametersGoo.Description + " for a (EN) " + DesignCodeGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      var csparams = new CreepShrinkageParametersEN();

      double creepmultiplier = 0;
      if (DA.GetData(0, ref creepmultiplier)) {
        csparams.CreepCoefficient = creepmultiplier;
      }

      int ageLoad = 0;
      if (DA.GetData(1, ref ageLoad)) {
        csparams.ConcreteAgeAtLoad = ageLoad;
      }

      int ageFinal = 36500;
      if (DA.GetData(2, ref ageFinal)) {
        csparams.FinalConcreteAgeCreep = ageFinal;
      }

      if (Params.Input[3].Sources.Count > 0) {
        csparams.RelativeHumidity = (Ratio)Input.UnitNumber(this, DA, 3, RatioUnit.DecimalFraction);
      }

      DA.SetData(0, new CreepShrinkageParametersGoo(csparams));
    }
  }
}
