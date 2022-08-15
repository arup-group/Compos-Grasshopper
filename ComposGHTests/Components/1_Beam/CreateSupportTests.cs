using System;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using Xunit;
using ComposGHTests.Helpers;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Grasshopper.Kernel.Types;

namespace ComposGHTests
{
  public class SupportComponentFixture : GrasshopperFixture
  {
    public SupportComponentFixture() : base() { }
  }
  public class SupportComponentTests : IClassFixture<SupportComponentFixture>
  {
    SupportComponentFixture fixture { get; set; }

    public SupportComponentTests(SupportComponentFixture fixture)
    {
      this.fixture = fixture;
    }

    [Fact]
    public void CreateSupportComponentTest()
    {
      Assert.True(true);
    }
  }
}
