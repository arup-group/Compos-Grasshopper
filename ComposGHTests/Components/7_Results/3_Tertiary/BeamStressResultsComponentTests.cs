using Xunit;
using ComposGH.Parameters;
using ComposGH.Components;
using ComposGHTests.Helpers;
using UnitsNet.GH;
using OasysGH.Components;

namespace ComposGHTests.Result
{
  [Collection("GrasshopperFixture collection")]
  public class BeamStressResultsComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new BeamStressResults();
      comp.CreateAttributes();
      MemberGoo input = (MemberGoo)ComponentTestHelper.GetOutput(CompFile.FileComponentsTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input);
      
      return comp;
    }

    [Fact]
    public void CreateComponentWithInput()
    {
      var comp = ComponentMother();
      comp.ExpireSolution(true);

      int expectedNumberOfResults = 7;

      for (int i = 0; i < comp.Params.Output.Count; i++)
      {
        comp.Params.Output[i].CollectData();
        for (int j = 0; j < expectedNumberOfResults; j++)
        {
          GH_UnitNumber output = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i, 0, j);
          Assert.NotNull(output);
        }
      }
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
