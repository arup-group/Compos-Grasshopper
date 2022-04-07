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
    public class StudSpec : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("1ef0e7f8-bd0a-4a10-b6ed-009745062628");
        public StudSpec()
          : base("Stud Specification", "StudSpec", "Create Stud Specification for a Compos Stud",
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
                    ModeENClicked();
                }
                else
                {
                    ModeOtherClicked();
                }
            }
            else if (i == 1) // change is made to length unit
            {
                lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);
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
                ModeENClicked();
            else
                ModeOtherClicked();

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
            "Unit",
            "Settings"
        });

        private bool first = true;
        private LengthUnit lengthUnit = Units.LengthUnitGeometry;
        private DesignCode.Code code = DesignCode.Code.EN1994_1_1_2004;
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity length = new Length(0, lengthUnit);
            string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("No Stud Zone Start [" + unitAbbreviation + "]", 
                "NSZS", "Length of zone without shear studs at the start of the beam (default = 0)", GH_ParamAccess.item);
            pManager.AddGenericParameter("No Stud Zone End [" + unitAbbreviation + "]", 
                "NSZE", "Length of zone without shear studs at the end of the beam (default = 0)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Rebar Pos [" + unitAbbreviation + "]", 
                "RbP", "Reinforcement position distance below underside of stud head (default = 30mm)", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Welded", "Wld", "Welded through profiled steel sheeting", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("NCCI Limits", "NCCI", "Use NCCI limits on minimum percentage of interaction if applicable. " +
                "(Imposed load criteria will not be verified)", GH_ParamAccess.item, false);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stud Spec", "Spc", "Compos Shear Stud Specification", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // get default length inputs used for all cases
            Length noStudZoneStart = Length.Zero;
            if (this.Params.Input[0].Sources.Count > 0)
                noStudZoneStart = GetInput.Length(this, DA, 0, lengthUnit, true);
            Length noStudZoneEnd = Length.Zero;
            if (this.Params.Input[1].Sources.Count > 0)
                noStudZoneEnd = GetInput.Length(this, DA, 1, lengthUnit, true);

            bool welded = true;

            switch (code)
            {
                case DesignCode.Code.EN1994_1_1_2004:
                    // get rebar position
                    Length rebarPos = new Length(30, LengthUnit.Millimeter);
                    if (this.Params.Input[2].Sources.Count > 0)
                        rebarPos = GetInput.Length(this, DA, 2, lengthUnit, true);
                    DA.GetData(3, ref welded);
                    bool ncci = false;
                    DA.GetData(4, ref ncci);
                    StudSpecification specEN = new StudSpecification(
                        noStudZoneStart, noStudZoneEnd, rebarPos, welded, ncci);
                    DA.SetData(0, new StudSpecificationGoo(specEN));
                    return;

                case DesignCode.Code.BS5950_3_1_1990_Superseeded:
                case DesignCode.Code.BS5950_3_1_1990_A1_2010:
                    bool ec4 = true;
                    DA.GetData(2, ref ec4);

                    StudSpecification specBS = new StudSpecification(
                        ec4, noStudZoneStart, noStudZoneEnd);
                    DA.SetData(0, new StudSpecificationGoo(specBS));
                    return;

                default:
                    DA.GetData(2, ref welded);

                    StudSpecification specOther = new StudSpecification(
                        noStudZoneStart, noStudZoneEnd, welded);
                    DA.SetData(0, new StudSpecificationGoo(specOther));
                    return;
            }
        }

        #region update input params
        private void ModeENClicked()
        {
            RecordUndoEvent(code.ToString() + " Parameters");

            //remove input parameters
            while (Params.Input.Count > 2)
                Params.UnregisterInputParameter(Params.Input[2], true);

            //add input parameters
            Params.RegisterInputParam(new Param_GenericObject());
            Params.RegisterInputParam(new Param_Boolean());
            Params.RegisterInputParam(new Param_Boolean());
        }

        private void ModeOtherClicked()
        {
            RecordUndoEvent(code.ToString() + " Parameters");

            //remove input parameters
            while (Params.Input.Count > 2)
                Params.UnregisterInputParameter(Params.Input[2], true);

            //add input parameters
            Params.RegisterInputParam(new Param_Boolean());
        }
        #endregion

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
            Params.Input[0].Name = "No Stud Zone Start [" + unitAbbreviation + "]";
            Params.Input[1].Name = "No Stud Zone End [" + unitAbbreviation + "]";

            if (code == DesignCode.Code.EN1994_1_1_2004)
            {
                Params.Input[2].Name = "Rebar Pos [" + unitAbbreviation + "]";
                Params.Input[2].NickName = "RbP";
                Params.Input[2].Description = "Character strength";

                Params.Input[3].Name = "Welded";
                Params.Input[3].NickName = "Wld";
                Params.Input[3].Description = "Welded through profiled steel sheeting";

                Params.Input[4].Name = "NCCI Limits";
                Params.Input[4].NickName = "NCCI";
                Params.Input[4].Description = "Use NCCI limits on minimum percentage of interaction if applicable. " +
                    "(Imposed load criteria will not be verified)";
            }
            else
            {
                Params.Input[2].Name = "Welded";
                Params.Input[2].NickName = "Wld";
                Params.Input[2].Description = "Welded through profiled steel sheeting";
            }
        }
        #endregion
    }
}