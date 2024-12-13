using System.Collections.Generic;
using ComposGH.Helpers;
using ComposGHTests.Helper;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Beams.Tests {
  public static class BeamMother {

    public static Beam Example1Beam() {
      var constructionStageSupport = new Supports(IntermediateRestraint.Third_Points, true, true);

      var restraint = new Restraint(false, constructionStageSupport);

      var steelMaterial = new SteelMaterial(StandardSteelGrade.S355, Code.EN1994_1_1_2004);

      var section1 = new BeamSection(new Length(600, LengthUnit.Millimeter), new Length(200, LengthUnit.Millimeter), new Length(15, LengthUnit.Millimeter), new Length(25, LengthUnit.Millimeter)) {
        TaperedToNext = true
      };

      var section2 = new BeamSection(new Length(900, LengthUnit.Millimeter), new Length(200, LengthUnit.Millimeter), new Length(15, LengthUnit.Millimeter), new Length(25, LengthUnit.Millimeter)) {
        TaperedToNext = true,
        StartPosition = new Ratio(50, RatioUnit.Percent)
      };

      var section3 = new BeamSection(new Length(600, LengthUnit.Millimeter), new Length(200, LengthUnit.Millimeter), new Length(15, LengthUnit.Millimeter), new Length(25, LengthUnit.Millimeter)) {
        TaperedToNext = true,
        StartPosition = new Ratio(100, RatioUnit.Percent)
      };

      var sections = new List<IBeamSection>() { section1, section2, section3 };

      var rect1 = new WebOpening(new Length(0.4, LengthUnit.Meter), new Length(0.3, LengthUnit.Meter), new Ratio(20, RatioUnit.Percent), new Ratio(50, RatioUnit.Percent));

      var circ1 = new WebOpening(new Length(0.4, LengthUnit.Meter), new Length(3.5, LengthUnit.Meter), new Ratio(50, RatioUnit.Percent));

      var rightNotch = new WebOpening(new Length(0.4, LengthUnit.Meter), new Length(0.3, LengthUnit.Meter), NotchPosition.End);

      var leftNotch = new WebOpening(new Length(0.4, LengthUnit.Meter), new Length(0.3, LengthUnit.Meter), NotchPosition.Start);

      var oneSideStiffener = new WebOpeningStiffeners(new Length(0.05, LengthUnit.Meter), new Length(0.1, LengthUnit.Meter), new Length(0.01, LengthUnit.Meter), new Length(0.1, LengthUnit.Meter), new Length(0.01, LengthUnit.Meter), false);

      var rect2 = new WebOpening(new Length(0.4, LengthUnit.Meter), new Length(0.3, LengthUnit.Meter), new Length(5.5, LengthUnit.Meter), new Length(0.4, LengthUnit.Meter), oneSideStiffener);

      var bothSideStiffener = new WebOpeningStiffeners(new Length(0.06, LengthUnit.Meter), new Length(0.15, LengthUnit.Meter), new Length(0.02, LengthUnit.Meter), new Length(0.15, LengthUnit.Meter), new Length(0.02, LengthUnit.Meter), true);

      var circ2 = new WebOpening(new Length(0.3, LengthUnit.Meter), new Ratio(80, RatioUnit.Percent), new Length(0.35, LengthUnit.Meter), bothSideStiffener);

      var webOpenings = new List<IWebOpening>() { rect1, circ1, rightNotch, leftNotch, rect2, circ2 };

      return new Beam(new Length(9, LengthUnit.Meter), restraint, steelMaterial, sections, webOpenings);
    }

    public static string Example1CoaString() {
      return
        "BEAM_STEEL_MATERIAL_STD	MEMBER-1	S355 (EN)" + '\n' +
        "BEAM_WELDING_MATERIAL	MEMBER-1	Grade 42" + '\n' +
        "BEAM_SPAN_LENGTH	MEMBER-1	1	9.00000" + '\n' +
        "BEAM_SECTION_AT_X	MEMBER-1	3	1	0.000000	STD I 600 200 15 25	TAPERED_YES" + '\n' +
        "BEAM_SECTION_AT_X	MEMBER-1	3	2	-0.500000	STD I 900 200 15 25	TAPERED_YES" + '\n' +
        "BEAM_SECTION_AT_X	MEMBER-1	3	3	-1.00000	STD I 600 200 15 25	TAPERED_YES" + '\n' +
        "RESTRAINT_POINT	MEMBER-1	STANDARD	2" + '\n' +
        "RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FREE" + '\n' +
        "RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST" + '\n' +
        "END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE" + '\n' +
        "FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	0" + '\n' +
        "FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FIXED" + '\n' +
        "FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST" + '\n' +
        "FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE" + '\n' +
        "WEB_OPEN_DIMENSION	MEMBER-1	RECTANGULAR	0.400000	0.300000	20.0000%	50.0000%	STIFFENER_NO" + '\n' +
        "WEB_OPEN_DIMENSION	MEMBER-1	CIRCULAR	0.400000	0.400000	3.50000	50.0000%	STIFFENER_NO" + '\n' +
        "WEB_OPEN_DIMENSION	MEMBER-1	RIGHT_NOTCH	0.400000	0.300000	50.0000%	50.0000%	STIFFENER_NO" + '\n' +
        "WEB_OPEN_DIMENSION	MEMBER-1	LEFT_NOTCH	0.400000	0.300000	50.0000%	50.0000%	STIFFENER_NO" + '\n' +
        "WEB_OPEN_DIMENSION	MEMBER-1	RECTANGULAR	0.400000	0.300000	5.50000	0.400000	STIFFENER_YES	ONE_SIDE_STIFFENER	0.0500000	0.100000	0.0100000	0.100000	0.0100000" + '\n' +
        "WEB_OPEN_DIMENSION	MEMBER-1	CIRCULAR	0.300000	0.300000	80.0000%	0.350000	STIFFENER_YES	BOTH_SIDE_STIFFENER	0.0600000	0.150000	0.0200000	0.150000	0.0200000" + '\n';
    }
  }

  [Collection("ComposAPI Fixture collection")]
  public class BeamTest {

    // 1 setup inputs
    [Theory]
    [InlineData(6)]
    public void ConstructorTest(double length) {
      var units = ComposUnits.GetStandardUnits();

      // 2 create object instance with constructor
      var restraint = new Restraint();
      var material = new SteelMaterial();
      var sections = new List<IBeamSection>() { new BeamSection() };
      var openings = new List<IWebOpening>() { new WebOpening() };

      var beam = new Beam(new Length(length, units.Length), restraint, material, sections, openings);

      // 3 check that inputs are set in object's members
      Assert.Equal(length, beam.Length.Value);
      Assert.Equal(restraint, beam.Restraint);
      Assert.Equal(material, beam.Material);
      Assert.Equal(sections, beam.Sections);
      Assert.Equal(openings, beam.WebOpenings);
    }

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      Beam original = BeamMother.Example1Beam();
      var duplicate = (Beam)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void FromCoaStringTest() {
      // Assemble
      Beam expectedBeam = BeamMother.Example1Beam();

      // Act
      var beam = (Beam)Beam.FromCoaString(BeamMother.Example1CoaString(), "MEMBER-1", ComposUnits.GetStandardUnits(), Code.EN1994_1_1_2004);

      // Assert
      ObjectExtension.Equals(expectedBeam, beam);
    }

    [Fact]
    public void ToCoaStringTest() {
      // Assemble
      Beam beam = BeamMother.Example1Beam();

      // Act
      string coaString = beam.ToCoaString("MEMBER-1", Code.EN1994_1_1_2004, ComposUnits.GetStandardUnits());

      // Assert
      Assert.Equal(BeamMother.Example1CoaString(), coaString);
    }
  }
}
