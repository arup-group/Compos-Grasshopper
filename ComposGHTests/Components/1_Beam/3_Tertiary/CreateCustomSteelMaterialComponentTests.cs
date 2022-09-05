using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using OasysGH.Components;

namespace ComposGHTests.Beam
{
  [Collection("GrasshopperFixture collection")]
  public class CreateCustomSteelMaterialComponentTests
  {
    public static GH_OasysDropDownComponent CreateCustomSteelMaterialComponentMother()
    {
      var comp = new CreateCustomSteelMaterial();
      comp.CreateAttributes();

      double input1 = 500;
      double input2 = 205000;
      double input3 = 7850;

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, input3, 2);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputsTest1()
    {
      GH_OasysDropDownComponent comp = CreateCustomSteelMaterialComponentMother();

      SteelMaterialGoo output = (SteelMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(500, output.Value.fy.Value);
      Assert.Equal(205000, output.Value.E.Value);
      Assert.Equal(7850, output.Value.Density.Value);
      Assert.False(output.Value.ReductionFactorMpl);
      Assert.Equal(WeldMaterialGrade.Grade_35, output.Value.WeldGrade);
    }

    [Fact]
    public void CreateComponentWithInputsTest2()
    {
      GH_OasysDropDownComponent comp = CreateCustomSteelMaterialComponentMother();

      bool input4 = true;
      string input5 = "Grade_50";
      ComponentTestHelper.SetInput(comp, input4, 3);
      ComponentTestHelper.SetInput(comp, input5, 4);
      
      SteelMaterialGoo output = (SteelMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(input4, output.Value.ReductionFactorMpl);
      Assert.Equal(WeldMaterialGrade.Grade_50, output.Value.WeldGrade);
    }

    [Fact]
    public void DeserializeTest()
    {
      GH_OasysDropDownComponent comp = CreateCustomSteelMaterialComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      GH_OasysDropDownComponent comp = CreateCustomSteelMaterialComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
