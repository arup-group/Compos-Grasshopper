using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.IO;
using ComposAPI;
using System.Threading.Tasks;

namespace ComposGH
{
  public class AddReferencePriority : GH_AssemblyPriority
  {
    public override GH_LoadingInstruction PriorityLoad()
    {
      // ### Search for plugin path ###

      // initially look in %appdata% folder where package manager will store the plugin
      string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      path = Path.Combine(path, "McNeel", "Rhinoceros", "Packages", Rhino.RhinoApp.ExeVersion + ".0", "Compos");

      if (!File.Exists(Path.Combine(path, "Compos.gha"))) // if no plugin file is found there continue search
      {
        // look in all the other Grasshopper assembly (plugin) folders
        foreach (GH_AssemblyFolderInfo pluginFolder in Grasshopper.Folders.AssemblyFolders)
        {
          if (File.Exists(Path.Combine(pluginFolder.Folder, "Compos.gha"))) // if the folder contains the plugin
          {
            path = pluginFolder.Folder;
            break;
          }
        }
      }
      PluginPath = Path.GetDirectoryName(path);

      // ### Set system environment variables to allow user rights to read above dll ###
      const string name = "PATH";
      string pathvar = Environment.GetEnvironmentVariable(name);
      var value = pathvar + ";" + InstallPath;
      var target = EnvironmentVariableTarget.Process;
      Environment.SetEnvironmentVariable(name, value, target);

      // ### use the API and trigger a license check if possible


      // ### Create Ribbon Category name and icon ###
      Grasshopper.Instances.ComponentServer.AddCategorySymbolName("Compos", 'C');
      Grasshopper.Instances.ComponentServer.AddCategoryIcon("Compos", Properties.Resources.ComposLogo128);

      // ### Queue up Main menu loader ###
      Task task = new Task(UI.Menu.MenuLoad.OnStartup);
      task.Start();
      //Helpers.Loader menuLoad = new Helpers.Loader();
      //menuLoad.CreateMainMenuItem();

      // ### Setup units ###
      Units.SetupUnits();

      // subscribe to rhino closing event
      Rhino.RhinoApp.Closing += CloseFile;

      return GH_LoadingInstruction.Proceed;
    }

    public static string PluginPath;
    public static string InstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oasys", "Compos 8.6");

    internal static void CloseFile(object sender, EventArgs args)
    {
      ComposFile.Close();
      Rhino.RhinoApp.Closing -= CloseFile;
    }
  }

  public class ComposGHInfo : GH_AssemblyInfo
  {
    internal static Guid GUID = new Guid("c3884cdc-ac5b-4151-afc2-93590cef4f8f");
    internal const string Company = "Oasys";
    internal const string Copyright = "Copyright © Oasys 1985 - 2022";
    internal const string Contact = "https://www.oasys-software.com/";
    internal const string Vers = "0.0.3";
    internal static bool isBeta = true;
    internal static string Disclaimer = PluginName + " is pre-release and under active development, including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using " + PluginName + " should not be relied upon without thorough and independent checking.";
    internal const string ProductName = "Compos";
    internal const string PluginName = "ComposGH";

    public override string Name
    {
      get
      {
        return ProductName;
      }
    }

    public override Bitmap Icon
    {
      get
      {
        //Return a 24x24 pixel bitmap to represent this GHA library.
        return null;
      }
    }

    public override Bitmap AssemblyIcon
    {
      get
      {
        return Icon;
      }
    }

    public override string Description
    {
      get
      {
        //Return a short string describing the purpose of this GHA library.
        return "Official Oasys Compos Grasshopper Plugin" + Environment.NewLine
          + (isBeta ? Disclaimer : "")
        + Environment.NewLine + "The plugin requires a licensed version of Compos to load."
        + Environment.NewLine
        + Environment.NewLine + "Contact oasys@arup.com to request a free trial version."
        + Environment.NewLine + Environment.NewLine + Copyright;
      }
    }

    public override Guid Id
    {
      get
      {
        return GUID;
      }
    }

    public override string AuthorName
    {
      get
      {
        //Return a string identifying you or your company.
        return Company;
      }
    }
    public override string AuthorContact
    {
      get
      {
        //Return a string representing your preferred contact details.
        return Contact;
      }
    }
    public override string Version
    {
      get
      {
        if (isBeta)
          return Vers + "-beta";
        else
          return Vers;
      }
    }
  }
}
