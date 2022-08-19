﻿using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGHTests.Stud
{
  [Collection("GrasshopperFixture collection")]
  public class CreateStudSpecBSComponentTests
  {
    public static GH_OasysDropDownComponent CreateStudSpecBSMother()
    {
      var comp = new CreateStudSpecBS();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = CreateStudSpecBSMother();

      StudSpecificationGoo output = (StudSpecificationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(Length.Zero, output.Value.NoStudZoneStart);
      Assert.Equal(Length.Zero, output.Value.NoStudZoneEnd);
      Assert.True(output.Value.EC4_Limit);
    }

    [Fact]
    public void CreateComponentWithInputsTest()
    {
      var comp = CreateStudSpecBSMother();

      int i = 0;
      ComponentTestHelper.SetInput(comp, 250, i++);
      ComponentTestHelper.SetInput(comp, "15 %", i++);
      ComponentTestHelper.SetInput(comp, false, i++);

      comp.SetSelected(0, 1); // change the dropdown to cm

      StudSpecificationGoo output = (StudSpecificationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(250, output.Value.NoStudZoneStart.As(LengthUnit.Centimeter));
      Assert.Equal(15, output.Value.NoStudZoneEnd.As(RatioUnit.Percent));
      Assert.False(output.Value.EC4_Limit);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateStudSpecBSMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateStudSpecBSMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}