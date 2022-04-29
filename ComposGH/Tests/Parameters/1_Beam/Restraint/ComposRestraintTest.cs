using Xunit;
using UnitsNet;
using UnitsNet.Units;
using System.Collections.Generic;

namespace ComposGH.Parameters.Tests
{
  public partial class ComposRestraintTest
  {
    // 1 setup inputs
    public static Supports construction = TestSupportConstructor(Supports.IntermediateRestraint.Mid__Span, false, false);
    Supports final = TestSupportConstructor(Supports.IntermediateRestraint.None, true, true);
    [Fact]
    public ComposRestraint TestConstructor()
    {

      // 2 create object instance with constructor
      ComposRestraint restraint = new ComposRestraint(true, construction, final);

      // 3 check that inputs are set in object's members
      Assert.True(restraint.TopFlangeRestrained);
      Assert.Equal(Supports.IntermediateRestraint.Mid__Span, restraint.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(restraint.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(restraint.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Equal(Supports.IntermediateRestraint.None, restraint.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(restraint.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(restraint.FinalStageSupports.SecondaryMemberIntermediateRestraint);
      return restraint;
    }

    [Fact]
    public ComposRestraint TestConstructorNoFinalSupports()
    {
      // 1 setup inputs
      Supports construction = TestSupportConstructor(Supports.IntermediateRestraint.Mid__Span, true, false);

      // 2 create object instance with constructor
      ComposRestraint restraint = new ComposRestraint(false, construction);

      // 3 check that inputs are set in object's members
      Assert.False(restraint.TopFlangeRestrained);
      Assert.Equal(Supports.IntermediateRestraint.Mid__Span, restraint.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(restraint.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(restraint.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Null(restraint.FinalStageSupports);
      return restraint;
    }

    [Fact]
    public void TestDuplicate()
    {
      // 1 create with constructor and duplicate
      ComposRestraint original = TestConstructor();
      ComposRestraint duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.True(duplicate.TopFlangeRestrained);
      Assert.Equal(Supports.IntermediateRestraint.Mid__Span, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(duplicate.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Equal(Supports.IntermediateRestraint.None, duplicate.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(duplicate.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.FinalStageSupports.SecondaryMemberIntermediateRestraint);

      // 3 make some changes to duplicate
      duplicate.TopFlangeRestrained = false;
      duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds = true;
      duplicate.ConstructionStageSupports.SecondaryMemberIntermediateRestraint = true;
      duplicate.ConstructionStageSupports.IntermediateRestraintPositions = Supports.IntermediateRestraint.None;
      duplicate.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds = false;
      duplicate.FinalStageSupports.SecondaryMemberIntermediateRestraint = false;
      duplicate.FinalStageSupports.IntermediateRestraintPositions = Supports.IntermediateRestraint.Mid__Span;

      // 4 check that duplicate has set changes
      Assert.False(duplicate.TopFlangeRestrained);
      Assert.Equal(Supports.IntermediateRestraint.None, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.True(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Equal(Supports.IntermediateRestraint.Mid__Span, duplicate.FinalStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(duplicate.FinalStageSupports.SecondaryMemberIntermediateRestraint);

      // 5 check that original has not been changed
      Assert.True(original.TopFlangeRestrained);
      Assert.Equal(Supports.IntermediateRestraint.Mid__Span, original.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(original.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.False(original.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Equal(Supports.IntermediateRestraint.None, original.FinalStageSupports.IntermediateRestraintPositions);
      Assert.True(original.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(original.FinalStageSupports.SecondaryMemberIntermediateRestraint);

      // 1 create with new constructor and duplicate
      original = TestConstructorNoFinalSupports();
      duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.False(duplicate.TopFlangeRestrained);
      Assert.Equal(Supports.IntermediateRestraint.Mid__Span, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Null(duplicate.FinalStageSupports);

      // 3 make some changes to duplicate
      duplicate.TopFlangeRestrained = true;
      duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds = false;
      duplicate.ConstructionStageSupports.SecondaryMemberIntermediateRestraint = true;
      duplicate.ConstructionStageSupports.IntermediateRestraintPositions = Supports.IntermediateRestraint.Third_Points;

      // 4 check that duplicate has set changes
      Assert.True(duplicate.TopFlangeRestrained);
      Assert.Equal(Supports.IntermediateRestraint.Third_Points, duplicate.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(duplicate.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(duplicate.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Null(duplicate.FinalStageSupports);

      // 5 check that original has not been changed
      Assert.False(original.TopFlangeRestrained);
      Assert.Equal(Supports.IntermediateRestraint.Mid__Span, original.ConstructionStageSupports.IntermediateRestraintPositions);
      Assert.False(original.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.True(original.ConstructionStageSupports.SecondaryMemberIntermediateRestraint);
      Assert.Null(original.FinalStageSupports);
    }
  }
}
