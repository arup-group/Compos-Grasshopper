using System;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Test;
using Xunit;
using ComposGHTests.Helpers;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace ComposGHTests
{
  public class GH_OasysGooTest : Rhino.Test.GrasshopperFixture
  {
    [Fact]
    public void ConstructorTest()
    {
      ISupports supports = new Supports();
      SupportsGoo goo = new SupportsGoo(supports);

      ObjectExtensionTest.IsEqual(supports, goo.Value);
    }
  }
}
