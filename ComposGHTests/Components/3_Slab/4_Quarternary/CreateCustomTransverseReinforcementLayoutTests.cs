using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet.Units;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateCustomTransverseReinforcementLayoutTests
  {
    public static GH_OasysDropDownComponent CreateCustomTransverseReinforcementLayoutMother()
    {
      var comp = new CreateCustomTransverseReinforcementLayout();
      comp.CreateAttributes();
      
      int i = 0;
      ComponentTestHelper.SetInput(comp, -0.5, i++);
      ComponentTestHelper.SetInput(comp, 2000, i++);
      ComponentTestHelper.SetInput(comp, 20, i++);
      ComponentTestHelper.SetInput(comp, 250, i++);
      ComponentTestHelper.SetInput(comp, 35, i++);

      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = CreateCustomTransverseReinforcementLayoutMother();

      comp.SetSelected(0, 0); // set dropdown to mm

      CustomTransverseReinforcementLayoutGoo output = (CustomTransverseReinforcementLayoutGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(50, output.Value.StartPosition.As(RatioUnit.Percent));
      Assert.Equal(2000, output.Value.EndPosition.As(LengthUnit.Millimeter));
      Assert.Equal(20, output.Value.Diameter.As(LengthUnit.Millimeter));
      Assert.Equal(250, output.Value.Spacing.As(LengthUnit.Millimeter));
      Assert.Equal(35, output.Value.Cover.As(LengthUnit.Millimeter));
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateCustomTransverseReinforcementLayoutMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateCustomTransverseReinforcementLayoutMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
