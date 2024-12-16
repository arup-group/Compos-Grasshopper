using System;
using System.Collections.Generic;
using System.IO;
using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using ComposGHTests.Member;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Design {
  [Collection("GrasshopperFixture collection")]
  public class DesignMemberComponentTests {

    public static GH_OasysComponent DesignMemberMother() {
      GH_OasysComponent comp = new DesignMember();
      comp.CreateAttributes();

      var input1 = (MemberGoo)ComponentTestHelper.GetOutput(CreateMemberComponentTests.CreateMemberMother());

      var input2 = (DesignCriteriaGoo)ComponentTestHelper.GetOutput(CreateDesignCriteriaComponentTests.CreateDesignCriteriaMother());

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input2, 1);

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysComponent comp = DesignMemberMother();

      var output = (MemberGoo)ComponentTestHelper.GetOutput(comp);

      Assert.StartsWith("CAT HE HE220.A", output.Value.Beam.Sections[0].SectionDescription);

      var input1 = (MemberGoo)ComponentTestHelper.GetOutput(CreateMemberComponentTests.CreateMemberMother());
      var composFile = new ComposFile(new List<IMember>() { input1.Value, output.Value });

      string path = Path.Combine(Environment.CurrentDirectory, "GH-Test-Files");
      Directory.CreateDirectory(path);
      Type myType = comp.GetType();
      string pathFileName = Path.Combine(path, myType.Name);
      Assert.Equal(0, composFile.SaveAs(pathFileName));
    }
  }
}
