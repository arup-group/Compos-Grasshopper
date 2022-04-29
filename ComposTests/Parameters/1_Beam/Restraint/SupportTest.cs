using Xunit;
using UnitsNet;
using UnitsNet.Units;
using System.Collections.Generic;
using ComposAPI.SteelBeam;

namespace ComposAPI.Tests
{
  public partial class RestraintTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(Supports.IntermediateRestraint.None, true, false)]
    [InlineData(Supports.IntermediateRestraint.Mid__Span, true, true)]
    [InlineData(Supports.IntermediateRestraint.Third_Points, false, false)]
    [InlineData(Supports.IntermediateRestraint.Quarter_Points, false, true)]
    public static Supports TestSupportConstructor(Supports.IntermediateRestraint intermediateRestraint, bool secondaryMemberIntermediateRestraint, bool bothFlangesFreeToRotateOnPlanAtEnds)
    {

      // 2 create object instance with constructor
      Supports sup = new Supports(intermediateRestraint, secondaryMemberIntermediateRestraint, bothFlangesFreeToRotateOnPlanAtEnds);

      // 3 check that inputs are set in object's members
      Assert.Equal(intermediateRestraint, sup.IntermediateRestraintPositions);
      Assert.Equal(secondaryMemberIntermediateRestraint, sup.SecondaryMemberIntermediateRestraint);
      Assert.Equal(bothFlangesFreeToRotateOnPlanAtEnds, sup.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Null(sup.CustomIntermediateRestraintPositions);

      return sup;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(100, 500, 1500, true, false)]
    public Supports TestSupportConstructorCustom(double val1, double val2, double val3, bool secondaryMemberIntermediateRestraint, bool bothFlangesFreeToRotateOnPlanAtEnds)
    {
      LengthUnit unit = LengthUnit.Millimeter;
      List<Length> customIntermediateRestraintPositions = new List<Length>();
      customIntermediateRestraintPositions.Add(new Length(val1, unit));
      customIntermediateRestraintPositions.Add(new Length(val2, unit));
      customIntermediateRestraintPositions.Add(new Length(val3, unit));

      // 2 create object instance with constructor
      Supports sup = new Supports(customIntermediateRestraintPositions, secondaryMemberIntermediateRestraint, bothFlangesFreeToRotateOnPlanAtEnds);

      // 3 check that inputs are set in object's members
      Assert.Equal(val1, sup.CustomIntermediateRestraintPositions[0].Millimeters);
      Assert.Equal(val2, sup.CustomIntermediateRestraintPositions[1].Millimeters);
      Assert.Equal(val3, sup.CustomIntermediateRestraintPositions[2].Millimeters);
      Assert.Equal(secondaryMemberIntermediateRestraint, sup.SecondaryMemberIntermediateRestraint);
      Assert.Equal(bothFlangesFreeToRotateOnPlanAtEnds, sup.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(Supports.IntermediateRestraint.Custom, sup.IntermediateRestraintPositions);

      return sup;
    }

    [Fact]
    public void TestSupportDuplicate()
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1 create with constructor and duplicate
      Supports original = TestSupportConstructor(Supports.IntermediateRestraint.None, true, false);
      Supports duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(Supports.IntermediateRestraint.None, duplicate.IntermediateRestraintPositions);
      Assert.True(duplicate.SecondaryMemberIntermediateRestraint);
      Assert.False(duplicate.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Null(duplicate.CustomIntermediateRestraintPositions);

      // 3 make some changes to duplicate
      duplicate.SecondaryMemberIntermediateRestraint = false;
      duplicate.BothFlangesFreeToRotateOnPlanAtEnds = true;
      duplicate.IntermediateRestraintPositions = Supports.IntermediateRestraint.Mid__Span;

      // 4 check that duplicate has set changes
      Assert.Equal(Supports.IntermediateRestraint.Mid__Span, duplicate.IntermediateRestraintPositions);
      Assert.False(duplicate.SecondaryMemberIntermediateRestraint);
      Assert.True(duplicate.BothFlangesFreeToRotateOnPlanAtEnds);

      // 5 check that original has not been changed
      Assert.Equal(Supports.IntermediateRestraint.None, original.IntermediateRestraintPositions);
      Assert.True(original.SecondaryMemberIntermediateRestraint);
      Assert.False(original.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Null(original.CustomIntermediateRestraintPositions);

      // 1 create with new constructor and duplicate
      original = TestSupportConstructorCustom(1, 2, 3, false, true);
      duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(Supports.IntermediateRestraint.Custom, duplicate.IntermediateRestraintPositions);
      Assert.False(duplicate.SecondaryMemberIntermediateRestraint);
      Assert.True(duplicate.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(1, duplicate.CustomIntermediateRestraintPositions[0].Millimeters);
      Assert.Equal(2, duplicate.CustomIntermediateRestraintPositions[1].Millimeters);
      Assert.Equal(3, duplicate.CustomIntermediateRestraintPositions[2].Millimeters);

      // 3 make some changes to duplicate
      duplicate.SecondaryMemberIntermediateRestraint = true;
      duplicate.BothFlangesFreeToRotateOnPlanAtEnds = false;
      List<Length> customIntermediateRestraintPositions = new List<Length>();
      customIntermediateRestraintPositions.Add(new Length(4, unit));
      customIntermediateRestraintPositions.Add(new Length(5, unit));
      customIntermediateRestraintPositions.Add(new Length(6, unit));
      customIntermediateRestraintPositions.Add(new Length(7, unit));
      duplicate.CustomIntermediateRestraintPositions = customIntermediateRestraintPositions;

      // 4 check that duplicate has set changes
      Assert.Equal(Supports.IntermediateRestraint.Custom, duplicate.IntermediateRestraintPositions);
      Assert.True(duplicate.SecondaryMemberIntermediateRestraint);
      Assert.False(duplicate.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(4, duplicate.CustomIntermediateRestraintPositions[0].Millimeters);
      Assert.Equal(5, duplicate.CustomIntermediateRestraintPositions[1].Millimeters);
      Assert.Equal(6, duplicate.CustomIntermediateRestraintPositions[2].Millimeters);
      Assert.Equal(7, duplicate.CustomIntermediateRestraintPositions[3].Millimeters);

      // 5 check that original has not been changed
      Assert.Equal(Supports.IntermediateRestraint.Custom, original.IntermediateRestraintPositions);
      Assert.False(original.SecondaryMemberIntermediateRestraint);
      Assert.True(original.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(1, original.CustomIntermediateRestraintPositions[0].Millimeters);
      Assert.Equal(2, original.CustomIntermediateRestraintPositions[1].Millimeters);
      Assert.Equal(3, original.CustomIntermediateRestraintPositions[2].Millimeters);
    }
  }
}
