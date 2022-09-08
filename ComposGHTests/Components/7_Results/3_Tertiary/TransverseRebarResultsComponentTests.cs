using Xunit;
using ComposGH.Parameters;
using ComposGH.Components;
using ComposGHTests.Helpers;
using UnitsNet.GH;
using Grasshopper.Kernel.Types;
using OasysGH.Components;

namespace ComposGHTests.Result
{
  [Collection("GrasshopperFixture collection")]
  public class TransverseRebarResultsComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new TransverseRebarResults();
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

      int expectedNumberOfResults = 1;

      for (int i = 0; i < comp.Params.Output.Count; i++)
      {
        if (i == 1) // natural freq is single item results
        {
          GH_String output = (GH_String)ComponentTestHelper.GetOutput(comp, i);
          Assert.NotNull(output);
          Assert.True(output.Value.Length > 0);
        }
        else
        {
          comp.Params.Output[i].CollectData();
          for (int j = 0; j < expectedNumberOfResults; j++)
          {
            GH_UnitNumber output = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i, 0, j);
            Assert.NotNull(output);
          }
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
