using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]
namespace ComposAPITests.Helper {
  [Collection("ComposAPI Fixture collection")]
  public class ComposUnitsHelperTest {
    [Theory]
    [InlineData(LengthUnit.LightYear, ForceUnit.Newton)]
    [InlineData(LengthUnit.Meter, ForceUnit.Newton)]
    [InlineData(LengthUnit.Meter, ForceUnit.Kilonewton)]
    [InlineData(LengthUnit.Meter, ForceUnit.TonneForce)]
    [InlineData(LengthUnit.Meter, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.Newton)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.Kilonewton)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.TonneForce)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.Newton)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.Kilonewton)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.TonneForce)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Foot, ForceUnit.PoundForce)]
    [InlineData(LengthUnit.Foot, ForceUnit.KilopoundForce)]
    [InlineData(LengthUnit.Foot, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Inch, ForceUnit.PoundForce)]
    [InlineData(LengthUnit.Inch, ForceUnit.KilopoundForce)]
    [InlineData(LengthUnit.Inch, ForceUnit.ShortTonForce)]
    public void ForcePerAreaUnitTest(LengthUnit lengthUnit, ForceUnit forceUnit) {
      if (forceUnit == ForceUnit.ShortTonForce || lengthUnit == LengthUnit.LightYear) {
        Assert.Throws<System.ArgumentException>(() => ComposUnitsHelper.GetForcePerAreaUnit(forceUnit, lengthUnit));
      }
      else {
        string expectedUnitString = forceUnit.ToString() + "PerSquare" + lengthUnit.ToString();
        string unitString = ComposUnitsHelper.GetForcePerAreaUnit(forceUnit, lengthUnit).ToString();
        Assert.Equal(expectedUnitString, unitString);
      }
    }

    [Theory]
    [InlineData(LengthUnit.LightYear, ForceUnit.Newton)]
    [InlineData(LengthUnit.Meter, ForceUnit.Newton)]
    [InlineData(LengthUnit.Meter, ForceUnit.Kilonewton)]
    [InlineData(LengthUnit.Meter, ForceUnit.TonneForce)]
    [InlineData(LengthUnit.Meter, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.Newton)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.Kilonewton)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.TonneForce)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.Newton)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.Kilonewton)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.TonneForce)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Foot, ForceUnit.PoundForce)]
    [InlineData(LengthUnit.Foot, ForceUnit.KilopoundForce)]
    [InlineData(LengthUnit.Foot, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Inch, ForceUnit.PoundForce)]
    [InlineData(LengthUnit.Inch, ForceUnit.KilopoundForce)]
    [InlineData(LengthUnit.Inch, ForceUnit.ShortTonForce)]
    public void ForcePerLengthUnitTest(LengthUnit lengthUnit, ForceUnit forceUnit) {
      if (forceUnit == ForceUnit.ShortTonForce || lengthUnit == LengthUnit.LightYear) {
        Assert.Throws<System.ArgumentException>(() => ComposUnitsHelper.GetForcePerLengthUnit(forceUnit, lengthUnit));
      }
      else {
        string expectedUnitString = forceUnit.ToString() + "Per" + lengthUnit.ToString();
        string unitString = ComposUnitsHelper.GetForcePerLengthUnit(forceUnit, lengthUnit).ToString();
        Assert.Equal(expectedUnitString, unitString);
      }
    }

    [Theory]
    [InlineData(LengthUnit.LightYear, ForceUnit.Newton)]
    [InlineData(LengthUnit.Meter, ForceUnit.Newton)]
    [InlineData(LengthUnit.Meter, ForceUnit.Kilonewton)]
    [InlineData(LengthUnit.Meter, ForceUnit.TonneForce)]
    [InlineData(LengthUnit.Meter, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.Newton)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.Kilonewton)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.TonneForce)]
    [InlineData(LengthUnit.Centimeter, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.Newton)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.Kilonewton)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.TonneForce)]
    [InlineData(LengthUnit.Millimeter, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Foot, ForceUnit.PoundForce)]
    [InlineData(LengthUnit.Foot, ForceUnit.KilopoundForce)]
    [InlineData(LengthUnit.Foot, ForceUnit.ShortTonForce)]
    [InlineData(LengthUnit.Inch, ForceUnit.PoundForce)]
    [InlineData(LengthUnit.Inch, ForceUnit.KilopoundForce)]
    [InlineData(LengthUnit.Inch, ForceUnit.ShortTonForce)]
    public void MomentUnitTest(LengthUnit lengthUnit, ForceUnit forceUnit) {
      if (forceUnit == ForceUnit.ShortTonForce || lengthUnit == LengthUnit.LightYear) {
        Assert.Throws<System.ArgumentException>(() => ComposUnitsHelper.GetMomentUnit(forceUnit, lengthUnit));
      }
      else {
        string expectedUnitString = forceUnit.ToString() + lengthUnit.ToString();
        string unitString = ComposUnitsHelper.GetMomentUnit(forceUnit, lengthUnit).ToString();
        Assert.Equal(expectedUnitString, unitString);
      }
    }

    [Theory]
    [InlineData(LengthUnit.LightYear)]
    [InlineData(LengthUnit.Meter)]
    [InlineData(LengthUnit.Millimeter)]
    [InlineData(LengthUnit.Centimeter)]
    [InlineData(LengthUnit.Foot)]
    [InlineData(LengthUnit.Inch)]
    [InlineData(LengthUnit.Microinch)]
    public void SectionAreaMomentOfInertiaUnitTest(LengthUnit lengthUnit) {
      if (lengthUnit == LengthUnit.Microinch || lengthUnit == LengthUnit.LightYear) {
        Assert.Throws<System.ArgumentException>(() => ComposUnitsHelper.GetSectionAreaMomentOfInertiaUnit(lengthUnit));
      }
      else {
        string expectedUnitString = lengthUnit.ToString() + "ToTheFourth";
        string unitString = ComposUnitsHelper.GetSectionAreaMomentOfInertiaUnit(lengthUnit).ToString();
        Assert.Equal(expectedUnitString, unitString);
      }
    }

    [Theory]
    [InlineData(LengthUnit.LightYear)]
    [InlineData(LengthUnit.Meter)]
    [InlineData(LengthUnit.Millimeter)]
    [InlineData(LengthUnit.Centimeter)]
    [InlineData(LengthUnit.Foot)]
    [InlineData(LengthUnit.Inch)]
    [InlineData(LengthUnit.Microinch)]
    public void SectionAreaUnitTest(LengthUnit lengthUnit) {
      ComposUnitsHelper.LengthUnitSection = lengthUnit;
      if (lengthUnit == LengthUnit.Microinch || lengthUnit == LengthUnit.LightYear) {
        Assert.Throws<System.ArgumentException>(() => ComposUnitsHelper.SectionAreaUnit);
      }
      else {
        string expectedUnitString = "Square" + lengthUnit.ToString();
        string unitString = ComposUnitsHelper.SectionAreaUnit.ToString();
        Assert.Equal(expectedUnitString, unitString);
      }
    }

    [Theory]
    [InlineData(LengthUnit.LightYear, MassUnit.Gram)]
    [InlineData(LengthUnit.Meter, MassUnit.Gram)]
    [InlineData(LengthUnit.Meter, MassUnit.Kilogram)]
    [InlineData(LengthUnit.Meter, MassUnit.Tonne)]
    [InlineData(LengthUnit.Meter, MassUnit.LongTon)]
    [InlineData(LengthUnit.Centimeter, MassUnit.Gram)]
    [InlineData(LengthUnit.Centimeter, MassUnit.Kilogram)]
    [InlineData(LengthUnit.Centimeter, MassUnit.Tonne)]
    [InlineData(LengthUnit.Centimeter, MassUnit.LongTon)]
    [InlineData(LengthUnit.Millimeter, MassUnit.Gram)]
    [InlineData(LengthUnit.Millimeter, MassUnit.Kilogram)]
    [InlineData(LengthUnit.Millimeter, MassUnit.Tonne)]
    [InlineData(LengthUnit.Millimeter, MassUnit.LongTon)]
    [InlineData(LengthUnit.Foot, MassUnit.Pound)]
    [InlineData(LengthUnit.Foot, MassUnit.Kilopound)]
    [InlineData(LengthUnit.Foot, MassUnit.LongTon)]
    [InlineData(LengthUnit.Inch, MassUnit.Pound)]
    [InlineData(LengthUnit.Inch, MassUnit.Kilopound)]
    [InlineData(LengthUnit.Inch, MassUnit.LongTon)]
    public void DensityUnitTest(LengthUnit lengthUnit, MassUnit massUnit) {
      if (massUnit == MassUnit.LongTon || lengthUnit == LengthUnit.LightYear) {
        Assert.Throws<System.ArgumentException>(() => ComposUnitsHelper.GetDensityUnit(massUnit, lengthUnit));
      }
      else {
        string expectedUnitString = massUnit.ToString() + "PerCubic" + lengthUnit.ToString();
        string unitString = ComposUnitsHelper.GetDensityUnit(massUnit, lengthUnit).ToString();
        Assert.Equal(expectedUnitString, unitString);
      }
    }

    [Fact]
    public void EqualityForDifferentUnitWillThrowException() {
      var length = Length.From(1, LengthUnit.Meter);
      var mass = Mass.From(1, MassUnit.Kilogram);
      Assert.Throws<System.ArgumentException>(() => ComposUnitsHelper.IsEqual(length, mass));
    }
  }
}
