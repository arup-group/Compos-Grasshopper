using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISteelMaterial"/> class can be used in Grasshopper.
  /// </summary>
  public class SteelMaterialGoo : GH_OasysGoo<ISteelMaterial>
  {
    public static string Name => "Steel Material";
    public static string NickName => "SMt";
    public static string Description => "Compos Steel Material";
    public SteelMaterialGoo(ISteelMaterial item) : base(item) { }
    public override IGH_Goo Duplicate() => new SteelMaterialGoo(this.Value);
  }
}
