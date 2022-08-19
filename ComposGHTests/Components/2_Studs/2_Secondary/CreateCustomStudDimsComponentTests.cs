﻿using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;

namespace ComposGHTests.Stud
{
  [Collection("GrasshopperFixture collection")]
  public class CreateCustomStudDimsComponentTests
  {
    public static GH_OasysDropDownComponent CreateCustomStudDimsMother()
    {
      var comp = new CreateCustomStudDimensions();
      comp.CreateAttributes();

      int i = 0;
      ComponentTestHelper.SetInput(comp, 21, i++);
      ComponentTestHelper.SetInput(comp, 110, i++);
      ComponentTestHelper.SetInput(comp, 500, i++);

      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = CreateCustomStudDimsMother();
      comp.SetSelected(0, 1); // change the dropdown to cm
      comp.SetSelected(1, 0); // change the dropdown to N
      StudDimensionsGoo output = (StudDimensionsGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(21, output.Value.Diameter.Centimeters);
      Assert.Equal(110, output.Value.Height.Centimeters);
      Assert.Equal(500, output.Value.CharacterStrength.Newtons);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateCustomStudDimsMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateCustomStudDimsMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}