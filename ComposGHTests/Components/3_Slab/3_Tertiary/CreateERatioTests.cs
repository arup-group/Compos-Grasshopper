﻿using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet.Units;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateERatioTests
  {
    public static GH_OasysComponent CreateERatioMother()
    {
      var comp = new CreateERatio();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = CreateERatioMother();
      ERatioGoo output = (ERatioGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(6.24304, output.Value.ShortTerm);
      Assert.Equal(23.5531, output.Value.LongTerm);
      Assert.Equal(5.526, output.Value.Vibration);
      Assert.Equal(22.3517, output.Value.Shrinkage);
    }
  }
}
