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
using UnitsNet;
using System.IO;
using System.Xml.Linq;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class CreateRestraintComponentTests
  { 
    [Fact]
    public void CreateComponentTest()
    {
      var comp = new CreateRestraint();
      comp.CreateAttributes();

      RestraintGoo output = (RestraintGoo)ComponentTestHelper.GetOutput(comp);
      Assert.True(output.Value.TopFlangeRestrained);
      Duplicates.AreEqual(new Supports(), output.Value.ConstructionStageSupports);
      Duplicates.AreEqual(new Supports(IntermediateRestraint.None, true, true), output.Value.FinalStageSupports);
    }

    [Fact]
    public void CreateComponentWithInputsTest1()
    {
      var comp = new CreateRestraint();
      comp.CreateAttributes();

      bool input1 = false;
      SupportsGoo input2 = new SupportsGoo(new Supports());
      SupportsGoo input3 = new SupportsGoo(new Supports(IntermediateRestraint.None, false, false));

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input2, 1);

      RestraintGoo output = (RestraintGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(input1, output.Value.TopFlangeRestrained);
      Duplicates.AreEqual(input2.Value, output.Value.ConstructionStageSupports);
    }

    [Fact]
    public void CreateComponentWithInputsTest2()
    {
      var comp = new CreateRestraint();
      comp.CreateAttributes();

      bool input1 = false;
      SupportsGoo input2 = new SupportsGoo(new Supports());
      SupportsGoo input3 = new SupportsGoo(new Supports(IntermediateRestraint.None, false, false));

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, input3, 2);

      RestraintGoo output = (RestraintGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(input1, output.Value.TopFlangeRestrained);
      Duplicates.AreEqual(input2.Value, output.Value.ConstructionStageSupports);
      Duplicates.AreEqual(input3.Value, output.Value.FinalStageSupports);
    }

    [Fact]
    public void CreateComponentWithInputsTest3()
    {
      var comp = new CreateRestraint();
      comp.CreateAttributes();

      bool input1 = false;
      SupportsGoo input3 = new SupportsGoo(new Supports(IntermediateRestraint.None, false, false));

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input3, 2);

      RestraintGoo output = (RestraintGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(input1, output.Value.TopFlangeRestrained);
      Duplicates.AreEqual(input3.Value, output.Value.FinalStageSupports);
    }
  }
}
