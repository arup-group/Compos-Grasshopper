using System;
using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Results.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class SlabResultsTest {

    [Fact]
    public void SlabStrainAddDeadLoadTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

      var expectedAddDL = new List<double>()
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
        Assert.Equal(expectedAddDL[i], res.ConcreteStrainAdditionalDeadLoad[i].MilliStrain, 4);
      }
    }

    [Fact]
    public void SlabStrainFinalTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

      var expectedFinal = new List<double>()
      {
        0.0,
        -0.1272,
        -0.2036,
        -0.2290,
        -0.2036,
        -0.1272,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedFinal[i], res.ConcreteStrainFinal[i].MilliStrain, 4);
      }
    }

    [Fact]
    public void SlabStrainLiveLoadTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

      var expectedFinalLL = new List<double>()
      {
        0.0,
        -0.1272,
        -0.2036,
        -0.2290,
        -0.2036,
        -0.1272,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedFinalLL[i], res.ConcreteStrainFinalLiveLoad[i].MilliStrain, 4);
      }
    }

    [Fact]
    public void SlabStrainShrinkageTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

      var expectedShrink = new List<double>()
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
        Assert.Equal(expectedShrink[i], res.ConcreteStrainFinalShrinkage[i].MilliStrain, 4);
      }
    }

    [Fact]
    public void SlabStrainUnitTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

      var expectedFinal = new List<double>()
      {
        0.0,
        -0.1272,
        -0.2036,
        -0.2290,
        -0.2036,
        -0.1272,
        0.0
      };

      // check that millistrain is the correct unit
      // values in compos is given as 'x 1000':
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedFinal[i] / 1000, res.ConcreteStrainFinal[i].Value, 7);
      }
    }

    [Fact]
    public void SlabStressAddDeadLoadTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

      var expectedAddDL = new List<double>()
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
        Assert.Equal(expectedAddDL[i] * 1E-6, res.ConcreteStressAdditionalDeadLoad[i].NewtonsPerSquareMeter * 1E-6, 3);
      }
    }

    [Fact]
    public void SlabStressFinalTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

      var expectedFinal = new List<double>()
      {
        0.0,
        -4.347E+6,
        -6.955E+6,
        -7.825E+6,
        -6.955E+6,
        -4.347E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedFinal[i] * 1E-6, res.ConcreteStressFinal[i].NewtonsPerSquareMeter * 1E-6, 3);
      }
    }

    [Fact]
    public void SlabStressLiveLoadTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

      var expectedFinalLL = new List<double>()
      {
        0.0,
        -4.347E+6,
        -6.955E+6,
        -7.825E+6,
        -6.955E+6,
        -4.347E+6,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedFinalLL[i] * 1E-6, res.ConcreteStressFinalLiveLoad[i].NewtonsPerSquareMeter * 1E-6, 3);
      }
    }

    [Fact]
    public void SlabStressShrinkageTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ISlabStressResult res = r.SlabStresses;

      var expectedShrink = new List<double>()
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
        Assert.Equal(expectedShrink[i] * 1E-6, res.ConcreteStressFinalShrinkage[i].NewtonsPerSquareMeter * 1E-6, 3);
      }
    }
  }
}
