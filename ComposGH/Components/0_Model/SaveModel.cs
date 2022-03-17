using System;
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
    public class SaveModel : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("36795650-3E41-47B3-B5CA-00E58DDAC09C");
        public SaveModel()
          : base("Save Model", "Save", "Save a Compos Model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { this.Hidden = true; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.SaveModel;

        #endregion

        #region Input and output
        // This region handles input and output parameters

        string fileName = null;
        bool usersetFileName = false;
        dynamic composSaveModel;
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Compos Model", "Compos", "Compos model to save", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Save?", "Save", "Input 'True' to save or use button", GH_ParamAccess.item, false);
            pManager.AddTextParameter("File and Path", "File", "Filename and path", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Compos Model", "Compos", "Compos Model", GH_ParamAccess.item);
        }
  
        #endregion
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ComposModel composModel = new ComposModel();
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                if (gh_typ == null) { return; }
                if (gh_typ.Value is ComposModelGoo)
                {
                    gh_typ.CastTo(ref composModel);
                    composSaveModel = composModel.Model;
                    Message = "";
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to Compos Model");
                    return;
                }

                if (!usersetFileName)
                {
                    if (composModel.FileName != "")
                        fileName = composModel.FileName;
                }

                string tempfile = "";
                if (DA.GetData(2, ref tempfile))
                    fileName = tempfile;

                bool save = false;
                if (DA.GetData(1, ref save))
                {
                    if (save)
                        composModel.Model.SaveAs(fileName);
                }

                DA.SetData(0, new ComposModelGoo(composModel));
            }


        }
    }
}
