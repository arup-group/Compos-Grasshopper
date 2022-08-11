using System;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using Xunit;
using ComposGHTests.Helpers;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace ComposGHTests
{
  public class GH_OasysGooTest
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
