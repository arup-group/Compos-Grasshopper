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


// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace ComposGH.Components._4_Studs
{
    public class CreateStud : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("1451E11C-69D0-47D3-8730-FCA80E838E25");
        public CreateStud()
          : base("Create Stud Zone Length", "Zone Length", "Create the zone length for the studs",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = false; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateStudZoneLength;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            m_attributes = new UI.CheckBoxComponentUI(this, SetWelding, checkboxText, initialCheckState, "Settings");
        }

        List<string> checkboxText = new List<string>() { "Welded", "Use NCCI" };
        List<bool> initialCheckState = new List<bool>() { true, true };
        bool Welding = true;
        bool NCCILimits = true;

        public void SetWelding(List<bool> value)
        {
            Welding = value[0];
            NCCILimits = value[1];
        }
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Diameter", "D", "Diameter of Stud", GH_ParamAccess.item);                                         //0
            pManager.AddGenericParameter("Height", "H", "Height of Stud", GH_ParamAccess.item);                                             //1
            pManager.AddGenericParameter("Left", "LE[m or %]", "Stud Zone Length of the left end of the slab", GH_ParamAccess.item);        //2
            pManager.AddGenericParameter("Right", "RE[m or %]", "Stud Zone Length of the right end of the slab", GH_ParamAccess.item);      //3
            pManager.AddGenericParameter("Strength", "fy", "Steel Strength of Stud", GH_ParamAccess.item);                                  //4
            pManager.AddGenericParameter("Reinf", "RP", "Distance below underside of the stud head", GH_ParamAccess.item);                  //5
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stud Zone", "SZ", "Gives your stud zone left and right", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //double StudZL = new double();
            //double StudZR = new double();

            //DA.GetData(0, ref StudZL);
            //DA.GetData(1, ref StudZR);

            Length diameter = GetInput.Length(this, DA, 0, UnitsNet.Units.LengthUnit.Millimeter);
            Length height = GetInput.Length(this, DA, 1, UnitsNet.Units.LengthUnit.Millimeter);
            Length studZL = GetInput.Length(this, DA, 2, UnitsNet.Units.LengthUnit.Millimeter);
            Length studZR = GetInput.Length(this, DA, 3, UnitsNet.Units.LengthUnit.Millimeter);
            Pressure steelStrength = GetInput.Stress(this, DA, 4, UnitsNet.Units.PressureUnit.NewtonPerSquareMillimeter);
            Length reinfPosition = GetInput.Length(this, DA, 5, UnitsNet.Units.LengthUnit.Millimeter);

            //List<string> StudZoneGoo = new List<string>() { StudZL.ToString(), StudZR.ToString() };
            //DA.SetData(0, string.Join<string>(", ", StudZoneGoo));

            List<string> checkStuds = new List<string>();

            #region Welding
            if (Welding)
                checkStuds.Add("Welding is applied");
            else
                checkStuds.Add("Welding is not applied");
            #endregion

            #region NCCI
            if (NCCILimits)
                checkStuds.Add("NCCI Limit is applied");
            else
                checkStuds.Add("NCCI Limit is not applied");
            #endregion

            ComposStud studs = new ComposStud(diameter, height, studZL, studZR, steelStrength, reinfPosition);

            DA.SetData(0, new ComposStudGoo(studs));
        }
    }
}