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

namespace ComposGH.Components
{
    public class CustomRebarSpacing : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public CustomRebarSpacing()
          : base("Transeverse Reinforcement", "TransRb", "Create Transverse Reinforcement for Compos Slab",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        { this.Hidden = false; } // sets the initial state of the component to hidden

        public override Guid ComponentGuid => new Guid("E832E3E8-1EF9-4F31-BC2A-683881E4BAC3");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.RebarSpacing;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // length
                dropdownitems.Add(Units.FilteredLengthUnits);
                selecteditems.Add(lengthUnit.ToString());

                first = false;
            }
            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];
            
            lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }

        private void UpdateUIFromSelectedItems()
        {
            lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[0]);

            CreateAttributes();
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Unit",
        });

        private bool first = true;
        private LengthUnit lengthUnit = Units.LengthUnitGeometry;
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity length = new Length(0, lengthUnit);
            string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
            
            pManager.AddGenericParameter("Material", "RMt", "Reinforcement Materia", GH_ParamAccess.item);
            pManager.AddGenericParameter("From [" + unitAbbreviation + "]", "PS", "Starting Point from beam start where this Rebar Spacing Groups begins", GH_ParamAccess.item);
            pManager.AddGenericParameter("To [" + unitAbbreviation + "]", "PE", "End Point from beam start point where this Rebar Spacing Groups begins", GH_ParamAccess.item);
            pManager.AddGenericParameter("Diameter [" + unitAbbreviation + "]", "Ø", "Transverse rebar diameter", GH_ParamAccess.item);
            pManager.AddGenericParameter("Spacing [" + unitAbbreviation + "]", "S", "The centre/centre distance rebars in this group", GH_ParamAccess.item);
            pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinforcement cover", GH_ParamAccess.item);

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Reinforcement", "Rb", "Transverse Reinforcement for Compos Slab", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            RebarMaterial mat = GetInput.RebarMaterial(this, DA, 0);
            // get default length inputs used for all cases
            Length start = GetInput.Length(this, DA, 1, lengthUnit);
            Length end = GetInput.Length(this, DA, 2, lengthUnit);
            Length dia = GetInput.Length(this, DA, 3, lengthUnit);
            Length spacing = GetInput.Length(this, DA, 4, lengthUnit);
            Length cov = GetInput.Length(this, DA, 5, lengthUnit);

            DA.SetData(0, new ComposReinforcementGoo(new ComposReinforcement(mat, start, end, dia, spacing, cov)));
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

            Params.Input[1].Name = "From [" + unitAbbreviation + "]";
            Params.Input[2].Name = "To [" + unitAbbreviation + "]";
            Params.Input[3].Name = "Diameter [" + unitAbbreviation + "]";
            Params.Input[4].Name = "Spacing [" + unitAbbreviation + "]";
            Params.Input[5].Name = "Cover [" + unitAbbreviation + "]";
        }
        #endregion
    }
}