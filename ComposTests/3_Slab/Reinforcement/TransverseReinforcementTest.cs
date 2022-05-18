using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public class TransverseReinforcementTest
  {
    [Theory]
    [InlineData(RebarGrade.AS_D500E, "REBAR_MATERIAL	MEMBER-1	STANDARD	D500E\nREBAR_TRANSVERSE	MEMBER-1	PROGRAM_DESIGNED\n")]
    public void ToCoaStringTest(RebarGrade grade, string expected_coaString)
    {
      TransverseReinforcement transverseReinforcement = new TransverseReinforcement(new ReinforcementMaterial(grade));

      string coaString = transverseReinforcement.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    [Theory]
    [InlineData(RebarGrade.BS_500X, "REBAR_MATERIAL	MEMBER-1	STANDARD	500X\nREBAR_TRANSVERSE	MEMBER-1	USER_DEFINED	0.000000	1.00000	8.00000	100.000	35.0000\n")]
    public void ToCoaStringCustomLayoutTest(RebarGrade grade, string expected_coaString)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();
      List<ICustomTransverseReinforcementLayout> customReinforcementLayout = new List<ICustomTransverseReinforcementLayout>() { new CustomTransverseReinforcementLayout(new Length(0, units.Length), new Length(1, units.Length), new Length(8, units.Length), new Length(100, units.Length), new Length(35, units.Length)) };

      TransverseReinforcement transverseReinforcement = new TransverseReinforcement(new ReinforcementMaterial(grade), customReinforcementLayout);

      string coaString = transverseReinforcement.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    [Theory]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	500B\nREBAR_TRANSVERSE	MEMBER-1	PROGRAM_DESIGNED\n", RebarGrade.EN_500B)]
    public void CoaConstructorTest(string coaString, RebarGrade expected_grade)
    {
      List<string> parameters = CoaHelper.Split(coaString);

      ITransverseReinforcement transverseReinforcement = TransverseReinforcement.FromCoaString(parameters, Code.BS5950_3_1_1990_A1_2010, ComposUnits.GetStandardUnits());


      Assert.Equal(expected_grade, transverseReinforcement.Material.Grade);
      Assert.Equal(LayoutMethod.Automatic, transverseReinforcement.LayoutMethod);
    }

    [Theory]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	250R\nREBAR_TRANSVERSE	MEMBER-1	USER_DEFINED	0.000000	1.00000	8.00000	100.000	35.0000\n", RebarGrade.BS_250R)]
    public void CoaConstructorCustomLayoutTest(string coaString, RebarGrade expected_grade)
    {
      List<string> parameters = CoaHelper.Split(coaString);

      ITransverseReinforcement transverseReinforcement = TransverseReinforcement.FromCoaString(parameters, Code.BS5950_3_1_1990_A1_2010, ComposUnits.GetStandardUnits());

      Assert.Equal(expected_grade, transverseReinforcement.Material.Grade);
      Assert.Equal(LayoutMethod.Custom, transverseReinforcement.LayoutMethod);
      Assert.Equal(0, transverseReinforcement.CustomReinforcementLayouts[0].DistanceFromStart.Value);
      Assert.Equal(1, transverseReinforcement.CustomReinforcementLayouts[0].DistanceFromEnd.Value);
      Assert.Equal(8, transverseReinforcement.CustomReinforcementLayouts[0].Diameter.Value);
      Assert.Equal(100, transverseReinforcement.CustomReinforcementLayouts[0].Spacing.Value);
      Assert.Equal(35, transverseReinforcement.CustomReinforcementLayouts[0].Cover.Value);
    }
  }
}
