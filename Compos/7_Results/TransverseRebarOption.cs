namespace ComposAPI
{
  public enum TransverseRebarOption
  {
    REBAR_DIST_LEFT_SIDE, // Rebar starting position
    REBAR_DIST_RIGHT_SIDE, // Rebar ending position
    REBAR_DIAMETER, // Rebar diameter
    REBAR_INTERVAL, // Rebar interval
    REBAR_COVER, // Rebar cover
    REBAR_AREA, // Rebar area per meter
    REBAR_CRITI_DIST, // Critical transverse shear position
    REBAR_CRITI_SURFACE, // Failure surface, 1: a-a section; 2:b-b section; 5: e-e section
    REBAR_CRITI_PERI, // Effective perimeter
    REBAR_CRITI_ACTUAL_SHEAR, // Transverse shear force
    REBAR_CRITI_SHEAR_CONC, // Concrete shear resistance
    REBAR_CRITI_SHEAR_DECK, // Decking shear resistance
    REBAR_CRITI_SHEAR_MESH, // Mesh bar shear resistance
    REBAR_CRITI_SHEAR_REBAR, // Rebar shear resistance
    REBAR_CRITI_SHEAR_MAX_ALLOW, // Maximum allowable shear resistance
  }
}
