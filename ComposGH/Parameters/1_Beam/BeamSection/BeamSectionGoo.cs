using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IBeamSection"/> class can be used in Grasshopper.
  /// </summary>
  public class BeamSectionGoo : GH_OasysGoo<IBeamSection>
  {
    public static string Name => "Beam Section";
    public static string NickName => "Bs";
    public static string Description => "Compos Beam Section.";
    public BeamSectionGoo(IBeamSection item) : base(item) { }
    public override IGH_Goo Duplicate() => new BeamSectionGoo(this.Value);
  }
}
