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

  public class TransverseRebarResultsTest
  {
    [Fact]
    public void RebarLengthTests()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      List<double> expectedDist = new List<double>()
      {
        0.1100
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedDist[i],
          res.Positions[i].Meters, 4);

      List<double> expectedStart = new List<double>()
      {
        0.0
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedStart[i],
          res.StartPosition[i].Meters, 4);
      
      List<double> expectedEnd = new List<double>()
      {
        8.0
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedEnd[i],
          res.EndPosition[i].Meters, 4);

      List<double> expectedDia = new List<double>()
      {
        0.0
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedDia[i],
          res.Diameter[i].Meters, 4);

      List<double> expectedSpacing = new List<double>()
      {
        0.3
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedSpacing[i],
          res.Spacing[i].Meters, 4);

      List<double> expectedArea = new List<double>()
      {
        0.0
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedArea[i],
          res.Area[i].SquareMeters, 4);

      List<double> expectedCover = new List<double>()
      {
        0.035
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedCover[i],
          res.Cover[i].Meters, 4);

      List<double> expectedPerim = new List<double>()
      {
        0.1403
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedPerim[i],
          res.EffectiveShearPerimeter[i].Meters, 4);
    }

    [Fact]
    public void RebarStringTests()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      List<string> expectedSrf = new List<string>()
      {
        "e-e section"
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedSrf[i],
          res.ControlSurface[i]);
    }

    [Fact]
    public void RebarForceTests()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      List<double> expectedTransShear = new List<double>()
      {
        119200
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedTransShear[i] / 1000,
          res.TransverseShearForce[i].Kilonewtons, 1);

      List<double> expectedTot = new List<double>()
      {
        213900
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedTot[i] / 1000,
          res.TotalShearResistance[i].Kilonewtons, 1);

      List<double> expectedConc = new List<double>()
      {
        168400
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedConc[i] / 1000,
          res.ConcreteShearResistance[i].Kilonewtons, 1);

      List<double> expectedDeck = new List<double>()
      {
        0
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedDeck[i] / 1000,
          res.DeckingShearResistance[i].Kilonewtons, 1);

      List<double> expectedMesh = new List<double>()
      {
        45520
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedMesh[i] / 1000,
          res.MeshBarShearResistance[i].Kilonewtons, 1);

      List<double> expectedRebar = new List<double>()
      {
        0
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedRebar[i] / 1000,
          res.RebarShearResistance[i].Kilonewtons, 1);

      List<double> expectedMaxAllow = new List<double>()
      {
        710000
      };
      for (int i = 0; i < res.Positions.Count; i++)
        Assert.Equal(expectedMaxAllow[i] / 1000,
          res.MaxAllowedShearResistance[i].Kilonewtons, 1);
    }
  }
}
