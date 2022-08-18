using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;
using ComposGHTests.Member;

namespace ComposGHTests.Design
{
  [Collection("GrasshopperFixture collection")]
  public class DesignMemberComponentTests
  {
    public static GH_OasysComponent DesignMemberMother()
    {
      var comp = new DesignMember();
      comp.CreateAttributes();

      MemberGoo input1 = (MemberGoo)ComponentTestHelper.GetOutput(CreateMemberComponentTests.CreateMemberMother());

      DesignCriteriaGoo input2 = (DesignCriteriaGoo)ComponentTestHelper.GetOutput(CreateDesignCriteriaComponentTests.CreateDesignCriteriaMother());

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input2, 1);

      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = DesignMemberMother();

      MemberGoo output = (MemberGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
      //"Failed to design member"
    }
  }
}
