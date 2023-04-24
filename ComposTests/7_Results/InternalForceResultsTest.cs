using System;
using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Results.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class InternalForceResultsTest {

    [Fact]
    public void AxialAddDeadLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

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
        Assert.Equal(expectedAddDL[i] * 1E-6, res.AxialFinalAdditionalDeadLoad[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void AxialConstructionDeadLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedConsDL = new List<double>()
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
        Assert.Equal(expectedConsDL[i] * 1E-6, res.AxialConstructionDeadLoad[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void AxialConstructionLiveLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedConsLL = new List<double>()
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
        Assert.Equal(expectedConsLL[i] * 1E-6, res.AxialConstructionLiveLoad[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void AxialLiveLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedLL = new List<double>()
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
        Assert.Equal(expectedLL[i] * 1E-6, res.AxialFinalLiveLoad[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void AxialULSConstructionResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedULSCons = new List<double>()
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
        Assert.Equal(expectedULSCons[i] * 1E-6, res.AxialULSConstruction[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void AxialULSResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedULS = new List<double>()
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
        Assert.Equal(expectedULS[i] * 1E-6, res.AxialULS[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void MomentAddDeadLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

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
        Assert.Equal(expectedAddDL[i] * 1E-6, res.MomentFinalAdditionalDeadLoad[i].NewtonMeters * 1E-6, 4);
      }
    }

    [Fact]
    public void MomentConstructionDeadLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedConsDL = new List<double>()
      {
        0.0,
        -17250,
        -27600,
        -31050,
        -27600,
        -17250,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedConsDL[i] * 1E-6, res.MomentConstructionDeadLoad[i].NewtonMeters * 1E-6, 4);
      }
    }

    [Fact]
    public void MomentConstructionLiveLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedConsLL = new List<double>()
      {
        0.0,
        -24000,
        -38400,
        -43200,
        -38400,
        -24000,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedConsLL[i] * 1E-6, res.MomentConstructionLiveLoad[i].NewtonMeters * 1E-6, 4);
      }
    }

    [Fact]
    public void MomentLiveLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedLL = new List<double>()
      {
        0.0,
        -80000,
        -128000,
        -144000,
        -128000,
        -80000,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedLL[i] * 1E-6, res.MomentFinalLiveLoad[i].NewtonMeters * 1E-6, 4);
      }
    }

    [Fact]
    public void MomentShrinkageResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedShrinkage = new List<double>()
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
        Assert.Equal(expectedShrinkage[i] * 1E-6, res.MomentFinalShrinkage[i].NewtonMeters * 1E-6, 4);
      }
    }

    [Fact]
    public void MomentULSConstructionResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedULSCons = new List<double>()
      {
        0.0,
        -62548,
        -100100,
        -112600,
        -100100,
        -62548,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedULSCons[i] * 1E-6, res.MomentULSConstruction[i].NewtonMeters * 1E-6, 4);
      }
    }

    [Fact]
    public void MomentULSResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedULS = new List<double>()
      {
        0.0,
        -152100,
        -243400,
        -273900,
        -243400,
        -152100,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedULS[i] * 1E-6, res.MomentULS[i].NewtonMeters * 1E-6, 4);
      }
    }

    [Fact]
    public void ShearAddDeadLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

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
        Assert.Equal(expectedAddDL[i] * 1E-6, res.ShearFinalAdditionalDeadLoad[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void ShearConstructionDeadLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedConsDL = new List<double>()
      {
        15520,
        10349,
        5175,
        702.7E-9,
        -5175,
        -10349,
        -15520
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedConsDL[i] * 1E-6, res.ShearConstructionDeadLoad[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void ShearConstructionLiveLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedConsLL = new List<double>()
      {
        21600,
        14400,
        7200,
        338.3E-12,
        -7200,
        -14400,
        -21600
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedConsLL[i] * 1E-6, res.ShearConstructionLiveLoad[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void ShearLiveLoadResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedLL = new List<double>()
      {
        72000,
        48000,
        24000,
        1.139E-9,
        -24000,
        -48000,
        -72000
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedLL[i] * 1E-6, res.ShearFinalLiveLoad[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void ShearULSConstructionResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedULSCons = new List<double>()
      {
        56290,
        37530,
        18760,
        984.3E-9,
        -18760,
        -37530,
        -56290
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedULSCons[i] * 1E-6, res.ShearULSConstruction[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void ShearULSResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      var expectedULS = new List<double>()
      {
        136900,
        91290,
        45640,
        985.6E-9,
        -45640,
        -91290,
        -136900,
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedULS[i] * 1E-6, res.ShearULS[i].Newtons * 1E-6, 4);
      }
    }
  }
}
