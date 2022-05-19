using Xunit;
using UnitsNet;
using UnitsNet.Units;
using System.Collections.Generic;
using ComposAPI.Helpers;

namespace ComposAPI.Tests
{
  public partial class RestraintTest
  {
    // 1 setup inputs
    Supports construction = TestSupportConstructor(IntermediateRestraint.Mid__Span, false, false);
    Supports final = TestSupportConstructor(IntermediateRestraint.None, true, true);
    [Fact]
    public Restraint TestConstructor()
    {

      // 2 create object instance with constructor
      Restraint restraint = new Restraint(true, construction, final);

      // 3 check that inputs are set in object's members
      Assert.True(restraint.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, restraint.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(restraint.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(restraint.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.None, restraint.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(restraint.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(restraint.FinalStageSupports.SecondaryMemberIntermediateRestraint);
      return restraint;
    }

    [Fact]
    public Restraint TestConstructorNoFinalSupports()
    {
      // 1 setup inputs
      Supports construction = TestSupportConstructor(IntermediateRestraint.Mid__Span, true, false);

      // 2 create object instance with constructor
      Restraint restraint = new Restraint(false, construction);

      // 3 check that inputs are set in object's members
      Assert.False(restraint.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, restraint.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(restraint.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(restraint.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Null(restraint.FinalStageSupports);
      return restraint;
    }

    [Fact]
    public void DuplicateTest()
    {
      // 1 create with constructor and duplicate
      Restraint original = TestConstructor();
      Restraint duplicate = original.Duplicate() as Restraint;

      // 2 check that duplicate has duplicated values
      Assert.True(duplicate.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(duplicate.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.None, duplicate.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(duplicate.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.FinalStageSupports.SecondaryMemberIntermediateRestraint);

      // 3 make some changes to duplicate
      duplicate.TopFlangeRestrained = false;
      Supports constructionStageSupports = duplicate.ConstructionStageSupports as Supports;
      constructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds = true;
      constructionStageSupports.SecondaryMemberIntermediateRestraint = true;
      constructionStageSupports.IntermediateRestraintPositions = IntermediateRestraint.None;
      Supports finalStageSupports = duplicate.FinalStageSupports as Supports;
      finalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds = false;
      finalStageSupports.SecondaryMemberIntermediateRestraint = false;
      finalStageSupports.IntermediateRestraintPositions = IntermediateRestraint.Mid__Span;

      // 4 check that duplicate has set changes
      Assert.False(duplicate.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.None, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.True(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.Mid__Span, duplicate.FinalStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(duplicate.FinalStageSupports.SecondaryMemberIntermediateRestraint);

      // 5 check that original has not been changed
      Assert.True(original.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, original.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(original.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(original.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.None, original.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(original.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(original.FinalStageSupports.SecondaryMemberIntermediateRestraint);
    }

    [Fact]
    public void DuplicateTest2()
    {
      // 1 create with constructor and duplicate
      Restraint original = TestConstructorNoFinalSupports();
      Restraint duplicate = original.Duplicate() as Restraint;

      // 2 check that duplicate has duplicated values
      Assert.False(duplicate.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Null(duplicate.FinalStageSupports);

      // 3 make some changes to duplicate
      duplicate.TopFlangeRestrained = true;
      Supports constructionStageSupports2 = duplicate.ConstructionStageSupports as Supports;
      constructionStageSupports2.BothFlangesFreeToRotateOnPlanAtEnds = false;
      constructionStageSupports2.SecondaryMemberIntermediateRestraint = true;
      constructionStageSupports2.IntermediateRestraintPositions = IntermediateRestraint.Third_Points;

      // 4 check that duplicate has set changes
      Assert.True(duplicate.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Third_Points, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Null(duplicate.FinalStageSupports);

      // 5 check that original has not been changed
      Assert.False(original.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, original.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(original.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(original.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Null(original.FinalStageSupports);
    }

    [Theory]
    [InlineData(true, IntermediateRestraint.None, true, true, false, IntermediateRestraint.None, true, true,
"RESTRAINT_POINT	MEMBER-1	STANDARD	0\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FIXED\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	0\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FIXED\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n")]
    [InlineData(false, IntermediateRestraint.Third_Points, false, false, true, IntermediateRestraint.None, true, true,
"RESTRAINT_POINT	MEMBER-1	STANDARD	2\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FREE\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	0\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n")]
    [InlineData(true, IntermediateRestraint.None, true, true, true, IntermediateRestraint.Mid__Span, false, false,
"RESTRAINT_POINT	MEMBER-1	STANDARD	0\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FIXED\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	1\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE\n")]

    public void StandardToCoaStringTest(bool topFlangeRestrained, IntermediateRestraint CSintermediateRestraintPositions, bool CSsecondaryMemberIntermediateRestraint, bool CSbothFlangesFreeToRotateOnPlanAtEnds, bool setFinal, IntermediateRestraint FSintermediateRestraintPositions, bool FSsecondaryMemberIntermediateRestraint, bool FSbothFlangesFreeToRotateOnPlanAtEnds, string expected_coaString)
    {
      Supports construction = TestSupportConstructor(CSintermediateRestraintPositions, CSsecondaryMemberIntermediateRestraint, CSbothFlangesFreeToRotateOnPlanAtEnds);
      Supports final = TestSupportConstructor(FSintermediateRestraintPositions, FSsecondaryMemberIntermediateRestraint, FSbothFlangesFreeToRotateOnPlanAtEnds);
      IRestraint restraint = (setFinal) ? new Restraint(topFlangeRestrained, construction, final) : new Restraint(topFlangeRestrained, construction);
      string coaString = restraint.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    [Theory]
    [InlineData(true, IntermediateRestraint.None, true, true, false, IntermediateRestraint.None, true, true,
"RESTRAINT_POINT	MEMBER-1	STANDARD	0\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FIXED\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	0\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FIXED\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n")]
    [InlineData(false, IntermediateRestraint.Third_Points, false, false, true, IntermediateRestraint.None, true, true,
"RESTRAINT_POINT	MEMBER-1	STANDARD	2\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FREE\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	0\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n")]
    [InlineData(true, IntermediateRestraint.None, true, true, true, IntermediateRestraint.Mid__Span, false, false,
"RESTRAINT_POINT	MEMBER-1	STANDARD	0\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FIXED\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	1\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE\n")]

    public void StandardFromCoaStringTest(bool expected_topFlangeRestrained, IntermediateRestraint expected_CSintermediateRestraintPositions, bool expected_CSsecondaryMemberIntermediateRestraint, bool expected_CSbothFlangesFreeToRotateOnPlanAtEnds, bool expected_setFinal, IntermediateRestraint expected_FSintermediateRestraintPositions, bool expected_FSsecondaryMemberIntermediateRestraint, bool expected_FSbothFlangesFreeToRotateOnPlanAtEnds, string coaString)
    {
      List<string> lines = CoaHelper.SplitLines(coaString);
      Restraint restraint = new Restraint();
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);
        restraint.FromCoaString(parameters, ComposUnits.GetStandardUnits());
      }

      Assert.Equal(expected_topFlangeRestrained, restraint.TopFlangeRestrained);
      Assert.Equal(expected_CSintermediateRestraintPositions, restraint.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.Equal(expected_CSsecondaryMemberIntermediateRestraint, restraint.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Equal(expected_CSbothFlangesFreeToRotateOnPlanAtEnds, restraint.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(expected_setFinal, restraint.finalSupportsSet);
      Assert.Equal(expected_FSintermediateRestraintPositions, restraint.FinalStageSupports.IntermediateRestraintPositions);
      Assert.Equal(expected_FSsecondaryMemberIntermediateRestraint, restraint.FinalStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Equal(expected_FSbothFlangesFreeToRotateOnPlanAtEnds, restraint.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
    }
  }
}
