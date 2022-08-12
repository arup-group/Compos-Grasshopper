using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compos_8_6;
using Xunit;

namespace ComposAPI.File.Tests
{
  public class ComposFileTest
  {
    static string RelativePath = "..\\..\\..\\..\\TestFiles\\";

    [Theory]
    [InlineData("*.coa")]
    [InlineData("*.cob")]
    public void AnalyseTest(string searchPattern)
    {
      string path = Path.GetFullPath(ComposFileTest.RelativePath);

      foreach (string fileName in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly))
      {
        ComposFile file = ComposFile.Open(fileName);
        short status = file.Analyse();
        Assert.Equal(0, status);
      }
    }

    [Theory]
    [InlineData("Compos1.coa", "MEMBER-1")]
    public void AnalyseMemberTest(string fileName, string memberName)
    {
      ComposFile file = ComposFile.Open(Path.GetFullPath(ComposFileTest.RelativePath + fileName));
      short status = file.Analyse(memberName);
      Assert.Equal(0, status);
    }

    [Theory]
    [InlineData("Compos1.coa", "MEMBER-1", 3)]
    public void CodeSatisfiedTest(string fileName, string memberName, int expextedStatus)
    {
      ComposFile file = ComposFile.Open(Path.GetFullPath(ComposFileTest.RelativePath + fileName));
      short status = file.CodeSatisfied(memberName);
      Assert.Equal(expextedStatus, status);
    }


    [Theory]
    [InlineData("*.coa")]
    [InlineData("*.cob")]
    public void DesignTest(string searchPattern)
    {
      string path = Path.GetFullPath(ComposFileTest.RelativePath);

      foreach (string fileName in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly))
      {
        ComposFile file = ComposFile.Open(fileName);
        short status = file.Design();
        Assert.Equal(0, status);
      }
    }

    [Theory]
    [InlineData("Compos1.coa", "MEMBER-1")]
    public void DesignMemberTest(string fileName, string memberName)
    {
      ComposFile file = ComposFile.Open(Path.GetFullPath(ComposFileTest.RelativePath + fileName));
      short status = file.Design(memberName);
      Assert.Equal(0, status);
    }

    [Theory]
    [InlineData("Compos1.coa", "MEMBER-1")]
    public void BeamSectDescTest(string fileName, string memberName)
    {
      ComposFile file = ComposFile.Open(Path.GetFullPath(ComposFileTest.RelativePath + fileName));
      string status = file.BeamSectDesc(memberName);
      Assert.Equal("STD I 600. 200. 15. 25.", status);
    }

  }
}
