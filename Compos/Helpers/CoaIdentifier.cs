namespace ComposAPI.Helpers
{
  internal class CoaIdentifier
  {
    internal const string UnitData = "UNIT_DATA";
    internal class Units
    {
      public const string Force = "FORCE";
      public const string Length_Geometry = "LENGTH";
      public const string Length_Results = "DISP";
      public const string Length_Section = "SECTION";
      public const string Stress = "STRESS";
      public const string Mass = "MASS";
    }

    internal const string MemberName = "MEMBER_TITLE";

    internal const string BeamSectionAtX = "BEAM_SECTION_AT_X";
    internal const string BeamSpanLength = "BEAM_SPAN_LENGTH";
    internal const string DeckingCatalogue = "DECKING_CATALOGUE";
    internal const string DeckingUser = "DECKING_USER";
    internal const string DesignOption = "DESIGN_OPTION";
    internal const string MemberTitle = "MEMBER_TITLE";
    internal const string RebarTransverse = "REBAR_TRANSVERSE";
    internal const string RebarWesh = "REBAR_WESH";
    internal const string SlabConcreteMaterial = "SLAB_CONCRETE_MATERIAL";
    internal const string SlabDimension = "SLAB_DIMENSION";

    internal const string Load = "LOAD";
    internal class Loads
    {
      public const string PointLoad = "Point";
      public const string UniformLoad = "Uniform";
      public const string LinearLoad = "Linear";
      public const string TriLinearLoad = "Tri-Linear";
      public const string PatchLoad = "Patch";
      public const string MemberLoad = "Member load";
      public const string AxialLoad = "Axial";
      //public const string MomentLoad = "Moment";
      public const string DistributionLinear = "Line";
      public const string DistributionArea = "Area";
    }
    
  }
}
