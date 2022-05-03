using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compos_8_6;
using Xunit;

namespace ComposAPI.Tests
{
  public class ComposIOTest
  {
    [Fact]
    public void TestOpenCov()
    {
      string pathName = Path.GetFullPath("..\\..\\..\\TestFiles\\Compos1.cob");

      IAutomation automation = ComposIO.Open(pathName);

      Assert.NotNull(automation); 
    }

    [Fact]
    public void TestOpenCoa()
    {
      string pathName = Path.GetFullPath("..\\..\\..\\TestFiles\\Compos1.coa");

      IAutomation automation = ComposIO.Open(pathName);

      Assert.NotNull(automation);
    }

  }
}
