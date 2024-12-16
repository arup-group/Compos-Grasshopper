using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Member {
  [Collection("GrasshopperFixture collection")]
  public class CreateDesignCodeComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateDesignCode();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp, true);
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (DesignCodeGoo)ComponentTestHelper.GetOutput(comp);
      var code = (EN1994)output.Value;
      Duplicates.AreEqual(new SafetyFactorsEN(), code.SafetyFactors);
      Duplicates.AreEqual(new DesignOption(), code.DesignOption);
      Duplicates.AreEqual(new CodeOptionsEN(), code.CodeOptions);
      Assert.Equal(Code.EN1994_1_1_2004, code.Code);
      Assert.Equal(NationalAnnex.Generic, code.NationalAnnex);

      comp.SetSelected(1, 1); // change dropdown to british na
      output = (DesignCodeGoo)ComponentTestHelper.GetOutput(comp);
      code = (EN1994)output.Value;
      Assert.Equal(NationalAnnex.United_Kingdom, code.NationalAnnex);
    }

    [Fact]
    public void CreateComponentASNZWithInputs() {
      GH_OasysDropDownComponent comp = ComponentMother();
      Assert.Equal(3, comp.Params.Input.Count);

      comp.SetSelected(0, 5); // change to asnz
      Assert.Equal(3, comp.Params.Input.Count);

      ComponentTestHelper.SetInput(comp, 1.8, 1);
      ComponentTestHelper.SetInput(comp, 2.2, 2);

      var output = (DesignCodeGoo)ComponentTestHelper.GetOutput(comp);
      var code = (ASNZS2327)output.Value;
      Assert.Equal(Code.AS_NZS2327_2017, code.Code);
      Assert.Equal(1.8, code.CodeOptions.ShortTerm.CreepCoefficient);
      Assert.Equal(2.2, code.CodeOptions.LongTerm.CreepCoefficient);
    }

    [Fact]
    public void CreateComponentBSHKToggleDropdown() {
      GH_OasysDropDownComponent comp = ComponentMother();
      Assert.Equal(3, comp.Params.Input.Count);

      comp.SetSelected(0, 0); // change to bs superseded
      Assert.Single(comp.Params.Input);

      var output = (DesignCodeGoo)ComponentTestHelper.GetOutput(comp);
      var code = (DesignCode)output.Value;
      Assert.Equal(Code.BS5950_3_1_1990_Superseded, code.Code);

      comp.SetSelected(0, 5); // change to asnz
      Assert.Equal(3, comp.Params.Input.Count);

      comp.SetSelected(0, 1); // change to bs
      output = (DesignCodeGoo)ComponentTestHelper.GetOutput(comp, 0, 0, 0, true);
      code = (DesignCode)output.Value;
      Assert.Equal(Code.BS5950_3_1_1990_A1_2010, code.Code);

      comp.SetSelected(0, 2); // change to en
      Assert.Equal(3, comp.Params.Input.Count);

      comp.SetSelected(0, 3); // change to hk2005
      Assert.Single(comp.Params.Input);
      output = (DesignCodeGoo)ComponentTestHelper.GetOutput(comp, 0, 0, 0, true);
      code = (DesignCode)output.Value;
      Assert.Equal(Code.HKSUOS_2005, code.Code);

      comp.SetSelected(0, 5); // change to asnz
      Assert.Equal(3, comp.Params.Input.Count);

      comp.SetSelected(0, 4); // change to hk2005
      Assert.Single(comp.Params.Input);
      output = (DesignCodeGoo)ComponentTestHelper.GetOutput(comp, 0, 0, 0, true);
      code = (DesignCode)output.Value;
      Assert.Equal(Code.HKSUOS_2011, code.Code);

      comp.SetSelected(0, 2); // change to en
      Assert.Equal(3, comp.Params.Input.Count);
    }

    [Fact]
    public void CreateComponentENWithInputs() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var input1 = new SafetyFactorsENGoo(new SafetyFactorsEN());
      ComponentTestHelper.SetInput(comp, input1, 0);

      var input2 = new CreepShrinkageParametersGoo(new CreepShrinkageParametersEN());
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, input2, 2);

      var output = (DesignCodeGoo)ComponentTestHelper.GetOutput(comp);
      var code = (EN1994)output.Value;
      Duplicates.AreEqual(input1.Value, code.SafetyFactors);
      Duplicates.AreEqual(input2.Value, code.CodeOptions.ShortTerm);
      Duplicates.AreEqual(input2.Value, code.CodeOptions.LongTerm);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
