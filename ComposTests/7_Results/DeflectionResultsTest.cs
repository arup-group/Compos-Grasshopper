using System;
using System.Collections.Generic;
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

  public class DeflectionResultsTest
  {
    [Fact]
    public void DeflectionResultTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IDeflectionResult res = r.Deflections;

      List<double> expectedDL = new List<double>()
      {
        0.0,
        -0.02589,
        -0.04447,
        -0.05117,
        -0.04447,
        -0.02589,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedDL[i],
          res.ConstructionDeadLoad[i].Meters, 5);
    }

    [Fact]
    public void AdditionalDeadLoadDeflectionResultTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IDeflectionResult res = r.Deflections;

      List<double> expectedAddDL = new List<double>()
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
        Assert.Equal(expectedAddDL[i],
          res.AdditionalDeadLoad[i].Meters, 5);
    }

    [Fact]
    public void LiveLoadDeflectionResultTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IDeflectionResult res = r.Deflections;

      List<double> expectedLL = new List<double>()
      {
        0.0,
        -0.01541,
        -0.02605,
        -0.02989,
        -0.02605,
        -0.01541,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedLL[i],
          res.LiveLoad[i].Meters, 5);
    }

    [Fact]
    public void ShrinkageLoadDeflectionResultTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IDeflectionResult res = r.Deflections;

      List<double> expectedShrinkage = new List<double>()
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
        Assert.Equal(expectedShrinkage[i],
          res.Shrinkage[i].Meters, 5);
    }

    [Fact]
    public void PostConstructionDeflectionResultTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IDeflectionResult res = r.Deflections;

      List<double> expectedPost = new List<double>()
      {
        0.0,
        -0.01541,
        -0.02605,
        -0.02989,
        -0.02605,
        -0.01541,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedPost[i],
          res.PostConstruction[i].Meters, 5);
    }

    [Fact]
    public void TotalDeflectionResultTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IDeflectionResult res = r.Deflections;

      List<double> expectedTot = new List<double>()
      {
        0.0,
        -0.04130,
        -0.07052,
        -0.08105,
        -0.07052,
        -0.04130,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedTot[i],
          res.Total[i].Meters, 5);
    }

    // bug in COM, ModalShape giving only 0 values
    //[Fact]
    //public void ModeShapeDeflectionResultTest()
    //{
    //  IResult r = ResultsTest.ResultMember.Result;
    //  IDeflectionResult res = r.Deflections;

    //  List<double> expectedModeShape = new List<double>()
    //  {
    //    0.0,
    //    -0.002950,
    //    -0.005066,
    //    -0.005829,
    //    -0.005066,
    //    -0.002950,
    //    0.0
    //  };
    //  for (int i = 0; i < r.Positions.Count; i++)
    //    Assert.Equal(expectedModeShape[i],
    //      res.ModalShape[i].Meters, 6);
    //}
  }
}
