using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGH.Helpers;
using ComposGHTests.Helper;
using OasysGH;
using OasysUnits;
using Xunit;

namespace ComposAPI.Slabs.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class CustomTransverseReinforcementLayoutTest {

    // 1 setup inputs
    [Theory]
    [InlineData(0, 1, 8, 100, 35)]
    public CustomTransverseReinforcementLayout ConstructorTest(double distanceFromStart, double distanceFromEnd, double diameter, double spacing, double cover) {
      var units = ComposUnits.GetStandardUnits();

      // 2 create object instance with constructor
      var layout = new CustomTransverseReinforcementLayout(new Length(distanceFromStart, units.Length), new Length(distanceFromEnd, units.Length), new Length(diameter, units.Length), new Length(spacing, units.Length), new Length(cover, units.Length));

      // 3 check that inputs are set in object's members
      Assert.Equal(distanceFromStart, layout.StartPosition.Value);
      Assert.Equal(distanceFromEnd, layout.EndPosition.Value);
      Assert.Equal(diameter, layout.Diameter.Value);
      Assert.Equal(spacing, layout.Spacing.Value);
      Assert.Equal(cover, layout.Cover.Value);

      return layout;
    }

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      CustomTransverseReinforcementLayout original = ConstructorTest(0, 1, 8, 100, 35);
      var duplicate = (CustomTransverseReinforcementLayout)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Theory]
    [InlineData("REBAR_TRANSVERSE	MEMBER-1	USER_DEFINED	0.000000	1.00000	8.00000	100.000	35.0000\n", 0, 1, 8, 100, 35)]
    public void FromCoaStringTest(string coaString, double expected_distanceFromStart, double expected_distanceFromEnd, double expected_diameter, double expected_spacing, double expected_cover) {
      List<string> parameters = CoaHelper.Split(coaString);

      ICustomTransverseReinforcementLayout customTransverseReinforcementLayout = CustomTransverseReinforcementLayout.FromCoaString(parameters, ComposUnits.GetStandardUnits());

      Assert.Equal(expected_distanceFromStart, customTransverseReinforcementLayout.StartPosition.Value);
      Assert.Equal(expected_distanceFromEnd, customTransverseReinforcementLayout.EndPosition.Value);
      Assert.Equal(expected_diameter, customTransverseReinforcementLayout.Diameter.Value);
      Assert.Equal(expected_spacing, customTransverseReinforcementLayout.Spacing.Value);
      Assert.Equal(expected_cover, customTransverseReinforcementLayout.Cover.Value);
    }

    [Theory]
    [InlineData(0, 1, 8, 100, 35, "REBAR_TRANSVERSE	MEMBER-1	USER_DEFINED	0.000000	1.00000	8.00000	100.000	35.0000\n")]
    public void ToCoaStringTest(double distanceFromStart, double distanceFromEnd, double diameter, double spacing, double cover, string expected_coaString) {
      var units = ComposUnits.GetStandardUnits();

      var layout = new CustomTransverseReinforcementLayout(new Length(distanceFromStart, units.Length), new Length(distanceFromEnd, units.Length), new Length(diameter, units.Length), new Length(spacing, units.Length), new Length(cover, units.Length));

      string coaString = layout.ToCoaString("MEMBER-1", units);

      Assert.Equal(expected_coaString, coaString);
    }
  }
}
