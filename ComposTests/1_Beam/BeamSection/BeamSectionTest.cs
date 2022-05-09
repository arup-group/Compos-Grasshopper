using Xunit;
using UnitsNet;
using UnitsNet.Units;
using System.Collections.Generic;
using Moq;

namespace ComposAPI.Tests
{
  public partial class ComposBeamSectionTest
  {
    [Theory]
    [InlineData(7, 1, 0, 600, 200, 200, 25, 25, 0, false, 15, "STD I 600. 200. 15. 25.", true, "BEAM_SECTION_AT_X	MEMBER-1	7	1	0.000000	STD I 600. 200. 15. 25.	TAPERED_YES\n")]
    [InlineData(7, 2, 0.5, 400, 200, 200, 25, 25, 0, false, 15, "STD I(m) 0.4 0.2 0.015 0.025", false, "BEAM_SECTION_AT_X	MEMBER-1	7	2	0.500000	STD I(m) 0.4 0.2 0.015 0.025	TAPERED_NO\n")]
    [InlineData(7, 3, 1, 600, 200, 200, 25, 25, 0, false, 15, "STD I 600 200 15 25", false, "BEAM_SECTION_AT_X	MEMBER-1	7	3	1.00000	STD I 600 200 15 25	TAPERED_NO\n")]
    [InlineData(7, 4, 2, 260, 200, 200, 25, 25, 0, true, 15, "CAT HE HE260.B 19920101", false, "BEAM_SECTION_AT_X	MEMBER-1	7	4	2.00000	CAT HE HE260.B 19920101	TAPERED_NO\n")]
    [InlineData(7, 5, 3, 600, 200, 200, 25, 25, 0, false, 15, "STD I 600 200 15 25", true, "BEAM_SECTION_AT_X	MEMBER-1	7	5	3.00000	STD I 600 200 15 25	TAPERED_YES\n")]
    [InlineData(7, 6, 4, 100000, 200000, 2000000, 100000, 100000, 0, false, 20000, "STD I(m) 100. 200. 20. 10.", true, "BEAM_SECTION_AT_X	MEMBER-1	7	6	4.00000	STD I(m) 100. 200. 20. 10.	TAPERED_YES\n")]
    [InlineData(7, 7, 5, 100000, 200000, 2000000, 20000, 30000, 0, false, 10000, "STD GI(m) 100. 200. 300. 10. 20. 30.", false, "BEAM_SECTION_AT_X	MEMBER-1	7	7	5.00000	STD GI(m) 100. 200. 300. 10. 20. 30.	TAPERED_NO\n")]
    public void ToCoaStringTest(int num, int index, double startPosition, double depth, double topFlangeWidth, double bottomFlangeWidth,
      double topFlangeThickness, double bottomFlangeThickness, double rootRadius, bool isCatalogue, double webThickness, string sectionDescription, bool taperToNext, string expected_coaString)
    {

      BeamSection beamSection = new BeamSection();
      beamSection.StartPosition = new Length(startPosition, LengthUnit.Meter);
      beamSection.Depth = new Length(depth, LengthUnit.Millimeter);
      beamSection.TopFlangeWidth = new Length(topFlangeWidth, LengthUnit.Millimeter);
      beamSection.BottomFlangeWidth = new Length(bottomFlangeWidth, LengthUnit.Millimeter);
      beamSection.TopFlangeThickness = new Length(topFlangeThickness, LengthUnit.Millimeter);
      beamSection.BottomFlangeThickness = new Length(bottomFlangeThickness, LengthUnit.Millimeter);
      beamSection.RootRadius = new Length(rootRadius, LengthUnit.Millimeter);
      beamSection.WebThickness = new Length(webThickness, LengthUnit.Millimeter);
      beamSection.isCatalogue = isCatalogue;
      beamSection.SectionDescription = sectionDescription;
      beamSection.TaperedToNext = taperToNext;

      string coaString = beamSection.ToCoaString("MEMBER-1", num, index, ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    // 1 setup inputs
    [Theory]
    [InlineData("STD I 200 190.5 8.5 12.7", 200, 190.5, 190.5, 8.5, 12.7, 12.7)]
    [InlineData("STD I(cm) 20. 19.05 0.85 1.27", 200, 190.5, 190.5, 8.5, 12.7, 12.7)]
    [InlineData("STD I(m) 0.2 0.1905 0.0085 0.0127", 200, 190.5, 190.5, 8.5, 12.7, 12.7)]
    [InlineData("STD I(ft) 0,9 0,4 0,01 0,02", 274.3201, 121.92, 121.92, 3.048001, 6.096002, 6.096002)]
    [InlineData("STD GI 400. 300. 250. 12. 25. 20.", 400, 300, 250, 12, 25, 20)]
    [InlineData("STD GI(cm) 15. 15. 12. 3. 1. 2.", 150, 150, 120, 30, 10, 20)]
    //[InlineData("CAT IPE IPE100", 100, 55, 55, 4.1, 5.7, 5.7)] //issue with loading GH referencing in testing environment
    public BeamSection TestBeamSectionConstructorProfile(string profile, double expDepth, double expTopFlangeWidth, double expBottomFlangeWidth, double expWebThickness, double expTopFlangeThickness, double expBottomFlangeThickness)
    {
      //var mock = new Mock<BeamSection>();
      //mock.Setup(x => x.)

      // 2 create object instance with constructor
      BeamSection beam = new BeamSection(profile);

      // 3 check that inputs are set in object's members
      Assert.Equal(expDepth, beam.Depth.Millimeters, 3);
      Assert.Equal(expTopFlangeWidth, beam.TopFlangeWidth.Millimeters, 3);
      Assert.Equal(expBottomFlangeWidth, beam.BottomFlangeWidth.Millimeters, 3);
      Assert.Equal(expWebThickness, beam.WebThickness.Millimeters, 3);
      Assert.Equal(expTopFlangeThickness, beam.TopFlangeThickness.Millimeters, 3);
      Assert.Equal(expBottomFlangeThickness, beam.BottomFlangeThickness.Millimeters, 3);

      return beam;
    }

    [Theory]
    [InlineData("STD R 200 190.5")]
    [InlineData("STD O 200")]
    [InlineData("STD RHS 500 400 40 20")]
    public void TestBeamSectionConstructorProfileExceptions(string profile)
    {
      // check that exceptions are thrown if inputs does not comply with allowed
      Assert.Throws<System.ArgumentException>(() => TestBeamSectionConstructorProfile(profile, 0, 0, 0, 0, 0, 0));
    }

    // 1 setup inputs
    [Theory]
    [InlineData(200, 190.5, 8.5, 12.7, false, 200, 190.5, 190.5, 8.5, 12.7, 12.7, "STD I 200 190.5 8.5 12.7")]
    public BeamSection TestBeamSectionConstructorSymmetric(
      double depth, double flangeWidth, double webThickness, double flangeThickness, bool taperToNext,
      double expDepth, double expTopFlangeWidth, double expBottomFlangeWidth, double expWebThickness,
      double expTopFlangeThickness, double expBottomFlangeThickness, string expProfile)
    {
      LengthUnit unit = LengthUnit.Millimeter;
      // 
      // 2 create object instance with constructor
      BeamSection beam = new BeamSection(new Length(depth, unit), new Length(flangeWidth, unit), new Length(webThickness, unit), new Length(flangeThickness, unit), taperToNext);

      // 3 check that inputs are set in object's members
      Assert.Equal(expDepth, beam.Depth.Millimeters, 3);
      Assert.Equal(expTopFlangeWidth, beam.TopFlangeWidth.Millimeters, 3);
      Assert.Equal(expBottomFlangeWidth, beam.BottomFlangeWidth.Millimeters, 3);
      Assert.Equal(expWebThickness, beam.WebThickness.Millimeters, 3);
      Assert.Equal(expTopFlangeThickness, beam.TopFlangeThickness.Millimeters, 3);
      Assert.Equal(expBottomFlangeThickness, beam.BottomFlangeThickness.Millimeters, 3);
      Assert.Equal(taperToNext, beam.TaperedToNext);
      Assert.Equal(expProfile, beam.SectionDescription);

      return beam;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(400, 300, 250, 12, 25, 20, true, 400, 300, 250, 12, 25, 20, "STD GI 400 300 250 12 25 20")]
    public BeamSection TestBeamSectionConstructorAsymmetric(double depth, double topFlangeWidth, double bottomFlangeWidth,
      double webThickness, double topFlangeThickness, double bottomFlangeThickness, bool taperToNext,
      double expDepth, double expTopFlangeWidth, double expBottomFlangeWidth, double expWebThickness,
      double expTopFlangeThickness, double expBottomFlangeThickness, string expProfile)
    {
      LengthUnit unit = LengthUnit.Millimeter;
      // 
      // 2 create object instance with constructor
      BeamSection beam = new BeamSection(new Length(depth, unit), new Length(topFlangeWidth, unit),
        new Length(bottomFlangeWidth, unit), new Length(webThickness, unit),
        new Length(topFlangeThickness, unit), new Length(bottomFlangeThickness, unit), taperToNext);

      // 3 check that inputs are set in object's members
      Assert.Equal(expDepth, beam.Depth.Millimeters, 3);
      Assert.Equal(expTopFlangeWidth, beam.TopFlangeWidth.Millimeters, 3);
      Assert.Equal(expBottomFlangeWidth, beam.BottomFlangeWidth.Millimeters, 3);
      Assert.Equal(expWebThickness, beam.WebThickness.Millimeters, 3);
      Assert.Equal(expTopFlangeThickness, beam.TopFlangeThickness.Millimeters, 3);
      Assert.Equal(expBottomFlangeThickness, beam.BottomFlangeThickness.Millimeters, 3);
      Assert.Equal(taperToNext, beam.TaperedToNext);
      Assert.Equal(expProfile, beam.SectionDescription);

      return beam;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(20, 19.05, 0.85, 1.27, false, "STD I(cm) 20 19.05 0.85 1.27")]
    public BeamSection TestBeamSectionConstructorSymmetricCM(
      double depth, double flangeWidth, double webThickness, double flangeThickness, bool taperToNext,
      string expProfile)
    {
      LengthUnit unit = LengthUnit.Centimeter;
      // 
      // 2 create object instance with constructor
      BeamSection beam = new BeamSection(new Length(depth, unit), new Length(flangeWidth, unit), new Length(webThickness, unit), new Length(flangeThickness, unit), taperToNext);

      // 3 check that inputs are set in object's members
      Assert.Equal(expProfile, beam.SectionDescription);

      return beam;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(40, 30, 25, 1.2, 2.5, 2, true, "STD GI(cm) 40 30 25 1.2 2.5 2")]
    public BeamSection TestBeamSectionConstructorAsymmetricCM(double depth, double topFlangeWidth, double bottomFlangeWidth,
      double webThickness, double topFlangeThickness, double bottomFlangeThickness, bool taperToNext,
      string expProfile)
    {
      LengthUnit unit = LengthUnit.Centimeter;
      // 
      // 2 create object instance with constructor
      BeamSection beam = new BeamSection(new Length(depth, unit), new Length(topFlangeWidth, unit),
        new Length(bottomFlangeWidth, unit), new Length(webThickness, unit),
        new Length(topFlangeThickness, unit), new Length(bottomFlangeThickness, unit), taperToNext);

      // 3 check that inputs are set in object's members
      Assert.Equal(expProfile, beam.SectionDescription);

      return beam;
    }

    [Fact]
    public void TestBeamSectionDuplicate1()
    {
      LengthUnit unit = LengthUnit.Millimeter;
      // 1 create with constructor and duplicate
      BeamSection original = new BeamSection(new Length(400, unit), new Length(300, unit),
        new Length(15, unit), new Length(12, unit), true);
      BeamSection duplicate = original.Duplicate() as BeamSection;

      // 2 check that duplicate has duplicated values
      Assert.Equal(400, duplicate.Depth.Millimeters);
      Assert.Equal(300, duplicate.TopFlangeWidth.Millimeters);
      Assert.Equal(300, duplicate.BottomFlangeWidth.Millimeters);
      Assert.Equal(15, duplicate.WebThickness.Millimeters);
      Assert.Equal(12, duplicate.TopFlangeThickness.Millimeters);
      Assert.Equal(12, duplicate.BottomFlangeThickness.Millimeters);
      Assert.True(duplicate.TaperedToNext);

      // 3 make some changes to duplicate
      duplicate.Depth = new Length(450, unit);
      duplicate.TopFlangeWidth = new Length(350, unit);
      duplicate.BottomFlangeWidth = new Length(370, unit);
      duplicate.WebThickness = new Length(12.5, unit);
      duplicate.TopFlangeThickness = new Length(10, unit);
      duplicate.BottomFlangeThickness = new Length(9, unit);
      duplicate.TaperedToNext = false;

      // 4 check that duplicate has set changes
      Assert.Equal(450, duplicate.Depth.Millimeters);
      Assert.Equal(350, duplicate.TopFlangeWidth.Millimeters);
      Assert.Equal(370, duplicate.BottomFlangeWidth.Millimeters);
      Assert.Equal(12.5, duplicate.WebThickness.Millimeters);
      Assert.Equal(10, duplicate.TopFlangeThickness.Millimeters);
      Assert.Equal(9, duplicate.BottomFlangeThickness.Millimeters);
      Assert.False(duplicate.TaperedToNext);

      // 5 check that original has not been changed
      Assert.Equal(400, original.Depth.Millimeters);
      Assert.Equal(300, original.TopFlangeWidth.Millimeters);
      Assert.Equal(300, original.BottomFlangeWidth.Millimeters);
      Assert.Equal(15, original.WebThickness.Millimeters);
      Assert.Equal(12, original.TopFlangeThickness.Millimeters);
      Assert.Equal(12, original.BottomFlangeThickness.Millimeters);
      Assert.True(original.TaperedToNext);
    }

    [Fact]
    public void TestBeamSectionDuplicate2()
    {
      LengthUnit unit = LengthUnit.Millimeter;
      // 1 create with new constructor and duplicate
      BeamSection original = new BeamSection(new Length(420, unit), new Length(310, unit), new Length(350, unit),
        new Length(10, unit), new Length(11, unit), new Length(12, unit), false);
      BeamSection duplicate = original.Duplicate() as BeamSection;

      // 2 check that duplicate has duplicated values
      Assert.Equal(420, duplicate.Depth.Millimeters);
      Assert.Equal(310, duplicate.TopFlangeWidth.Millimeters);
      Assert.Equal(350, duplicate.BottomFlangeWidth.Millimeters);
      Assert.Equal(10, duplicate.WebThickness.Millimeters);
      Assert.Equal(11, duplicate.TopFlangeThickness.Millimeters);
      Assert.Equal(12, duplicate.BottomFlangeThickness.Millimeters);
      Assert.False(duplicate.TaperedToNext);

      // 3 make some changes to duplicate
      duplicate.Depth = new Length(400, unit);
      duplicate.TopFlangeWidth = new Length(300, unit);
      duplicate.BottomFlangeWidth = new Length(290, unit);
      duplicate.WebThickness = new Length(9, unit);
      duplicate.TopFlangeThickness = new Length(10, unit);
      duplicate.BottomFlangeThickness = new Length(11, unit);
      duplicate.TaperedToNext = true;

      // 4 check that duplicate has set changes
      Assert.Equal(400, duplicate.Depth.Millimeters);
      Assert.Equal(300, duplicate.TopFlangeWidth.Millimeters);
      Assert.Equal(290, duplicate.BottomFlangeWidth.Millimeters);
      Assert.Equal(9, duplicate.WebThickness.Millimeters);
      Assert.Equal(10, duplicate.TopFlangeThickness.Millimeters);
      Assert.Equal(11, duplicate.BottomFlangeThickness.Millimeters);
      Assert.True(duplicate.TaperedToNext);

      // 5 check that original has not been changed
      Assert.Equal(420, original.Depth.Millimeters);
      Assert.Equal(310, original.TopFlangeWidth.Millimeters);
      Assert.Equal(350, original.BottomFlangeWidth.Millimeters);
      Assert.Equal(10, original.WebThickness.Millimeters);
      Assert.Equal(11, original.TopFlangeThickness.Millimeters);
      Assert.Equal(12, original.BottomFlangeThickness.Millimeters);
      Assert.False(original.TaperedToNext);
    }
  }
}
