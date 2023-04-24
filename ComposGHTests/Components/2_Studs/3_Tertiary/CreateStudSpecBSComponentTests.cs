using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposGHTests.Stud {
  [Collection("GrasshopperFixture collection")]
  public class CreateStudSpecBSComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateStudSpecBS();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (StudSpecificationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(Length.Zero, output.Value.NoStudZoneStart);
      Assert.Equal(Length.Zero, output.Value.NoStudZoneEnd);
      Assert.True(output.Value.EC4_Limit);
    }

    [Fact]
    public void CreateComponentWithInputsTest() {
      GH_OasysDropDownComponent comp = ComponentMother();

      int i = 0;
      ComponentTestHelper.SetInput(comp, 250, i++);
      ComponentTestHelper.SetInput(comp, "15 %", i++);
      ComponentTestHelper.SetInput(comp, false, i++);

      comp.SetSelected(0, 1); // change the dropdown to cm

      var output = (StudSpecificationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(250, output.Value.NoStudZoneStart.As(LengthUnit.Centimeter));
      Assert.Equal(15, output.Value.NoStudZoneEnd.As(RatioUnit.Percent));
      Assert.False(output.Value.EC4_Limit);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
