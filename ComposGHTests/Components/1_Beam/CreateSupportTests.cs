using System;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Grasshopper.Kernel;
using Xunit;
using ComposGHTests.Helpers;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Grasshopper.Kernel.Types;
using static Rhino.Render.RenderEnvironment;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class SupportComponentTests
  { 
    [Fact]
    public void CreateDefaultSupportComponentTest()
    {
      // create the component
      var comp = new CreateSupport();
      comp.CreateAttributes();

      SupportsGoo output = (SupportsGoo)Component.GetOutput(comp);
      Assert.True(output.Value.SecondaryMemberAsIntermediateRestraint);
      Assert.True(output.Value.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(0, output.Value.CustomIntermediateRestraintPositions.Count);
      Assert.Equal(IntermediateRestraint.None, output.Value.IntermediateRestraintPositions);
    }
  }
}
