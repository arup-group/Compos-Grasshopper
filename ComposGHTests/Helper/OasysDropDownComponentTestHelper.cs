using ComposGH.Components;
using Grasshopper.Kernel;
using System;
using System.IO;
using Xunit;

namespace ComposGHTests.Helpers
{
  public class OasysDropDownComponentTestHelper
  {
    public static void TestDeserialize(GH_OasysDropDownComponent comp)
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

    public static void ChangeDropDownTest(GH_OasysDropDownComponent comp)
    {
      Assert.True(comp.IsInitialised);
      Assert.Equal(comp.DropDownItems.Count, comp.SpacerDescriptions.Count);
      Assert.Equal(comp.DropDownItems.Count, comp.SelectedItems.Count);

      for (int i = 0; i < comp.DropDownItems.Count; i++)
      {
        comp.SetSelected(0, i);

        for (int j = 0; j < comp.DropDownItems[i].Count; j++)
        {
          comp.SetSelected(i, j);
          Assert.Equal(comp.SelectedItems[i], comp.DropDownItems[i][j]);
        }
      }
    }
  }
}
