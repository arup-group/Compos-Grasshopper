using System;
using System.Drawing;
using System.IO;
using ComposAPI;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Helpers;

namespace ComposGH
{
  public class AddReferencePriority : GH_AssemblyPriority
  {
    public override GH_LoadingInstruction PriorityLoad()
    {
      if (!TryFindPluginPath("Compos.gha"))
        return GH_LoadingInstruction.Abort;

      // ### Set system environment variables to allow user rights to read above dll ###
      const string name = "PATH";
      string pathvar = Environment.GetEnvironmentVariable(name);
      var value = pathvar + ";" + InstallPath;
      var target = EnvironmentVariableTarget.Process;
      Environment.SetEnvironmentVariable(name, value, target);

      // ### Queue up Main menu loader ###
      Grasshopper.Instances.CanvasCreated += UI.Menu.MenuLoad.OnStartup;

      // ### Create Ribbon Category name and icon ###
      Grasshopper.Instances.ComponentServer.AddCategorySymbolName("Compos", 'C');
      Grasshopper.Instances.ComponentServer.AddCategoryIcon("Compos", Properties.Resources.ComposLogo128);

      // ### Setup OasysGH and shared Units ###
      Utility.InitialiseMainMenuAndDefaultUnits();

      // subscribe to rhino closing event
      Rhino.RhinoApp.Closing += CloseFile;

      PostHog.PluginLoaded(PluginInfo.Instance);
      
      return GH_LoadingInstruction.Proceed;
    }

    

    public static string PluginPath;
    public static string InstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oasys", "Compos 8.6");

    internal static void CloseFile(object sender, EventArgs args)
    {
      ComposFile.Close();
      Rhino.RhinoApp.Closing -= CloseFile;
    }

    private bool TryFindPluginPath(string keyword)
    {
      // ### Search for plugin path ###

      // initially look in %appdata% folder where package manager will store the plugin
      string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      path = Path.Combine(path, "McNeel", "Rhinoceros", "Packages", Rhino.RhinoApp.ExeVersion + ".0", ComposGHInfo.ProductName);

      if (!File.Exists(Path.Combine(path, keyword))) // if no plugin file is found there continue search
      {
        // search grasshopper libraries folder
        string sDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Grasshopper",
          "Libraries");

        string[] files = Directory.GetFiles(sDir, keyword, SearchOption.AllDirectories);
        if (files.Length > 0)
          path = files[0].Replace(keyword, string.Empty);

        if (!File.Exists(Path.Combine(path, keyword))) // if no plugin file is found there continue search
        {
          // look in all the other Grasshopper assembly (plugin) folders
          foreach (GH_AssemblyFolderInfo pluginFolder in Grasshopper.Folders.AssemblyFolders)
          {
            files = Directory.GetFiles(pluginFolder.Folder, keyword, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
              path = files[0].Replace(keyword, string.Empty);
              PluginPath = Path.GetDirectoryName(path);
              return true;
            }
          }
          string message =
            "Error loading the file " + keyword + " from any Grasshopper plugin folders - check if the file exist."
            + Environment.NewLine + "The plugin cannot be loaded."
            + Environment.NewLine + "Folders (including subfolder) that was searched:"
            + Environment.NewLine + sDir;
          foreach (GH_AssemblyFolderInfo pluginFolder in Grasshopper.Folders.AssemblyFolders)
            message += Environment.NewLine + pluginFolder.Folder;

          Exception exception = new Exception(message);
          GH_LoadingException gH_LoadingException = new GH_LoadingException(ComposGHInfo.ProductName + ": " + keyword + " loading failed", exception);
          Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
          PostHog.PluginLoaded(PluginInfo.Instance, message);
          return false;
        }
      }
      PluginPath = Path.GetDirectoryName(path);
      return true;
    }
  }
  internal sealed class PluginInfo
  {
    private static readonly Lazy<OasysPluginInfo> lazy =
        new Lazy<OasysPluginInfo>(() => new OasysPluginInfo(
          ComposGHInfo.ProductName,
          ComposGHInfo.PluginName,
          ComposGHInfo.Vers,
          ComposGHInfo.isBeta,
          "phc_alOp3OccDM3D18xJTWDoW44Y1cJvbEScm5LJSX8qnhs"
          ));

    public static OasysPluginInfo Instance { get { return lazy.Value; } }

    private PluginInfo()
    {
    }
  }

  public class ComposGHInfo : GH_AssemblyInfo
  {
    public static Guid GUID = new Guid("c3884cdc-ac5b-4151-afc2-93590cef4f8f");
    public const string Company = "Oasys";
    public const string Copyright = "Copyright © Oasys 1985 - 2022";
    public const string Contact = "https://www.oasys-software.com/";
    public const string Vers = "0.9.0";
    public static bool isBeta = true;
    public static string Disclaimer = PluginName + " is pre-release and under active development, including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using " + PluginName + " should not be relied upon without thorough and independent checking.";
    public const string ProductName = "Compos";
    public const string PluginName = "ComposGH";

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
