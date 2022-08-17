using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet.GH;
using Grasshopper.Kernel.Types;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class BeamSectionPropertiesTests
  {
    [Theory]
    [InlineData("CAT IPE IPE100", 100, 55, 55, 4.1, 5.7, 5.7, 7)]
    [InlineData("STD GI 400 300 250 12 25 20", 400, 300, 250, 12, 25, 20, 0)]
    [InlineData("STD I(cm) 20. 19. 8.5 1.27", 200, 190, 190, 85, 12.7, 12.7, 0)]
    public void CreateComponentWithInputsTest(string profile, double exp_depth, double exp_topwidth, double exp_botwidth, double exp_webthk, double exp_topflngthk, double exp_botflngthk, double exp_rootrad)
    {
      var comp = new BeamSectionProperties();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, profile, 0);

      GH_UnitNumber depth = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal(exp_depth, depth.Value.As(UnitsNet.Units.LengthUnit.Millimeter), 5);

      GH_UnitNumber topwidth = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 1);
      Assert.Equal(exp_topwidth, topwidth.Value.As(UnitsNet.Units.LengthUnit.Millimeter), 5);

      GH_UnitNumber botwidth = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 2);
      Assert.Equal(exp_botwidth, botwidth.Value.As(UnitsNet.Units.LengthUnit.Millimeter), 5);

      GH_UnitNumber webthk = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 3);
      Assert.Equal(exp_webthk, webthk.Value.As(UnitsNet.Units.LengthUnit.Millimeter), 5);

      GH_UnitNumber topflngthk = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 4);
      Assert.Equal(exp_topflngthk, topflngthk.Value.As(UnitsNet.Units.LengthUnit.Millimeter), 5);

      GH_UnitNumber botflngthk = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 5);
      Assert.Equal(exp_botflngthk, botflngthk.Value.As(UnitsNet.Units.LengthUnit.Millimeter), 5);

      GH_UnitNumber rootrad = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 6);
      Assert.Equal(exp_rootrad, rootrad.Value.As(UnitsNet.Units.LengthUnit.Millimeter), 5);

      GH_Boolean cat = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 7);
      bool isCatalogue = profile.StartsWith("CAT");
      Assert.Equal(isCatalogue, cat.Value);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = new BeamSectionProperties();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, "CAT IPE IPE200", 0);
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = new BeamSectionProperties();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, "CAT IPE IPE200", 0);
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
