using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;

using Grasshopper.Kernel.Parameters;
using System.Resources;
using ComposGH.Parameters;

namespace ComposGH.Components
{
    /// <summary>
    /// Component to create a new Material
    /// </summary>
    public class CreateSteelMaterial : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("E07C80F4-55D3-4DC1-B6FB-6580416DC8E5");
        public CreateSteelMaterial()
          : base("Create Steel Material", "Material", "Create Compos Steel Material",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => ComposGH.Properties.Resources.CreateSteelMaterial;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                selecteditem = _mode.ToString();
                //first = false;
            }

            m_attributes = new UI.DropDownComponentUI(this, SetSelected, dropdownitems, selecteditem, "Steel Type");
        }

        public void SetSelected(string selected)
        {
            selecteditem = selected;
            switch (selected)
            {
                case "Custom":
                    Mode1Clicked();
                    break;
                case "S235":
                    Mode2Clicked();
                    break;
                case "S275":
                    Mode3Clicked();
                    break;
                case "S355":
                    Mode4Clicked();
                    break;
                case "S450":
                    Mode5Clicked();
                    break;
                case "S460":
                    Mode6Clicked();
                    break;
            }
        }
        #endregion

        #region Input and output
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "Custom",
            "S235",
            "S275",
            "S355",
            "S450",
            "S460"
        });


        string selecteditem;

       
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Steel Material", "Ma", "Compos Steel Material", GH_ParamAccess.item);
        }

        #endregion
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ComposSteelMaterial material = new ComposSteelMaterial();


            // element type (picked in dropdown)
            if (_mode == FoldMode.Custom)
            {
                GH_Number gh_fy = new GH_Number();
                GH_Number gh_E = new GH_Number();
                GH_Number gh_ρ = new GH_Number();

                DA.GetData(0, ref gh_fy);
                DA.GetData(1, ref gh_E);
                DA.GetData(2, ref gh_ρ);

                material.SType = ComposSteelMaterial.SteelType.CUSTOM;

                if (GH_Convert.ToDouble(gh_fy, out double fy, GH_Conversion.Both))
                    material.YeldStrength = fy;
                if (GH_Convert.ToDouble(gh_E, out double E, GH_Conversion.Both))
                    material.YoungModulus = E;
                if (GH_Convert.ToDouble(gh_ρ, out double ρ, GH_Conversion.Both))
                    material.Density = ρ;
            }
            if (_mode == FoldMode.S235)
            {
                material.SType = ComposSteelMaterial.SteelType.S235;
                material.WeldMaterial = "Grade 35";
                material.YeldStrength = double.NaN;
                material.YoungModulus = double.NaN;
                material.Density = double.NaN;
            }
            if (_mode == FoldMode.S275)
            {
                material.SType = ComposSteelMaterial.SteelType.S275;
                material.WeldMaterial = "Grade 35";
                material.YeldStrength = double.NaN;
                material.YoungModulus = double.NaN;
                material.Density = double.NaN;
            }
            if (_mode == FoldMode.S355)
            {
                material.SType = ComposSteelMaterial.SteelType.S355;
                material.WeldMaterial = "Grade 42";
                material.YeldStrength = double.NaN;
                material.YoungModulus = double.NaN;
                material.Density = double.NaN;
            }
            if (_mode == FoldMode.S450)
            {
                material.SType = ComposSteelMaterial.SteelType.S450;
                material.WeldMaterial = "Grade 50";
                material.YeldStrength = double.NaN;
                material.YoungModulus = double.NaN;
                material.Density = double.NaN;
            }
            if (_mode == FoldMode.S460)
            {
                material.SType = ComposSteelMaterial.SteelType.S460;
                material.WeldMaterial = "Grade 50";
                material.YeldStrength = double.NaN;
                material.YoungModulus = double.NaN;
                material.Density = double.NaN;
            }


            DA.SetData(0, new ComposSteelMaterialGoo(material));
        }
        #region menu override
        private enum FoldMode
        {
            Custom,
            S235,
            S275,
            S355,
            S450,
            S460
        }
        private bool first = true;
        private FoldMode _mode = FoldMode.S235;


        private void Mode1Clicked()
        {
            FoldMode myMode = FoldMode.Custom;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param = 3;

            for (int i = 0; i < param; i++)
                Params.RegisterInputParam(new Param_Number());

            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            FoldMode myMode = FoldMode.S235;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param = 0;


            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);


            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode3Clicked()
        {
            FoldMode myMode = FoldMode.S275;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param = 0;


            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);


            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode4Clicked()
        {
            FoldMode myMode = FoldMode.S355;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param = 0;


            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);


            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode5Clicked()
        {
            FoldMode myMode = FoldMode.S450;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param = 0;


            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);


            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode6Clicked()
        {
            FoldMode myMode = FoldMode.S460;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param = 0;


            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);


            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        #endregion
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("Mode", (int)_mode);
            writer.SetString("select", selecteditem);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            _mode = (FoldMode)reader.GetInt32("Mode");
            selecteditem = reader.GetString("select");
            this.CreateAttributes();
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
            if (_mode == FoldMode.Custom)
            {
                int i = 0;
                Params.Input[i].NickName = "fy";
                Params.Input[i].Name = "Yeld Strength";
                Params.Input[i].Description = "Yeld Strength of steel material";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "E";
                Params.Input[i].Name = "Young's Modulus";
                Params.Input[i].Description = "young's Modulus of steel material";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "ρ";
                Params.Input[i].Name = "Density";
                Params.Input[i].Description = "Density of steel material";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;

            }
        }
        #endregion  
    }
}