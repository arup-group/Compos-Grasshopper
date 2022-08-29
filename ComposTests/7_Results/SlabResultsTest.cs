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

  public class SlabResultsTest
  {
    [Fact]
    public void SlabStressTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

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
        Assert.Equal(expectedAddDL[i] * Math.Pow(10, -6),
          res.ConcreteStressAdditionalDeadLoad[i].NewtonsPerSquareMeter * Math.Pow(10, -6), 3);

      List<double> expectedFinalLL = new List<double>()
      {
        0.0,
        -4.347E+6,
        -6.955E+6,
        -7.825E+6,
        -6.955E+6,
        -4.347E+6,
        0.0

      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedFinalLL[i] * Math.Pow(10, -6),
          res.ConcreteStressFinalLiveLoad[i].NewtonsPerSquareMeter * Math.Pow(10, -6), 3);

      List<double> expectedShrink = new List<double>()
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
        Assert.Equal(expectedShrink[i] * Math.Pow(10, -6),
          res.ConcreteStressFinalShrinkage[i].NewtonsPerSquareMeter * Math.Pow(10, -6), 3);

      List<double> expectedFinal = new List<double>()
      {
        0.0,
        -4.347E+6,
        -6.955E+6,
        -7.825E+6,
        -6.955E+6,
        -4.347E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedFinal[i] * Math.Pow(10, -6),
          res.ConcreteStressFinal[i].NewtonsPerSquareMeter * Math.Pow(10, -6), 3);
    }

    [Fact]
    public void SlabStrainTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

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
          res.ConcreteStrainAdditionalDeadLoad[i].MilliStrain, 4);

      List<double> expectedFinalLL = new List<double>()
      {
        0.0,
        -0.1272,
        -0.2036,
        -0.2290,
        -0.2036,
        -0.1272,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedFinalLL[i],
          res.ConcreteStrainFinalLiveLoad[i].MilliStrain, 4);

      List<double> expectedShrink = new List<double>()
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
        Assert.Equal(expectedShrink[i],
          res.ConcreteStrainFinalShrinkage[i].MilliStrain, 4);

      List<double> expectedFinal = new List<double>()
      {
        0.0,
        -0.1272,
        -0.2036,
        -0.2290,
        -0.2036,
        -0.1272,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedFinal[i],
          res.ConcreteStrainFinal[i].MilliStrain, 4);

      // check that millistrain is the correct unit
      // values in compos is given as 'x 1000':
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedFinal[i] / 1000,
          res.ConcreteStrainFinal[i].Value, 7);
    }
  }
}
