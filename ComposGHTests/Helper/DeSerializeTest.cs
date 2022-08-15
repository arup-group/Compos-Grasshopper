using ComposGH.Components;
using ComposGHTests.Helpers;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ComposGHTests.Helpers
{
  public class DeSerialize
  {
    public static void TestDeserialize(GH_Component comp)
    {
      comp.CreateAttributes();

      var doc = new GH_Document();
      doc.AddObject(comp, true);

      GH_DocumentIO serialize = new GH_DocumentIO();
      serialize.Document = doc;
      serialize.Document.Objects[0].Attributes.PerformLayout();

      string path = Path.Combine(Environment.CurrentDirectory, "GH-Test-Files");
      Directory.CreateDirectory(path);
      Type myType = comp.GetType();
      string pathFileName = Path.Combine(path, myType.Name) + ".gh";
      Assert.True(serialize.SaveQuiet(pathFileName));

      GH_DocumentIO deserialize = new GH_DocumentIO();
      Assert.True(deserialize.Open(pathFileName));
      Duplicates.AreEqual(serialize.Document.Objects[0], deserialize.Document.Objects[0], true);
    }
  }
}
