﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using Xunit;
using ComposAPI;

namespace ComposAPI.Tests
{
  public static class SlabDimensionMother
  {
    public static ISlabDimension CreateSlabDimension()
    {
      LengthUnit lenghtUnit = LengthUnit.Millimeter;
      return new SlabDimension(new Length(1000, lenghtUnit), new Length(200, lenghtUnit), new Length(300, lenghtUnit), new Length(300, lenghtUnit), new Length(250, lenghtUnit), new Length(250, lenghtUnit), true);
    }
  }

  public class SlabDimensionTest
  {
    [Theory]
    [InlineData(1, 0, 0.15, 1, 1.5, false, 0, 0, false, "SLAB_DIMENSION	MEMBER-1	4	1	0.000000	0.150000	1.00000	1.50000	TAPERED_NO	EFFECTIVE_WIDTH_NO\n")]
    [InlineData(2, 1, 0.25, 1, 1.5, false, 0, 0, false, "SLAB_DIMENSION	MEMBER-1	4	2	1.00000	0.250000	1.00000	1.50000	TAPERED_NO	EFFECTIVE_WIDTH_NO\n")]
    [InlineData(3, 2, 0.2, 1, 1, true, 2, 3, false, "SLAB_DIMENSION	MEMBER-1	4	3	2.00000	0.200000	1.00000	1.00000	TAPERED_NO	EFFECTIVE_WIDTH_YES	2.00000	3.00000\n")]
    [InlineData(4, 3, 0.1, 1, 1, true, 2, 3, true, "SLAB_DIMENSION	MEMBER-1	4	4	3.00000	0.100000	1.00000	1.00000	TAPERED_YES	EFFECTIVE_WIDTH_YES	2.00000	3.00000\n")]
    public void ToCoaStringTest(int index, double startPosition, double overallDepth, double availableWidthLeft, double availableWidthRight, bool userEffectiveWidth, double effectiveWidthLeft, double effectiveWidthRight, bool taperedToNext, string expected_coaString)
    {
      SlabDimension slabDimension;
      if (userEffectiveWidth)
        slabDimension = new SlabDimension(new Length(startPosition, LengthUnit.Meter), new Length(overallDepth, LengthUnit.Meter), new Length(availableWidthLeft, LengthUnit.Meter), new Length(availableWidthRight, LengthUnit.Meter), new Length(effectiveWidthLeft, LengthUnit.Meter), new Length(effectiveWidthRight, LengthUnit.Meter), taperedToNext);
      else
        slabDimension = new SlabDimension(new Length(startPosition, LengthUnit.Meter), new Length(overallDepth, LengthUnit.Meter), new Length(availableWidthLeft, LengthUnit.Meter), new Length(availableWidthRight, LengthUnit.Meter), taperedToNext);
      string coaString = slabDimension.ToCoaString("MEMBER-1", 4, index, LengthUnit.Meter);

      Assert.Equal(expected_coaString, coaString);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(800, 250, 310, 300, true)]
    [InlineData(800, 250, 310, 300, false)]
    public void TestConstructor1(double startPosition, double overallDepth, double availableWidthLeft, double availableWidthRight, bool taperedToNext)
    {
      // 2 create object instance with constructor
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ISlabDimension slabDimension = new SlabDimension(new Length(startPosition, lengthUnit), new Length(overallDepth, lengthUnit), new Length(availableWidthLeft, lengthUnit), new Length(availableWidthRight, lengthUnit), taperedToNext);

      // 3 check that inputs are set in object's members
      Assert.Equal(startPosition, slabDimension.StartPosition.Value);
      Assert.Equal(overallDepth, slabDimension.OverallDepth.Value);
      Assert.Equal(availableWidthLeft, slabDimension.AvailableWidthLeft.Value);
      Assert.Equal(availableWidthRight, slabDimension.AvailableWidthRight.Value);
      Assert.Equal(taperedToNext, slabDimension.TaperedToNext);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(800, 250, 310, 300, true, 250, 240)]
    [InlineData(800, 250, 310, 300, false, 250, 240)]
    public void TestConstructor2(double startPosition, double overallDepth, double availableWidthLeft, double availableWidthRight, bool taperedToNext, double effectiveWidthLeft, double effectiveWidthRight)
    {
      // 2 create object instance with constructor
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ISlabDimension slabDimension = new SlabDimension(new Length(startPosition, lengthUnit), new Length(overallDepth, lengthUnit), new Length(availableWidthLeft, lengthUnit), new Length(availableWidthRight, lengthUnit), new Length(effectiveWidthLeft, lengthUnit), new Length(effectiveWidthRight, lengthUnit), taperedToNext);

      // 3 check that inputs are set in object's members
      Assert.Equal(startPosition, slabDimension.StartPosition.Value);
      Assert.Equal(overallDepth, slabDimension.OverallDepth.Value);
      Assert.Equal(availableWidthLeft, slabDimension.AvailableWidthLeft.Value);
      Assert.Equal(availableWidthRight, slabDimension.AvailableWidthRight.Value);
      Assert.Equal(effectiveWidthLeft, slabDimension.EffectiveWidthLeft.Value);
      Assert.Equal(effectiveWidthRight, slabDimension.EffectiveWidthRight.Value);
      Assert.Equal(taperedToNext, slabDimension.TaperedToNext);
    }

    [Theory]
    [InlineData(800, 250, 310, 300, true, 250, 240)]
    public void TestDuplicate(double startPosition, double overallDepth, double availableWidthLeft, double availableWidthRight, bool taperedToNext, double effectiveWidthLeft, double effectiveWidthRight)
    {
      // 1 create with constructor and duplicate
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ISlabDimension original = new SlabDimension(new Length(startPosition, lengthUnit), new Length(overallDepth, lengthUnit), new Length(availableWidthLeft, lengthUnit), new Length(availableWidthRight, lengthUnit), new Length(effectiveWidthLeft, lengthUnit), new Length(effectiveWidthRight, lengthUnit), taperedToNext);
      ISlabDimension duplicate = original.Duplicate() as ISlabDimension;

      // 2 check that duplicate has duplicated values
      //Assert.Equal(shortTerm, duplicate.ShortTerm);
      //Assert.Equal(longTerm, duplicate.LongTerm);
      //Assert.Equal(vibration, duplicate.Vibration);
      //Assert.Equal(shrinkage, duplicate.Shrinkage);
      //Assert.True(duplicate.UserDefined);

      //// 3 make some changes to duplicate
      //duplicate.ShortTerm = 6;
      //duplicate.LongTerm = 18;
      //duplicate.Vibration = 5.39;
      //duplicate.Shrinkage = 0;

      //// 4 check that duplicate has set changes
      //Assert.Equal(6, duplicate.ShortTerm);
      //Assert.Equal(18, duplicate.LongTerm);
      //Assert.Equal(5.39, duplicate.Vibration);
      //Assert.Equal(0, duplicate.Shrinkage);

      //// 5 check that original has not been changed
      //Assert.Equal(shortTerm, original.ShortTerm);
      //Assert.Equal(longTerm, original.LongTerm);
      //Assert.Equal(vibration, original.Vibration);
      //Assert.Equal(shrinkage, original.Shrinkage);
      //Assert.True(original.UserDefined);
    }
  }
}