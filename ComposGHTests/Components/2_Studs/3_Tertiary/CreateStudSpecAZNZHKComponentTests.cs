using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposGHTests.Stud {
  [Collection("GrasshopperFixture collection")]
  public class CreateStudSpecAZNZHKComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateStudSpecAZNZHK();
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
      Assert.True(output.Value.Welding);
    }

    [Fact]
    public void CreateComponentWithInputsTest() {
      GH_OasysDropDownComponent comp = ComponentMother();

      int i = 0;
      ComponentTestHelper.SetInput(comp, "50 %", i++);
      ComponentTestHelper.SetInput(comp, 250, i++);
      ComponentTestHelper.SetInput(comp, false, i++);

      comp.SetSelected(0, 3); // change the dropdown to mm

      var output = (StudSpecificationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(50, output.Value.NoStudZoneStart.As(RatioUnit.Percent));
      Assert.Equal(250, output.Value.NoStudZoneEnd.As(LengthUnit.Inch));
      Assert.False(output.Value.Welding);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
