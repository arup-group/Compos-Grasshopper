using Grasshopper.Kernel;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace ComposGH.UI
{
  partial class AboutBox : Form
  {
    public AboutBox()
    {
      GH_AssemblyInfo composPlugin = Grasshopper.Instances.ComponentServer.FindAssembly(new Guid("c3884cdc-ac5b-4151-afc2-93590cef4f8f"));

      string api = "Compos 8.6 COM API"; // IVersion.Api();
      string pluginvers = composPlugin.Version;
      string pluginloc = composPlugin.Location;

      InitializeComponent();
      Text = String.Format("About {0}", ComposGHInfo.PluginName);
      labelProductName.Text = ComposGHInfo.ProductName + " Grasshopper plugin";
      labelVersion.Text = String.Format("Version {0}", pluginvers);
      labelApiVersion.Text = String.Format("API Version {0}", api);
      labelCompanyName.Text = AssemblyCompany;
      linkWebsite.Text = @"www.oasys-software.com";
      labelContact.Text = "Contact and support:";
      linkEmail.Text = @"oasys@arup.com";
      disclaimer.Text = ComposGHInfo.Disclaimer;
    }

    #region Assembly Attribute Accessors

    public string AssemblyTitle
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (attributes.Length > 0)
        {
          AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
          if (titleAttribute.Title != "")
          {
            return titleAttribute.Title;
          }
        }
        return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    public string AssemblyDescription
    {
      get
      {
        return "Compos is a unique composite beam analysis and design software program. In addition to composite beam design and analysis, " +
            "Compos can also perform footfall induced vibration analysis for regular composite floors using Resotec Damping System.";
      }
    }

    public string AssemblyCompany
    {
      get
      {
        return ComposGHInfo.Copyright;
      }
    }
    #endregion

    private void labelProductName_Click(object sender, EventArgs e)
    {

    }

    private void labelVersion_Click(object sender, EventArgs e)
    {

    }

    private void AboutBox_Load(object sender, EventArgs e)
    {

    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start(@"https://www.oasys-software.com/");
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      Close();
    }


    private void button1_Click(object sender, EventArgs e)
    {
      Process.Start(@"rhino://package/search?name=compos");
    }

    private void linkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      GH_AssemblyInfo gsaPlugin = Grasshopper.Instances.ComponentServer.FindAssembly(new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f"));
      string pluginvers = gsaPlugin.Version;
      Process.Start(@"mailto:oasys@arup.com?subject=Oasys " + ComposGHInfo.PluginName + " version " + pluginvers);
    }

    private void labelApiVersion_Click(object sender, EventArgs e)
    {

    }
  }
}
