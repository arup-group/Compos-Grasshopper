namespace ComposAPI {
  public class DesignOption : IDesignOption {
    public bool ConsiderShearDeflection { get; set; } = false;
    public bool InclConcreteSlabWeight { get; set; } = false;
    public bool InclSteelBeamWeight { get; set; } = false;
    // include beam weight or not in analysis
    //	include slab weight or not in analysis
    //	shear deformation
    public bool InclThinFlangeSections { get; set; } = false;
    public bool ProppedDuringConstruction { get; set; } = true; // construction type
                                                                // include thin flange section or not in the selection of steel beams in design

    public DesignOption() {
      // default initialiser
    }
  }
}
