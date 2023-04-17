using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using Grasshopper.Kernel.Types;
using OasysGH.Components;
using OasysGH.Parameters;
using Xunit;

namespace ComposGHTests.Result {
  [Collection("GrasshopperFixture collection")]
  public class StudResultsComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new StudResults();
      comp.CreateAttributes();
      MemberGoo input = (MemberGoo)ComponentTestHelper.GetOutput(CompFile.FileComponentsTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input);

      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentWithInput() {
      var comp = ComponentMother();
      comp.ExpireSolution(true);

      int expectedNumberOfResults = 7;

      int i = 0;
      comp.Params.Output[i].CollectData();
      for (int j = 0; j < expectedNumberOfResults; j++) {
        GH_UnitNumber output = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i, 0, j);
        Assert.NotNull(output);
      }
      i++;

      while (i < 5) {
        comp.Params.Output[i].CollectData();
        for (int j = 0; j < expectedNumberOfResults; j++) {
          GH_Integer output = (GH_Integer)ComponentTestHelper.GetOutput(comp, i, 0, j);
          Assert.NotNull(output);
          Assert.True(output.Value >= 0);
        }
        i++;
      }

      comp.Params.Output[5].CollectData();
      for (int j = 0; j < expectedNumberOfResults; j++) {
        GH_UnitNumber output = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 5, 0, j);
        Assert.NotNull(output);
      }
      i++;

      comp.Params.Output[6].CollectData();
      for (int j = 0; j < expectedNumberOfResults; j++) {
        GH_Number output = (GH_Number)ComponentTestHelper.GetOutput(comp, 6, 0, j);
        Assert.NotNull(output);
        Assert.True(output.Value >= 0);
      }
      i++;

      comp.Params.Output[7].CollectData();
      GH_UnitNumber output1 = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 7);
      Assert.NotNull(output1);
      i++;

      comp.Params.Output[8].CollectData();
      for (int j = 0; j < expectedNumberOfResults; j++) {
        GH_Number output = (GH_Number)ComponentTestHelper.GetOutput(comp, 8, 0, j);
        Assert.NotNull(output);
        Assert.True(output.Value >= 0);
      }
      i++;

      while (i < comp.Params.Output.Count) {
        comp.Params.Output[i].CollectData();
        for (int j = 0; j < expectedNumberOfResults; j++) {
          GH_UnitNumber output = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i, 0, j);
          Assert.NotNull(output);
        }
        i++;
      }
    }

    [Fact]
    public void DeserializeTest() {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
