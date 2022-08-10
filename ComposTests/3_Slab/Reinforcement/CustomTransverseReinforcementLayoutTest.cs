using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Slabs.Tests
{
  public class CustomTransverseReinforcementLayoutTest
  {
    [Theory]
    [InlineData(0, 1, 8, 100, 35, "REBAR_TRANSVERSE	MEMBER-1	USER_DEFINED	0.000000	1.00000	8.00000	100.000	35.0000\n")]
    public void ToCoaStringTest(double distanceFromStart, double distanceFromEnd, double diameter, double spacing, double cover, string expected_coaString)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();

      CustomTransverseReinforcementLayout layout = new CustomTransverseReinforcementLayout(new Length(distanceFromStart, units.Length), new Length(distanceFromEnd, units.Length), new Length(diameter, units.Length), new Length(spacing, units.Length), new Length(cover, units.Length));

      string coaString = layout.ToCoaString("MEMBER-1", units);

      Assert.Equal(expected_coaString, coaString);
    }

    [Theory]
    [InlineData("REBAR_TRANSVERSE	MEMBER-1	USER_DEFINED	0.000000	1.00000	8.00000	100.000	35.0000\n", 0, 1, 8, 100, 35)]
    public void FromCoaStringTest(string coaString, double expected_distanceFromStart, double expected_distanceFromEnd, double expected_diameter, double expected_spacing, double expected_cover)
    {
      List<string> parameters = CoaHelper.Split(coaString);

      ICustomTransverseReinforcementLayout customTransverseReinforcementLayout = CustomTransverseReinforcementLayout.FromCoaString(parameters, ComposUnits.GetStandardUnits());

      Assert.Equal(expected_distanceFromStart, customTransverseReinforcementLayout.StartPosition.Value);
      Assert.Equal(expected_distanceFromEnd, customTransverseReinforcementLayout.EndPosition.Value);
      Assert.Equal(expected_diameter, customTransverseReinforcementLayout.Diameter.Value);
      Assert.Equal(expected_spacing, customTransverseReinforcementLayout.Spacing.Value);
      Assert.Equal(expected_cover, customTransverseReinforcementLayout.Cover.Value);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(0, 1, 8, 100, 35)]
    public void ConstructorTes(double distanceFromStart, double distanceFromEnd, double diameter, double spacing, double cover)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();
   
      // 2 create object instance with constructor
      CustomTransverseReinforcementLayout layout = new CustomTransverseReinforcementLayout(new Length(distanceFromStart, units.Length), new Length(distanceFromEnd, units.Length), new Length(diameter, units.Length), new Length(spacing, units.Length), new Length(cover, units.Length));

      // 3 check that inputs are set in object's members
      Assert.Equal(distanceFromStart, layout.StartPosition.Value);
      Assert.Equal(distanceFromEnd, layout.EndPosition.Value);
      Assert.Equal(diameter, layout.Diameter.Value);
      Assert.Equal(spacing, layout.Spacing.Value);
      Assert.Equal(cover, layout.Cover.Value);
    }
  }
}
