using System;
using ComposGHTests.Helpers;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;

namespace IntegrationTests
{
  [Collection("GrasshopperFixture collection")]
  public class Example_BS_01Test
  {
    public static GH_Document Document()
    {
      string fileName = "ComposGH_" + MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.", string.Empty).Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");
      GH_DocumentIO io = new GH_DocumentIO();
      Assert.True(File.Exists(Path.Combine(path, fileName)));
      Assert.True(io.Open(Path.Combine(path, fileName)));
      return io.Document;
    }

    [Fact]
    public void CodeSatisfiedTest01()
    {
      GH_Document doc = Document();
      foreach (var obj in (doc.Objects))
        if (obj is GH_Group group)
        {
          if (group.NickName == "TEST01")
          {
            Guid componentguid = group.ObjectIDs[0];

            foreach (var obj2 in (doc.Objects))
            {
              if (obj2.InstanceGuid == componentguid)
              {
                GH_Component comp = (GH_Component)obj2;

                GH_String output = (GH_String)ComponentTestHelper.GetOutput(comp);

                Assert.Equal("One or more code requirements are not met", output.Value);
                return;
              }
            }
          }
        }
      Assert.True(false, "Did not find output");
    }

    [Fact]
    public void CodeSatisfiedTest02()
    {
      GH_Document doc = Document();
      foreach (var obj in (doc.Objects))
        if (obj is GH_Group group)
        {
          if (group.NickName == "TEST02")
          {
            Guid componentguid = group.ObjectIDs[0];

            foreach (var obj2 in (doc.Objects))
            {
              if (obj2.InstanceGuid == componentguid)
              {
                GH_Component comp = (GH_Component)obj2;

                GH_String output = (GH_String)ComponentTestHelper.GetOutput(comp);

                Assert.Equal("All code requirements are met", output.Value);
                return;
              }
            }
          }
        }
      Assert.True(false, "Did not find output");
    }
  }
}
