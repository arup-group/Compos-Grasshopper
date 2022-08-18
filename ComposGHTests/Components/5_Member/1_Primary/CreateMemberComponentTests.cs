using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;
using ComposGHTests.Beam;
using ComposGHTests.Stud;
using ComposGHTests.Slab;
using ComposGHTests.Load;

namespace ComposGHTests.Member
{
  [Collection("GrasshopperFixture collection")]
  public class CreateMemberComponentTests
  {
    public static GH_OasysComponent CreateMemberMother()
    {
      var comp = new CreateMember();
      comp.CreateAttributes();



      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs()
    {
      var comp = CreateMemberMother();

      DesignCodeGoo output = (DesignCodeGoo)ComponentTestHelper.GetOutput(comp);
      EN1994 code = (EN1994)output.Value;
      Duplicates.AreEqual(new SafetyFactorsEN(), code.SafetyFactors);
      Duplicates.AreEqual(new DesignOption(), code.DesignOption);
      Duplicates.AreEqual(new CodeOptionsEN(), code.CodeOptions);
      Assert.Equal(Code.EN1994_1_1_2004, code.Code);
      Assert.Equal(NationalAnnex.Generic, code.NationalAnnex);

      output = (DesignCodeGoo)ComponentTestHelper.GetOutput(comp);
      code = (EN1994)output.Value;
      Assert.Equal(NationalAnnex.United_Kingdom, code.NationalAnnex);
    }
  }
}
