﻿using System;
using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Results.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class StudResultsTest {

    [Fact]
    public void StudForceCapacityLeftResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedLeft = new List<double>()
       {
        0.0,
        357500,
        643500,
        1.001E+6,
        1.287E+6,
        1.573E+6,
        1.931E+6,
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedLeft[i] * 1E-6, res.StudCapacityStart[i].Newtons * 1E-6, 3);
      }
    }

    [Fact]
    public void StudForceCapacityRequiredResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedRequired = new List<double>()
      {
        0.0,
        433000,
        9.999999680285693E+37,
        9.999999680285693E+37,
        9.999999680285693E+37,
        433000,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedRequired[i] * 1E-6, res.StudCapacityRequired[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void StudForceCapacityResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedActual = new List<double>()
      {
        0.0,
        357500,
        643500,
        1.0011E+6,
        643500,
        357500,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedActual[i] * 1E-6, res.StudCapacity[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void StudForceCapacityRightResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedRight = new List<double>()
      {
        1.931E+6,
        1.573E+6,
        1.287E+6,
        1.001E+6,
        643500,
        357500,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedRight[i] * 1E-6, res.StudCapacityEnd[i].Newtons * 1E-6, 3);
      }
    }

    [Fact]
    public void StudForceRequiredCapacityFullInteractionResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedRequired100 = new List<double>()
      {
        0.0,
        951500,
        951500,
        951500,
        951500,
        951500,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedRequired100[i] * 1E-6, res.StudCapacityRequiredForFullShearInteraction[i].Newtons * 1E-6, 4);
      }
    }

    [Fact]
    public void StudForceSingleCapacityResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      Assert.Equal(71500 * 1E-6, res.SingleStudCapacity.Newtons * 1E-6, 4);
    }

    [Fact]
    public void StudRatioActualInteractionResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedActual = new List<double>()
      {
        0.0,
        37.57,
        67.63,
        105.21,
        67.63,
        37.57,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedActual[i], res.ShearInteraction[i].Percent, 2);
      }
    }

    [Fact]
    public void StudRatioInteractionRequiredResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedReqInteract = new List<double>()
      {
        0.0,
        45.51,
        110,
        110,
        110,
        45.51,
        0.0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedReqInteract[i], res.ShearInteractionRequired[i].Percent, 2);
      }
    }

    [Fact]
    public void StudScalarLeftResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedStudsLeft = new List<int>()
      {
        0,
        5,
        9,
        14,
        18,
        22,
        27
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedStudsLeft[i], res.NumberOfStudsStart[i]);
      }
    }

    [Fact]
    public void StudScalarRightResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedStudsRight = new List<int>()
      {
        27,
        22,
        18,
        14,
        9,
        5,
        0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedStudsRight[i], res.NumberOfStudsEnd[i]);
      }
    }

    [Fact]
    public void StudScalarUsedLeftResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedUsedStudsLeft = new List<int>()
      {
        0,
        5,
        9,
        14,
        18,
        22,
        27
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedUsedStudsLeft[i], res.NumberOfStudsRequiredStart[i]);
      }
    }

    [Fact]
    public void StudScalarUsedRightResultsTest() {
      IResult r = ResultsTest.ResultMember.Result;
      IStudResult res = r.StudResults;

      var expectedUsedStudsRight = new List<int>()
      {
        27,
        22,
        18,
        14,
        9,
        5,
        0
      };
      for (int i = 0; i < r.Positions.Count; i++) {
        Assert.Equal(expectedUsedStudsRight[i], res.NumberOfStudsRequiredEnd[i]);
      }
    }
  }
}
