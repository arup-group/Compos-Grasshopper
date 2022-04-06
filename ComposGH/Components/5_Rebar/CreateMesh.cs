  using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using UnitsNet;
using ComposGH.Components;
using ComposGH.Helpers;

namespace ComposGH.Components
{
    public class CreateMesh : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public CreateMesh()
            : base("Create Rebar Mesh", "Rebar Mesh", "Create Rebar Mesh for a Compos Slab",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; }
        public override Guid ComponentGuid => new Guid("17960644-0DFC-4F5D-B17C-45E6FBC3732E");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.RebarMesh;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                List<string> list = Enum.GetNames(typeof(FoldMode)).ToList();
                dropdownitems = new List<List<string>>();
                dropdownitems.Add(list);

                selecteditems = new List<string>();
                selecteditems.Add(dropdownitems[0][0]);

                // length
                //dropdownitems.Add(Enum.GetNames(typeof(UnitsNet.Units.LengthUnit)).ToList());
                dropdownitems.Add(Units.FilteredLengthUnits);
                selecteditems.Add(lengthUnit.ToString());

                IQuantity quantity = new Length(0, lengthUnit);
                unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }

        public void SetSelected(int i, int j)
        {
            // set selected item
            selecteditems[i] = dropdownitems[i][j];
            if (i == 0)
            {
                _mode = (FoldMode)Enum.Parse(typeof(FoldMode), selecteditems[i]);
                ToggleInput();
            }
            else
            {
                lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[i]);
            }
        }
        private void UpdateUIFromSelectedItems()
        {
            lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[1]);
            CreateAttributes();
            ToggleInput();
        }
        #endregion

        #region Input and output
        List<List<string>> dropdownitems;
        List<string> selecteditems;
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Mesh Type",
            "Measure"
        });
        private UnitsNet.Units.LengthUnit lengthUnit = Units.LengthUnitGeometry;
        string unitAbbreviation;
        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinfrcement cover", GH_ParamAccess.item);
            _mode = FoldMode.A393;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mesh reinforcement", "Mesh", "Mesh reinforcement type for Compos Slab Reinforcement", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string rebar = "";
            DA.SetData(0, rebar);

        }

        #region menu override

        private bool first = true;
        private enum FoldMode
        {
            A393,
            A252,
            A193,
            A142,
            A98,
            B1131,
            B785,
            B503,
            B385,
            B283,
            B196,
            C785,
            C636,
            C503,
            C385,
            C283
        }

        private FoldMode _mode = FoldMode.A393;

        private void ToggleInput()
        {
            RecordUndoEvent("Changed dropdown");
            ExpireSolution(true);
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            Helpers.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            writer.SetString("mode", _mode.ToString());
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            Helpers.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
            _mode = (FoldMode)Enum.Parse(typeof(FoldMode), reader.GetString("mode"));
            UpdateUIFromSelectedItems();
            first = false;
            return base.Read(reader);
        }

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

        #endregion

        #region IGH_VariableParameterComponent null implementation
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            IQuantity quantity = new Length(0, lengthUnit);
            unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));
            Params.Input[0].Name = "Cover [" + unitAbbreviation + "]";
        }
        #endregion
    }
}
