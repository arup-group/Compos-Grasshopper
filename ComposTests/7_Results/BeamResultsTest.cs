using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using ComposAPI.Helpers;
using ComposAPI.Tests;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using ComposGHTests.Slab;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Results.Tests
{
  [Collection("ComposAPI Fixture collection")]

  public class BeamResultsTest
  {
    [Theory]
    [InlineData("04_SteelMaterial[Custom].coa", "04_SteelMaterail[Custom]", "Class 1", "Class 1")]
    [InlineData("05_SteelMaterial[AZCode].coa", "05_SteelMaterail[AZCode]", "Compact", "Compact")]
    public void BeamClassificationTest(string fileName, string memberName, string expectedClassPart, string expectedClassSection)
    {
      ComposFile file = ComposFile.Open(Path.GetFullPath(ResultsTest.RelativePath + fileName));
      Assert.Equal(0, file.Analyse(memberName));
      IMember member = file.GetMember(memberName);

      Assert.Equal(7, member.Result.Positions.Count);

      IBeamClassification res = member.Result.BeamClassification;
      foreach (string className in res.FlangeConstruction)
        Assert.Equal(expectedClassPart, className);
      foreach (string className in res.WebConstruction)
        Assert.Equal(expectedClassPart, className);
      foreach (string className in res.SectionConstruction)
        Assert.Equal(expectedClassSection, className);
      foreach (string className in res.Flange)
        Assert.Equal(expectedClassPart, className);
      foreach (string className in res.Web)
        Assert.Equal(expectedClassPart, className);
      foreach (string className in res.Section)
        Assert.Equal(expectedClassSection, className);
    }

    [Fact]
    public void BeamClassificationTest2()
    {
      IBeamClassification res = ResultsTest.ResultMember.Result.BeamClassification;

      string expectedFlangeConstr = "Semi-compact";
      string expectedWebConstr = "Plastic";
      string expectedSectConstr = "Elastic";
      string expectedFlange = "Compact"; // this looks like a bug in Compos
      string expectedWeb = "Plastic";
      string expectedSect = "Plastic";

      foreach (string className in res.FlangeConstruction)
        Assert.Equal(expectedFlangeConstr, className);
      foreach (string className in res.WebConstruction)
        Assert.Equal(expectedWebConstr, className);
      foreach (string className in res.SectionConstruction)
        Assert.Equal(expectedSectConstr, className);
      foreach (string className in res.Flange)
        Assert.Equal(expectedFlange, className);
      foreach (string className in res.Web)
        Assert.Equal(expectedWeb, className);
      foreach (string className in res.Section)
        Assert.Equal(expectedSect, className);
    }

    [Fact]
    public void BeamStressTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IBeamStressResult res = r.BeamStresses;

      List<double> expectedBottomConst = new List<double>()
      {
        0.0,
        175.1E+6,
        280.2E+6,
        315.2E+6,
        280.2E+6,
        175.1E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBottomConst[i] * Math.Pow(10, -9), 
          res.BottomFlangeConstruction[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedWebConst = new List<double>()
      {
        0.0,
        -159.4E+6,
        -255.0E+6,
        -286.9E+6,
        -255.0E+6,
        -159.4E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedWebConst[i] * Math.Pow(10, -9), 
          res.WebConstruction[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedTopConst = new List<double>()
      {
        0.0,
        -175.1E+6,
        -280.2E+6,
        -315.2E+6,
        -280.2E+6,
        -175.1E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedTopConst[i] * Math.Pow(10, -9), 
          res.TopFlangeConstruction[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedBottomAddDL = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBottomAddDL[i] * Math.Pow(10, -9),
          res.BottomFlangeFinalAdditionalDeadLoad[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedWebAddDL = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedWebAddDL[i] * Math.Pow(10, -9),
          res.WebFinalAdditionalDeadLoad[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedTopAddDL = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedTopAddDL[i] * Math.Pow(10, -9),
          res.TopFlangeFinalAdditionalDeadLoad[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedBottomLL = new List<double>()
      {
        0.0,
        128.0E+6,
        204.7E+6,
        230.3E+6,
        204.7E+6,
        128.0E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBottomLL[i] * Math.Pow(10, -9),
          res.BottomFlangeFinalLiveLoad[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedWebLL = new List<double>()
      {
        0.0,
        47.84E+6,
        76.55E+6,
        86.12E+6,
        76.55E+6,
        47.84E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedWebLL[i] * Math.Pow(10, -9),
          res.WebFinalLiveLoad[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 5);

      List<double> expectedTopLL = new List<double>()
      {
        0.0,
        44.08E+6,
        70.52E+6,
        79.34E+6,
        70.52E+6,
        44.08E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedTopLL[i] * Math.Pow(10, -9),
          res.TopFlangeFinalLiveLoad[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 5);

      List<double> expectedBottomShrink = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBottomShrink[i] * Math.Pow(10, -9),
          res.BottomFlangeFinalShrinkage[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedWebShrink = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedWebShrink[i] * Math.Pow(10, -9),
          res.WebFinalShrinkage[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedTopShrink = new List<double>()
      {
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedTopShrink[i] * Math.Pow(10, -9),
          res.TopFlangeFinalShrinkage[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedBottomFinal = new List<double>()
      {
        0.0,
        201.2E+6,
        321.9E+6,
        362.1E+6,
        321.9E+6,
        201.2E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBottomFinal[i] * Math.Pow(10, -9),
          res.BottomFlangeFinal[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 4);

      List<double> expectedWebFinal = new List<double>()
      {
        0.0,
        -18.81E+6,
        -30.09E+6,
        -33.85E+6,
        -30.09E+6,
        -18.81E+6,
        0.0

      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedWebFinal[i] * Math.Pow(10, -9),
          res.WebFinal[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 5);

      List<double> expectedTopFinal = new List<double>()
      {
        0.0,
        -29.15E+6,
        -46.64E+6,
        -52.47E+6,
        -46.64E+6,
        -29.15E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedTopFinal[i] * Math.Pow(10, -9),
          res.TopFlangeFinal[i].NewtonsPerSquareMeter * Math.Pow(10, -9), 5);
    }
  }
}
