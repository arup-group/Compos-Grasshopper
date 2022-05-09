using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public class DeckingTest
  {
    [Theory]
    [InlineData("RLD", "Ribdeck AL (1.2)", DeckingSteelGrade.S350, 90, true, true, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (1.2)	S350	90.0000	DECKING_JOINTED	JOINT_WELDED\n")]
    public void ToCoaStringTest1(string catalogue, string profile, DeckingSteelGrade deckingSteelGrade, double angle, bool isDiscontinous, bool isWelded, string expected_coaString)
    {
      Decking decking = new CatalogueDecking(catalogue, profile, deckingSteelGrade, new DeckingConfiguration(new Angle(angle, AngleUnit.Degree), isDiscontinous, isWelded));
      string coaString = decking.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    public void TestConstructor()
    {
      // todo
    }
  }
}
