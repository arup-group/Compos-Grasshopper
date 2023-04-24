using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ComposAPI.Results.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class BeamResultsTest {

    [Theory]
    [InlineData("04_SteelMaterial[Custom].coa", "04_SteelMaterail[Custom]", "Class 1", "Class 1")]
    [InlineData("05_SteelMaterial[AZCode].coa", "05_SteelMaterail[AZCode]", "Compact", "Compact")]
    public void BeamClassificationTest(string fileName, string memberName, string expectedClassPart, string expectedClassSection) {
      var file = ComposFile.Open(Path.GetFullPath(ResultsTest.RelativePath + fileName));
      Assert.Equal(0, file.Analyse(memberName));
      IMember member = file.GetMember(memberName);

      Assert.Equal(7, member.Result.Positions.Count);

      IBeamClassification res = member.Result.BeamClassification;
      foreach (string className in res.FlangeConstruction) {
        Assert.Equal(expectedClassPart, className);
      }
      foreach (string className in res.WebConstruction) {
        Assert.Equal(expectedClassPart, className);
      }
      foreach (string className in res.SectionConstruction) {
        Assert.Equal(expectedClassSection, className);
      }
      foreach (string className in res.Flange) {
        Assert.Equal(expectedClassPart, className);
      }
      foreach (string className in res.Web) {
        Assert.Equal(expectedClassPart, className);
      }
      foreach (string className in res.Section) {
        Assert.Equal(expectedClassSection, className);
      }
    }

    [Fact]
    public void BeamClassificationTest2() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamClassification res = r.BeamClassification;

      string expectedFlangeConstr = "Semi-compact";
      string expectedWebConstr = "Plastic";
      string expectedSectConstr = "Elastic";
      string expectedFlange = "Compact"; // this looks like a bug in Compos
      string expectedWeb = "Plastic";
      string expectedSect = "Plastic";

      foreach (string className in res.FlangeConstruction) {
        Assert.Equal(expectedFlangeConstr, className);
      }
      foreach (string className in res.WebConstruction) {
        Assert.Equal(expectedWebConstr, className);
      }
      foreach (string className in res.SectionConstruction) {
        Assert.Equal(expectedSectConstr, className);
      }
      foreach (string className in res.Flange) {
        Assert.Equal(expectedFlange, className);
      }
      foreach (string className in res.Web) {
        Assert.Equal(expectedWeb, className);
      }
      foreach (string className in res.Section) {
        Assert.Equal(expectedSect, className);
      }
    }

    [Fact]
    public void BottomFlangeStressAddDLTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;

      var expectedBottomAddDL = new List<double>()
       {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedBottomAddDL[i] * 1E-9, res.BottomFlangeFinalAdditionalDeadLoad[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }

    [Fact]
    public void BottomFlangeStressConstructionTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;

      var expectedBottomConst = new List<double>()
      {
        0.0,
        175.1E+6,
        280.2E+6,
        315.2E+6,
        280.2E+6,
        175.1E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedBottomConst[i] * 1E-9, res.BottomFlangeConstruction[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }

    [Fact]
    public void BottomFlangeStressFinalTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedBottomFinal = new List<double>()
      {
        0.0,
        201.2E+6,
        321.9E+6,
        362.1E+6,
        321.9E+6,
        201.2E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedBottomFinal[i] * 1E-9, res.BottomFlangeFinal[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }

    [Fact]
    public void BottomFlangeStressLLTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedBottomLL = new List<double>()
      {
        0.0,
        128.0E+6,
        204.7E+6,
        230.3E+6,
        204.7E+6,
        128.0E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedBottomLL[i] * 1E-9, res.BottomFlangeFinalLiveLoad[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }

    [Fact]
    public void BottomFlangeStressShrinkageTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedBottomShrink = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedBottomShrink[i] * 1E-9, res.BottomFlangeFinalShrinkage[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }

    [Fact]
    public void TopFlangeStressAddDLTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedTopAddDL = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedTopAddDL[i] * 1E-9, res.TopFlangeFinalAdditionalDeadLoad[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }

    [Fact]
    public void TopFlangeStressConstructionTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;

      var expectedTopConst = new List<double>()
      {
        0.0,
        -175.1E+6,
        -280.2E+6,
        -315.2E+6,
        -280.2E+6,
        -175.1E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedTopConst[i] * 1E-9, res.TopFlangeConstruction[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }

    [Fact]
    public void TopFlangeStressFinalTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedTopFinal = new List<double>()
      {
        0.0,
        -29.15E+6,
        -46.64E+6,
        -52.47E+6,
        -46.64E+6,
        -29.15E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedTopFinal[i] * 1E-9, res.TopFlangeFinal[i].NewtonsPerSquareMeter * 1E-9, 5);
      }
    }

    [Fact]
    public void TopFlangeStressLLTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedTopLL = new List<double>()
      {
        0.0,
        44.08E+6,
        70.52E+6,
        79.34E+6,
        70.52E+6,
        44.08E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedTopLL[i] * 1E-9, res.TopFlangeFinalLiveLoad[i].NewtonsPerSquareMeter * 1E-9, 5);
      }
    }

    [Fact]
    public void TopFlangeStressShrinkageTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedTopShrink = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedTopShrink[i] * 1E-9, res.TopFlangeFinalShrinkage[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }

    [Fact]
    public void WebStressAddDLTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedWebAddDL = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedWebAddDL[i] * 1E-9, res.WebFinalAdditionalDeadLoad[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }

    [Fact]
    public void WebStressConstructionTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;

      var expectedWebConst = new List<double>()
      {
        0.0,
        -159.4E+6,
        -255.0E+6,
        -286.9E+6,
        -255.0E+6,
        -159.4E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedWebConst[i] * 1E-9, res.WebConstruction[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }

    [Fact]
    public void WebStressFinalTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedWebFinal = new List<double>()
      {
        0.0,
        -18.81E+6,
        -30.09E+6,
        -33.85E+6,
        -30.09E+6,
        -18.81E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedWebFinal[i] * 1E-9, res.WebFinal[i].NewtonsPerSquareMeter * 1E-9, 5);
      }
    }

    [Fact]
    public void WebStressLLTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedWebLL = new List<double>()
      {
        0.0,
        47.84E+6,
        76.55E+6,
        86.12E+6,
        76.55E+6,
        47.84E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedWebLL[i] * 1E-9, res.WebFinalLiveLoad[i].NewtonsPerSquareMeter * 1E-9, 5);
      }
    }

    [Fact]
    public void WebStressShrinkageTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;
      var expectedWebShrink = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedWebShrink[i] * 1E-9, res.WebFinalShrinkage[i].NewtonsPerSquareMeter * 1E-9, 4);
      }
    }
  }
}
