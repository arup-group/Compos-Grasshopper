using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using Xunit;

namespace ComposGHTests.Beam {
  [Collection("GrasshopperFixture collection")]
  public class CreateRestraintComponentTests {

    [Fact]
    public void CreateComponentTest() {
      var comp = new CreateRestraint();
      comp.CreateAttributes();

      var output = (RestraintGoo)ComponentTestHelper.GetOutput(comp);
      Assert.True(output.Value.TopFlangeRestrained);
      Duplicates.AreEqual(new Supports(), output.Value.ConstructionStageSupports);
      Duplicates.AreEqual(new Supports(IntermediateRestraint.None, true, true), output.Value.FinalStageSupports);
    }

    [Fact]
    public void CreateComponentWithInputsTest1() {
      var comp = new CreateRestraint();
      comp.CreateAttributes();

      bool input1 = false;
      var input2 = new SupportsGoo(new Supports());

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input2, 1);

      var output = (RestraintGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(input1, output.Value.TopFlangeRestrained);
      Duplicates.AreEqual(input2.Value, output.Value.ConstructionStageSupports);
    }

    [Fact]
    public void CreateComponentWithInputsTest2() {
      var comp = new CreateRestraint();
      comp.CreateAttributes();

      bool input1 = false;
      var input2 = new SupportsGoo(new Supports());
      var input3 = new SupportsGoo(new Supports(IntermediateRestraint.None, false, false));

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, input3, 2);

      var output = (RestraintGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(input1, output.Value.TopFlangeRestrained);
      Duplicates.AreEqual(input2.Value, output.Value.ConstructionStageSupports);
      Duplicates.AreEqual(input3.Value, output.Value.FinalStageSupports);
    }

    [Fact]
    public void CreateComponentWithInputsTest3() {
      var comp = new CreateRestraint();
      comp.CreateAttributes();

      bool input1 = false;
      var input3 = new SupportsGoo(new Supports(IntermediateRestraint.None, false, false));

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input3, 2);

      var output = (RestraintGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(input1, output.Value.TopFlangeRestrained);
      Duplicates.AreEqual(input3.Value, output.Value.FinalStageSupports);
    }
  }
}
