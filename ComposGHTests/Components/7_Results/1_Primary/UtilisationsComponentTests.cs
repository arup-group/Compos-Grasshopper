﻿using Xunit;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;
using ComposGH.Components;
using ComposGHTests.Helpers;

namespace ComposGHTests.Result
{
  [Collection("GrasshopperFixture collection")]
  public class UtilisationsComponentTests
  {
    [Fact]
    public void CreateComponentWithInput()
    {
      var comp = new Utilisations();
      MemberGoo input = (MemberGoo)ComponentTestHelper.GetOutput(CompFile.FileComponentsTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input);

      int expectedOutputCount = 9;
      Assert.Equal(expectedOutputCount, comp.Params.Output.Count);

      for (int i = 0; i < expectedOutputCount; i++)
      {
        GH_Number output = (GH_Number)ComponentTestHelper.GetOutput(comp, i);
        Assert.NotNull(output);
      }
    }
  }
}
