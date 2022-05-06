﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compos_8_6;
using Xunit;

namespace ComposAPI.Tests
{
  public class ComposFileTest
  {
    [Fact]
    public void OpenCobTest()
    {
      string pathName = Path.GetFullPath("..\\..\\..\\TestFiles\\Compos1.cob");

      ComposFile file = new ComposFile();

      IAutomation automation = file.Open(pathName);

      Assert.NotNull(automation); 
    }

    [Fact]
    public void OpenCoaTest()
    {
      string pathName = Path.GetFullPath("..\\..\\..\\TestFiles\\Compos1.coa");

      ComposFile file = new ComposFile();

      IAutomation automation = file.Open(pathName);

      Assert.NotNull(automation);
    }

  }
}