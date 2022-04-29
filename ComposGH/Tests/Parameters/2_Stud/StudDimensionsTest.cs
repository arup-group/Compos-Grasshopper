using Xunit;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Parameters.Tests
{
  public partial class ComposStudTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(19, 100, 450)]
    public StudDimensions TestConstructorStudDimensionsCustomSizeStress(double diameter, double height, double fu)
    {
      LengthUnit length = LengthUnit.Millimeter;
      PressureUnit stress = PressureUnit.Megapascal;

      // 2 create object instance with constructor
      StudDimensions studDims = new StudDimensions(
        new Length(diameter, length), new Length(height, length), new Pressure(fu, stress));

      // 3 check that inputs are set in object's members
      Assert.Equal(diameter, studDims.Diameter.Millimeters);
      Assert.Equal(height, studDims.Height.Millimeters);
      Assert.Equal(fu, studDims.Fu.Megapascals);
      Assert.Equal(Force.Zero, studDims.CharacterStrength);

      return studDims;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(19, 100, 90)]
    public StudDimensions TestConstructorStudDimensionsCustomSizeForce(double diameter, double height, double strength)
    {
      LengthUnit length = LengthUnit.Millimeter;
      ForceUnit force = ForceUnit.Kilonewton;

      // 2 create object instance with constructor
      StudDimensions studDims = new StudDimensions(
        new Length(diameter, length), new Length(height, length), new Force(strength, force));

      // 3 check that inputs are set in object's members
      Assert.Equal(diameter, studDims.Diameter.Millimeters);
      Assert.Equal(height, studDims.Height.Millimeters);
      Assert.Equal(Pressure.Zero, studDims.Fu);
      Assert.Equal(strength, studDims.CharacterStrength.Kilonewtons);

      return studDims;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(StudDimensions.StandardSize.D13mmH65mm, 90, 13, 65)]
    [InlineData(StudDimensions.StandardSize.D16mmH70mm, 95, 16, 70)]
    [InlineData(StudDimensions.StandardSize.D16mmH75mm, 100, 16, 75)]
    [InlineData(StudDimensions.StandardSize.D19mmH75mm, 80, 19, 75)]
    [InlineData(StudDimensions.StandardSize.D19mmH95mm, 85, 19, 95)]
    [InlineData(StudDimensions.StandardSize.D19mmH100mm, 90, 19, 100)]
    [InlineData(StudDimensions.StandardSize.D19mmH125mm, 95, 19, 125)]
    [InlineData(StudDimensions.StandardSize.D22mmH95mm, 100, 22, 95)]
    [InlineData(StudDimensions.StandardSize.D22mmH100mm, 110, 22, 100)]
    [InlineData(StudDimensions.StandardSize.D25mmH95mm, 80, 25, 95)]
    [InlineData(StudDimensions.StandardSize.D25mmH100mm, 90, 25, 100)]
    public StudDimensions TestConstructorStudDimensionsStandardSizeForce(StudDimensions.StandardSize size, double strength,
      double expectedDiameter, double expectedHeight)
    {
      ForceUnit force = ForceUnit.Kilonewton;

      // 2 create object instance with constructor
      StudDimensions studDims = new StudDimensions(size, new Force(strength, force));

      // 3 check that inputs are set in object's members
      Assert.Equal(expectedDiameter, studDims.Diameter.Millimeters);
      Assert.Equal(expectedHeight, studDims.Height.Millimeters);
      Assert.Equal(Pressure.Zero, studDims.Fu);
      Assert.Equal(strength, studDims.CharacterStrength.Kilonewtons);

      return studDims;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(StudDimensions.StandardSize.D13mmH65mm, 400, 13, 65)]
    [InlineData(StudDimensions.StandardSize.D16mmH70mm, 450, 16, 70)]
    [InlineData(StudDimensions.StandardSize.D16mmH75mm, 500, 16, 75)]
    [InlineData(StudDimensions.StandardSize.D19mmH75mm, 400, 19, 75)]
    [InlineData(StudDimensions.StandardSize.D19mmH95mm, 450, 19, 95)]
    [InlineData(StudDimensions.StandardSize.D19mmH100mm, 500, 19, 100)]
    [InlineData(StudDimensions.StandardSize.D19mmH125mm, 400, 19, 125)]
    [InlineData(StudDimensions.StandardSize.D22mmH95mm, 450, 22, 95)]
    [InlineData(StudDimensions.StandardSize.D22mmH100mm, 500, 22, 100)]
    [InlineData(StudDimensions.StandardSize.D25mmH95mm, 400, 25, 95)]
    [InlineData(StudDimensions.StandardSize.D25mmH100mm, 450, 25, 100)]
    public StudDimensions TestConstructorStudDimensionsStandardSizeStress(StudDimensions.StandardSize size, double fu,
      double expectedDiameter, double expectedHeight)
    {
      PressureUnit stress = PressureUnit.Megapascal;

      // 2 create object instance with constructor
      StudDimensions studDims = new StudDimensions(size, new Pressure(fu, stress));

      // 3 check that inputs are set in object's members
      Assert.Equal(expectedDiameter, studDims.Diameter.Millimeters);
      Assert.Equal(expectedHeight, studDims.Height.Millimeters);
      Assert.Equal(fu, studDims.Fu.Megapascals);
      Assert.Equal(Force.Zero, studDims.CharacterStrength);

      return studDims;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(StudDimensions.StandardSize.D13mmH65mm, StudDimensions.StandardGrade.SD1_EN13918, 400, 13, 65)]
    [InlineData(StudDimensions.StandardSize.D16mmH70mm, StudDimensions.StandardGrade.SD2_EN13918, 450, 16, 70)]
    [InlineData(StudDimensions.StandardSize.D16mmH75mm, StudDimensions.StandardGrade.SD3_EN13918, 500, 16, 75)]
    [InlineData(StudDimensions.StandardSize.D19mmH75mm, StudDimensions.StandardGrade.SD1_EN13918, 400, 19, 75)]
    [InlineData(StudDimensions.StandardSize.D19mmH95mm, StudDimensions.StandardGrade.SD2_EN13918, 450, 19, 95)]
    [InlineData(StudDimensions.StandardSize.D19mmH100mm, StudDimensions.StandardGrade.SD3_EN13918, 500, 19, 100)]
    [InlineData(StudDimensions.StandardSize.D19mmH125mm, StudDimensions.StandardGrade.SD1_EN13918, 400, 19, 125)]
    [InlineData(StudDimensions.StandardSize.D22mmH95mm, StudDimensions.StandardGrade.SD2_EN13918, 450, 22, 95)]
    [InlineData(StudDimensions.StandardSize.D22mmH100mm, StudDimensions.StandardGrade.SD3_EN13918, 500, 22, 100)]
    [InlineData(StudDimensions.StandardSize.D25mmH95mm, StudDimensions.StandardGrade.SD1_EN13918, 400, 25, 95)]
    [InlineData(StudDimensions.StandardSize.D25mmH100mm, StudDimensions.StandardGrade.SD2_EN13918, 450, 25, 100)]
    public StudDimensions TestConstructorStudDimensionsStandardSizeStandardGrade(
      StudDimensions.StandardSize size, StudDimensions.StandardGrade grade,
      double expectedFu, double expectedDiameter, double expectedHeight)
    {
      // 2 create object instance with constructor
      StudDimensions studDims = new StudDimensions(size, grade);

      // 3 check that inputs are set in object's members
      Assert.Equal(expectedDiameter, studDims.Diameter.Millimeters);
      Assert.Equal(expectedHeight, studDims.Height.Millimeters);
      Assert.Equal(expectedFu, studDims.Fu.Megapascals);
      Assert.Equal(Force.Zero, studDims.CharacterStrength);

      return studDims;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(13, 65, StudDimensions.StandardGrade.SD1_EN13918, 400)]
    [InlineData(16, 70, StudDimensions.StandardGrade.SD2_EN13918, 450)]
    [InlineData(16, 75, StudDimensions.StandardGrade.SD3_EN13918, 500)]
    public StudDimensions TestConstructorStudDimensionsCustomSizeStandardGrade(double diameter, double height,
      StudDimensions.StandardGrade grade, double expectedFu)
    {
      LengthUnit length = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      StudDimensions studDims = new StudDimensions(new Length(diameter, length), new Length(height, length), grade);

      // 3 check that inputs are set in object's members
      Assert.Equal(diameter, studDims.Diameter.Millimeters);
      Assert.Equal(height, studDims.Height.Millimeters);
      Assert.Equal(expectedFu, studDims.Fu.Megapascals);
      Assert.Equal(Force.Zero, studDims.CharacterStrength);

      return studDims;
    }

    [Fact]
    public void TestStudDimensionsDuplicate()
    {
      LengthUnit length = LengthUnit.Millimeter;
      PressureUnit stress = PressureUnit.Megapascal;
      ForceUnit force = ForceUnit.Kilonewton;

      // 1 create with constructor and duplicate
      StudDimensions original = TestConstructorStudDimensionsCustomSizeStress(19, 100, 450);
      StudDimensions duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(19, duplicate.Diameter.Millimeters);
      Assert.Equal(100, duplicate.Height.Millimeters);
      Assert.Equal(450, duplicate.Fu.Megapascals);
      Assert.Equal(Force.Zero, duplicate.CharacterStrength);

      // 3 make some changes to duplicate
      duplicate.Diameter = new Length(13, length);
      duplicate.Height = new Length(65, length);
      duplicate.Fu = new Pressure(500, stress);

      // 4 check that duplicate has set changes
      Assert.Equal(13, duplicate.Diameter.Millimeters);
      Assert.Equal(65, duplicate.Height.Millimeters);
      Assert.Equal(500, duplicate.Fu.Megapascals);
      Assert.Equal(Force.Zero, duplicate.CharacterStrength);

      // 5 check that original has not been changed
      Assert.Equal(19, original.Diameter.Millimeters);
      Assert.Equal(100, original.Height.Millimeters);
      Assert.Equal(450, original.Fu.Megapascals);
      Assert.Equal(Force.Zero, original.CharacterStrength);

      // 1 create with new constructor and duplicate
      original = TestConstructorStudDimensionsCustomSizeForce(16, 75, 90);
      duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(16, duplicate.Diameter.Millimeters);
      Assert.Equal(75, duplicate.Height.Millimeters);
      Assert.Equal(Pressure.Zero, duplicate.Fu);
      Assert.Equal(90, duplicate.CharacterStrength.Kilonewtons);

      // 3 make some changes to duplicate
      duplicate.Diameter = new Length(19, length);
      duplicate.Height = new Length(125, length);
      duplicate.CharacterStrength = new Force(110, force);

      // 4 check that duplicate has set changes
      Assert.Equal(19, duplicate.Diameter.Millimeters);
      Assert.Equal(125, duplicate.Height.Millimeters);
      Assert.Equal(Pressure.Zero, duplicate.Fu);
      Assert.Equal(110, duplicate.CharacterStrength.Kilonewtons);

      // 5 check that original has not been changed
      Assert.Equal(16, original.Diameter.Millimeters);
      Assert.Equal(75, original.Height.Millimeters);
      Assert.Equal(Pressure.Zero, original.Fu);
      Assert.Equal(90, original.CharacterStrength.Kilonewtons);
    }
  }
}
