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
  public class CreateSupportTests : IClassFixture<GH_Fixture>
  {
    GH_Fixture fixture { get; set; }

    public CreateSupportTests(GH_Fixture fixture)
    {
      this.fixture = fixture;
    }

    [Fact]
    public void CreateSupportComponentTest()
    {
      
    }
  }
}
