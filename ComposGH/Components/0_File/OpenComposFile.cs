using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ComposGH.Components {
  public class OpenComposFile : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("51e4fa31-a626-45a0-a3f6-70175ebb80e4");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    internal string FileName = null;
    protected override Bitmap Icon => Resources.OpenModel;
    private Guid panelGUID = Guid.NewGuid();

    public OpenComposFile()
          : base("OpenCompos", "Open", "Open an existing Compos .coa file",
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat0()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void CreateAttributes() {
      m_attributes = new ButtonComponentAttributes(this, "Open", OpenFile, "Open Compos file");
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

    internal void OpenFile() {
      var fdi = new Rhino.UI.OpenFileDialog { Filter = "Compos Files(*.coa)|*.coa|All files (*.*)|*.*" };
      var res = fdi.ShowOpenDialog();
      if (res) // == DialogResult.OK)
      {
        FileName = fdi.FileName;

        // instantiate  new panel
        var panel = new Grasshopper.Kernel.Special.GH_Panel();
        panel.CreateAttributes();

        // set the location relative to the open component on the canvas
        panel.Attributes.Pivot = new PointF((float)Attributes.DocObject.Attributes.Bounds.Left -
            panel.Attributes.Bounds.Width - 30, (float)Params.Input[0].Attributes.Pivot.Y - panel.Attributes.Bounds.Height / 2);

        // check for existing input
        while (Params.Input[0].Sources.Count > 0) {
          var input = Params.Input[0].Sources[0];
          // check if input is the one we automatically create below
          if (Params.Input[0].Sources[0].InstanceGuid == panelGUID) {
            // update the UserText in existing panel
            //RecordUndoEvent("Changed OpenGSA Component input");
            panel = input as Grasshopper.Kernel.Special.GH_Panel;
            panel.UserText = FileName;
            panel.ExpireSolution(true); // update the display of the panel
          }

          // remove input
          Params.Input[0].RemoveSource(input);
        }

        //populate panel with our own content
        panel.UserText = FileName;

        // record the panel's GUID if new, so that we can update it on change
        panelGUID = panel.InstanceGuid;

        //Until now, the panel is a hypothetical object.
        // This command makes it 'real' and adds it to the canvas.
        Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

        //Connect the new slider to this component
        Params.Input[0].AddSource(panel);

        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        Params.OnParametersChanged();

        ExpireSolution(true);
      }
    }

    protected override void InitialiseDropdowns() { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Filename and path", "File", "Compos file to open and work with." +
                                                                System.Environment.NewLine + "Input either path component, a text string with path and " +
                                                                System.Environment.NewLine + "filename or an existing Compos File created in Grasshopper.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter(MemberGoo.Name + "(s)", MemberGoo.NickName, MemberGoo.Description + "s contained in the file.", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ)) {
        if (gh_typ.Value is GH_String) {
          string tempfile = "";
          if (GH_Convert.ToString(gh_typ, out tempfile, GH_Conversion.Both))
            FileName = tempfile;

          if (!FileName.EndsWith(".coa"))
            FileName = FileName + ".coa";

          IComposFile composFile = ComposFile.Open(FileName);
          PostHog.ModelIO(PluginInfo, "openCOA", (int)(new FileInfo(FileName).Length / 1024));

          List<MemberGoo> members = new List<MemberGoo>();
          foreach (IMember mem in composFile.GetMembers())
            members.Add(new MemberGoo(mem));
          DA.SetDataList(0, members);
        }
      }
    }
  }
}
