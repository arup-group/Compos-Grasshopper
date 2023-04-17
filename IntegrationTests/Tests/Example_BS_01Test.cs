using ComposGHTests.Helpers;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System.IO;
using System.Reflection;
using Xunit;

namespace IntegrationTests {
  [Collection("GrasshopperFixture collection")]
  public class Example_BS_01Test {

    public static GH_Document Document() {
      string fileName = "ComposGH_" + MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.", string.Empty).Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");
      GH_DocumentIO io = new GH_DocumentIO();
      Assert.True(File.Exists(Path.Combine(path, fileName)));
      Assert.True(io.Open(Path.Combine(path, fileName)));
      io.Document.NewSolution(true);
      return io.Document;
    }

    [Fact]
    public void CodeSatisfiedTest01() {
      GH_Document doc = Document();
      GH_Component comp = Helper.FindComponentInDocumentByGroup(doc, "CodeCheckFails");
      Assert.NotNull(comp);
      GH_String output = (GH_String)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("One or more code requirements are not met", output.Value);
    }

    [Fact]
    public void CodeSatisfiedTest02() {
      GH_Document doc = Document();
      GH_Component comp = Helper.FindComponentInDocumentByGroup(doc, "CodeCheckSucceeded");
      Assert.NotNull(comp);
      GH_String output = (GH_String)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("All code requirements are met", output.Value);
    }

    [Fact]
    public void NoRuntimeErrorsTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
