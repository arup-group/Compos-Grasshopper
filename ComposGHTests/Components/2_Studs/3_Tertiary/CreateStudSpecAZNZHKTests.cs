using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGHTests.Stud
{
  [Collection("GrasshopperFixture collection")]
  public class CreateStudSpecAZNZHKTests
  {
    public static GH_OasysDropDownComponent CreateStudSpecAZNZHKMother()
    {
      var comp = new CreateStudSpecAZNZHK();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = CreateStudSpecAZNZHKMother();

      StudSpecificationGoo output = (StudSpecificationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(Length.Zero, output.Value.NoStudZoneStart);
      Assert.Equal(Length.Zero, output.Value.NoStudZoneEnd);
      Assert.True(output.Value.Welding);
    }

    [Fact]
    public void CreateComponentWithInputsTest()
    {
      var comp = CreateStudSpecAZNZHKMother();

      int i = 0;
      ComponentTestHelper.SetInput(comp, "50 %", i++);
      ComponentTestHelper.SetInput(comp, 250, i++);
      ComponentTestHelper.SetInput(comp, false, i++);

      comp.SetSelected(0, 3); // change the dropdown to mm

      StudSpecificationGoo output = (StudSpecificationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(50, output.Value.NoStudZoneStart.As(RatioUnit.Percent));
      Assert.Equal(250, output.Value.NoStudZoneEnd.As(LengthUnit.Inch));
      Assert.False(output.Value.Welding);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateStudSpecAZNZHKMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateStudSpecAZNZHKMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
