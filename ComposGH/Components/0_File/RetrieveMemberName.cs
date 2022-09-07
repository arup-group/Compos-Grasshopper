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
    public class RetrieveMemberName : GH_OasysComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("AAB466A1-61D8-47FF-9D25-34C349B895A6");
        public RetrieveMemberName()
          : base("Member Name", "Retrieve", "Retrieve a Compos member name",
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
            pManager.AddGenericParameter("Compos Model", "Compos", "Compos model from which retrive member name.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Index", "i", "Index of the member name.", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Member Name", "MN", "Compos member Name", GH_ParamAccess.item);
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
            string index = "";
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            DA.GetData(0, ref gh_typ);
            DA.GetData(1, ref index);

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
            object MN = composModel.Model.MemberName(index);


            DA.SetData(0, MN.ToString());



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
    public class RetrieveMemberName : GH_OasysComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("AAB466A1-61D8-47FF-9D25-34C349B895A6");
        public RetrieveMemberName()
          : base("Member Name", "Retrieve", "Retrieve a Compos member name",
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
            pManager.AddGenericParameter("Compos Model", "Compos", "Compos model from which retrive member name.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Index", "i", "Index of the member name.", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Member Name", "MN", "Compos member Name", GH_ParamAccess.item);
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
            string index = "";
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            DA.GetData(0, ref gh_typ);
            DA.GetData(1, ref index);

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
            object MN = composModel.Model.MemberName(index);


            DA.SetData(0, MN.ToString());



        }
    }
}
>>>>>>> origin/main
