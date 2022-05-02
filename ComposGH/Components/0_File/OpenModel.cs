<<<<<<< HEAD
﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace ComposGH.Components
{
    public class OpenModel : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("555839bf-08ae-45bd-87c2-84c65d3dc115");
        public OpenModel()
          : base("Open Model", "Open", "Open a Compos Model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { this.Hidden = true; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.OpenModel;

        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            m_attributes = new UI.ButtonComponentUI(this, "Open", OpenFile, "Open Compos file");
        }
        public void OpenFile()
        {
            var fdi = new Rhino.UI.OpenFileDialog { Filter = "Compos Files(*.cob)|*.coa|All files (*.*)|*.*" };
            var res = fdi.ShowOpenDialog();
            if (res)
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
                        //RecordUndoEvent("Changed OpenCompos Component input");
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
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Filename and path", "File", "Compos model to open and work with." +
                    System.Environment.NewLine + "Input either path component, a text string with path and " +
                    System.Environment.NewLine + "filename or an existing Compos model created in Grasshopper.", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Compos Model", "Compos", "Compos Model", GH_ParamAccess.item);
        }

        #endregion

        #region (de)serialization
        //This region handles serialisation and deserialisation, meaning that 
        // component states will be remembered when reopening GH script
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            //writer.SetInt32("Mode", (int)_mode);
            writer.SetString("File", (string)fileName);
            //writer.SetBoolean("Advanced", (bool)advanced);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            //_mode = (FoldMode)reader.GetInt32("Mode");
            fileName = (string)reader.GetString("File");
            //advanced = (bool)reader.GetBoolean("Advanced");
            return base.Read(reader);
        }
        #endregion

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();

            DA.GetData(0, ref gh_typ);

            if (gh_typ.Value is GH_String)
            {
                string tempfile = "";
                if (GH_Convert.ToString(gh_typ, out tempfile, GH_Conversion.Both))
                    fileName = tempfile;

                if (!fileName.EndsWith(".cob"))
                    fileName += ".cob";


                ComposModel composModel = new ComposModel
                {
                    FileName = fileName
                };

                string path = System.IO.Path.GetTempPath() + "Compos.coa";

                composModel.Model.Open(fileName);
                composModel.Model.SaveAs(path);

                DA.SetData(0, new ComposModelGoo(composModel));
                return;
            }


        }
    }
}
=======
﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace ComposGH.Components
{
    public class OpenModel : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("555839bf-08ae-45bd-87c2-84c65d3dc115");
        public OpenModel()
          : base("Open Model", "Open", "Open a Compos Model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { this.Hidden = true; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.OpenModel;

        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            m_attributes = new UI.ButtonComponentUI(this, "Open", OpenFile, "Open Compos file");
        }
        public void OpenFile()
        {
            var fdi = new Rhino.UI.OpenFileDialog { Filter = "Compos Files(*.cob)|*.coa|All files (*.*)|*.*" };
            var res = fdi.ShowOpenDialog();
            if (res)
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
                        //RecordUndoEvent("Changed OpenCompos Component input");
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
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Filename and path", "File", "Compos model to open and work with." +
                    System.Environment.NewLine + "Input either path component, a text string with path and " +
                    System.Environment.NewLine + "filename or an existing Compos model created in Grasshopper.", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Compos Model", "Compos", "Compos Model", GH_ParamAccess.item);
        }

        #endregion

        #region (de)serialization
        //This region handles serialisation and deserialisation, meaning that 
        // component states will be remembered when reopening GH script
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            //writer.SetInt32("Mode", (int)_mode);
            writer.SetString("File", (string)fileName);
            //writer.SetBoolean("Advanced", (bool)advanced);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            //_mode = (FoldMode)reader.GetInt32("Mode");
            fileName = (string)reader.GetString("File");
            //advanced = (bool)reader.GetBoolean("Advanced");
            return base.Read(reader);
        }
        #endregion

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();

            DA.GetData(0, ref gh_typ);

            if (gh_typ.Value is GH_String)
            {
                string tempfile = "";
                if (GH_Convert.ToString(gh_typ, out tempfile, GH_Conversion.Both))
                    fileName = tempfile;

                if (!fileName.EndsWith(".cob"))
                    fileName += ".cob";


                ComposModel composModel = new ComposModel
                {
                    FileName = fileName
                };

                string path = System.IO.Path.GetTempPath() + "Compos.coa";

                composModel.Model.Open(fileName);
                composModel.Model.SaveAs(path);

                DA.SetData(0, new ComposModelGoo(composModel));
                return;
            }


        }
    }
}
>>>>>>> origin/main
