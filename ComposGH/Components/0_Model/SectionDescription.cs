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
    public class SectionDescription : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("A13D287B-0697-4CDB-ABB4-5B4636D31422");
        public SectionDescription()
          : base("Section Description", "SectionDesc", "Show a section description of a Compos member",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat6())
        { this.Hidden = true; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.OpenModel;

        #endregion

        #region Input and output
        // This region handles input and output parameters

        Guid panelGUID = Guid.NewGuid();
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Compos Model", "Compos", "Compos model from which retrive section description.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Member Name", "MN", "member name.", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Section Description", "Section", "Compos member section description", GH_ParamAccess.item);
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
            string MN = "";
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            DA.GetData(0, ref gh_typ);
            DA.GetData(1, ref MN);

            if (gh_typ.Value is ComposModelGoo)
            {
                gh_typ.CastTo(ref composModel);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to Compos Model");
                return;
            }

            composModel.Model.Open(System.IO.Path.GetTempPath() + "Compos.coa");
            object SD = composModel.Model.BeamSectDesc(MN);


            DA.SetData(0, SD.ToString());



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
    public class SectionDescription : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("A13D287B-0697-4CDB-ABB4-5B4636D31422");
        public SectionDescription()
          : base("Section Description", "SectionDesc", "Show a section description of a Compos member",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat6())
        { this.Hidden = true; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.OpenModel;

        #endregion

        #region Input and output
        // This region handles input and output parameters

        Guid panelGUID = Guid.NewGuid();
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Compos Model", "Compos", "Compos model from which retrive section description.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Member Name", "MN", "member name.", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Section Description", "Section", "Compos member section description", GH_ParamAccess.item);
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
            string MN = "";
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            DA.GetData(0, ref gh_typ);
            DA.GetData(1, ref MN);

            if (gh_typ.Value is ComposModelGoo)
            {
                gh_typ.CastTo(ref composModel);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to Compos Model");
                return;
            }

            composModel.Model.Open(System.IO.Path.GetTempPath() + "Compos.coa");
            object SD = composModel.Model.BeamSectDesc(MN);


            DA.SetData(0, SD.ToString());



        }
    }
}
>>>>>>> origin/main
