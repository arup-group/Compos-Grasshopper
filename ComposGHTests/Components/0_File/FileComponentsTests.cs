using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;
using ComposGHTests.Member;
using System.Collections.Generic;
using System.IO;
using System;
using System.ComponentModel;
using Grasshopper.Kernel;

namespace ComposGHTests.CompFile
{
  [Collection("GrasshopperFixture collection")]
  public class FileComponentsTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new OpenComposFile();
      comp.CreateAttributes();
      
      string solutiondir = Directory.GetParent(
        Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      
      string input1 = Path.Combine(solutiondir, "ComposTests", "TestFiles", "Compos2.coa");

      ComponentTestHelper.SetInput(comp, input1, 0);

      return comp;
    }

    [Fact]
    public void CreateOpenComponent()
    {
      var comp = ComponentMother();

      MemberGoo output = (MemberGoo)ComponentTestHelper.GetOutput(comp);

      Assert.StartsWith("CAT BSI-HE HE180AA", output.Value.Beam.Sections[0].SectionDescription);
    }

    [Fact]
    public void CreateSaveComponent()
    {
      SaveComposFile comp = new SaveComposFile();
      comp.CreateAttributes();

      MemberGoo input1 = (MemberGoo)ComponentTestHelper.GetOutput(ComponentMother());

      ComponentTestHelper.SetInput(comp, input1, 0);

      ComponentTestHelper.SetInput(comp, true, 1);

      string path = Path.Combine(Environment.CurrentDirectory, "GH-Test-Files");
      Directory.CreateDirectory(path);
      Type myType = comp.GetType();
      string input2 = Path.Combine(path, myType.Name) + "_Compos2" + ".coa";

      ComponentTestHelper.SetInput(comp, input2, 2);

      var doc = new GH_Document();
      doc.AddObject(comp, true);
      doc.NewSolution(true);

      Assert.True(File.Exists(input2));
    }
  }
}
