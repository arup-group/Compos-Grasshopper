using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using Xunit;
using static ComposAPI.ConcreteMaterial;

namespace ComposGHTests.Slab {
  [Collection("GrasshopperFixture collection")]
  public class CreateConcreteMaterialENComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateConcreteMaterialEN();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(ConcreteGradeEN.C20_25.ToString(), output.Value.Grade);
      Assert.Equal(2400, output.Value.DryDensity.KilogramsPerCubicMeter);
      Assert.False(output.Value.UserDensity);
      Assert.Equal(-0.5, output.Value.ShrinkageStrain.MilliStrain);
      Assert.False(output.Value.UserStrain);
      Assert.Equal(33, output.Value.ImposedLoadPercentage.Percent);
      Assert.Equal(6, output.Value.ERatio.ShortTerm);
      Assert.Equal(18, output.Value.ERatio.LongTerm);
      Assert.Equal(5.39, output.Value.ERatio.Vibration);
    }

    [Fact]
    public void CreateComponentWithInputs1() {
      GH_OasysDropDownComponent comp = ComponentMother();

      comp.SetSelected(1, 5); // change dropdown to KilogramPerCubicMeter

      ComponentTestHelper.SetInput(comp, 1864, 0);

      var input2 = (ERatioGoo)ComponentTestHelper.GetOutput(CreateERatioComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, 0.2, 2);
      ComponentTestHelper.SetInput(comp, -0.4, 3);

      var output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.True(output.Value.UserDensity);
      Assert.Equal(1864, output.Value.DryDensity.KilogramsPerCubicMeter);
      Assert.Equal(20, output.Value.ImposedLoadPercentage.Percent);
      Assert.Equal(-0.4, output.Value.ShrinkageStrain.MilliStrain);
      Duplicates.AreEqual(input2.Value, output.Value.ERatio);
      Assert.Equal(DensityClass.NOT_APPLY, output.Value.Class);
    }

    [Fact]
    public void CreateComponentWithInputs2() {
      GH_OasysDropDownComponent comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, "C30/37", 4);

      var output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(ConcreteGradeEN.C30_37.ToString(), output.Value.Grade);
    }

    [Fact]
    public void CreateComponentWithInputs3() {
      GH_OasysDropDownComponent comp = ComponentMother();
      Assert.Equal(3, comp._dropDownItems.Count);
      comp.SetSelected(0, comp._dropDownItems[0].Count - 1); // change dropdown to last grade which should be a lightweight one
      Assert.Equal(4, comp._dropDownItems.Count);

      var output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotEqual(DensityClass.NOT_APPLY, output.Value.Class);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
