﻿using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using System.Collections.Generic;
using UnitsNet;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class CreateSupportComponentTests
  { 
    [Fact]
    public void CreateComponentTest()
    {
      var comp = new CreateSupport();
      comp.CreateAttributes();

      SupportsGoo output = (SupportsGoo)ComponentTestHelper.GetOutput(comp);
      Assert.True(output.Value.SecondaryMemberAsIntermediateRestraint);
      Assert.True(output.Value.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Null(output.Value.CustomIntermediateRestraintPositions);
      Assert.Equal(IntermediateRestraint.None, output.Value.IntermediateRestraintPositions);
    }

    [Fact]
    public void CreateComponentWithInputsTest()
    {
      var comp = new CreateSupport();
      comp.CreateAttributes();

      bool input1 = false;
      bool input2 = false;
      List<object> input3 = new List<object>() { "2 %", "5000 mm", "8.7 m" };
      List<IQuantity> quantities = new List<IQuantity>()
      {
        new Ratio(2, UnitsNet.Units.RatioUnit.Percent),
        new Length(5000, UnitsNet.Units.LengthUnit.Millimeter),
        new Length(8.7, UnitsNet.Units.LengthUnit.Meter)
      };

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, input3, 2);

      SupportsGoo output = (SupportsGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(input1, output.Value.SecondaryMemberAsIntermediateRestraint);
      Assert.Equal(input2, output.Value.BothFlangesFreeToRotateOnPlanAtEnds);
      Assert.Equal(quantities, output.Value.CustomIntermediateRestraintPositions);
      Assert.Equal(IntermediateRestraint.Custom, output.Value.IntermediateRestraintPositions);
    }

    [Fact]
    public void DeserializeTest()
    {
      GH_OasysDropDownComponent comp = new CreateSupport();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      GH_OasysDropDownComponent comp = new CreateSupport();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
