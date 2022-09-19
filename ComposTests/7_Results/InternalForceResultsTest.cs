using System;
using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Results.Tests
{
    [Collection("ComposAPI Fixture collection")]

  public class InternalForceResultsTest
  {
    [Fact]
    public void MomentULSConstructionResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedULSCons = new List<double>()
      {
        0.0,
        -62548,
        -100100,
        -112600,
        -100100,
        -62548,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedULSCons[i] * Math.Pow(10, -6),
          res.MomentULSConstruction[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentULSResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedULS = new List<double>()
      {
        0.0,
        -152100,
        -243400,
        -273900,
        -243400,
        -152100,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedULS[i] * Math.Pow(10, -6),
          res.MomentULS[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentConstructionDeadLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedConsDL = new List<double>()
      {
        0.0,
        -17250,
        -27600,
        -31050,
        -27600,
        -17250,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedConsDL[i] * Math.Pow(10, -6),
          res.MomentConstructionDeadLoad[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentConstructionLiveLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedConsLL = new List<double>()
      {
        0.0,
        -24000,
        -38400,
        -43200,
        -38400,
        -24000,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedConsLL[i] * Math.Pow(10, -6),
          res.MomentConstructionLiveLoad[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentAddDeadLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

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
          res.MomentFinalAdditionalDeadLoad[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentLiveLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedLL = new List<double>()
      {
        0.0,
        -80000,
        -128000,
        -144000,
        -128000,
        -80000,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedLL[i] * Math.Pow(10, -6),
          res.MomentFinalLiveLoad[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentShrinkageResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

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
        Assert.Equal(expectedShrinkage[i] * Math.Pow(10, -6),
          res.MomentFinalShrinkage[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void ShearULSConstructionResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedULSCons = new List<double>()
      {
        56290,
        37530,
        18760,
        984.3E-9,
        -18760,
        -37530,
        -56290
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedULSCons[i] * Math.Pow(10, -6),
          res.ShearULSConstruction[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void ShearULSResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedULS = new List<double>()
      {
        136900,
        91290,
        45640,
        985.6E-9,
        -45640,
        -91290,
        -136900,
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedULS[i] * Math.Pow(10, -6),
          res.ShearULS[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void ShearConstructionDeadLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedConsDL = new List<double>()
      {
        15520,
        10349,
        5175,
        702.7E-9,
        -5175,
        -10349,
        -15520
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedConsDL[i] * Math.Pow(10, -6),
          res.ShearConstructionDeadLoad[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void ShearConstructionLiveLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedConsLL = new List<double>()
      {
        21600,
        14400,
        7200,
        338.3E-12,
        -7200,
        -14400,
        -21600
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedConsLL[i] * Math.Pow(10, -6),
          res.ShearConstructionLiveLoad[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void ShearAddDeadLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

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
          res.ShearFinalAdditionalDeadLoad[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void ShearLiveLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedLL = new List<double>()
      {
        72000,
        48000,
        24000,
        1.139E-9,
        -24000,
        -48000,
        -72000
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedLL[i] * Math.Pow(10, -6),
          res.ShearFinalLiveLoad[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void AxialULSConstructionResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedULSCons = new List<double>()
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
        Assert.Equal(expectedULSCons[i] * Math.Pow(10, -6),
          res.AxialULSConstruction[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void AxialULSResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedULS = new List<double>()
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
        Assert.Equal(expectedULS[i] * Math.Pow(10, -6),
          res.AxialULS[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void AxialConstructionDeadLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedConsDL = new List<double>()
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
        Assert.Equal(expectedConsDL[i] * Math.Pow(10, -6),
          res.AxialConstructionDeadLoad[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void AxialConstructionLiveLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedConsLL = new List<double>()
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
        Assert.Equal(expectedConsLL[i] * Math.Pow(10, -6),
          res.AxialConstructionLiveLoad[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void AxialAddDeadLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

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
          res.AxialFinalAdditionalDeadLoad[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void AxialLiveLoadResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IInternalForceResult res = r.InternalForces;

      List<double> expectedLL = new List<double>()
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
        Assert.Equal(expectedLL[i] * Math.Pow(10, -6),
          res.AxialFinalLiveLoad[i].Newtons * Math.Pow(10, -6), 4);
    }
  }
}
