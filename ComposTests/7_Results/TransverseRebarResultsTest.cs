using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Results.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class TransverseRebarResultsTest {

    [Fact]
    public void RebarAreaTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedArea = new List<double>()
      {
        0.0
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedArea[i], res.Area[i].SquareMeters, 4);
      }
    }

    [Fact]
    public void RebarConcreteResistanceTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedConc = new List<double>()
      {
        168400
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedConc[i] / 1000, res.ConcreteShearResistance[i].Kilonewtons, 1);
      }
    }

    [Fact]
    public void RebarCoverTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedCover = new List<double>()
      {
        0.035
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedCover[i], res.Cover[i].Meters, 4);
      }
    }

    [Fact]
    public void RebarDeckingResistanceTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedDeck = new List<double>()
      {
        0
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedDeck[i] / 1000, res.DeckingShearResistance[i].Kilonewtons, 1);
      }
    }

    [Fact]
    public void RebarDiameterTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedDia = new List<double>()
      {
        0.0
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedDia[i], res.Diameter[i].Meters, 4);
      }
    }

    [Fact]
    public void RebarEndPositionTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedEnd = new List<double>()
      {
        8.0
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedEnd[i], res.EndPosition[i].Meters, 4);
      }
    }

    [Fact]
    public void RebarMaxAllowedShearTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedMaxAllow = new List<double>()
      {
        710000
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedMaxAllow[i] / 1000, res.MaxAllowedShearResistance[i].Kilonewtons, 1);
      }
    }

    [Fact]
    public void RebarMeshResistanceTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedMesh = new List<double>()
      {
        45520
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedMesh[i] / 1000, res.MeshBarShearResistance[i].Kilonewtons, 1);
      }
    }

    [Fact]
    public void RebarPerimeterTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedPerim = new List<double>()
      {
        0.1403
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedPerim[i], res.EffectiveShearPerimeter[i].Meters, 4);
      }
    }

    [Fact]
    public void RebarPositionsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedDist = new List<double>()
      {
        0.1100
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedDist[i], res.Positions[i].Meters, 4);
      }
    }

    [Fact]
    public void RebarRebarResistanceTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedRebar = new List<double>()
      {
        0
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedRebar[i] / 1000, res.RebarShearResistance[i].Kilonewtons, 1);
      }
    }

    [Fact]
    public void RebarShearForceTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedTransShear = new List<double>()
      {
        119200
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedTransShear[i] / 1000, res.TransverseShearForce[i].Kilonewtons, 1);
      }
    }

    [Fact]
    public void RebarShearResistanceTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedTot = new List<double>()
      {
        213900
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedTot[i] / 1000, res.TotalShearResistance[i].Kilonewtons, 1);
      }
    }

    [Fact]
    public void RebarSpacingTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedSpacing = new List<double>()
      {
        0.3
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedSpacing[i], res.Spacing[i].Meters, 4);
      }
    }

    [Fact]
    public void RebarStartPositionTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedStart = new List<double>()
      {
        0.0
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedStart[i], res.StartPosition[i].Meters, 4);
      }
    }

    [Fact]
    public void RebarStringTest() {
      IResult r = ResultsTest.ResultMember.Result;
      ITransverseRebarResult res = r.TransverseRebarResults;

      var expectedSrf = new List<string>()
      {
        "e-e section"
      };
      for (int i = 0; i < res.Positions.Count; i++) {
        Assert.Equal(expectedSrf[i], res.ControlSurface[i]);
      }
    }
  }
}
