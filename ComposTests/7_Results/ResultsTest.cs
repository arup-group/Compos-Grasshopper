﻿using System.IO;
using Xunit;

namespace ComposAPI.Results.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class ResultsTest {
    public static IMember ResultMember {
      get {
        if (m_Member == null) {
          string fileName = "Compos2_UTF8.coa";
          string memberName = "MEMBER-1";

          var file = ComposFile.Open(Path.GetFullPath(RelativePath + fileName));
          Assert.Equal(0, file.Analyse(memberName));
          m_Member = file.GetMember(memberName);
        }
        return m_Member;
      }
    }
    public static readonly string RelativePath = "..\\..\\..\\..\\TestFiles\\";
    private static IMember m_Member = null;

    [Fact]
    public void PositionsTest() {
      IMember member = ResultsTest.ResultMember;
      Assert.Equal(7, member.Result.Positions.Count);
      Assert.Equal(0, member.Result.Positions[0].Meters, 3);
      Assert.Equal(1.333, member.Result.Positions[1].Meters, 3);
      Assert.Equal(2.667, member.Result.Positions[2].Meters, 3);
      Assert.Equal(4, member.Result.Positions[3].Meters, 3);
      Assert.Equal(5.333, member.Result.Positions[4].Meters, 3);
      Assert.Equal(6.667, member.Result.Positions[5].Meters, 3);
      Assert.Equal(8, member.Result.Positions[6].Meters, 3);
    }
  }
}
