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
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using Grasshopper.Kernel.Parameters;

namespace ComposGH.Components
{
    public class CreateReinf : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("236F55AF-8AC1-4349-8240-23F5C52D6E79");
        public CreateReinf()
          : base("Create Reinf", "Reinf", "Create Compos Reinforcement",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        { this.Hidden = false; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateReinfZoneLength;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Mesh reinforcement", "Mesh", "Mesh reinforcement type for Compos Slab Reinforcement", GH_ParamAccess.item);
            pManager.AddGenericParameter("Reinforcement material", "Ref", "Reinforcement steel material", GH_ParamAccess.item); //TODO
            pManager.AddGenericParameter("Rebar Spacing", "RbS", "Custom Compos Transverse Rebar Spacing", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Reinforcement", "Rf", "Compos Reinforcement", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            RebarMesh mesh = GetInput.ComposReinforcement(this, DA, 0);
            Rebar mat = GetInput.Rebar(this, DA, 1);
            List<RebarGroupSpacing> spacings = GetInput.RebarSpacings(this, DA, 2);
            
            DA.SetData(0, new ComposReinfGoo(new ComposReinf(mesh, mat, spacings)));

        }


        #region IGH_VariableParameterComponent null implementation
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
        }
        #endregion
    }
}
