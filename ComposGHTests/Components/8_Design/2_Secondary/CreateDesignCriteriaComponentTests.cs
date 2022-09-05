using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;
using OasysGH.Components;

namespace ComposGHTests.Design
{
  [Collection("GrasshopperFixture collection")]
  public class CreateDesignCriteriaComponentTests
  {
    public static GH_OasysDropDownComponent CreateDesignCriteriaMother()
    {
      var comp = new CreateDesignCriteria();
      comp.CreateAttributes();

      BeamSizeLimitsGoo input1 = (BeamSizeLimitsGoo)ComponentTestHelper.GetOutput(CreateBeamSizeLimitsComponentTests.CreateBeamSizeLimitsMother());

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, 27, 1);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs1()
    {
      var comp = CreateDesignCriteriaMother();

      DesignCriteriaGoo output = (DesignCriteriaGoo)ComponentTestHelper.GetOutput(comp);

      BeamSizeLimitsGoo expected_input1 = (BeamSizeLimitsGoo)ComponentTestHelper.GetOutput(CreateBeamSizeLimitsComponentTests.CreateBeamSizeLimitsMother());

      Duplicates.AreEqual(expected_input1.Value, output.Value.BeamSizeLimits);
      Assert.Equal(27, output.Value.CatalogueSectionTypes[0]);
      Assert.Equal(OptimiseOption.MinimumWeight, output.Value.OptimiseOption);
    }

    [Fact]
    public void CreateComponentWithInputs2()
    {
      var comp = CreateDesignCriteriaMother();

      comp.SetSelected(0, 1); // change dropdown to min height

      ComponentTestHelper.SetInput(comp, 26, 1);

      DeflectionLimitGoo expectedInput2 = (DeflectionLimitGoo)ComponentTestHelper.GetOutput(CreateDeflectionLimitComponentTests.CreateDeflectionLimitMother());
      ComponentTestHelper.SetInput(comp, expectedInput2, 2);
      ComponentTestHelper.SetInput(comp, expectedInput2, 3);
      ComponentTestHelper.SetInput(comp, expectedInput2, 4);
      ComponentTestHelper.SetInput(comp, expectedInput2, 5);
      ComponentTestHelper.SetInput(comp, expectedInput2, 6);

      FrequencyLimitsGoo expectedInput6 = (FrequencyLimitsGoo)ComponentTestHelper.GetOutput(CreateFrequencyLimitsComponentTests.CreateFrequencyLimitsMother());
      ComponentTestHelper.SetInput(comp, expectedInput6, 7);

      DesignCriteriaGoo output = (DesignCriteriaGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(OptimiseOption.MinimumHeight, output.Value.OptimiseOption);
      Assert.Equal(26, output.Value.CatalogueSectionTypes[1]);

      Duplicates.AreEqual(expectedInput2.Value, output.Value.ConstructionDeadLoad);
      Duplicates.AreEqual(expectedInput2.Value, output.Value.AdditionalDeadLoad);
      Duplicates.AreEqual(expectedInput2.Value, output.Value.FinalLiveLoad);
      Duplicates.AreEqual(expectedInput2.Value, output.Value.TotalLoads);
      Duplicates.AreEqual(expectedInput2.Value, output.Value.PostConstruction);

      Duplicates.AreEqual(expectedInput6.Value, output.Value.FrequencyLimits);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateDesignCriteriaMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateDesignCriteriaMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
