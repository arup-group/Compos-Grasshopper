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
    public class DesignOption : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("C6C4C3A5-575F-473F-96C4-23D1A3A483E7");
        public DesignOption()
          : base("Design Option", "Design", "Create design option of a Compos model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { this.Hidden = true; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        // protected override System.Drawing.Bitmap Icon => Properties.Resources.OpenModel;

        #endregion

        #region Input and output
        // This region handles input and output parameters
        Guid panelGUID = Guid.NewGuid();


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Member Name", "MN", "member name.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("is propped", "P", "is propped or unpropped", GH_ParamAccess.item);
            pManager.AddBooleanParameter("is beam weight inc", "BW", "beam weight yes or beam weight no", GH_ParamAccess.item);
            pManager.AddBooleanParameter("is slab weight inc", "SW", "slab weight yes or slab weight no", GH_ParamAccess.item);
            pManager.AddBooleanParameter("is shear deform inc", "SD", "shear deform yes or shear deform no", GH_ParamAccess.item);
            pManager.AddBooleanParameter("is thin section inc", "TS", "thin section yes or thin section no", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Design Option", "Option", "Design option of Compos Model", GH_ParamAccess.item);
        }

        #endregion


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            
            string MN = "";
            bool P = false;
            bool BW = false;
            bool SW = false;
            bool SD = false;
            bool TS = false;

            string POption = "";
            string BWOption = "";
            string SWOption = "";
            string SDOption = "";
            string TSOption = "";

            DA.GetData(0, ref MN);
            DA.GetData(1, ref P);
            DA.GetData(2, ref BW);
            DA.GetData(3, ref SW);
            DA.GetData(4, ref SD);
            DA.GetData(5, ref TS);

            if (P == true) POption = "PROPPED";
            else POption = "UNPROPPED";

            if (BW == true) BWOption = "BEAM_WEIGHT_YES";
            else BWOption = "BEAM_WEIGHT_NO";

            if (SW == true) SWOption = "SLAB_WEIGHT_YES";
            else SWOption = "SLAB_WEIGHT_NO";

            if (SD == true) SDOption = "SHEAR_DEFORM_YES";
            else SDOption = "SHEAR_DEFORM_NO";

            if (TS == true) TSOption = "THIN_SECTION_YES";
            else TSOption = "THIN_SECTION_NO";


            List<string> strings = new List<string>() {"DESIGN_OPTION", MN, "BS5950-3.1:1990+A1:2010", POption, BWOption, SWOption, SDOption, TSOption };




            DA.SetData(0, string.Join(" ",strings));

        }
    }
}
