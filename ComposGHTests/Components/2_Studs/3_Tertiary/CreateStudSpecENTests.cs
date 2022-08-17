using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class CreateStudSpecENTests
  {
    public static GH_OasysDropDownComponent CreateStudSpecENMother()
    {
      var comp = new CreateStudSpecEN();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = CreateStudSpecENMother();

      StudSpecificationGoo output = (StudSpecificationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(Length.Zero, output.Value.NoStudZoneStart);
      Assert.Equal(Length.Zero, output.Value.NoStudZoneEnd);
      Assert.Equal(new Length(30, LengthUnit.Millimeter), output.Value.ReinforcementPosition);
      Assert.True(output.Value.Welding);
      Assert.False(output.Value.NCCI);
    }

    [Fact]
    public void CreateComponentWithInputsTest()
    {
      var comp = CreateStudSpecENMother();

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
    public void DeserializeTest()
    {
      var comp = CreateStudSpecENMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateStudSpecENMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
