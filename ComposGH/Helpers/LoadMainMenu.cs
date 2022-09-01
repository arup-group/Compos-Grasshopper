using System;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

namespace ComposGH.UI.Menu
{
  public class MenuLoad
  {
    private static ToolStripMenuItem oasysMenu;
    internal static void OnStartup(GH_Canvas canvas)
    {
      oasysMenu = new ToolStripMenuItem("Compos");

      PopulateSub(oasysMenu);

      GH_DocumentEditor editor = null;

      while (editor == null)
      {
        editor = Grasshopper.Instances.DocumentEditor;
        Thread.Sleep(750);
      }
      editor.MainMenuStrip.Items.Add(oasysMenu);

      Grasshopper.Instances.CanvasCreated -= OnStartup;
    }

    private static void PopulateSub(ToolStripMenuItem menutItem)
    {
      // add units
      menutItem.DropDown.Items.Add("Compos Units", Properties.Resources.Units, (s, a) =>
      {
        UnitSettingsBox unitBox = new UnitSettingsBox();
        unitBox.ShowDialog();
      });
      // add info
      menutItem.DropDown.Items.Add("Compos Info", Properties.Resources.ComposInfo, (s, a) =>
      {
        AboutBox aboutBox = new AboutBox();
        aboutBox.ShowDialog();
      });
    }
  }
}
