using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;
using ComposGHTests.Member;
using System.Collections.Generic;
using System.IO;
using System;
using OasysGH.Components;

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

      Assert.StartsWith("CAT HE HE220.A", output.Value.Beam.Sections[0].SectionDescription);

      MemberGoo input1 = (MemberGoo)ComponentTestHelper.GetOutput(CreateMemberComponentTests.CreateMemberMother());
      ComposFile composFile = new ComposFile(new List<IMember>() { input1.Value, output.Value });

      string path = Path.Combine(Environment.CurrentDirectory, "GH-Test-Files");
      Directory.CreateDirectory(path);
      Type myType = comp.GetType();
      string pathFileName = Path.Combine(path, myType.Name);
      Assert.Equal(0, composFile.SaveAs(pathFileName));
    }
  }
}
