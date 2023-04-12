using System;
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

namespace ComposGH.Components
{
  /// <summary>
  /// Component to save to a compos data file
  /// </summary>
  public class SaveComposFile : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("d2fa9fa1-9507-4f57-b383-8b573699906d");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    string FileName = null;
    ComposFile ComposFile;
    bool CanOpen = false;
    protected override Bitmap Icon => Resources.SaveModel;

    public SaveComposFile()
      : base("SaveCompos", "Save", "Saves your Compos File from this parametric nightmare",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, MemberGoo.Description + "s to save.", GH_ParamAccess.list);
      pManager.AddBooleanParameter("Save?", "Save", "Input 'True' to save or use button", GH_ParamAccess.item, false);
      pManager.AddTextParameter("File and Path", "File", "Filename and path", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, "Saved " + MemberGoo.Description, GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      List<GH_ObjectWrapper> list = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(0, list))
      {
        if (list == null || list.Count < 1) { return; }
        if (list[0].Value is MemberGoo)
        {
          List<IMember> members = new List<IMember>();
          foreach (GH_ObjectWrapper wrapper in list)
          {
            MemberGoo goo = (MemberGoo)wrapper.Value;
            IMember member = (IMember)goo.Value;
            members.Add(member);
            Message = "";
          }
          this.ComposFile = new ComposFile(members);
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to Compos File");
          return;
        }

        this.FileName = null;

        string tempfile = "";
        if (DA.GetData(2, ref tempfile))
          this.FileName = tempfile;

        bool save = false;
        if (DA.GetData(1, ref save))
        {
          if (save)
          {
            SaveFile();
            this.Message = this.FileName;
          }
        }

        List<MemberGoo> savedMembers = new List<MemberGoo>();
        foreach (IMember mem in this.ComposFile.GetMembers())
          savedMembers.Add(new MemberGoo(mem));
        DA.SetDataList(0, savedMembers);
      }
    }

    #region Custom UI
    public override void SetSelected(int i, int j) { }

    protected override void InitialiseDropdowns() { }

    public override void CreateAttributes()
    {
      m_attributes = new OasysGH.UI.ThreeButtonAtrributes(this, "Save", "Save As", "Open in Compos", SaveFile, SaveAsFile, OpenCompos, true, "Save Compos file");
    }

    internal void SaveFile()
    {
      if (this.FileName == null)
      {
        this.Message = "Please provide filename and path";
        return;
      }
      this.CanOpen = false;
      int status = this.ComposFile.SaveAs(this.FileName);
      switch (status)
      {
        case 0:
          this.CanOpen = true;
          this.Message = "File saved";
          PostHog.ModelIO(PluginInfo, "saveCOA", (int)(new FileInfo(this.FileName).Length / 1024));
          return;
        case 1:
          this.Message = "No Compos file is open";
          return;
        case 2:
          this.Message = "Invalid file extension";
          return;
        case 3:
        default:
          this.Message = "Failed to save";
          return;
      }
    }

    internal void SaveAsFile()
    {
      var fdi = new Rhino.UI.SaveFileDialog { Filter = "Compos File (*.coa)|*.coa|All files (*.*)|*.*" };
      var res = fdi.ShowSaveDialog();
      if (res) // == DialogResult.OK)
      {
        this.FileName = fdi.FileName;

        SaveFile();

        // add panel input with string
        // delete existing inputs if any
        while (Params.Input[2].Sources.Count > 0)
          Grasshopper.Instances.ActiveCanvas.Document.RemoveObject(Params.Input[2].Sources[0], false);

        // instantiate new panel
        var panel = new Grasshopper.Kernel.Special.GH_Panel();
        panel.CreateAttributes();

        panel.Attributes.Pivot = new PointF((float)Attributes.DocObject.Attributes.Bounds.Left -
            panel.Attributes.Bounds.Width - 40, (float)Attributes.DocObject.Attributes.Bounds.Bottom - panel.Attributes.Bounds.Height);

        // populate value list with our own data
        panel.UserText = this.FileName;

        // Until now, the panel is a hypothetical object.
        // This command makes it 'real' and adds it to the canvas.
        Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

        // Connect the new slider to this component
        Params.Input[2].AddSource(panel);
        Params.OnParametersChanged();
        ExpireSolution(true);
      }
    }

    internal void OpenCompos()
    {
      string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
      string fileName = programFiles + @"\Oasys\Compos 8.6\Compos.exe";

      if (this.FileName == null || this.FileName == "")
        this.FileName = Path.GetTempPath() + this.ComposFile.Guid + ".coa";
      this.SaveFile();
      if (this.CanOpen)
        System.Diagnostics.Process.Start(fileName, this.FileName);
    }

    public override void VariableParameterMaintenance()
    {
      Params.Input[0].Optional = this.FileName != null; //filename can have input from user input
      Params.Input[0].ClearRuntimeMessages(); // this needs to be called to avoid having a runtime warning message after changed to optional
    }

    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("File", (string)this.FileName);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this.FileName = (string)reader.GetString("File");
      return base.Read(reader);
    }
    #endregion
  }
}
