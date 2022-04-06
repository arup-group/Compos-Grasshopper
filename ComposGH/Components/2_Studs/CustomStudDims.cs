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

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateStudZoneLength;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // code
                dropdownitems.Add(Enum.GetValues(typeof(DesignCode.Code)).Cast<DesignCode.Code>().Select(x => x.ToString()).ToList());
                selecteditems.Add(code.ToString());

                // length
                dropdownitems.Add(Units.FilteredLengthUnits);
                selecteditems.Add(lengthUnit.ToString());

                // strength
                dropdownitems.Add(Units.FilteredStressUnits);
                selecteditems.Add(stressUnit.ToString());

                first = false;
            }
            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            if (i == 0) // change is made to code 
            {
                if (code.ToString() == selecteditems[i])
                    return; // return if selected value is same as before

                code = (DesignCode.Code)Enum.Parse(typeof(DesignCode.Code), selecteditems[i]);

                if (code == DesignCode.Code.EN1994_1_1_2004)
                {
                    dropdownitems[2] = Units.FilteredStressUnits;
                    selecteditems[2] = stressUnit.ToString();
                }
                else
                {
                    dropdownitems[2] = Units.FilteredForceUnits;
                    selecteditems[2] = forceUnit.ToString();
                }
            }
            else if (i == 1) // change is made to length unit
            {
                lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);
            }
            else if (code == DesignCode.Code.EN1994_1_1_2004) 
            {
                stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);
            }
            else 
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
            code = (DesignCode.Code)Enum.Parse(typeof(DesignCode.Code), selecteditems[0]);
            lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[1]);
            if (code == DesignCode.Code.EN1994_1_1_2004)
                stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[2]);
            else
                forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[2]);

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
            "Design Code",
            "Length Unit",
            "Strength Unit"
        });

        private bool first = true;
        private LengthUnit lengthUnit = Units.LengthUnitGeometry;
        private PressureUnit stressUnit = Units.StressUnit;
        private ForceUnit forceUnit = Units.ForceUnit;
        private DesignCode.Code code = DesignCode.Code.EN1994_1_1_2004;
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
