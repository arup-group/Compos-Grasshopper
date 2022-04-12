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
    public class CreateReinf : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("236F55AF-8AC1-4349-8240-23F5C52D6E79");
        public CreateReinf()
          : base("Create Stud", "Stud", "Create Compos Stud",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        { this.Hidden = false; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateReinfZoneLength;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            //if (first)
            //{
            //    dropdownitems = new List<List<string>>();
            //    selecteditems = new List<string>();

            //    // spacing
            //    dropdownitems.Add(Enum.GetValues(typeof(StudGroupSpacing.StudSpacingType)).Cast<StudGroupSpacing.StudSpacingType>()
            //        .Select(x => x.ToString().Replace("_", " ")).ToList());
            //    selecteditems.Add(StudGroupSpacing.StudSpacingType.Automatic.ToString().Replace("_", " "));

            //    first = false;
            //}
            //m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            //if (spacingType.ToString().Replace("_", " ") == selecteditems[i])
            //    return;

            //spacingType = (StudGroupSpacing.StudSpacingType)Enum.Parse(typeof(StudGroupSpacing.StudSpacingType), selecteditems[i].Replace(" ", "_"));

            ModeChangeClicked();

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }

        private void UpdateUIFromSelectedItems()
        {
            //spacingType = (StudGroupSpacing.StudSpacingType)Enum.Parse(typeof(StudGroupSpacing.StudSpacingType), selecteditems[0]);

            ModeChangeClicked();

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
            "Spacing Type",
        });

        private bool first = true;
        private StudGroupSpacing.StudSpacingType spacingType = StudGroupSpacing.StudSpacingType.Automatic;
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Mesh reinforcement", "Mesh", "Mesh reinforcement type for Compos Slab Reinforcement", GH_ParamAccess.item);
            pManager.AddGenericParameter("Reinforcement material", "Ref", "Reinforcement steel material", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rebar Spacing", "RbS", "Custom Compos Transverse Rebar Spacing", GH_ParamAccess.item);
            //pManager[2].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Reinf", "Rf", "Compos Reinforcement", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            RebarMesh mesh = GetInput.ComposReinforcement(this, DA, 0);
            Rebar mat = GetInput.Rebar(this, DA, 0);
            List<RebarGroupSpacing> spacings = GetInput.RebarSpacings(this, DA, 2);
            
            DA.SetData(0, new ComposReinfGoo(new ComposReinf(mesh, mat, spacings)));

        }
        #region update input params
        private void ModeChangeClicked()
        {
            RecordUndoEvent("Changed Parameters");
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

        }
        #endregion
    }
}
