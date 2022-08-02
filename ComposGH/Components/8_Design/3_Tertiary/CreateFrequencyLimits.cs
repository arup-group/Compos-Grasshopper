using System;

using ComposGH.Parameters;
using Grasshopper.Kernel;
using ComposAPI;
using System.Collections.Generic;
using UnitsNet.Units;
using UnitsNet;

namespace ComposGH.Components
{
  public class CreateFrequencyLimits : GH_Component 
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f86088eb-6cfa-4e73-99d6-af5d33fafb7f");
    public CreateFrequencyLimits()
      : base("BeamSizeLimit", "BLm", "Create Compos Beam Size Limits for Design Criteria",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    //protected override System.Drawing.Bitmap Icon => Properties.Resources.CreepShrinkageParams;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Min Required [Hz]", "fmin", "Minimum required frequency", GH_ParamAccess.item);
      pManager.AddGenericParameter("Dead Load Incl.", "DL", "(Optional) Portion of Dead load included in frequency calculation (as decimal fraction). Default is 1.0 => 100%.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Live Load Incl.", "DL", "(Optional) Portion of Dead load included in frequency calculation (as decimal fraction). Default is 0.1 => 10%.", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Frequency Limit", "FLm", "Compos Frequency Limit for Design Criteria", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      FrequencyLimits frequencyLimits = new FrequencyLimits()
      { MinimumRequired = GetInput.Frequency(this, DA, 0, FrequencyUnit.Hertz) };
      
      if (this.Params.Input[1].Sources.Count > 0)
        frequencyLimits.DeadLoadIncl = GetInput.Ratio(this, DA, 1, RatioUnit.DecimalFraction);

      if (this.Params.Input[2].Sources.Count > 0)
        frequencyLimits.LiveLoadIncl = GetInput.Ratio(this, DA, 2, RatioUnit.DecimalFraction);

      DA.SetData(0, new FrequencyLimitsGoo(frequencyLimits));
    }
  }
}
