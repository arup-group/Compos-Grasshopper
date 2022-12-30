using System.Threading;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

namespace ComposGH.UI
{
    public class MenuLoad
    {
        private static ToolStripMenuItem oasysMenu;
        internal static void OnStartup(GH_Canvas canvas)
        {
            oasysMenu = new ToolStripMenuItem("Oasys");
            oasysMenu.Name = "Oasys";

            PopulateSub(oasysMenu);

            GH_DocumentEditor editor = null;

            while (editor == null)
            {
                editor = Grasshopper.Instances.DocumentEditor;
                Thread.Sleep(750);
            }

            if (!editor.MainMenuStrip.Items.ContainsKey("Oasys"))
                editor.MainMenuStrip.Items.Add(oasysMenu);
            else
            {
                oasysMenu = (ToolStripMenuItem)editor.MainMenuStrip.Items["Oasys"];
                lock (oasysMenu)
                {
                    oasysMenu.DropDown.Items.Add(new ToolStripSeparator());
                    PopulateSub(oasysMenu);
                }
            }

            Grasshopper.Instances.CanvasCreated -= OnStartup;
        }

        private static void PopulateSub(ToolStripMenuItem menutItem)
        {
            menutItem.DropDown.Items.Add("Compos Info", Properties.Resources.ComposInfo, (s, a) =>
            {
                AboutBox aboutBox = new AboutBox();
                aboutBox.ShowDialog();
            });
        }
    }
}
