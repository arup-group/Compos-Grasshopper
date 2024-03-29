﻿using System;
using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IFrequencyLimits"/> class can be used in Grasshopper.
  /// </summary>
  public class FrequencyLimitsGoo : GH_OasysGoo<IFrequencyLimits> {
    public static string Description => "Compos Frequency Limit Criteria";
    public static string Name => "Frequency Limits";
    public static string NickName => "fLm";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public FrequencyLimitsGoo(IFrequencyLimits item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new FrequencyLimitsGoo(Value);
    }
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class FrequencyLimitsParam : GH_Param<FrequencyLimitsGoo> {
    public override Guid ComponentGuid => new Guid("becd58f8-ab27-4e5f-944c-f7b9806c8e73");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + FrequencyLimitsGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? FrequencyLimitsGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.FrequencyLimit;

    public FrequencyLimitsParam()
                                          : base(new GH_InstanceDescription(
    FrequencyLimitsGoo.Name,
    FrequencyLimitsGoo.NickName,
    FrequencyLimitsGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
