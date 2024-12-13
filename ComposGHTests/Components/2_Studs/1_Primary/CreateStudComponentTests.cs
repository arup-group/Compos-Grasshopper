using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Stud {
  [Collection("GrasshopperFixture collection")]
  public class CreateStudComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateStud();
      comp.CreateAttributes();

      var input1 = (StudDimensionsGoo)ComponentTestHelper.GetOutput(CreateStandardStudDimsComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input1, 0);

      var input2 = (StudSpecificationGoo)ComponentTestHelper.GetOutput(CreateStudSpecBSComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input2, 1);

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

      var output = (StudGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(StudSpacingType.Min_Num_of_Studs, output.Value.StudSpacingType);

      var input1 = (StudDimensionsGoo)ComponentTestHelper.GetOutput(CreateStandardStudDimsComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input1, 0);
      Duplicates.AreEqual(input1.Value, output.Value.Dimensions);

      var input2 = (StudSpecificationGoo)ComponentTestHelper.GetOutput(CreateStudSpecBSComponentTests.ComponentMother());
      Duplicates.AreEqual(input2.Value, output.Value.Specification);

      Assert.Equal(0.2, output.Value.MinSavingMultipleZones);
    }

    [Fact]
    public void CreateComponentWithInputsTest1() {
      GH_OasysDropDownComponent comp = ComponentMother();
      Assert.Equal(3, comp.Params.Input.Count);
      comp.SetSelected(0, 1); // change the dropdown to Partial_Interaction
      Assert.Equal(4, comp.Params.Input.Count);

      var input1 = (StudDimensionsGoo)ComponentTestHelper.GetOutput(CreateStandardStudDimsComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input1, 0);
      var input2 = (StudSpecificationGoo)ComponentTestHelper.GetOutput(CreateStudSpecBSComponentTests.ComponentMother());

      var output = (StudGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(StudSpacingType.Partial_Interaction, output.Value.StudSpacingType);
      Duplicates.AreEqual(input1.Value, output.Value.Dimensions);
      Duplicates.AreEqual(input2.Value, output.Value.Specification);
      // test default values
      Assert.Equal(0.2, output.Value.MinSavingMultipleZones);
      Assert.Equal(0.85, output.Value.Interaction);

      // override default values
      ComponentTestHelper.SetInput(comp, 0.4, 2);
      ComponentTestHelper.SetInput(comp, 0.8, 3);
      output = (StudGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0.4, output.Value.MinSavingMultipleZones);
      Assert.Equal(0.80, output.Value.Interaction);

      // change the dropdown to Min_Num_of_Studs
      comp.SetSelected(0, 2);
      Assert.Equal(3, comp.Params.Input.Count);
      ComponentTestHelper.SetInput(comp, 0.1, 2);
      output = (StudGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0.1, output.Value.MinSavingMultipleZones);

      // change the dropdown to Custom
      comp.SetSelected(0, 3);
      Assert.Equal(4, comp.Params.Input.Count);
      var input3_1 = (StudGroupSpacingGoo)ComponentTestHelper.GetOutput(CreateCustomStudSpacingComponentTests.ComponentMother());
      var input3_2 = (StudGroupSpacingGoo)input3_1.Duplicate();
      ComponentTestHelper.SetInput(comp, input3_1, 2);
      ComponentTestHelper.SetInput(comp, input3_2, 2);
      output = (StudGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(2, output.Value.CustomSpacing.Count);
      Duplicates.AreEqual(input3_1.Value, output.Value.CustomSpacing[0]);
      Duplicates.AreEqual(input3_2.Value, output.Value.CustomSpacing[1]);
      Assert.False(output.Value.CheckStudSpacing);
      output = (StudGoo)ComponentTestHelper.GetOutput(comp);

      ComponentTestHelper.SetInput(comp, true, 3);
      output = (StudGoo)ComponentTestHelper.GetOutput(comp);
      Assert.True(output.Value.CheckStudSpacing);

      comp.SetSelected(0, 0); // change the dropdown to Automatci
      Assert.Equal(3, comp.Params.Input.Count);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
