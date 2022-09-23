using System;
using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Results.Tests
{
    [Collection("ComposAPI Fixture collection")]

  public class CapacityResultsTest
  {
    [Fact]
    public void MomentULSConstructionResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedULSCons = new List<double>()
      {
        67900,
        67900,
        67900,
        67900,
        67900,
        67900,
        67900
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedULSCons[i] * Math.Pow(10, -6),
          res.MomentConstruction[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentULSResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedULS = new List<double>()
      {
        56030,
        137900,
        186100,
        241700,
        186100,
        137900,
        56030
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedULS[i] * Math.Pow(10, -6),
          res.Moment[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentULSConstructionHoggingResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedULSConsHogging = new List<double>()
      {
        67900,
        67900,
        67900,
        67900,
        67900,
        67900,
        67900
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedULSConsHogging[i] * Math.Pow(10, -6),
          res.MomentHoggingConstruction[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentULSHoggingResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedULSHoggin = new List<double>()
      {
        56030,
        66470,
        67900,
        67900,
        67900,
        66470,
        56030
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedULSHoggin[i] * Math.Pow(10, -6),
          res.MomentHoggingFinal[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentBeamPlasticResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedBeamPlastic = new List<double>()
      {
        71020,
        71020,
        71020,
        71020,
        71020,
        71020,
        71020,
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBeamPlastic[i] * Math.Pow(10, -6),
          res.AssumedBeamPlasticMoment[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentBeamPlasticHoggingResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedBeamPlasticHogging = new List<double>()
      {
        71020,
        71020,
        71020,
        71020,
        71020,
        71020,
        71020
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBeamPlasticHogging[i] * Math.Pow(10, -6),
          res.AssumedBeamPlasticMomentHogging[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentFullInteractionResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedFullInteraction = new List<double>()
      {
        56030,
        236000,
        241700,
        241700,
        241700,
        236000,
        56030
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedFullInteraction[i] * Math.Pow(10, -6),
          res.AssumedMomentFullShearInteraction[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void MomentFullInteractionHoggingResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedFullInteractionHogging = new List<double>()
      {
        56030,
        66470,
        67900,
        67900,
        67900,
        66470,
        56030
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedFullInteractionHogging[i] * Math.Pow(10, -6),
          res.AssumedMomentFullShearInteractionHogging[i].NewtonMeters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void NeutralPosULSConstructionResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedULSCons = new List<double>()
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
        Assert.Equal(expectedULSCons[i] * Math.Pow(10, -6),
          res.NeutralAxisConstruction[i].Meters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void NeutralPosULSResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;
      List<double> expectedULS = new List<double>()
      {
        0.08350,
        0.1610,
        0.1639,
        0.3169,
        0.1639,
        0.1610,
        0.08350
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedULS[i] * Math.Pow(10, -6),
          res.NeutralAxis[i].Meters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void NeutralPosULSConstructionHoggingResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;
      List<double> expectedULSConsHogging = new List<double>()
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
        Assert.Equal(expectedULSConsHogging[i] * Math.Pow(10, -6),
          res.NeutralAxisHoggingConstruction[i].Meters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void NeutralPosULSHoggingResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;
      List<double> expectedULSHoggin = new List<double>()
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
        Assert.Equal(expectedULSHoggin[i] * Math.Pow(10, -6),
          res.NeutralAxisHoggingFinal[i].Meters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void NeutralPosPlasticResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;
      List<double> expectedBeamPlastic = new List<double>()
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
        Assert.Equal(expectedBeamPlastic[i] * Math.Pow(10, -6),
          res.AssumedPlasticNeutralAxis[i].Meters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void NeutralPosPlasticHoggingResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;
      List<double> expectedBeamPlasticHogging = new List<double>()
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
        Assert.Equal(expectedBeamPlasticHogging[i] * Math.Pow(10, -6),
          res.AssumedPlasticNeutralAxisHogging[i].Meters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void NeutralPosFullInteractionResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;
      List<double> expectedFullInteraction = new List<double>()
      {
        0.08350,
        0.3169,
        0.3169,
        0.3169,
        0.3169,
        0.3169,
        0.08350
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedFullInteraction[i] * Math.Pow(10, -6),
          res.AssumedNeutralAxisFullShearInteraction[i].Meters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void NeutralPosFullInteractionHoggingResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;
      List<double> expectedFullInteractionHogging = new List<double>()
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
        Assert.Equal(expectedFullInteractionHogging[i] * Math.Pow(10, -6),
          res.AssumedNeutralAxisFullShearInteractionHogging[i].Meters * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void ShearResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedShear = new List<double>()
      {
        137800,
        137800,
        137800,
        137800,
        137800,
        137800,
        137800,
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedShear[i] * Math.Pow(10, -6),
          res.Shear[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void ShearBucklingResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedBuckling = new List<double>()
      {
        137800,
        137800,
        137800,
        137800,
        137800,
        137800,
        137800,
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedBuckling[i] * Math.Pow(10, -6),
          res.ShearBuckling[i].Newtons * Math.Pow(10, -6), 4);
    }

    [Fact]
    public void ShearRequiredResultsTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      ICapacityResult res = r.Capacities;

      List<double> expectedRequired = new List<double>()
      {
        137800,
        137800,
        137800,
        137800,
        137800,
        137800,
        137800,
      };
      for (int i = 0; i < r.Positions.Count; i++)
        Assert.Equal(expectedRequired[i] * Math.Pow(10, -6),
          res.ShearRequired[i].Newtons * Math.Pow(10, -6), 4);
    }
  }
}
