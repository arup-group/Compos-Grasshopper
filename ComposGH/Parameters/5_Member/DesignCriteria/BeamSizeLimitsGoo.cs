﻿using System;
using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IBeamSizeLimits"/> class can be used in Grasshopper.
  /// </summary>
  public class BeamSizeLimitsGoo : GH_OasysGoo<IBeamSizeLimits> {
    public static string Description => "Compos Beam Size Limit Criteria";
    public static string Name => "Beam Size Limits";
    public static string NickName => "BLm";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public BeamSizeLimitsGoo(IBeamSizeLimits item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new BeamSizeLimitsGoo(Value);
    }
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class BeamSizeLimitsParam : GH_Param<BeamSizeLimitsGoo> {
    public override Guid ComponentGuid => new Guid("bef8188b-0874-43c4-94b1-4285fbbdec2e");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + BeamSizeLimitsGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? BeamSizeLimitsGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.BeamSizeLimits;

    public BeamSizeLimitsParam() : base(new GH_InstanceDescription(
      BeamSizeLimitsGoo.Name,
      BeamSizeLimitsGoo.NickName,
      BeamSizeLimitsGoo.Description + " parameter",
      Components.Ribbon.CategoryName.Name(),
      Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
