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
    public void OpenTest(string searchPattern)
    {
      string path = Path.GetFullPath(ComposFileTest.RelativePath);

      foreach (string fileName in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly))
      {
        ComposFile file = ComposFile.Open(fileName);
        Assert.NotNull(file);
      }
    }

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
  }
}
