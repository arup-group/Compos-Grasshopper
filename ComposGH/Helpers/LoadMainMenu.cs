using System;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.Kernel;

namespace ComposGH.UI.Menu
{
  public class MenuLoad
  {
    internal static void OnStartup()
    {
      GH_DocumentEditor editor = null;

      while (editor == null)
      {
        editor = Grasshopper.Instances.DocumentEditor;
        Thread.Sleep(750);
      }
      Populate(editor.MainMenuStrip);
    }

    private static void Populate(MenuStrip mms)
    {
      var tl = "Compos";
      //Can not find anything
      var s = mms.Items.Find(tl, false);

      ToolStripMenuItem menu;
      if (s.Length == 0)
        menu = new ToolStripMenuItem(tl);
      else
        menu = s[0] as ToolStripMenuItem;
      mms.Items.Add(menu);
      PopulateSub(menu);
    }

    private static void PopulateSub(ToolStripMenuItem oasysMenu)
    {
      // add units
      oasysMenu.DropDown.Items.Add("Compos Units", Properties.Resources.Units, (s, a) =>
      {
        UI.UnitSettingsBox unitBox = new UI.UnitSettingsBox();
        unitBox.ShowDialog();
      });
      // add info
      oasysMenu.DropDown.Items.Add("Compos Info", Properties.Resources.ComposInfo, (s, a) =>
      {
        UI.AboutBox aboutBox = new UI.AboutBox();
        aboutBox.ShowDialog();
      });
    }
  }
}
