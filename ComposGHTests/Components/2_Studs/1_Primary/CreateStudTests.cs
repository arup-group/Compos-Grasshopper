using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;

namespace ComposGHTests.Stud
{
  [Collection("GrasshopperFixture collection")]
  public class CreateStudTests
  {
    public static GH_OasysDropDownComponent CreateStudMother()
    {
      var comp = new CreateStud();
      comp.CreateAttributes();

      StudDimensionsGoo input1 = (StudDimensionsGoo)ComponentTestHelper.GetOutput(CreateStandardStudDimsTests.CreateStandardStudDimsMother());
      ComponentTestHelper.SetInput(comp, input1, 0);

      StudSpecificationGoo input2 = (StudSpecificationGoo)ComponentTestHelper.GetOutput(CreateStudSpecBSTests.CreateStudSpecBSMother());
      ComponentTestHelper.SetInput(comp, input2, 1);

      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = CreateStudMother();
      comp.SetSelected(0, 0); // change the dropdown to Automatic
      
      StudGoo output = (StudGoo)ComponentTestHelper.GetOutput(comp);
      
      Assert.Equal(StudSpacingType.Automatic, output.Value.StudSpacingType);
      
      StudDimensionsGoo input1 = (StudDimensionsGoo)ComponentTestHelper.GetOutput(CreateStandardStudDimsTests.CreateStandardStudDimsMother());
      ComponentTestHelper.SetInput(comp, input1, 0);
      Duplicates.AreEqual(input1.Value, output.Value.Dimensions);

      StudSpecificationGoo input2 = (StudSpecificationGoo)ComponentTestHelper.GetOutput(CreateStudSpecBSTests.CreateStudSpecBSMother());
      Duplicates.AreEqual(input2.Value, output.Value.Specification);

      Assert.Equal(0.2, output.Value.MinSavingMultipleZones);
    }

    [Fact]
    public void CreateComponentWithInputsTest1()
    {
      var comp = CreateStudMother();
      Assert.Equal(3, comp.Params.Input.Count);
      comp.SetSelected(0, 1); // change the dropdown to Partial_Interaction
      Assert.Equal(4, comp.Params.Input.Count);

      StudDimensionsGoo input1 = (StudDimensionsGoo)ComponentTestHelper.GetOutput(CreateStandardStudDimsTests.CreateStandardStudDimsMother());
      ComponentTestHelper.SetInput(comp, input1, 0);
      StudSpecificationGoo input2 = (StudSpecificationGoo)ComponentTestHelper.GetOutput(CreateStudSpecBSTests.CreateStudSpecBSMother());

      StudGoo output = (StudGoo)ComponentTestHelper.GetOutput(comp);
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
      StudGroupSpacingGoo input3_1 = (StudGroupSpacingGoo)ComponentTestHelper.GetOutput(CreateCustomStudSpacingTests.CreateCustomStudSpacingMother());
      StudGroupSpacingGoo input3_2 = (StudGroupSpacingGoo)input3_1.Duplicate();
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
    public void DeserializeTest()
    {
      var comp = CreateStudMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateStudMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
