﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.UI;

namespace ComposGH.Components {
  /// <summary>
  /// Component to save to a compos data file
  /// </summary>
  public class SaveComposFile : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("d2fa9fa1-9507-4f57-b383-8b573699906d");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.SaveModel;
    private bool CanOpen = false;
    private ComposFile ComposFile;
    private string FileName = null;

    public SaveComposFile() : base("SaveCompos", "Save", "Saves your Compos File from this parametric nightmare",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat0()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void CreateAttributes() {
      m_attributes = new ThreeButtonComponentAttributes(this, "Save", "Save As", "Open in Compos", SaveFile, SaveAsFile, OpenCompos, true, "Save Compos file");
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      FileName = (string)reader.GetString("File");
      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) { }

    public override void VariableParameterMaintenance() {
      Params.Input[0].Optional = FileName != null; //filename can have input from user input
      Params.Input[0].ClearRuntimeMessages(); // this needs to be called to avoid having a runtime warning message after changed to optional
    }

    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetString("File", (string)FileName);
      return base.Write(writer);
    }

    internal void OpenCompos() {
      string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
      string fileName = programFiles + @"\Oasys\Compos 8.6\Compos.exe";

      if (FileName == null || FileName == "") {
        FileName = Path.GetTempPath() + ComposFile.Guid + ".coa";
      }
      SaveFile();
      if (CanOpen) {
        System.Diagnostics.Process.Start(fileName, FileName);
      }
    }

    internal void SaveAsFile() {
      var fdi = new Rhino.UI.SaveFileDialog { Filter = "Compos File (*.coa)|*.coa|All files (*.*)|*.*" };
      bool res = fdi.ShowSaveDialog();
      if (res) // == DialogResult.OK)
      {
        FileName = fdi.FileName;

        SaveFile();

        // add panel input with string
        // delete existing inputs if any
        while (Params.Input[2].Sources.Count > 0) {
          Grasshopper.Instances.ActiveCanvas.Document.RemoveObject(Params.Input[2].Sources[0], false);
        }

        // instantiate new panel
        var panel = new Grasshopper.Kernel.Special.GH_Panel();
        panel.CreateAttributes();

        panel.Attributes.Pivot = new PointF((float)Attributes.DocObject.Attributes.Bounds.Left -
            panel.Attributes.Bounds.Width - 40, (float)Attributes.DocObject.Attributes.Bounds.Bottom - panel.Attributes.Bounds.Height);

        // populate value list with our own data
        panel.UserText = FileName;

        // Until now, the panel is a hypothetical object.
        // This command makes it 'real' and adds it to the canvas.
        Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

        // Connect the new slider to this component
        Params.Input[2].AddSource(panel);
        Params.OnParametersChanged();
        ExpireSolution(true);
      }
    }

    internal void SaveFile() {
      if (FileName == null) {
        Message = "Please provide filename and path";
        return;
      }
      CanOpen = false;
      int status = ComposFile.SaveAs(FileName);
      switch (status) {
        case 0:
          CanOpen = true;
          Message = "File saved";
          PostHog.ModelIO(PluginInfo, "saveCOA", (int)(new FileInfo(FileName).Length / 1024));
          return;

        case 1:
          Message = "No Compos file is open";
          return;

        case 2:
          Message = "Invalid file extension";
          return;

        case 3:
        default:
          Message = "Failed to save";
          return;
      }
    }

    protected override void InitialiseDropdowns() { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, MemberGoo.Description + "s to save.", GH_ParamAccess.list);
      pManager.AddBooleanParameter("Save?", "Save", "Input 'True' to save or use button", GH_ParamAccess.item, false);
      pManager.AddTextParameter("File and Path", "File", "Filename and path", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, "Saved " + MemberGoo.Description, GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      var list = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(0, list)) {
        if (list == null || list.Count < 1) { return; }
        if (list[0].Value is MemberGoo) {
          var members = new List<IMember>();
          foreach (GH_ObjectWrapper wrapper in list) {
            var goo = (MemberGoo)wrapper.Value;
            var member = (IMember)goo.Value;
            members.Add(member);
            Message = "";
          }
          ComposFile = new ComposFile(members);
        } else {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to Compos File");
          return;
        }

        FileName = null;

        string tempfile = "";
        if (DA.GetData(2, ref tempfile)) {
          FileName = tempfile;
        }

        bool save = false;
        if (DA.GetData(1, ref save)) {
          if (save) {
            SaveFile();
            Message = FileName;
          }
        }

        var savedMembers = new List<MemberGoo>();
        foreach (IMember mem in ComposFile.GetMembers()) {
          savedMembers.Add(new MemberGoo(mem));
        }
        DA.SetDataList(0, savedMembers);
      }
    }
  }
}
