using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Beam;
using ComposGHTests.Helpers;
using ComposGHTests.Load;
using ComposGHTests.Slab;
using ComposGHTests.Stud;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Member {
  [Collection("GrasshopperFixture collection")]
  public class CreateMemberComponentTests {

    public static GH_OasysComponent CreateMemberMother() {
      var comp = new CreateMember();
      comp.CreateAttributes();

      var input1 = (BeamGoo)ComponentTestHelper.GetOutput(CreateBeamComponentTests.ComponentMother());
      var input2 = (StudGoo)ComponentTestHelper.GetOutput(CreateStudComponentTests.ComponentMother());
      var input3 = (SlabGoo)ComponentTestHelper.GetOutput(CreateSlabComponentTests.ComponentMother());
      var input4_1 = (LoadGoo)ComponentTestHelper.GetOutput(CreateUniformLoadComponentTests.ComponentMother());
      var input4_2 = (LoadGoo)ComponentTestHelper.GetOutput(CreatePointLoadComponentTests.ComponentMother());
      var input5 = (DesignCodeGoo)ComponentTestHelper.GetOutput(CreateDesignCodeComponentTests.ComponentMother());

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, input3, 2);
      ComponentTestHelper.SetInput(comp, input4_1, 3);
      ComponentTestHelper.SetInput(comp, input4_2, 3);
      ComponentTestHelper.SetInput(comp, input5, 4);
      ComponentTestHelper.SetInput(comp, "MEMBER-1", 5);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs1() {
      GH_OasysComponent comp = CreateMemberMother();

      var expected_input1 = (BeamGoo)ComponentTestHelper.GetOutput(CreateBeamComponentTests.ComponentMother());
      var expected_input2 = (StudGoo)ComponentTestHelper.GetOutput(CreateStudComponentTests.ComponentMother());
      var expected_input3 = (SlabGoo)ComponentTestHelper.GetOutput(CreateSlabComponentTests.ComponentMother());
      var expected_input4_1 = (LoadGoo)ComponentTestHelper.GetOutput(CreateUniformLoadComponentTests.ComponentMother());
      var expected_input4_2 = (LoadGoo)ComponentTestHelper.GetOutput(CreatePointLoadComponentTests.ComponentMother());
      var expected_input5 = (DesignCodeGoo)ComponentTestHelper.GetOutput(CreateDesignCodeComponentTests.ComponentMother());

      var output = (MemberGoo)ComponentTestHelper.GetOutput(comp);

      Duplicates.AreEqual(expected_input1.Value, output.Value.Beam);
      Duplicates.AreEqual(expected_input2.Value, output.Value.Stud);
      Duplicates.AreEqual(expected_input3.Value, output.Value.Slab);
      Duplicates.AreEqual(2, output.Value.Loads.Count);
      Duplicates.AreEqual(expected_input4_1.Value, output.Value.Loads[0]);
      Duplicates.AreEqual(expected_input4_2.Value, output.Value.Loads[1]);
      Duplicates.AreEqual(expected_input5.Value, output.Value.DesignCode);
      Assert.Equal("MEMBER-1", output.Value.Name);
    }

    [Fact]
    public void CreateComponentWithInputs2() {
      GH_OasysComponent comp = CreateMemberMother();

      ComponentTestHelper.SetInput(comp, "Grid-1", 6);
      ComponentTestHelper.SetInput(comp, "Note this down", 7);

      var output = (MemberGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal("Grid-1", output.Value.GridReference);
      Assert.Equal("Note this down", output.Value.Note);
    }
  }
}
