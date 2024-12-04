using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Slab {
  [Collection("GrasshopperFixture collection")]
  public class CreateTransverseReinforcementComponentTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateTransverseReinforcement();
      comp.CreateAttributes();

      var input1 = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(CreateRebarMaterialComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input1, 0);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs1() {
      GH_OasysComponent comp = ComponentMother();

      var output = (TransverseReinforcementGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(LayoutMethod.Automatic, output.Value.LayoutMethod);

      var input1 = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(CreateRebarMaterialComponentTests.ComponentMother());
      Duplicates.AreEqual(input1.Value, output.Value.Material);
    }

    [Fact]
    public void CreateComponentWithInputs2() {
      GH_OasysComponent comp = ComponentMother();

      var input2 = (CustomTransverseReinforcementLayoutGoo)ComponentTestHelper.GetOutput(CreateCustomTransverseReinforcementLayoutComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, input2.Duplicate(), 1);

      var output = (TransverseReinforcementGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(LayoutMethod.Custom, output.Value.LayoutMethod);

      var input1 = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(CreateRebarMaterialComponentTests.ComponentMother());
      Duplicates.AreEqual(input1.Value, output.Value.Material);

      Assert.Equal(2, output.Value.CustomReinforcementLayouts.Count);
      Duplicates.AreEqual(input2.Value, output.Value.CustomReinforcementLayouts[0]);
    }
  }
}
