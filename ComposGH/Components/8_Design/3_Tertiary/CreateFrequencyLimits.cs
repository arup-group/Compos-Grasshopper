using System;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components
{
  public class CreateFrequencyLimits : GH_OasysComponent 
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f86088eb-6cfa-4e73-99d6-af5d33fafb7f");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.FrequencyLimit;
    public CreateFrequencyLimits()
      : base("Create" + FrequencyLimitsGoo.Name.Replace(" ", string.Empty),
          FrequencyLimitsGoo.Name.Replace(" ", string.Empty),
          "Create a " + FrequencyLimitsGoo.Description + " for a " + DesignCriteriaGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { Hidden = true; } // sets the initial state of the component to hidden
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
      pManager.AddParameter(new FrequencyLimitsParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      FrequencyLimits frequencyLimits = new FrequencyLimits()
      { MinimumRequired = (Frequency)Input.UnitNumber(this, DA, 0, FrequencyUnit.Hertz) };
      
      if (Params.Input[1].Sources.Count > 0)
        frequencyLimits.DeadLoadIncl = (Ratio)Input.UnitNumber(this, DA, 1, RatioUnit.DecimalFraction);

      if (Params.Input[2].Sources.Count > 0)
        frequencyLimits.LiveLoadIncl = (Ratio)Input.UnitNumber(this, DA, 2, RatioUnit.DecimalFraction);

      DA.SetData(0, new FrequencyLimitsGoo(frequencyLimits));
    }
  }
}
