using System.Collections.Generic;
using ComposGHTests.Helpers;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Beams.Tests {
  public partial class RestraintTest {

    // 1 setup inputs
    [Theory]
    [InlineData(IntermediateRestraint.None, true, false)]
    [InlineData(IntermediateRestraint.Mid__Span, true, true)]
    [InlineData(IntermediateRestraint.Third_Points, false, false)]
    [InlineData(IntermediateRestraint.Quarter_Points, false, true)]
    public static Supports TestSupportConstructor(IntermediateRestraint intermediateRestraint, bool secondaryMemberIntermediateRestraint, bool bothFlangesFreeToRotateOnPlanAtEnds) {
      // 2 create object instance with constructor
      var sup = new Supports(intermediateRestraint, secondaryMemberIntermediateRestraint, bothFlangesFreeToRotateOnPlanAtEnds);

      // 3 check that inputs are set in object's members
      Assert.Equal(intermediateRestraint, sup.IntermediateRestraintPositions);
      Assert.Equal(secondaryMemberIntermediateRestraint, sup.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(bothFlangesFreeToRotateOnPlanAtEnds, sup.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Null(sup.CustomIntermediateRestraintPositions);

      return sup;
    }

    [Fact]
    public void DuplicateSupportCustomTest() {
      // 1 create with constructor and duplicate
      Supports original = TestSupportConstructorCustom(-0, 4000, -1, false, true);
      var duplicate = (Supports)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void DuplicateSupportTest() {
      // 1 create with constructor and duplicate
      Supports original = TestSupportConstructor(IntermediateRestraint.None, true, false);
      var duplicate = (Supports)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(100, 500, 1500, true, false)]
    public Supports TestSupportConstructorCustom(double val1, double val2, double val3, bool secondaryMemberIntermediateRestraint, bool bothFlangesFreeToRotateOnPlanAtEnds) {
      LengthUnit unit = LengthUnit.Millimeter;
      var customIntermediateRestraintPositions = new List<IQuantity>();
      if (val1 < 0) {
        customIntermediateRestraintPositions.Add(new Ratio(val1 * -1, RatioUnit.DecimalFraction));
      } else {
        customIntermediateRestraintPositions.Add(new Length(val1, unit));
      }
      if (val2 < 0) {
        customIntermediateRestraintPositions.Add(new Ratio(val2 * -1, RatioUnit.DecimalFraction));
      } else {
        customIntermediateRestraintPositions.Add(new Length(val2, unit));
      }
      if (val3 < 0) {
        customIntermediateRestraintPositions.Add(new Ratio(val3 * -1, RatioUnit.DecimalFraction));
      } else {
        customIntermediateRestraintPositions.Add(new Length(val3, unit));
      }

      // 2 create object instance with constructor
      var sup = new Supports(customIntermediateRestraintPositions, secondaryMemberIntermediateRestraint, bothFlangesFreeToRotateOnPlanAtEnds);

      // 3 check that inputs are set in object's members
      if (val1 < 0) {
        Assert.Equal(val1 * -1, sup.CustomIntermediateRestraintPositions[0].As(RatioUnit.DecimalFraction));
      } else {
        Assert.Equal(val1, sup.CustomIntermediateRestraintPositions[0].As(LengthUnit.Millimeter));
      }
      if (val2 < 0) {
        Assert.Equal(val2 * -1, sup.CustomIntermediateRestraintPositions[1].As(RatioUnit.DecimalFraction));
      } else {
        Assert.Equal(val2, sup.CustomIntermediateRestraintPositions[1].As(LengthUnit.Millimeter));
      }
      if (val3 < 0) {
        Assert.Equal(val3 * -1, sup.CustomIntermediateRestraintPositions[2].As(RatioUnit.DecimalFraction));
      } else {
        Assert.Equal(val3, sup.CustomIntermediateRestraintPositions[2].As(LengthUnit.Millimeter));
      }
      Assert.Equal(secondaryMemberIntermediateRestraint, sup.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(bothFlangesFreeToRotateOnPlanAtEnds, sup.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(IntermediateRestraint.Custom, sup.IntermediateRestraintPositions);

      return sup;
    }

    [Fact]
    public void TestSupportDuplicate() {
      // 1 create with constructor and duplicate
      Supports original = TestSupportConstructor(IntermediateRestraint.None, true, false);
      var duplicate = (Supports)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(original.ToString(), duplicate.ToString());
      Assert.Equal(IntermediateRestraint.None, duplicate.IntermediateRestraintPositions);
      Assert.True(duplicate.SecondaryMemberAsIntermediateRestraint);
      Assert.False(duplicate.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Null(duplicate.CustomIntermediateRestraintPositions);

      // 3 make some changes to duplicate
      duplicate.SecondaryMemberAsIntermediateRestraint = false;
      duplicate.BothFlangesFreeToRotateOnPlanAtEnds = true;
      duplicate.IntermediateRestraintPositions = IntermediateRestraint.Mid__Span;

      // 4 check that duplicate has set changes
      Assert.Equal(IntermediateRestraint.Mid__Span, duplicate.IntermediateRestraintPositions);
      Assert.False(duplicate.SecondaryMemberAsIntermediateRestraint);
      Assert.True(duplicate.BothFlangesFreeToRotateOnPlanAtEnds);

      // 5 check that original has not been changed
      Assert.Equal(IntermediateRestraint.None, original.IntermediateRestraintPositions);
      Assert.True(original.SecondaryMemberAsIntermediateRestraint);
      Assert.False(original.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Null(original.CustomIntermediateRestraintPositions);
    }

    [Fact]
    public void TestSupportDuplicate2() {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1 create with constructor and duplicate
      Supports original = TestSupportConstructorCustom(1, 2, 3, false, true);
      var duplicate = (Supports)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(IntermediateRestraint.Custom, duplicate.IntermediateRestraintPositions);
      Assert.False(duplicate.SecondaryMemberAsIntermediateRestraint);
      Assert.True(duplicate.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(1, duplicate.CustomIntermediateRestraintPositions[0].As(LengthUnit.Millimeter));
      Assert.Equal(2, duplicate.CustomIntermediateRestraintPositions[1].As(LengthUnit.Millimeter));
      Assert.Equal(3, duplicate.CustomIntermediateRestraintPositions[2].As(LengthUnit.Millimeter));

      // 3 make some changes to duplicate
      duplicate.SecondaryMemberAsIntermediateRestraint = true;
      duplicate.BothFlangesFreeToRotateOnPlanAtEnds = false;
      var customIntermediateRestraintPositions = new List<IQuantity> {
        new Length(4, unit),
        new Length(5, unit),
        new Length(6, unit),
        new Length(7, unit)
      };
      duplicate.CustomIntermediateRestraintPositions = customIntermediateRestraintPositions;

      // 4 check that duplicate has set changes
      Assert.Equal(IntermediateRestraint.Custom, duplicate.IntermediateRestraintPositions);
      Assert.True(duplicate.SecondaryMemberAsIntermediateRestraint);
      Assert.False(duplicate.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(4, duplicate.CustomIntermediateRestraintPositions[0].As(LengthUnit.Millimeter));
      Assert.Equal(5, duplicate.CustomIntermediateRestraintPositions[1].As(LengthUnit.Millimeter));
      Assert.Equal(6, duplicate.CustomIntermediateRestraintPositions[2].As(LengthUnit.Millimeter));
      Assert.Equal(7, duplicate.CustomIntermediateRestraintPositions[3].As(LengthUnit.Millimeter));

      // 5 check that original has not been changed
      Assert.Equal(IntermediateRestraint.Custom, original.IntermediateRestraintPositions);
      Assert.False(original.SecondaryMemberAsIntermediateRestraint);
      Assert.True(original.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(1, original.CustomIntermediateRestraintPositions[0].As(LengthUnit.Millimeter));
      Assert.Equal(2, original.CustomIntermediateRestraintPositions[1].As(LengthUnit.Millimeter));
      Assert.Equal(3, original.CustomIntermediateRestraintPositions[2].As(LengthUnit.Millimeter));
    }
  }
}
