﻿using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using OasysGH.Components;

namespace ComposGHTests.Stud
{
  [Collection("GrasshopperFixture collection")]
  public class CreateStandardStudDimsComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateStandardStudDimensions();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = ComponentMother();
      
      StudDimensionsGoo output = (StudDimensionsGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(19, output.Value.Diameter.Millimeters);
      Assert.Equal(100, output.Value.Height.Millimeters);
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
