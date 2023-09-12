using System;
using System.Linq;
using ComposAPI.Helpers;
using ComposGHTests.Helpers;
using ComposGH.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Beams.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class ASNZSteelMaterialTest {

    // 1 setup inputs
    [Theory]
    [InlineData(StandardASNZSteelMaterialGrade.C450_AS1163, 450, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.C350_AS1163, 350, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.C250_AS1163, 250, WeldMaterialGrade.Grade_42)]
    [InlineData(StandardASNZSteelMaterialGrade.HA400_AS1594, 380, WeldMaterialGrade.Grade_50)]
    [InlineData(StandardASNZSteelMaterialGrade.HW350_AS1594, 340, WeldMaterialGrade.Grade_50)]
    [InlineData(StandardASNZSteelMaterialGrade.HA350_AS1594, 350, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.HA300_1_AS1594, 300, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.HA_HU300_AS1594, 300, WeldMaterialGrade.Grade_42)]
    [InlineData(StandardASNZSteelMaterialGrade.HA_HU250_AS1594, 250, WeldMaterialGrade.Grade_50)]
    [InlineData(StandardASNZSteelMaterialGrade.HA200_AS1594, 200, WeldMaterialGrade.Grade_50)]
    [InlineData(StandardASNZSteelMaterialGrade.HA4N_AS1594, 170, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.HA3_AS1594, 200, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.XF400_AS1594, 380, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.XF300_AS1594, 300, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr450_AS3678, 450, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr400_AS3678, 400, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr350_AS3678, 360, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.WR350_AS3678, 340, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr300_AS3678, 320, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr250_AS3678, 280, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr250L15_AS3678, 280, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr200_AS3678, 200, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr400_AS3679_1_Flats, 400, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr350_AS3679_1_Flats, 360, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr300_AS3679_1_Flats, 320, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr250_AS3679_1_Flats, 260, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr400_AS3679_1_Hollow, 400, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr350_AS3679_1_Hollow, 340, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr300_AS3679_1_Hollow, 300, WeldMaterialGrade.Grade_35)]
    [InlineData(StandardASNZSteelMaterialGrade.Gr250_AS3679_1_Hollow, 250, WeldMaterialGrade.Grade_35)]
    public void ConstructorTest(StandardASNZSteelMaterialGrade grade, double fy_expected, WeldMaterialGrade weldGrade_expected) {
      // 2 create object instance with constructor
      var material = new ASNZSteelMaterial(grade);

      // 3 check that inputs are set in object's members
      Assert.Equal(new Pressure(200, PressureUnit.Gigapascal), material.E);
      Assert.Equal(new Density(7850, DensityUnit.KilogramPerCubicMeter), material.Density);
      Assert.Equal(material.Fy.Megapascals, fy_expected);
      Assert.Equal(weldGrade_expected, material.WeldGrade);
    }

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      var original = new StandardASNZSteelMaterialGrade();
      var duplicate = (StandardASNZSteelMaterialGrade)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void ToCoaAndBack() {
      var grades = Enum.GetValues(typeof(StandardASNZSteelMaterialGrade)).Cast<StandardASNZSteelMaterialGrade>().ToList();

      foreach (StandardASNZSteelMaterialGrade grade in grades) {
        var steelMaterial = new ASNZSteelMaterial(grade);
        string coaString = steelMaterial.ToCoaString("MEMBER-1", Code.AS_NZS2327_2017, ComposUnits.GetStandardUnits());

        string line = CoaHelper.SplitAndStripLines(coaString)[0];
        var parameters = line.Split('\t').ToList();
        string gradeString = parameters.Last().Replace("\n", string.Empty);
        StandardASNZSteelMaterialGrade gradeFromString = ASNZSteelMaterial.FromString(gradeString);
        Assert.Equal(steelMaterial.Grade, gradeFromString);
      }
    }

    [Theory]
    [InlineData(StandardASNZSteelMaterialGrade.C450_AS1163, "BEAM_STEEL_MATERIAL_STD	MEMBER-1	C450(AS1163)\nBEAM_WELDING_MATERIAL\tMEMBER-1\tGrade 35\n")]
    [InlineData(StandardASNZSteelMaterialGrade.C250_AS1163, "BEAM_STEEL_MATERIAL_STD	MEMBER-1	C250(AS1163)\nBEAM_WELDING_MATERIAL\tMEMBER-1\tGrade 42\n")]
    public void ToCoaStringTest(StandardASNZSteelMaterialGrade steelMaterialGrade, string expected_coaString) {
      SteelMaterial steelMaterial = new ASNZSteelMaterial(steelMaterialGrade);
      string coaString = steelMaterial.ToCoaString("MEMBER-1", Code.AS_NZS2327_2017, ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }
  }
}
