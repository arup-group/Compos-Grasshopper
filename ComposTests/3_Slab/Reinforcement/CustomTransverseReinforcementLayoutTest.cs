using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public class CustomTransverseReinforcementLayoutTest
  {
    [Theory]
    [InlineData(0, 1, 8, 100, 35, "REBAR_TRANSVERSE	MEMBER-1	USER_DEFINED	0.000000	1.00000	8.00000	100.000	35.0000\n")]
    public void ToCoaStringTest(double distanceFromStart, double distanceFromEnd, double diameter, double spacing, double cover, string expected_coaString)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();

      CustomTransverseReinforcementLayout customTransverseReinforcementLayout = new CustomTransverseReinforcementLayout(new Length(distanceFromStart, units.Length), new Length(distanceFromEnd, units.Length), new Length(diameter, units.Length), new Length(spacing, units.Length), new Length(cover, units.Length));

      string coaString = customTransverseReinforcementLayout.ToCoaString("MEMBER-1", units);

      Assert.Equal(expected_coaString, coaString);
    }

    [Theory]
    [InlineData("REBAR_TRANSVERSE	MEMBER-1	USER_DEFINED	0.000000	1.00000	8.00000	100.000	35.0000\n", 0, 1, 8, 100, 35)]
    public void CoaConstructorTest(string coaString, double expected_distanceFromStart, double expected_distanceFromEnd, double expected_diameter, double expected_spacing, double expected_cover)
    {
      List<string> parameters = CoaHelper.Split(coaString);

      CustomTransverseReinforcementLayout customTransverseReinforcementLayout = new CustomTransverseReinforcementLayout(parameters, ComposUnits.GetStandardUnits());

      Assert.Equal(expected_distanceFromStart, customTransverseReinforcementLayout.DistanceFromStart.Value);
      Assert.Equal(expected_distanceFromEnd, customTransverseReinforcementLayout.DistanceFromEnd.Value);
      Assert.Equal(expected_diameter, customTransverseReinforcementLayout.Diameter.Value);
      Assert.Equal(expected_spacing, customTransverseReinforcementLayout.Spacing.Value);
      Assert.Equal(expected_cover, customTransverseReinforcementLayout.Cover.Value);
    }
  }
}
