using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGH.Helpers;
using ComposGHTests.Helpers;
using OasysGH;
using Xunit;

namespace ComposAPI.Beams.Tests {
  [Collection("ComposAPI Fixture collection")]
  public partial class RestraintTest {
    // 1 setup inputs
    private Supports construction = TestSupportConstructor(IntermediateRestraint.Mid__Span, false, false);
    private Supports final = TestSupportConstructor(IntermediateRestraint.None, true, true);

    [Theory]
    [InlineData(false, -0.05, 9000, 10000, true, false, true, -0.1, 7000, 9000, true, true,
"RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	5.00000%\n" +
"RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	2	9.00000\n" +
"RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	3	10.0000\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-2	TOP_FLANGE_FREE\n" +
"RESTRAINT_2ND_BEAM	MEMBER-2	SEC_BEAM_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-2	NOT_FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	10.0000%\n" +
"FINAL_RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	2	7.00000\n" +
"FINAL_RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	3	9.00000\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-2	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-2	SEC_BEAM_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-2	FREE_TO_ROTATE\n")]
    [InlineData(false, -0.035, 6000, 11000, false, true, true, -0.01, 4000, 12000, false, false,
"RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	3.50000%\n" +
"RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	2	6.00000\n" +
"RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	3	11.0000\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-2	TOP_FLANGE_FREE\n" +
"RESTRAINT_2ND_BEAM	MEMBER-2	2ND_BEAM_NOT_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-2	FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	1.00000%\n" +
"FINAL_RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	2	4.00000\n" +
"FINAL_RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	3	12.0000\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-2	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-2	2ND_BEAM_NOT_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-2	NOT_FREE_TO_ROTATE\n")]
    public void CustomToCoaStringTest(bool topFlangeRestrained,
      double CSintermediateRestraintPosition1, double CSintermediateRestraintPosition2, double CSintermediateRestraintPosition3, bool CSsecondaryMemberIntermediateRestraint,
      bool CSbothFlangesFreeToRotateOnPlanAtEnds, bool setFinal,
      double FSintermediateRestraintPosition1, double FSintermediateRestraintPosition2, double FSintermediateRestraintPosition3, bool FSsecondaryMemberIntermediateRestraint, bool FSbothFlangesFreeToRotateOnPlanAtEnds, string expected_coaString) {
      Supports construction = TestSupportConstructorCustom(CSintermediateRestraintPosition1, CSintermediateRestraintPosition2, CSintermediateRestraintPosition3, CSsecondaryMemberIntermediateRestraint, CSbothFlangesFreeToRotateOnPlanAtEnds);
      Supports final = TestSupportConstructorCustom(FSintermediateRestraintPosition1, FSintermediateRestraintPosition2, FSintermediateRestraintPosition3, FSsecondaryMemberIntermediateRestraint, FSbothFlangesFreeToRotateOnPlanAtEnds);
      IRestraint restraint = setFinal ? new Restraint(topFlangeRestrained, construction, final) : new Restraint(topFlangeRestrained, construction);
      string coaString = restraint.ToCoaString("MEMBER-2", ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      Restraint original = TestConstructorNoFinalSupports();
      var duplicate = (Restraint)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void DuplicateTest1() {
      // 1 create with constructor and duplicate
      Restraint original = TestConstructor();
      var duplicate = (Restraint)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(original.ToString(), duplicate.ToString());
      Assert.True(duplicate.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(duplicate.ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.None, duplicate.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(duplicate.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.FinalStageSupports.SecondaryMemberAsIntermediateRestraint);

      // 3 make some changes to duplicate
      duplicate.TopFlangeRestrained = false;
      var constructionStageSupports = (Supports)duplicate.ConstructionStageSupports;
      constructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds = true;
      constructionStageSupports.SecondaryMemberAsIntermediateRestraint = true;
      constructionStageSupports.IntermediateRestraintPositions = IntermediateRestraint.None;
      var finalStageSupports = (Supports)duplicate.FinalStageSupports;
      finalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds = false;
      finalStageSupports.SecondaryMemberAsIntermediateRestraint = false;
      finalStageSupports.IntermediateRestraintPositions = IntermediateRestraint.Mid__Span;

      // 4 check that duplicate has set changes
      Assert.False(duplicate.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.None, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.True(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.Mid__Span, duplicate.FinalStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(duplicate.FinalStageSupports.SecondaryMemberAsIntermediateRestraint);

      // 5 check that original has not been changed
      Assert.True(original.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, original.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(original.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(original.ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.None, original.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(original.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(original.FinalStageSupports.SecondaryMemberAsIntermediateRestraint);
    }

    [Fact]
    public void DuplicateTest2() {
      // 1 create with constructor and duplicate
      Restraint original = TestConstructorNoFinalSupports();
      var duplicate = (Restraint)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.False(duplicate.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.None, duplicate.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(duplicate.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.FinalStageSupports.SecondaryMemberAsIntermediateRestraint);

      // 3 make some changes to duplicate
      duplicate.TopFlangeRestrained = true;
      var constructionStageSupports2 = (Supports)duplicate.ConstructionStageSupports;
      constructionStageSupports2.BothFlangesFreeToRotateOnPlanAtEnds = false;
      constructionStageSupports2.SecondaryMemberAsIntermediateRestraint = true;
      constructionStageSupports2.IntermediateRestraintPositions = IntermediateRestraint.Third_Points;
      var finalStageSupports2 = (Supports)duplicate.FinalStageSupports;
      finalStageSupports2.BothFlangesFreeToRotateOnPlanAtEnds = false;
      finalStageSupports2.SecondaryMemberAsIntermediateRestraint = false;
      finalStageSupports2.IntermediateRestraintPositions = IntermediateRestraint.Mid__Span;

      // 4 check that duplicate has set changes
      Assert.True(duplicate.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Third_Points, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.Mid__Span, duplicate.FinalStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(duplicate.FinalStageSupports.SecondaryMemberAsIntermediateRestraint);

      // 5 check that original has not been changed
      Assert.False(original.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, original.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(original.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(original.ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.None, original.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(original.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(original.FinalStageSupports.SecondaryMemberAsIntermediateRestraint);
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
    [InlineData(false, IntermediateRestraint.Mid__Span, true, true, true, IntermediateRestraint.Third_Points, false, false,
"RESTRAINT_POINT	MEMBER-1	STANDARD	1\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FREE\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	2\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE\n")]
    [InlineData(false, IntermediateRestraint.Third_Points, true, true, true, IntermediateRestraint.Quarter_Points, false, false,
"RESTRAINT_POINT	MEMBER-1	STANDARD	2\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FREE\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	3\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE\n")]
    [InlineData(false, IntermediateRestraint.Quarter_Points, true, true, true, IntermediateRestraint.None, false, false,
"RESTRAINT_POINT	MEMBER-1	STANDARD	3\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FREE\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	0\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE\n")]
    public void StandardFromCoaStringTest(bool expected_topFlangeRestrained, IntermediateRestraint expected_CSintermediateRestraintPositions, bool expected_CSsecondaryMemberIntermediateRestraint, bool expected_CSbothFlangesFreeToRotateOnPlanAtEnds, bool expected_setFinal, IntermediateRestraint expected_FSintermediateRestraintPositions, bool expected_FSsecondaryMemberIntermediateRestraint, bool expected_FSbothFlangesFreeToRotateOnPlanAtEnds, string coaString) {
      List<string> lines = CoaHelper.SplitLines(coaString);
      var restraint = new Restraint();
      foreach (string line in lines) {
        List<string> parameters = CoaHelper.Split(line);
        restraint.FromCoaString(parameters, ComposUnits.GetStandardUnits());
      }

      Assert.Equal(expected_topFlangeRestrained, restraint.TopFlangeRestrained);
      Assert.Equal(expected_CSintermediateRestraintPositions, restraint.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.Equal(expected_CSsecondaryMemberIntermediateRestraint, restraint.ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(expected_CSbothFlangesFreeToRotateOnPlanAtEnds, restraint.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(expected_setFinal, restraint.finalSupportsSet);
      Assert.Equal(expected_FSintermediateRestraintPositions, restraint.FinalStageSupports.IntermediateRestraintPositions);
      Assert.Equal(expected_FSsecondaryMemberIntermediateRestraint, restraint.FinalStageSupports.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(expected_FSbothFlangesFreeToRotateOnPlanAtEnds, restraint.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
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
    [InlineData(false, IntermediateRestraint.Mid__Span, false, false, true, IntermediateRestraint.Third_Points, true, true,
"RESTRAINT_POINT	MEMBER-1	STANDARD	1\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FREE\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	2\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n")]
    [InlineData(false, IntermediateRestraint.Third_Points, false, false, true, IntermediateRestraint.Quarter_Points, true, true,
"RESTRAINT_POINT	MEMBER-1	STANDARD	2\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FREE\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	3\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n")]
    [InlineData(false, IntermediateRestraint.Quarter_Points, true, true, true, IntermediateRestraint.Mid__Span, false, false,
"RESTRAINT_POINT	MEMBER-1	STANDARD	3\n" +
"RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FREE\n" +
"RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST\n" +
"END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE\n" +
"FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	1\n" +
"FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE\n" +
"FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST\n" +
"FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE\n")]
    public void StandardToCoaStringTest(bool topFlangeRestrained, IntermediateRestraint CSintermediateRestraintPositions, bool CSsecondaryMemberIntermediateRestraint, bool CSbothFlangesFreeToRotateOnPlanAtEnds, bool setFinal, IntermediateRestraint FSintermediateRestraintPositions, bool FSsecondaryMemberIntermediateRestraint, bool FSbothFlangesFreeToRotateOnPlanAtEnds, string expected_coaString) {
      Supports construction = TestSupportConstructor(CSintermediateRestraintPositions, CSsecondaryMemberIntermediateRestraint, CSbothFlangesFreeToRotateOnPlanAtEnds);
      Supports final = TestSupportConstructor(FSintermediateRestraintPositions, FSsecondaryMemberIntermediateRestraint, FSbothFlangesFreeToRotateOnPlanAtEnds);
      IRestraint restraint = setFinal ? new Restraint(topFlangeRestrained, construction, final) : new Restraint(topFlangeRestrained, construction);
      string coaString = restraint.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public Restraint TestConstructor() {
      // 2 create object instance with constructor
      var restraint = new Restraint(true, construction, final);

      // 3 check that inputs are set in object's members
      Assert.True(restraint.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, restraint.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(restraint.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(restraint.ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(IntermediateRestraint.None, restraint.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(restraint.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(restraint.FinalStageSupports.SecondaryMemberAsIntermediateRestraint);
      return restraint;
    }

    [Fact]
    public Restraint TestConstructorNoFinalSupports() {
      // 1 setup inputs
      Supports construction = TestSupportConstructor(IntermediateRestraint.Mid__Span, true, false);

      // 2 create object instance with constructor
      var restraint = new Restraint(false, construction);

      // 3 check that inputs are set in object's members
      Assert.False(restraint.TopFlangeRestrained);
      Assert.Equal(IntermediateRestraint.Mid__Span, restraint.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(restraint.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(restraint.ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint);

      Assert.Equal(IntermediateRestraint.None, restraint.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(restraint.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(restraint.FinalStageSupports.SecondaryMemberAsIntermediateRestraint);
      return restraint;
    }
  }
}
