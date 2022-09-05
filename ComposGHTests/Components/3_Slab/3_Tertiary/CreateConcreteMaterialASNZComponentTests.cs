using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using OasysGH.Components;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateConcreteMaterialASNZComponentTests
  {
    public static GH_OasysDropDownComponent CreateConcreteMaterialASNZMother()
    {
      var comp = new CreateConcreteMaterialASNZ();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = CreateConcreteMaterialASNZMother();

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(ConcreteGrade.C20.ToString(), output.Value.Grade);
      Assert.Equal(2450, output.Value.DryDensity.KilogramsPerCubicMeter);
      Assert.False(output.Value.UserDensity);
      Assert.Equal(-0.85, output.Value.ShrinkageStrain.MilliStrain);
      Assert.False(output.Value.UserStrain);
      Assert.Equal(33, output.Value.ImposedLoadPercentage.Percent);
      Duplicates.AreEqual(new ERatio(), output.Value.ERatio);
    }

    [Fact]
    public void CreateComponentWithInputs1()
    {
      var comp = CreateConcreteMaterialASNZMother();

      comp.SetSelected(1, 5); // change dropdown to KilogramPerCubicMeter
      
      ComponentTestHelper.SetInput(comp, 1864, 0);

      ERatioGoo input2 = (ERatioGoo)ComponentTestHelper.GetOutput(CreateERatioComponentTests.CreateERatioMother());
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, 0.2, 2);
      ComponentTestHelper.SetInput(comp, -0.4, 3);

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.True(output.Value.UserDensity);
      Assert.Equal(1864, output.Value.DryDensity.KilogramsPerCubicMeter);
      Assert.Equal(20, output.Value.ImposedLoadPercentage.Percent);
      Assert.Equal(-0.4, output.Value.ShrinkageStrain.MilliStrain);
      Duplicates.AreEqual(input2.Value, output.Value.ERatio);
    }
    
    [Fact]
    public void CreateComponentWithInputs2()
    {
      var comp = CreateConcreteMaterialASNZMother();

      ComponentTestHelper.SetInput(comp, "C30", 4);

      ConcreteMaterialGoo output = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(ConcreteGrade.C30.ToString(), output.Value.Grade);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateConcreteMaterialASNZMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateConcreteMaterialASNZMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
