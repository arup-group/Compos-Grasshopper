using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace ComposGH.Components
{
    public class ComposGHDropDownComponent : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("12b7aa4a-e7d4-4a9f-89b0-4d8467e4cc6e");
        public ComposGHDropDownComponent()
          : base("Analyse Beam", "Analyse", "Analyse a Compos Beam",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat6())
        { this.Hidden = false; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Solution;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // force
                dropdownitems.Add(Units.FilteredForceUnits);
                selecteditems.Add(forceUnit.ToString());

                // moment
                dropdownitems.Add(Units.FilteredMomentUnits);
                selecteditems.Add(momentUnit.ToString());

                momentUnitAbbreviation = Oasys.Units.Moment.GetAbbreviation(momentUnit);
                IQuantity stress = new Force(0, forceUnit);
                forceUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            switch (i)
            {
                case 0:
                    momentUnit = (Oasys.Units.MomentUnit)Enum.Parse(typeof(Oasys.Units.MomentUnit), selecteditems[i]);
                    break;
                case 1:
                    forceUnit = (UnitsNet.Units.ForceUnit)Enum.Parse(typeof(UnitsNet.Units.ForceUnit), selecteditems[i]);
                    break;
            }

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            CreateAttributes();
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region Input and output
        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Moment Unit"
            "Force Unit",
        });
        private bool first = true;

        private Oasys.Units.MomentUnit momentUnit = Units.MomentUnit;
        private UnitsNet.Units.ForceUnit forceUnit = Units.ForceUnit;
        string momentUnitAbbreviation;
        string forceUnitAbbreviation;
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Moment [" + momentUnitAbbreviation + "]", "M", "Value for moment (YY-axis)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Force [" + forceUnitAbbreviation + "]", "F", "Value for force (X-axis)", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Load", "Ld", "Compos Load", GH_ParamAccess.item);
        }

        #endregion
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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

            momentUnit = (Oasys.Units.MomentUnit)Enum.Parse(typeof(Oasys.Units.MomentUnit), selecteditems[0]);
            forceUnit = (UnitsNet.Units.ForceUnit)Enum.Parse(typeof(UnitsNet.Units.ForceUnit), selecteditems[1]);
            UpdateUIFromSelectedItems();
            first = false;
            return base.Read(reader);
        }
        #endregion
        
        #region IGH_VariableParameterComponent
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
            momentUnitAbbreviation = Oasys.Units.Moment.GetAbbreviation(momentUnit);
            IQuantity force = new Force(0, forceUnit);
            forceUnitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            Params.Input[0].Name = "Moment [" + momentUnitAbbreviation + "]";
            Params.Input[1].Name = "Force [" + forceUnitAbbreviation + "]";
        }
        #endregion
    }
}
