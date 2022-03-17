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
    public class ShowCoa : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("0C025134-4A12-4DE0-82E8-1DAE2E7A3736");
        public ShowCoa()
          : base("Coa Content", "Show", "Show the content of a Compos .coa file",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat6())
        { this.Hidden = true; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        // protected override System.Drawing.Bitmap Icon => Properties.Resources.OpenModel;

        #endregion

        #region Custom UI
        //This region overrides the typical component layout 
        #endregion

        #region Input and output
        // This region handles input and output parameters
        Guid panelGUID = Guid.NewGuid();
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(".coa file or ComposModel", "Compos", ".coa file to show.", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(".coa content", "Content", ".coa content of Compos Model", GH_ParamAccess.item);
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
                if (gh_typ.Value is ComposModelGoo)
                    gh_typ.CastTo(ref composModel);
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to Compos Model");
                    return;
                }

                string path = System.IO.Path.GetTempPath() + "Compos.coa";

                string content = File.ReadAllText(path);

                DA.SetData(0, content);
            }
        }
    }
}
