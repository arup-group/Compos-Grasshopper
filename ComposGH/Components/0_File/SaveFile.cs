using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;

namespace ComposGH.Components
{
  /// <summary>
  /// Component to save to a compos data file
  /// </summary>
  public class SaveModel : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("d2fa9fa1-9507-4f57-b383-8b573699906d");
    public SaveModel()
      : base("Save Compos File", "Save", "Saves your Compos File from this parametric nightmare",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override Bitmap Icon => Properties.Resources.SaveModel;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      m_attributes = new UI.Button3ComponentUI(this, "Save", "Save As", "Open in Compos", SaveFile, SaveAsFile, OpenComposexe, true, "Save Compos file");
    }

    public void SaveFile()
    {
      if (this.FileName == null | this.FileName == "")
        SaveAsFile();
      else
      {
        this.ComposFile.SaveAs(this.FileName);
        
        this.Message = "File saved";
      }
    }

    public void SaveAsFile()
    {
      var fdi = new Rhino.UI.SaveFileDialog { Filter = "Compos File (*.cob)|*.cob|All files (*.*)|*.*" };
      var res = fdi.ShowSaveDialog();
      if (res) // == DialogResult.OK)
      {
        this.FileName = fdi.FileName;
        this.UsersetFileName = true;
        
          this.CanOpen = true;
          //CreateAttributes();
          string mes = "File saved";

          //add panel input with string
          //delete existing inputs if any
          while (Params.Input[2].Sources.Count > 0)
            Grasshopper.Instances.ActiveCanvas.Document.RemoveObject(Params.Input[2].Sources[0], false);

          //instantiate  new panel
          var panel = new Grasshopper.Kernel.Special.GH_Panel();
          panel.CreateAttributes();

          panel.Attributes.Pivot = new PointF((float)Attributes.DocObject.Attributes.Bounds.Left -
              panel.Attributes.Bounds.Width - 40, (float)Attributes.DocObject.Attributes.Bounds.Bottom - panel.Attributes.Bounds.Height);

          //populate value list with our own data
          panel.UserText = this.FileName;

          //Until now, the panel is a hypothetical object.
          // This command makes it 'real' and adds it to the canvas.
          Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

          //Connect the new slider to this component
          Params.Input[2].AddSource(panel);
          Params.OnParametersChanged();
          ExpireSolution(true);
        this.Message = mes;
      }
    }

    public void OpenComposexe()
    {
      if (this.FileName != null)
      {
        if (this.FileName != "")
        {
          if (this.CanOpen)
            System.Diagnostics.Process.Start(this.FileName);
        }
      }
    }
    #endregion

    #region Input and output
    // This region handles input and output parameters

    string FileName = null;
    bool UsersetFileName = false;
    ComposFile ComposFile;
    bool CanOpen = false;
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Member", "Mem", "Compos member to save", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Save?", "Save", "Input 'True' to save or use button", GH_ParamAccess.item, false);
      pManager.AddTextParameter("File and Path", "File", "Filename and path", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("File", "Cob", "Compos File", GH_ParamAccess.item);
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
      Params.Input[0].Optional = this.FileName != null; //filename can have input from user input
      Params.Input[0].ClearRuntimeMessages(); // this needs to be called to avoid having a runtime warning message after changed to optional
    }
    #endregion
    #endregion

    #region (de)serialization
    //This region handles serialisation and deserialisation, meaning that 
    // component states will be remembered when reopening GH script
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("File", (string)FileName);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      FileName = (string)reader.GetString("File");
      UpdateUIFromSelectedItems();
      return base.Read(reader);
    }
    private void UpdateUIFromSelectedItems()
    {
      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ == null) { return; }
        if (gh_typ.Value is ComposFileGoo)
        {
          ComposFileGoo goo = (ComposFileGoo)gh_typ.Value;
          this.ComposFile = (ComposFile)goo.Value;
          Message = "";
        }
        else if(gh_typ.Value is MemberGoo)
        {
          MemberGoo goo = (MemberGoo)gh_typ.Value;
          IMember member  = (IMember)goo.Value;

          this.ComposFile = new ComposFile();
          this.ComposFile.Members.Add(member);
          Message = "";
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to Compos File");
          return;
        }

        if (!this.UsersetFileName)
        {
          if (this.ComposFile.FileName != "")
            this.FileName = this.ComposFile.FileName;
        }

        string tempfile = "";
        if (DA.GetData(2, ref tempfile))
          this.FileName = tempfile;

        bool save = false;
        if (DA.GetData(1, ref save))
        {
          if (save)
            Message = this.FileName;
        }

        DA.SetData(0, new ComposFileGoo(this.ComposFile));
      }
    }
  }
}

