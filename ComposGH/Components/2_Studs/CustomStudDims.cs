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
    public class CustomStudDimensions : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("e70db6bb-b4bf-4033-a3d0-3ad131fe09b1");
        public CustomStudDimensions()
          : base("Custom Stud Dimensions", "CustStudDim", "Create Custom Stud Dimensions for a Compos Stud",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        { this.Hidden = false; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.CustomStudDims;
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

                // strength
                dropdownitems.Add(Units.FilteredForceUnits);
                selecteditems.Add(forceUnit.ToString());

                first = false;
            }
            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            if (i == 0) // change is made to length unit
            {
                lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);
            }
            if (i == 1)
            {
                forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[i]);
            }

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }

        private void UpdateUIFromSelectedItems()
        {
            lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[0]);
            forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[1]);

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
            "Length Unit",
            "Strength Unit"
        });

        private bool first = true;
        private LengthUnit lengthUnit = Units.LengthUnitGeometry;
        private ForceUnit forceUnit = Units.ForceUnit;
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity force = new Force(0, forceUnit);
            string forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            IQuantity length = new Length(0, lengthUnit);
            string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("Diameter [" + unitAbbreviation + "]", "Ø", "Diameter of stud head", GH_ParamAccess.item);
            pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Height of stud", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grade [" + forceunitAbbreviation + "]", "fu", "Stud Character strength", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stud Dims", "Sdm", "Compos Shear Stud Dimensions", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Length dia = GetInput.Length(this, DA, 0, lengthUnit, true);
            Length h = GetInput.Length(this, DA, 1, lengthUnit, true);
            Force strengthF = GetInput.Force(this, DA, 2, forceUnit);
            DA.SetData(0, new StudDimensionsGoo(new StudDimensions(dia, h, strengthF)));
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
            
            IQuantity force = new Force(0, forceUnit);
            string forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            Params.Input[2].Name = "Strength [" + forceunitAbbreviation + "]";
        }
        #endregion
    }
}
