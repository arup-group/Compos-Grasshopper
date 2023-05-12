using System;
using System.Drawing;
using System.IO;
using ComposAPI;
using ComposGH.UI;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Helpers;

namespace ComposGH {
  public class AddReferencePriority : GH_AssemblyPriority {
    public static string InstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oasys", "Compos 8.6");

    public static string PluginPath;

    public override GH_LoadingInstruction PriorityLoad() {
      if (!TryFindPluginPath("Compos.gha")) {
        return GH_LoadingInstruction.Abort;
      }

      // ### Set system environment variables to allow user rights to read below dlls ###
      const string name = "PATH";
      string pathvar = System.Environment.GetEnvironmentVariable(name);
      string value = InstallPath + ";" + pathvar;
      EnvironmentVariableTarget target = EnvironmentVariableTarget.Process;
      System.Environment.SetEnvironmentVariable(name, value, target);

      // ### Queue up Main menu loader ###
      Grasshopper.Instances.CanvasCreated += MenuLoad.OnStartup;

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

    internal static void CloseFile(object sender, EventArgs args) {
      ComposFile.Close();
      Rhino.RhinoApp.Closing -= CloseFile;
    }

    private bool TryFindPluginPath(string keyword) {
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
        if (files.Length > 0) {
          path = files[0].Replace(keyword, string.Empty);
        }

        if (!File.Exists(Path.Combine(path, keyword))) // if no plugin file is found there continue search
        {
          // look in all the other Grasshopper assembly (plugin) folders
          foreach (GH_AssemblyFolderInfo pluginFolder in Grasshopper.Folders.AssemblyFolders) {
            files = Directory.GetFiles(pluginFolder.Folder, keyword, SearchOption.AllDirectories);
            if (files.Length > 0) {
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
          foreach (GH_AssemblyFolderInfo pluginFolder in Grasshopper.Folders.AssemblyFolders) {
            message += Environment.NewLine + pluginFolder.Folder;
          }

          var exception = new Exception(message);
          var gH_LoadingException = new GH_LoadingException(ComposGHInfo.ProductName + ": " + keyword + " loading failed", exception);
          Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
          PostHog.PluginLoaded(PluginInfo.Instance, message);
          return false;
        }
      }
      PluginPath = Path.GetDirectoryName(path);
      return true;
    }
  }

  public class ComposGHInfo : GH_AssemblyInfo {
    public override Bitmap AssemblyIcon => Icon;
    public override string AuthorContact => Contact;
    public override string AuthorName => Company;
    public override string Description =>
        "Official Oasys Compos Grasshopper Plugin" + Environment.NewLine
          + (isBeta ? Disclaimer : "")
        + Environment.NewLine + "The plugin requires a licensed version of Compos to load."
        + Environment.NewLine
        + Environment.NewLine + "Contact oasys@arup.com to request a free trial version."
        + Environment.NewLine + Environment.NewLine + Copyright;
    // Return a 24x24 pixel bitmap to represent this GHA library.
    public override Bitmap Icon => null;
    public override Guid Id => GUID;
    public override string Name => ProductName;
    public override string Version {
      get {
        if (isBeta) {
          return Vers + "-beta";
        } else {
          return Vers;
        }
      }
    }
    public const string Company = "Oasys";
    public const string Contact = "https://www.oasys-software.com/";
    public const string Copyright = "Copyright © Oasys 1985 - 2022";
    public const string PluginName = "ComposGH";
    public const string ProductName = "Compos";
    public const string Vers = "0.9.10";
    public static string Disclaimer = PluginName + " is pre-release and under active development, including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using " + PluginName + " should not be relied upon without thorough and independent checking.";
    public static Guid GUID = new Guid("c3884cdc-ac5b-4151-afc2-93590cef4f8f");
    public static bool isBeta = true;
  }

  internal sealed class PluginInfo {
    public static OasysPluginInfo Instance => lazy.Value;
    private static readonly Lazy<OasysPluginInfo> lazy =
            new Lazy<OasysPluginInfo>(() => new OasysPluginInfo(
      ComposGHInfo.ProductName,
      ComposGHInfo.PluginName,
      ComposGHInfo.Vers,
      ComposGHInfo.isBeta,
      "phc_alOp3OccDM3D18xJTWDoW44Y1cJvbEScm5LJSX8qnhs"
      ));

    private PluginInfo() {
    }
  }
}
