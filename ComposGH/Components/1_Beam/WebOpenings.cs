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
using UnitsNet;
using UnitsNet.Units;
using System.Linq;

namespace ComposGH.Components
{
    public class WebOpenings : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("BF28C2ED-FF68-48D9-8070-DCC704B64380");
        public WebOpenings()
          : base("Web Openings", "WebOpng", "Create Web Openings for a Steel Beam",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = false; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateStudZoneLength;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity stress = new Pressure(0, stressUnit);
            string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
            IQuantity length = new Length(0, lengthUnit);
            string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("Diameter [" + unitAbbreviation + "]", "Ø", "Diameter of stud head", GH_ParamAccess.item);
            pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Diameter of stud head", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grade [" + stressunitAbbreviation + "]", "fu", "Stud Steel Grade", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stud Dims", "Sdm", "Compos Shear Stud Dimensions", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // get default length inputs used for all cases
            Length dia = Length.Zero;
            if (this.Params.Input[0].Sources.Count > 0)
                dia = GetInput.Length(this, DA, 0, lengthUnit, true);
            Length h = Length.Zero;
            if (this.Params.Input[1].Sources.Count > 0)
                h = GetInput.Length(this, DA, 1, lengthUnit, true);

            if (code == DesignCode.Code.EN1994_1_1_2004)
            {
                Pressure strengthS = GetInput.Stress(this, DA, 2, stressUnit);
                DA.SetData(0, new StudDimensionsGoo(new StudDimensions(dia, h, strengthS)));
            }
            else
            {
                Force strengthF = GetInput.Force(this, DA, 2, forceUnit);
                DA.SetData(0, new StudDimensionsGoo(new StudDimensions(dia, h, strengthF)));
            }
        }

        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            Helpers.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            Helpers.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

            UpdateUIFromSelectedItems();

            first = false;

            return base.Read(reader);
        }
        #endregion

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
            IQuantity length = new Length(0, lengthUnit);
            string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            Params.Input[0].Name = "Diameter [" + unitAbbreviation + "]";
            Params.Input[1].Name = "Height [" + unitAbbreviation + "]";

            if (code == DesignCode.Code.EN1994_1_1_2004)
            {
                IQuantity stress = new Pressure(0, stressUnit);
                string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

                Params.Input[2].Name = "Grade [" + stressunitAbbreviation + "]";
                Params.Input[2].Description = "Stud Steel Grade";
            }
            else
            {
                IQuantity force = new Force(0, forceUnit);
                string forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

                Params.Input[2].Name = "Strength [" + forceunitAbbreviation + "]";
                Params.Input[2].Description = "Character strength";
            }
        }
        #endregion
    }
}
