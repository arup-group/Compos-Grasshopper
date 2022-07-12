using System;
using System.IO;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;
using ComposAPI;


namespace ComposGH.Components
{
  /// <summary>
  /// Component to open an existing compos data file
  /// </summary>
  public class OpenModel : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("51e4fa31-a626-45a0-a3f6-70175ebb80e4");
    public OpenModel()
      : base("Open Model", "Open", "Open an existing Compos data file",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override Bitmap Icon => Properties.Resources.OpenModel;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      m_attributes = new UI.ButtonComponentUI(this, "Open", OpenFile, "Open Compos file");
    }
    public void OpenFile()
    {
      var fdi = new Rhino.UI.OpenFileDialog { Filter = "Compos Files(*.cob)|*.cob|All files (*.*)|*.*" };
      var res = fdi.ShowOpenDialog();
      if (res) // == DialogResult.OK)
      {
        fileName = fdi.FileName;

        // instantiate  new panel
        var panel = new Grasshopper.Kernel.Special.GH_Panel();
        panel.CreateAttributes();

        // set the location relative to the open component on the canvas
        panel.Attributes.Pivot = new PointF((float)Attributes.DocObject.Attributes.Bounds.Left -
            panel.Attributes.Bounds.Width - 30, (float)Params.Input[0].Attributes.Pivot.Y - panel.Attributes.Bounds.Height / 2);

        // check for existing input
        while (Params.Input[0].Sources.Count > 0)
        {
          var input = Params.Input[0].Sources[0];
          // check if input is the one we automatically create below
          if (Params.Input[0].Sources[0].InstanceGuid == panelGUID)
          {
            // update the UserText in existing panel
            //RecordUndoEvent("Changed OpenGSA Component input");
            panel = input as Grasshopper.Kernel.Special.GH_Panel;
            panel.UserText = fileName;
            panel.ExpireSolution(true); // update the display of the panel
          }

          // remove input
          Params.Input[0].RemoveSource(input);
        }

        //populate panel with our own content
        panel.UserText = fileName;

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
    #endregion

    #region Input and output
    // This region handles input and output parameters

    string fileName = null;
    Guid panelGUID = Guid.NewGuid();
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Filename and path", "File", "Compos file to open and work with." +
              System.Environment.NewLine + "Input either path component, a text string with path and " +
              System.Environment.NewLine + "filename or an existing Compos File created in Grasshopper.", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Model", "Mod", "Compos Model", GH_ParamAccess.item);
    }
    #region IGH_VariableParameterComponent null implementation
    //This sub region handles any changes to the component after it has been placed on the canvas
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      Params.Input[0].Optional = fileName != null; //filename can have input from user input
      Params.Input[0].ClearRuntimeMessages(); // this needs to be called to avoid having a runtime warning message after changed to optional

    }
    #endregion
    #endregion

    #region (de)serialization
    //This region handles serialisation and deserialisation, meaning that 
    // component states will be remembered when reopening GH script
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("File", (string)fileName);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      fileName = (string)reader.GetString("File");
      return base.Read(reader);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ.Value is GH_String)
        {
          string tempfile = "";
          if (GH_Convert.ToString(gh_typ, out tempfile, GH_Conversion.Both))
            fileName = tempfile;

          if (!fileName.EndsWith(".cob"))
            fileName = fileName + ".cob";

          IComposFile composFile = ComposFile.Open(fileName);
          DA.SetData(0, new ComposFileGoo(composFile));
        }
      }
    }
  }
}