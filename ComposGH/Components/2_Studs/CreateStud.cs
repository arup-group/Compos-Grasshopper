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
    public class CreateStud : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("1451E11C-69D0-47D3-8730-FCA80E838E25");
        public CreateStud()
          : base("Create Stud Zone Length", "Zone Length", "Create the zone length for the studs",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        { this.Hidden = false; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateStudZoneLength;
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
                dropdownitems.Add(Units.FilteredStressUnits);
                selecteditems.Add(stressUnit.ToString());

                first = false;
            }
            m_attributes = new UI.MultiDropDownCheckBoxesComponentUI(this, SetSelected, dropdownitems, selecteditems, SetWelding, initialCheckState, checkboxText, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[i]);

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
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
        private void UpdateUIFromSelectedItems()
        {
            lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[0]);

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
            "Stress Unit",
            "Settings"
        });
        private bool first = true;
        private LengthUnit lengthUnit = Units.LengthUnitGeometry;
        private PressureUnit stressUnit = Units.StressUnit;
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity length = new Length(0, lengthUnit);
            string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
            IQuantity stress = new Pressure(0, stressUnit);
            string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("Diameter [" + unitAbbreviation + "]", "D", "Stud Zone Length of the right end of the slab", GH_ParamAccess.item);
            pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Stud Zone Length of the right end of the slab", GH_ParamAccess.item);
            pManager.AddGenericParameter("Strength [" + stressunitAbbreviation + "]", "fu", "Stud strength", GH_ParamAccess.item);
            pManager.AddGenericParameter("No Stud Start [" + unitAbbreviation + "]", "NSS", "[Optional] No Stud Zone Length at the start end of the beam", GH_ParamAccess.item);
            pManager.AddGenericParameter("No Stud End [" + unitAbbreviation + "]", "NSE", "[Optional] No Stud Zone Length at the end of the beam", GH_ParamAccess.item);
            pManager.AddGenericParameter("Rebar Pos. [" + unitAbbreviation + "]", "RbP", "[Optional] Position of reinforcement from underside of stud head", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stud", "Std", "Compos Shear Stud", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Length diameter = GetInput.Length(this, DA, 0, lengthUnit);
            Length height = GetInput.Length(this, DA, 1, lengthUnit);
            Pressure strength = GetInput.Stress(this, DA, 2, stressUnit);
            
            Length startZoneNoStud = new Length(0, lengthUnit);
            if (this.Params.Input[3].Sources.Count > 0)
                startZoneNoStud = GetInput.Length(this, DA, 3, lengthUnit, true);

            Length endZoneNoStud = new Length(0, lengthUnit);
            if (this.Params.Input[4].Sources.Count > 0)
                startZoneNoStud = GetInput.Length(this, DA, 4, lengthUnit, true);

            Length rebarPosition = new Length(0, lengthUnit);
            if (this.Params.Input[5].Sources.Count > 0)
                startZoneNoStud = GetInput.Length(this, DA, 5, lengthUnit, true);


            //ComposStud stud = new ComposStud(diameter, height, strength, startZoneNoStud, endZoneNoStud, rebarPosition, Welding, NCCILimits);

            //DA.SetData(0, new ComposStudGoo(stud));
        }
    }
}
