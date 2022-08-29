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

  public class CompositePropertiesResultsTest
  {
    [Fact]
    public void PropertiesTests()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICompositeSectionProperties res = r.SectionProperties;

      List<double> expectedWeldTHKtop = new List<double>()
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
        Assert.Equal(expectedWeldTHKtop[i],
          res.GirderWeldThicknessTop[i].Meters, 4);

      List<double> expectedWeldTHKbot = new List<double>()
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
        Assert.Equal(expectedWeldTHKbot[i],
          res.GirderWeldThicknessBottom[i].Meters, 4);

      List<double> expectedslabwidthLeft = new List<double>()
      {
        0.0,
        1.0,
        1.0,
        1.0,
        1.0,
        1.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedslabwidthLeft[i],
          res.EffectiveSlabWidthLeft[i].Meters, 4);

      List<double> expectedslabwidthRight = new List<double>()
      {
        0.0,
        1.0,
        1.0,
        1.0,
        1.0,
        1.0,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedslabwidthRight[i],
          res.EffectiveSlabWidthRight[i].Meters, 4);

      List<double> expectedBeamI = new List<double>()
      {
        19.67E-6,
        19.67E-6,
        19.67E-6,
        19.67E-6,
        19.67E-6,
        19.67E-6,
        19.67E-6
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBeamI[i],
          res.BeamMomentOfInertia[i].MetersToTheFourth, 8);

      List<double> expectedBeamX = new List<double>()
      {
        0.08350,
        0.08350,
        0.08350,
        0.08350,
        0.08350,
        0.08350,
        0.08350
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBeamX[i],
          res.BeamNeutralAxisPosition[i].Meters, 5);

      List<double> expectedBeamArea = new List<double>()
      {
        0.003653,
        0.003653,
        0.003653,
        0.003653,
        0.003653,
        0.003653,
        0.003653
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBeamArea[i],
          res.BeamArea[i].SquareMeters, 6);

      List<double> expectedLongTermI = new List<double>()
      {
        19.65E-6,
        137.16E-6,
        137.16E-6,
        137.16E-6,
        137.16E-6,
        137.16E-6,
        19.65E-6
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedLongTermI[i],
          res.MomentOfInertiaLongTerm[i].MetersToTheFourth, 8);

      List<double> expectedexpectedLongTermX = new List<double>()
      {
        0.08350,
        0.23288,
        0.23288,
        0.23288,
        0.23288,
        0.23288,
        0.08350
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedexpectedLongTermX[i],
          res.NeutralAxisPositionLongTerm[i].Meters, 5);

      List<double> expectedLongTermArea = new List<double>()
      {
        0.003653,
        0.014783,
        0.014783,
        0.014783,
        0.014783,
        0.014783,
        0.003653
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedLongTermArea[i],
          res.AreaLongTerm[i].SquareMeters, 6);

      List<double> expectedShortTermI = new List<double>()
      {
        19.65E-6,
        173.01E-6,
        173.01E-6,
        173.01E-6,
        173.01E-6,
        173.01E-6,
        19.65E-6
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedShortTermI[i],
          res.MomentOfInertiaShortTerm[i].MetersToTheFourth, 8);

      List<double> expectedexpectedShortTermX = new List<double>()
      {
        0.08350,
        0.26834,
        0.26834,
        0.26834,
        0.26834,
        0.26834,
        0.08350
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedexpectedShortTermX[i],
          res.NeutralAxisPositionShortTerm[i].Meters, 5);

      List<double> expectedShortTermArea = new List<double>()
      {
        0.003653,
        0.025291,
        0.025291,
        0.025291,
        0.025291,
        0.025291,
        0.003653
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedShortTermArea[i],
          res.AreaShortTerm[i].SquareMeters, 6);

      List<double> expectedShrinkageI = new List<double>()
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
        Assert.Equal(expectedShrinkageI[i],
          res.MomentOfInertiaShrinkage[i].MetersToTheFourth, 8);

      List<double> expectedexpectedShrinkageX = new List<double>()
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
        Assert.Equal(expectedexpectedShrinkageX[i],
          res.NeutralAxisPositionShrinkage[i].Meters, 5);

      List<double> expectedShrinkageArea = new List<double>()
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
        Assert.Equal(expectedShrinkageArea[i],
          res.AreaShrinkage[i].SquareMeters, 6);

      List<double> expectedEffI = new List<double>()
      {
        19.65E-6,
        153.1E-6,
        153.1E-6,
        153.1E-6,
        153.1E-6,
        153.1E-6,
        19.65E-6
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedEffI[i],
          res.MomentOfInertiaEffective[i].MetersToTheFourth, 8);

      List<double> expectedexpectedEffX = new List<double>()
      {
        0.08350,
        0.24902,
        0.24902,
        0.24902,
        0.24902,
        0.24902,
        0.08350
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedexpectedEffX[i],
          res.NeutralAxisPositionEffective[i].Meters, 5);

      List<double> expectedEffArea = new List<double>()
      {
        0.003653,
        0.018443,
        0.018443,
        0.018443,
        0.018443,
        0.018443,
        0.003653
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedEffArea[i],
          res.AreaEffective[i].SquareMeters, 6);

      List<double> expectedVibrationI = new List<double>()
      {
        252.74E-6,
        252.74E-6,
        252.74E-6,
        252.74E-6,
        252.74E-6,
        252.74E-6,
        252.74E-6
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedVibrationI[i],
          res.MomentOfInertiaVibration[i].MetersToTheFourth, 8);

      List<double> expectedexpectedVibrationX = new List<double>()
      {
        0.25292,
        0.25292,
        0.25292,
        0.25292,
        0.25292,
        0.25292,
        0.25292
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedexpectedVibrationX[i],
          res.NeutralAxisPositionVibration[i].Meters, 5);

      List<double> expectedVibrationArea = new List<double>()
      {
        0.073082,
        0.073082,
        0.073082,
        0.073082,
        0.073082,
        0.073082,
        0.073082
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedVibrationArea[i],
          res.AreaVibration[i].SquareMeters, 6);

      Assert.Equal(7.371, res.NaturalFrequency.Hertz, 3);
    }
  }
}
