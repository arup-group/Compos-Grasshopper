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
    internal const string DesignCode = "DESIGN_OPTION";
    internal class DesignCodes
    {
      //public const string Force = "FORCE";
      //public const string Length_Geometry = "LENGTH";
      //public const string Length_Results = "DISP";
      //public const string Length_Section = "SECTION";
      //public const string Stress = "STRESS";
      //public const string Mass = "MASS";
    }

    internal const string MemberName = "MEMBER_TITLE";

    internal const string BeamSectionAtX = "BEAM_SECTION_AT_X";
    internal const string BeamSpanLength = "BEAM_SPAN_LENGTH";

    internal const string BeamSteelMaterialStandard = "BEAM_STEEL_MATERIAL_STD";
    internal const string BeamSteelMaterialUser = "BEAM_STEEL_MATERIAL_USER";
    internal const string BeamWeldingMaterial = "BEAM_WELDING_MATERIAL";

    internal const string DeckingCatalogue = "DECKING_CATALOGUE";
    internal const string DeckingUser = "DECKING_USER";
    internal const string DesignOption = "DESIGN_OPTION";
    internal const string MemberTitle = "MEMBER_TITLE";
    internal const string RebarTransverse = "REBAR_TRANSVERSE";
    internal const string RebarWesh = "REBAR_WESH";
    internal const string RebarMaterial = "REBAR_MATERIAL";

    internal const string RetraintPoint = "RESTRAINT_POINT";
    internal const string RestraintTopFlange = "RESTRAINT_TOP_FALNGE";
    internal const string Restraint2ndBeam = "RESTRAINT_2ND_BEAM";
    internal const string EndFlangeFreeRotate = "END_FLANGE_FREE_ROTATE";
    internal const string FinalRestraintPoint = "FINAL_RESTRAINT_POINT";
    internal const string FinalRestraintNoStud = "FINAL_RESTRAINT_NOSTUD";
    internal const string FinalRestraint2ndBeam = "FINAL_RESTRAINT_2ND_BEAM";
    internal const string FinalEndFlangeFreeRotate = "FINAL_END_FLANGE_FREE_ROTATE";


    internal const string SlabConcreteMaterial = "SLAB_CONCRETE_MATERIAL";
    internal const string SlabDimension = "SLAB_DIMENSION";

    internal const string WebOpeningDimension = "WEB_OPEN_DIMENSION";

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

    internal class StudGroupSpacings
    {
      public const string StudLayout = "STUD_LAYOUT";

      public const string StudLayoutAutomatic = "AUTO_100";
      public const string StudLayoutPartial_Interaction = "AUTO_PERCENT";
      public const string StudLayoutMin_Num_of_Studs = "AUTO_MINIMUM_STUD";
      public const string StudLayoutCustom = "USER_DEFINED";
      public const string StudLayoutCheckCustom = "CHECK_SPACE_YES";
    }
    internal class StudDimensions
    {
      public const string StudDefinition = "STUD_DEFINITION";

      public const string StudDimensionStandard = "STANDARD";
      public const string StudDimensionCustom = "USER_DEFINED";

      public const string StudGradeEC4 = "EC4_STUD_GRADE";
      public const string StudGradeEC4Standard = "CODE_GRADE_YES";
      public const string StudGradeEC4Custom = "CODE_GRADE_NO";
    }

    internal class StudSpecifications
    {
      public const string StudNoZone = "STUD_NO_STUD_ZONE";
      public const string StudEC4 = "STUD_EC4_APPLY";
      public const string StudNCCI = "STUD_NCCI_LIMIT_APPLY";
      public const string StudReinfPos = "STUD_EC4_RFT_POS";
    }
  }
}
