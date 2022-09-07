using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using static ComposAPI.ConcreteMaterial;
using OasysGH.Components;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateConcreteMaterialENComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateConcreteMaterialEN();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = ComponentMother();

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
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
    public void CreateComponentWithInputs1()
    {
      var comp = ComponentMother();

      comp.SetSelected(1, 5); // change dropdown to KilogramPerCubicMeter
      
      ComponentTestHelper.SetInput(comp, 1864, 0);

      ERatioGoo input2 = (ERatioGoo)ComponentTestHelper.GetOutput(CreateERatioComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, 0.2, 2);
      ComponentTestHelper.SetInput(comp, -0.4, 3);

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.True(output.Value.UserDensity);
      Assert.Equal(1864, output.Value.DryDensity.KilogramsPerCubicMeter);
      Assert.Equal(20, output.Value.ImposedLoadPercentage.Percent);
      Assert.Equal(-0.4, output.Value.ShrinkageStrain.MilliStrain);
      Duplicates.AreEqual(input2.Value, output.Value.ERatio);
      Assert.Equal(DensityClass.NOT_APPLY, output.Value.Class);
    }
    
    [Fact]
    public void CreateComponentWithInputs2()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, "C30/37", 4);

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(ConcreteGradeEN.C30_37.ToString(), output.Value.Grade);
    }

    [Fact]
    public void CreateComponentWithInputs3()
    {
      var comp = ComponentMother();
      Assert.Equal(3, comp.DropDownItems.Count);
      comp.SetSelected(0, comp.DropDownItems[0].Count - 1); // change dropdown to last grade which should be a lightweight one
      Assert.Equal(4, comp.DropDownItems.Count);

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotEqual(DensityClass.NOT_APPLY, output.Value.Class);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
