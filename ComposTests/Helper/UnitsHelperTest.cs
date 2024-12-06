using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Xunit;
namespace ComposAPITests.Helper {
  [Collection("ComposAPI Fixture collection")]
  public class ComposUnitsHelperTest {
    [Fact]
    public void EqualityForDifferentUnitWillThrowException() {
      var length = Length.From(1, LengthUnit.Meter);
      var mass = Mass.From(1, MassUnit.Kilogram);
      Assert.Throws<System.ArgumentException>(() => ComposUnitsHelper.IsEqual(length, mass));
    }
  }
}
