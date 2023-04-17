using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposGHTests.Stud {
  [Collection("GrasshopperFixture collection")]
  public class CreateStudSpecENComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateStudSpecEN();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentTest() {
      var comp = ComponentMother();

      StudSpecificationGoo output = (StudSpecificationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(Length.Zero, output.Value.NoStudZoneStart);
      Assert.Equal(Length.Zero, output.Value.NoStudZoneEnd);
      Assert.Equal(new Length(30, LengthUnit.Millimeter), output.Value.ReinforcementPosition);
      Assert.True(output.Value.Welding);
      Assert.False(output.Value.NCCI);
    }

    [Fact]
    public void CreateComponentWithInputsTest() {
      var comp = ComponentMother();

      int i = 0;
      ComponentTestHelper.SetInput(comp, 1.7, i++);
      ComponentTestHelper.SetInput(comp, "5 %", i++);
      ComponentTestHelper.SetInput(comp, 0.03, i++);
      ComponentTestHelper.SetInput(comp, false, i++);
      ComponentTestHelper.SetInput(comp, true, i++);

      comp.SetSelected(0, 2); // change the dropdown to m

      StudSpecificationGoo output = (StudSpecificationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(1.7, output.Value.NoStudZoneStart.As(LengthUnit.Meter));
      Assert.Equal(5, output.Value.NoStudZoneEnd.As(RatioUnit.Percent));
      Assert.Equal(0.03, output.Value.ReinforcementPosition.As(LengthUnit.Meter));
      Assert.False(output.Value.Welding);
      Assert.True(output.Value.NCCI);
    }

    [Fact]
    public void DeserializeTest() {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
