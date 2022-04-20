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
using Grasshopper.Kernel.Parameters;

namespace ComposGH.Components
{
    public class StudSpecEN : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("467bb1c3-ea5e-4c63-a012-d088158fb173");
        public StudSpecEN()
          : base("Stud EN Specification", "StudSpecEN", "Create Stud Specification to EN1994-1-1 for a Compos Stud",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        { this.Hidden = false; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

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
            if (lengthUnit.ToString() == selecteditems[i])
                return;

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
            pManager[3].Optional = true;
            pManager[4].Optional = true;
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
            // get rebar position
            Length rebarPos = new Length(30, LengthUnit.Millimeter);
            if (this.Params.Input[2].Sources.Count > 0)
                rebarPos = GetInput.Length(this, DA, 2, lengthUnit, true);
            bool welded = true;
            DA.GetData(3, ref welded);
            bool ncci = false;
            DA.GetData(4, ref ncci);
            StudSpecification specEN = new StudSpecification(
                noStudZoneStart, noStudZoneEnd, rebarPos, welded, ncci);
            DA.SetData(0, new StudSpecificationGoo(specEN));
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
            Params.Input[0].Name = "No Stud Zone Start [" + unitAbbreviation + "]";
            Params.Input[1].Name = "No Stud Zone End [" + unitAbbreviation + "]";
            Params.Input[2].Name = "Rebar Pos [" + unitAbbreviation + "]";
        }
        #endregion
    }
}
