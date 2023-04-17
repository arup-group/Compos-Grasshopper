using ComposGHTests.Helpers;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Members.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class DesignCriteriaTests {

    [Fact]
    public void CheckSetCatalogueSectionIDs() {
      DesignCriteria designCriteria = new DesignCriteria();

      List<int> wrongIDs = new List<int>() { 4, 5 };

      Assert.Throws<System.Exception>(() => designCriteria.CatalogueSectionTypes = wrongIDs);
    }

    [Fact]
    public void ConstructorTest1() {
      BeamSizeLimits beamSizeLimits = new BeamSizeLimits(0.1, 1.1, 0.2, 0.6, LengthUnit.Meter);
      OptimiseOption optimiseOption = OptimiseOption.MinimumHeight;
      List<int> catalogues = new List<int>() { 1, 2, 3 };
      DesignCriteria designCriteria = new DesignCriteria(beamSizeLimits, optimiseOption, catalogues);

      Duplicates.AreEqual(beamSizeLimits, designCriteria.BeamSizeLimits);
      Assert.Equal(OptimiseOption.MinimumHeight, designCriteria.OptimiseOption);
      Assert.Equal(3, designCriteria.CatalogueSectionTypes.Count);
      Assert.Null(designCriteria.ConstructionDeadLoad);
      Assert.Null(designCriteria.AdditionalDeadLoad);
      Assert.Null(designCriteria.FinalLiveLoad);
      Assert.Null(designCriteria.TotalLoads);
      Assert.Null(designCriteria.PostConstruction);
      Assert.Null(designCriteria.FrequencyLimits);
    }

    [Fact]
    public DesignCriteria ConstructorTest2() {
      BeamSizeLimits beamSizeLimits = new BeamSizeLimits(0.1, 1.1, 0.2, 0.6, LengthUnit.Meter);
      OptimiseOption optimiseOption = OptimiseOption.MinimumHeight;
      List<int> catalogues = new List<int>() { 1, 2, 3, 4 };
      DeflectionLimit constructionDL = new DeflectionLimit(500);
      DeflectionLimit additionalDL = new DeflectionLimit(300);
      DeflectionLimit finalLL = new DeflectionLimit(400);
      DeflectionLimit total = new DeflectionLimit(360);
      DeflectionLimit postConstr = new DeflectionLimit(200);
      FrequencyLimits frequencyLimits = new FrequencyLimits(5, 90, 15);
      DesignCriteria designCriteria = new DesignCriteria(beamSizeLimits, optimiseOption, catalogues, constructionDL, additionalDL, finalLL, total, postConstr, frequencyLimits);

      Duplicates.AreEqual(beamSizeLimits, designCriteria.BeamSizeLimits);
      Assert.Equal(OptimiseOption.MinimumHeight, designCriteria.OptimiseOption);
      Assert.Equal(4, designCriteria.CatalogueSectionTypes.Count);
      Duplicates.AreEqual(constructionDL, designCriteria.ConstructionDeadLoad);
      Duplicates.AreEqual(additionalDL, designCriteria.AdditionalDeadLoad);
      Duplicates.AreEqual(finalLL, designCriteria.FinalLiveLoad);
      Duplicates.AreEqual(total, designCriteria.TotalLoads);
      Duplicates.AreEqual(postConstr, designCriteria.PostConstruction);
      Duplicates.AreEqual(frequencyLimits, designCriteria.FrequencyLimits);

      return designCriteria;
    }

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      DesignCriteria original = ConstructorTest2();
      DesignCriteria duplicate = (DesignCriteria)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void EmptyConstructorTest() {
      DesignCriteria designCriteria = new DesignCriteria();

      Duplicates.AreEqual(new BeamSizeLimits(), designCriteria.BeamSizeLimits);
      Assert.Equal(OptimiseOption.MinimumWeight, designCriteria.OptimiseOption);
      Assert.Equal(0, designCriteria.CatalogueSectionTypes.Count);
    }

    [Fact]
    public void FromCoaStringTest() {
      // Assemble
      string coaString =
        "CRITERIA_DEF_LIMIT	MEMBER-1	CONSTRUCTION_DEAD_LOAD	ABSOLUTE	30.0000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	CONSTRUCTION_DEAD_LOAD	SPAN/DEF_RATIO	360.000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	ADDITIONAL_DEAD_LOAD	ABSOLUTE	10.0000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	ADDITIONAL_DEAD_LOAD	SPAN/DEF_RATIO	300.000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	FINAL_LIVE_LOAD	ABSOLUTE	20.0000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	FINAL_LIVE_LOAD	SPAN/DEF_RATIO	500.000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	TOTAL	ABSOLUTE	35.0000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	TOTAL	SPAN/DEF_RATIO	200.000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	POST_CONSTRUCTION	ABSOLUTE	15.0000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	POST_CONSTRUCTION	SPAN/DEF_RATIO	340.000\n" +
        "CRITERIA_BEAM_SIZE_LIMIT	MEMBER-1	20.0000	100.000	10.0000	50.0000\n" +
        "CRITERIA_OPTIMISE_OPTION	MEMBER-1	MINIMUM_WEIGHT\n" +
        "CRITERIA_SECTION_TYPE	MEMBER-1	93\n" +
        "CRITERIA_FREQUENCY	MEMBER-1	CHECK_NATURAL_FREQUENCY	4.00000	1.00000	0.100000\n";

      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Displacement = LengthUnit.Millimeter;
      units.Section = LengthUnit.Centimeter;
      DeflectionLimit constrLim = new DeflectionLimit() {
        AbsoluteDeflection = new Length(30, LengthUnit.Millimeter),
        SpanOverDeflectionRatio = new Ratio(360, RatioUnit.DecimalFraction)
      };
      DeflectionLimit addLim = new DeflectionLimit() {
        AbsoluteDeflection = new Length(10, LengthUnit.Millimeter),
        SpanOverDeflectionRatio = new Ratio(300, RatioUnit.DecimalFraction)
      };
      DeflectionLimit finalLim = new DeflectionLimit() {
        AbsoluteDeflection = new Length(20, LengthUnit.Millimeter),
        SpanOverDeflectionRatio = new Ratio(500, RatioUnit.DecimalFraction)
      };
      DeflectionLimit totalLim = new DeflectionLimit() {
        AbsoluteDeflection = new Length(35, LengthUnit.Millimeter),
        SpanOverDeflectionRatio = new Ratio(200, RatioUnit.DecimalFraction)
      };
      DeflectionLimit postLim = new DeflectionLimit() {
        AbsoluteDeflection = new Length(15, LengthUnit.Millimeter),
        SpanOverDeflectionRatio = new Ratio(340, RatioUnit.DecimalFraction)
      };
      BeamSizeLimits beamSizeLimits = new BeamSizeLimits(20, 100, 10, 50, LengthUnit.Centimeter);
      List<int> cats = new List<int>() { 93 };
      FrequencyLimits frequencyLimits = new FrequencyLimits(4, 100, 10);

      DesignCriteria expectedDesignCriteria = new DesignCriteria(beamSizeLimits, OptimiseOption.MinimumWeight, cats, constrLim, addLim, finalLim, totalLim, postLim, frequencyLimits);

      // Act
      IDesignCriteria designCriteria = DesignCriteria.FromCoaString(coaString, "MEMBER-1", units);

      // Assert
      Duplicates.AreEqual(expectedDesignCriteria, designCriteria);
    }

    [Fact]
    public void ToCoaStringTest() {
      // Assemble
      string expectedCoaString =
        "CRITERIA_DEF_LIMIT	MEMBER-1	CONSTRUCTION_DEAD_LOAD	ABSOLUTE	30.0000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	CONSTRUCTION_DEAD_LOAD	SPAN/DEF_RATIO	360.000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	ADDITIONAL_DEAD_LOAD	ABSOLUTE	10.0000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	ADDITIONAL_DEAD_LOAD	SPAN/DEF_RATIO	300.000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	FINAL_LIVE_LOAD	ABSOLUTE	20.0000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	FINAL_LIVE_LOAD	SPAN/DEF_RATIO	500.000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	TOTAL	ABSOLUTE	35.0000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	TOTAL	SPAN/DEF_RATIO	200.000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	POST_CONSTRUCTION	ABSOLUTE	15.0000\n" +
        "CRITERIA_DEF_LIMIT	MEMBER-1	POST_CONSTRUCTION	SPAN/DEF_RATIO	340.000\n" +
        "CRITERIA_BEAM_SIZE_LIMIT	MEMBER-1	20.0000	100.000	10.0000	50.0000\n" +
        "CRITERIA_OPTIMISE_OPTION	MEMBER-1	MINIMUM_WEIGHT\n" +
        "CRITERIA_SECTION_TYPE	MEMBER-1	93\n" +
        "CRITERIA_FREQUENCY	MEMBER-1	CHECK_NATURAL_FREQUENCY	4.00000	1.00000	0.100000\n";

      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Displacement = LengthUnit.Millimeter;
      units.Section = LengthUnit.Centimeter;
      DeflectionLimit constrLim = new DeflectionLimit() {
        AbsoluteDeflection = new Length(30, LengthUnit.Millimeter),
        SpanOverDeflectionRatio = new Ratio(360, RatioUnit.DecimalFraction)
      };
      DeflectionLimit addLim = new DeflectionLimit() {
        AbsoluteDeflection = new Length(10, LengthUnit.Millimeter),
        SpanOverDeflectionRatio = new Ratio(300, RatioUnit.DecimalFraction)
      };
      DeflectionLimit finalLim = new DeflectionLimit() {
        AbsoluteDeflection = new Length(20, LengthUnit.Millimeter),
        SpanOverDeflectionRatio = new Ratio(500, RatioUnit.DecimalFraction)
      };
      DeflectionLimit totalLim = new DeflectionLimit() {
        AbsoluteDeflection = new Length(35, LengthUnit.Millimeter),
        SpanOverDeflectionRatio = new Ratio(200, RatioUnit.DecimalFraction)
      };
      DeflectionLimit postLim = new DeflectionLimit() {
        AbsoluteDeflection = new Length(15, LengthUnit.Millimeter),
        SpanOverDeflectionRatio = new Ratio(340, RatioUnit.DecimalFraction)
      };
      BeamSizeLimits beamSizeLimits = new BeamSizeLimits(20, 100, 10, 50, LengthUnit.Centimeter);
      List<int> cats = new List<int>() { 93 };
      FrequencyLimits frequencyLimits = new FrequencyLimits(4, 100, 10);

      DesignCriteria designCriteria = new DesignCriteria(beamSizeLimits, OptimiseOption.MinimumWeight, cats, constrLim, addLim, finalLim, totalLim, postLim, frequencyLimits);

      // Act
      string coaString = designCriteria.ToCoaString("MEMBER-1", units);

      // Assert
      Assert.Equal(expectedCoaString, coaString);
    }
  }
}
