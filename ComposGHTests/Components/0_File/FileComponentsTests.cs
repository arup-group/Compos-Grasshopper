using System;
using System.IO;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using Grasshopper.Kernel;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.CompFile {
  [Collection("GrasshopperFixture collection")]
  public class FileComponentsTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new OpenComposFile();
      comp.CreateAttributes();

      string solutiondir = Directory.GetParent(
        Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;

      string input1 = Path.Combine(solutiondir, "ComposTests", "TestFiles", "Compos2.coa");

      ComponentTestHelper.SetInput(comp, input1, 0);

      return comp;
    }

    [Fact]
    public void CreateOpenComponent() {
      GH_OasysComponent comp = ComponentMother();

      var output = (MemberGoo)ComponentTestHelper.GetOutput(comp);

      Assert.StartsWith("CAT BSI-HE HE180AA", output.Value.Beam.Sections[0].SectionDescription);
    }

    [Fact]
    public void CreateSaveComponent() {
      var comp = new SaveComposFile();
      comp.CreateAttributes();

      var input1 = (MemberGoo)ComponentTestHelper.GetOutput(ComponentMother());

      ComponentTestHelper.SetInput(comp, input1, 0);

      ComponentTestHelper.SetInput(comp, true, 1);

      string path = Path.Combine(Environment.CurrentDirectory, "GH-Test-Files");
      Directory.CreateDirectory(path);
      Type myType = comp.GetType();
      string input2 = Path.Combine(path, myType.Name) + "_Compos2" + ".coa";

      ComponentTestHelper.SetInput(comp, input2, 2);

      var doc = new GH_Document();
      doc.AddObject(comp, true);
      comp.Params.Output[0].CollectData();

      Assert.True(File.Exists(input2));
    }
  }
}
