using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using OasysGH.Components;
using static ComposAPI.ConcreteMaterial;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateConcreteMaterialBSComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateConcreteMaterialBS();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = ComponentMother();

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(ConcreteGrade.C25.ToString(), output.Value.Grade);
      Assert.Equal(2400, output.Value.DryDensity.KilogramsPerCubicMeter);
      Assert.False(output.Value.UserDensity);
      Assert.Equal(WeightType.Normal, output.Value.Type);
      Assert.Equal(33, output.Value.ImposedLoadPercentage.Percent);
      Duplicates.AreEqual(new ERatio(), output.Value.ERatio);
    }

    [Fact]
    public void CreateComponentWithInputs1()
    {
      var comp = ComponentMother();

      comp.SetSelected(2, 5); // change dropdown to KilogramPerCubicMeter
      
      ComponentTestHelper.SetInput(comp, 1864, 0);

      ERatioGoo input2 = (ERatioGoo)ComponentTestHelper.GetOutput(CreateERatioComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, 0.2, 2);

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.True(output.Value.UserDensity);
      Assert.Equal(1864, output.Value.DryDensity.KilogramsPerCubicMeter);
      Assert.Equal(20, output.Value.ImposedLoadPercentage.Percent);
      Duplicates.AreEqual(input2.Value, output.Value.ERatio);
      Assert.Equal(DensityClass.NOT_APPLY, output.Value.Class);
    }
    
    [Fact]
    public void CreateComponentWithInputs2()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, "C30", 3);

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(ConcreteGrade.C30.ToString(), output.Value.Grade);
    }

    [Fact]
    public void CreateComponentWithInputs3()
    {
      var comp = ComponentMother();
      comp.SetSelected(1, 1); // change dropdown to lightweight

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(WeightType.LightWeight, output.Value.Type);
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
