using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet.Units;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateTransverseReinforcementTests
  {
    public static GH_OasysComponent CreateTransverseReinforcementMother()
    {
      var comp = new CreateTransverseReinforcement();
      comp.CreateAttributes();

      ReinforcementMaterialGoo input1 = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(CreateRebarMaterialTests.CreateRebarMaterialMother());
      ComponentTestHelper.SetInput(comp, input1, 0);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs1()
    {
      var comp = CreateTransverseReinforcementMother();

      TransverseReinforcementGoo output = (TransverseReinforcementGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(LayoutMethod.Automatic, output.Value.LayoutMethod);

      ReinforcementMaterialGoo input1 = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(CreateRebarMaterialTests.CreateRebarMaterialMother());
      Duplicates.AreEqual(input1.Value, output.Value.Material);
    }

    [Fact]
    public void CreateComponentWithInputs2()
    {
      var comp = CreateTransverseReinforcementMother();

      CustomTransverseReinforcementLayoutGoo input2 = (CustomTransverseReinforcementLayoutGoo)ComponentTestHelper.GetOutput(CreateCustomTransverseReinforcementLayoutTests.CreateCustomTransverseReinforcementLayoutMother());
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, input2.Duplicate(), 1);

      TransverseReinforcementGoo output = (TransverseReinforcementGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(LayoutMethod.Custom, output.Value.LayoutMethod);

      ReinforcementMaterialGoo input1 = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(CreateRebarMaterialTests.CreateRebarMaterialMother());
      Duplicates.AreEqual(input1.Value, output.Value.Material);

      Assert.Equal(2, output.Value.CustomReinforcementLayouts.Count);
      Duplicates.AreEqual(input2.Value, output.Value.CustomReinforcementLayouts[0]);
    }
  }
}
