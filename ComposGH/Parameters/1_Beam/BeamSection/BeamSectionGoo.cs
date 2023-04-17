using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using System;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IBeamSection"/> class can be used in Grasshopper.
  /// </summary>
  public class BeamSectionGoo : GH_OasysGoo<IBeamSection> {
    public static string Description => "Compos Beam Section";
    public static string Name => "Beam Section";
    public static string NickName => "Bs";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public BeamSectionGoo(IBeamSection item) : base(item) { }

    public BeamSectionGoo(string item) : base(new BeamSection(item)) { }

    public override bool CastFrom(object source) {
      if (source == null) {
        return false;
      }

      if (typeof(IBeamSection).IsAssignableFrom(source.GetType())) {
        Value = (IBeamSection)source;
        return true;
      }

      //Cast from string
      if (GH_Convert.ToString(source, out string name, GH_Conversion.Both)) {
        Value = new BeamSection(name);
        return true;
      }

      return false;
    }

    public override bool CastTo<Q>(ref Q target) {
      if (typeof(Q).IsAssignableFrom(typeof(IBeamSection))) {
        if (Value == null) {
          target = default(Q);
        }
        else {
          target = (Q)(object)Value;
        }

        return true;
      }

      //Cast to string
      if (typeof(Q).IsAssignableFrom(typeof(string))) {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.SectionDescription;
        return true;
      }

      target = default(Q);
      return false;
    }

    public override IGH_Goo Duplicate() => new BeamSectionGoo(Value);
  }

  public class BeamSectionParam : GH_Param<BeamSectionGoo> {
    public override Guid ComponentGuid => new Guid("ed384b64-f147-483e-ae3d-a294016a90c3");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + BeamSectionGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? BeamSectionGoo.Name : base.TypeName;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.BeamSectionParam;

    public BeamSectionParam()
      : base(new GH_InstanceDescription(
        BeamSectionGoo.Name,
        BeamSectionGoo.NickName,
        BeamSectionGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
