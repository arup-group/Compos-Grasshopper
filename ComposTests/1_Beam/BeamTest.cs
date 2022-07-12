﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using Xunit;
using ComposAPITests.Helpers;

namespace ComposAPI.Beams.Tests
{
  public class BeamTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(6)]
    public void ConstructorTest(double length)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();

      // 2 create object instance with constructor
      Restraint restraint = new Restraint();
      SteelMaterial material = new SteelMaterial();
      List<IBeamSection> sections = new List<IBeamSection>() { new BeamSection() };
      List<IWebOpening> openings = new List<IWebOpening>() { new WebOpening() };

      Beam beam = new Beam(new Length(length, units.Length), restraint, material, sections, openings);

      // 3 check that inputs are set in object's members
      Assert.Equal(length, beam.Length.Value);
      Assert.Equal(restraint, beam.Restraint);
      Assert.Equal(material, beam.Material);
      Assert.Equal(sections, beam.Sections);
      Assert.Equal(openings, beam.WebOpenings);
    }

    [Fact]
    public void ToCoaStringTest()
    {
      // Assemble
      Beam beam = Example1Beam();

      // Act
      string coaString = beam.ToCoaString("MEMBER-1", Code.EN1994_1_1_2004, ComposUnits.GetStandardUnits());
      
      // Assert
      Assert.Equal(Example1CoaString(), coaString);
    }

    [Fact]
    public void FromCoaStringTest()
    {
      // Assemble 
      Beam expectedBeam = Example1Beam();

      // Act
      Beam beam = (Beam)Beam.FromCoaString(Example1CoaString(), "MEMBER-1", ComposUnits.GetStandardUnits());

      // Assert
      Assert.Equal(expectedBeam.Length.Millimeters, beam.Length.Millimeters);

      ISupports expectedCStage = expectedBeam.Restraint.ConstructionStageSupports;
      ISupports cStage = beam.Restraint.ConstructionStageSupports;
      Assert.Equal(expectedCStage.IntermediateRestraintPositions, cStage.IntermediateRestraintPositions);
      Assert.Equal(expectedCStage.CustomIntermediateRestraintPositions, cStage.CustomIntermediateRestraintPositions);
      Assert.Equal(expectedCStage.SecondaryMemberIntermediateRestraint, cStage.SecondaryMemberIntermediateRestraint);
      Assert.Equal(expectedCStage.BothFlangesFreeToRotateOnPlanAtEnds, cStage.BothFlangesFreeToRotateOnPlanAtEnds);
      ISupports expectedFStage = expectedBeam.Restraint.FinalStageSupports;
      ISupports fStage = beam.Restraint.FinalStageSupports;
      Assert.Equal(expectedFStage.IntermediateRestraintPositions, fStage.IntermediateRestraintPositions);
      Assert.Equal(expectedFStage.CustomIntermediateRestraintPositions, fStage.CustomIntermediateRestraintPositions);
      Assert.Equal(expectedFStage.SecondaryMemberIntermediateRestraint, fStage.SecondaryMemberIntermediateRestraint);
      Assert.Equal(expectedFStage.BothFlangesFreeToRotateOnPlanAtEnds, fStage.BothFlangesFreeToRotateOnPlanAtEnds);

      ISteelMaterial expectedMat = expectedBeam.Material;
      ISteelMaterial mat = beam.Material;
      Assert.Equal(expectedMat.Grade, mat.Grade);
      Assert.Equal(expectedMat.fy, mat.fy);
      Assert.Equal(expectedMat.Density, mat.Density);
      Assert.Equal(expectedMat.E, mat.E);
      Assert.Equal(expectedMat.isCustom, mat.isCustom);
      Assert.Equal(expectedMat.ReductionFactorMpl, mat.ReductionFactorMpl);
      Assert.Equal(expectedMat.WeldGrade, mat.WeldGrade);

      for (int i = 0; i < expectedBeam.Sections.Count; i++)
      {
        IBeamSection expectedSection = expectedBeam.Sections[i];
        IBeamSection section = beam.Sections[i];
        Assert.Equal(expectedSection.SectionDescription, section.SectionDescription);
        Assert.Equal(expectedSection.StartPosition, section.StartPosition);
        Assert.Equal(expectedSection.isCatalogue, section.isCatalogue);
        Assert.Equal(expectedSection.TaperedToNext, section.TaperedToNext);
      }
     
      if (expectedBeam.WebOpenings != null)
      {
        for (int i = 0; i < expectedBeam.WebOpenings.Count; i++)
        {
          IWebOpening expectedOpening = expectedBeam.WebOpenings[i];
          IWebOpening webOpening = beam.WebOpenings[i];
          Assert.Equal(expectedOpening.WebOpeningType, webOpening.WebOpeningType);
          Assert.Equal(expectedOpening.Width, webOpening.Width);
          Assert.Equal(expectedOpening.Diameter, webOpening.Diameter);
          Assert.Equal(expectedOpening.Height, webOpening.Height);
          Assert.Equal(expectedOpening.CentroidPosFromStart, webOpening.CentroidPosFromStart);
          Assert.Equal(expectedOpening.CentroidPosFromTop, webOpening.CentroidPosFromTop);
          if (expectedOpening.OpeningStiffeners != null)
          {
            IWebOpeningStiffeners expStiffener = expectedOpening.OpeningStiffeners;
            IWebOpeningStiffeners stiffener = webOpening.OpeningStiffeners;
            Assert.Equal(expStiffener.DistanceFrom, stiffener.DistanceFrom);
            Assert.Equal(expStiffener.TopStiffenerWidth, stiffener.TopStiffenerWidth);
            Assert.Equal(expStiffener.TopStiffenerThickness, stiffener.TopStiffenerThickness);
            Assert.Equal(expStiffener.BottomStiffenerWidth, stiffener.BottomStiffenerWidth);
            Assert.Equal(expStiffener.BottomStiffenerThickness, stiffener.TopStiffenerThickness);
            Assert.Equal(expStiffener.isBothSides, stiffener.isBothSides);
            Assert.Equal(expStiffener.isNotch, stiffener.isNotch);
          }
        }
      }
    }

    

    internal string Example1CoaString()
    {
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

    internal Beam Example1Beam()
    {
      Supports constructionStageSupport = new Supports(IntermediateRestraint.Third_Points, true, true);

      Restraint restraint = new Restraint(false, constructionStageSupport);

      SteelMaterial steelMaterial = new SteelMaterial(StandardSteelGrade.S355, Code.EN1994_1_1_2004);

      BeamSection section1 = new BeamSection(
        new Length(600, LengthUnit.Millimeter),
        new Length(200, LengthUnit.Millimeter),
        new Length(15, LengthUnit.Millimeter),
        new Length(25, LengthUnit.Millimeter));
      section1.TaperedToNext = true;

      BeamSection section2 = new BeamSection(
      new Length(900, LengthUnit.Millimeter),
      new Length(200, LengthUnit.Millimeter),
      new Length(15, LengthUnit.Millimeter),
      new Length(25, LengthUnit.Millimeter));
      section2.TaperedToNext = true;
      section2.StartPosition = new Ratio(50, RatioUnit.Percent);

      BeamSection section3 = new BeamSection(
      new Length(600, LengthUnit.Millimeter),
      new Length(200, LengthUnit.Millimeter),
      new Length(15, LengthUnit.Millimeter),
      new Length(25, LengthUnit.Millimeter));
      section3.TaperedToNext = true;
      section3.StartPosition = new Ratio(100, RatioUnit.Percent);

      List<IBeamSection> sections = new List<IBeamSection>();
      sections.Add(section1);
      sections.Add(section2);
      sections.Add(section3);

      WebOpening rect1 = new WebOpening(
        new Length(0.4, LengthUnit.Meter),
        new Length(0.3, LengthUnit.Meter),
        new Ratio(20, RatioUnit.Percent), new Ratio(50, RatioUnit.Percent));

      WebOpening circ1 = new WebOpening(
        new Length(0.4, LengthUnit.Meter),
        new Length(3.5, LengthUnit.Meter), new Ratio(50, RatioUnit.Percent));

      WebOpening rightNotch = new WebOpening(
        new Length(0.4, LengthUnit.Meter),
        new Length(0.3, LengthUnit.Meter), NotchPosition.End);

      WebOpening leftNotch = new WebOpening(
        new Length(0.4, LengthUnit.Meter),
        new Length(0.3, LengthUnit.Meter), NotchPosition.Start);

      WebOpeningStiffeners oneSideStiffener = new WebOpeningStiffeners(
        new Length(0.05, LengthUnit.Meter),
        new Length(0.1, LengthUnit.Meter),
        new Length(0.01, LengthUnit.Meter),
        new Length(0.1, LengthUnit.Meter),
        new Length(0.01, LengthUnit.Meter),
        false);

      WebOpening rect2 = new WebOpening(
        new Length(0.4, LengthUnit.Meter),
        new Length(0.3, LengthUnit.Meter),
        new Length(5.5, LengthUnit.Meter), new Length(0.4, LengthUnit.Meter),
        oneSideStiffener);

      WebOpeningStiffeners bothSideStiffener = new WebOpeningStiffeners(
        new Length(0.06, LengthUnit.Meter),
        new Length(0.15, LengthUnit.Meter),
        new Length(0.02, LengthUnit.Meter),
        new Length(0.15, LengthUnit.Meter),
        new Length(0.02, LengthUnit.Meter),
        true);

      WebOpening circ2 = new WebOpening(
        new Length(0.3, LengthUnit.Meter),
        new Ratio(80, RatioUnit.Percent), new Length(0.35, LengthUnit.Meter), 
        bothSideStiffener);

      List<IWebOpening> webOpenings = new List<IWebOpening>();
      webOpenings.Add(rect1);
      webOpenings.Add(circ1);
      webOpenings.Add(rightNotch);
      webOpenings.Add(leftNotch);
      webOpenings.Add(rect2);
      webOpenings.Add(circ2);

      Beam beam = new Beam(
        new Length(9, LengthUnit.Meter),
        restraint,
        steelMaterial,
        sections,
        webOpenings);

      return beam;
    }

    
  }
}
