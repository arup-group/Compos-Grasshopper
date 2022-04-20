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
    public class WebOpeningStiffener : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("4e7a2c23-0504-46d2-8fe1-846bf4ef6a37");
        public WebOpeningStiffener()
          : base("Web Opening Stiffeners", "Stiffener", "Create Horizontal Web Opening Stiffeners for a Compos Web Opening or Notch",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = false; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

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

                // type
                dropdownitems.Add(Enum.GetValues(typeof(stiff_types)).Cast<stiff_types>()
                    .Select(x => x.ToString()).ToList());
                selecteditems.Add(stiff_types.Web_Opening.ToString().Replace('_', ' '));

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

            if (i == 0)
            {
                if (selecteditems[i] == openingType.ToString().Replace('_', ' '))
                    return;
                openingType = (stiff_types)Enum.Parse(typeof(stiff_types), selecteditems[i].Replace(' ', '_'));
                ModeChangeClicked();
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
            openingType = (stiff_types)Enum.Parse(typeof(stiff_types), selecteditems[0].Replace(' ', '_'));
            lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[1]);
            
            CreateAttributes();
            ModeChangeClicked();
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
            "Type",
            "Unit"
        });
        private enum stiff_types
        {
            Web_Opening,
            Notch,
        }
        private bool first = true;
        private stiff_types openingType = stiff_types.Web_Opening;
        private LengthUnit lengthUnit = Units.LengthUnitGeometry;
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity length = new Length(0, lengthUnit);
            string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            pManager.AddBooleanParameter("Both Sides", "BS", "Set to true to apply horizontal stiffeners on both sides of web", GH_ParamAccess.item);
            pManager.AddGenericParameter("Dist. z [" + unitAbbreviation + "]", "Dz", "Vertical distance above/below opening edge to centre of stiffener (beam local z-axis)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Top Width [" + unitAbbreviation + "]", "Wt", "Top Stiffener Width", GH_ParamAccess.item);
            pManager.AddGenericParameter("Top Thickness [" + unitAbbreviation + "]", "Tt", "Top Stiffener Thickness", GH_ParamAccess.item);
            pManager.AddGenericParameter("Bottom Width [" + unitAbbreviation + "]", "Wb", "Bottom Stiffener Width", GH_ParamAccess.item);
            pManager.AddGenericParameter("Bottom Thickness [" + unitAbbreviation + "]", "Tb", "Bottom Stiffener Thickness", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stiffeners", "WS", "Web Opening Stiffeners for Compos Web Opening or Notch", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool bothSides = false;
            DA.GetData(0, ref bothSides);
            Length start = GetInput.Length(this, DA, 1, lengthUnit);
            Length topWidth = GetInput.Length(this, DA, 2, lengthUnit);
            Length topTHK = GetInput.Length(this, DA, 3, lengthUnit);
            if (openingType == stiff_types.Web_Opening)
            {
                Length bottomWidth = GetInput.Length(this, DA, 4, lengthUnit);
                Length bottomTHK = GetInput.Length(this, DA, 5, lengthUnit);
                DA.SetData(0, new WebOpeningStiffenersGoo(new WebOpeningStiffeners(
                    start, topWidth, topTHK, bottomWidth, bottomTHK, bothSides)));
            }
            else
            {
                DA.SetData(0, new WebOpeningStiffenersGoo(new WebOpeningStiffeners(
                    start, topWidth, topTHK, bothSides)));
            }
        }
        #region update input params
        private void ModeChangeClicked()
        {
            RecordUndoEvent("Changed Parameters");

            if (openingType == stiff_types.Web_Opening)
            {
                if (this.Params.Input.Count == 6)
                    return;

                Params.RegisterInputParam(new Param_GenericObject());
                Params.RegisterInputParam(new Param_GenericObject());
            }
            if (openingType == stiff_types.Notch)
            {
                if (this.Params.Input.Count == 4)
                    return;

                Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
                Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
            }
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
            Params.Input[1].Name = "Dist. z [" + unitAbbreviation + "]";
            Params.Input[2].Name = "Top Width [" + unitAbbreviation + "]";
            Params.Input[3].Name = "Top Thickness [" + unitAbbreviation + "]";

            if (openingType == stiff_types.Web_Opening)
            {
                int i = 4;
                Params.Input[i].Name = "Bottom Width [" + unitAbbreviation + "]";
                Params.Input[i].NickName = "Wb";
                Params.Input[i].Description = "Bottom Stiffener Width";
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].Name = "Bottom Thickness [" + unitAbbreviation + "]";
                Params.Input[i].NickName = "Tb";
                Params.Input[i].Description = "Bottom Stiffener Thickness";
                Params.Input[i].Optional = false;
            }
        }
        #endregion
    }
}
