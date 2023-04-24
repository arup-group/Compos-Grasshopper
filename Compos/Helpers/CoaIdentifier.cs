namespace ComposAPI.Helpers {
  internal class CoaIdentifier {
    internal class DesignCode {
      internal const string ASNZ = "AS/NZS2327:2017";
      internal const string BS = "BS5950-3.1:1990+A1:2010";
      internal const string BS_Superseded = "BS5950-3.1:1990 (superseded)";
      internal const string EN = "EN1994-1-1:2004";
      internal const string HKSUOS2005 = "HKSUOS:2005";
      internal const string HKSUOS2011 = "HKSUOS:2011";
    }

    internal class DesignCodes {
      //public const string Force = "FORCE";
      //public const string Length_Geometry = "LENGTH";
      //public const string Length_Results = "DISP";
      //public const string Length_Section = "SECTION";
      //public const string Stress = "STRESS";
      //public const string Mass = "MASS";
    }

    internal class DesignCriteria {
      internal const string BeamSizeLimit = "CRITERIA_BEAM_SIZE_LIMIT";
      internal const string DeflectionLimit = "CRITERIA_DEF_LIMIT";
      internal const string Frequency = "CRITERIA_FREQUENCY";
      internal const string OptimiseOption = "CRITERIA_OPTIMISE_OPTION";
      internal const string SectionType = "CRITERIA_SECTION_TYPE";
    }

    internal class Loads {
      public const string AxialLoad = "Axial";
      public const string DistributionArea = "Area";
      //public const string MomentLoad = "Moment";
      public const string DistributionLinear = "Line";
      public const string LinearLoad = "Linear";
      public const string MemberLoad = "Member load";
      public const string PatchLoad = "Patch";
      public const string PointLoad = "Point";
      public const string TriLinearLoad = "Tri-Linear";
      public const string UniformLoad = "Uniform";
    }

    internal class StudDimensions {
      public const string StudDefinition = "STUD_DEFINITION";

      public const string StudDimensionCustom = "USER_DEFINED";
      public const string StudDimensionStandard = "STANDARD";
      public const string StudGradeEC4 = "EC4_STUD_GRADE";
      public const string StudGradeEC4Custom = "CODE_GRADE_NO";
      public const string StudGradeEC4Standard = "CODE_GRADE_YES";
    }

    internal class StudGroupSpacings {
      public const string StudLayout = "STUD_LAYOUT";

      public const string StudLayoutAutomatic = "AUTO_100";
      public const string StudLayoutCheckCustom = "CHECK_SPACE_YES";
      public const string StudLayoutCustom = "USER_DEFINED";
      public const string StudLayoutMin_Num_of_Studs = "AUTO_MINIMUM_STUD";
      public const string StudLayoutPartial_Interaction = "AUTO_PERCENT";
    }

    internal class StudSpecifications {
      public const string StudEC4 = "STUD_EC4_APPLY";
      public const string StudNCCI = "STUD_NCCI_LIMIT_APPLY";
      public const string StudNoZone = "STUD_NO_STUD_ZONE";
      public const string StudReinfPos = "STUD_EC4_RFT_POS";
    }

    internal class Units {
      public const string Displacement = "DISP";
      public const string Force = "FORCE";
      public const string Length = "LENGTH";
      public const string Mass = "MASS";
      public const string Section = "SECTION";
      public const string Stress = "STRESS";
    }

    internal const string BeamSectionAtX = "BEAM_SECTION_AT_X";
    internal const string BeamSpanLength = "BEAM_SPAN_LENGTH";
    internal const string BeamSteelMaterialStandard = "BEAM_STEEL_MATERIAL_STD";
    internal const string BeamSteelMaterialUser = "BEAM_STEEL_MATERIAL_USER";
    internal const string BeamWeldingMaterial = "BEAM_WELDING_MATERIAL";
    internal const string DeckingCatalogue = "DECKING_CATALOGUE";
    internal const string DeckingUser = "DECKING_USER";
    internal const string DesignOption = "DESIGN_OPTION";
    internal const string EC4DesignOption = "EC4_DESIGN_OPTION";
    internal const string EC4LoadCombinationFactors = "EC4_LOAD_COMB_FACTORS";
    internal const string EndFlangeFreeRotate = "END_FLANGE_FREE_ROTATE";
    internal const string FinalEndFlangeFreeRotate = "FINAL_END_FLANGE_FREE_ROTATE";
    internal const string FinalRestraint2ndBeam = "FINAL_RESTRAINT_2ND_BEAM";
    internal const string FinalRestraintNoStud = "FINAL_RESTRAINT_NOSTUD";
    internal const string FinalRestraintPoint = "FINAL_RESTRAINT_POINT";
    internal const string Load = "LOAD";
    internal const string MemberName = "MEMBER_TITLE";
    internal const string MemberTitle = "MEMBER_TITLE";
    internal const string RebarMaterial = "REBAR_MATERIAL";
    internal const string RebarMesh = "REBAR_WESH";
    internal const string RebarTransverse = "REBAR_TRANSVERSE";
    internal const string Restraint2ndBeam = "RESTRAINT_2ND_BEAM";
    internal const string RestraintTopFlange = "RESTRAINT_TOP_FALNGE";
    internal const string RetraintPoint = "RESTRAINT_POINT";
    internal const string SafetyFactorLoad = "SAFETY_FACTOR_LOAD";
    internal const string SafetyFactorMaterial = "SAFETY_FACTOR_MATERIAL";
    internal const string SlabConcreteMaterial = "SLAB_CONCRETE_MATERIAL";
    internal const string SlabDimension = "SLAB_DIMENSION";
    internal const string UnitData = "UNIT_DATA";
    internal const string WebOpeningDimension = "WEB_OPEN_DIMENSION";
  }
}
