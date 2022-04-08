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
    public class RebarMat : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("E91D37A1-81D4-427D-9910-E8A514466F3C");
        public RebarMat()
          : base("Custom Stud Dimensions", "CustStudDim", "Create Custom Stud Dimensions for a Compos Stud",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
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

                //material type
                dropdownitems.Add(Enum.GetValues(typeof(Rebar.RebarMatType)).Cast<Rebar.RebarMatType>().Select(x => x.ToString()).ToList());
                selecteditems.Add(type.ToString());

                // grade
                dropdownitems.Add(Enum.GetValues(typeof(Rebar.StandardGrade)).Cast<Rebar.StandardGrade>().Select(x => x.ToString()).ToList());
                selecteditems.Add(mat.ToString());

                // strength
                dropdownitems.Add(Units.FilteredStressUnits);
                selecteditems.Add(stressUnit.ToString());

                spacerDescriptions = spacerDescriptionsStandard;
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
                if (type.ToString() == selecteditems[i])
                    return; // return if selected value is same as before

                type = (Rebar.RebarMatType)Enum.Parse(typeof(Rebar.RebarMatType), selecteditems[i]);

                if (type == Rebar.RebarMatType.Standard)
                {
                    mat = (Rebar.StandardGrade)Enum.Parse(typeof(Rebar.StandardGrade), selecteditems[i]);
                    dropdownitems[1] = Enum.GetValues(typeof(Rebar.StandardGrade)).Cast<Rebar.StandardGrade>().Select(x => x.ToString()).ToList();
                    selecteditems[1] = mat.ToString();
                    dropdownitems.Add(Units.FilteredStressUnits);
                    selecteditems.Add(stressUnit.ToString());
                    spacerDescriptions = spacerDescriptionsStandard;
                }
                else
                {
                    // remove additional dropdown lists
                    while (dropdownitems.Count > 1)
                        dropdownitems.RemoveAt(1);
                    while (selecteditems.Count > 1)
                        selecteditems.RemoveAt(1);

                    // re-add dropdown list for force unit
                    dropdownitems.Add(Units.FilteredStressUnits);
                    selecteditems.Add(stressUnit.ToString());

                    // update space descriptions
                    spacerDescriptions = spacerDescriptionsOther;
                }
            }
            if (type == Rebar.RebarMatType.Standard)
            {
                if (i == 1) // change is made to grade
                {
                    mat = (Rebar.StandardGrade)Enum.Parse(typeof(Rebar.StandardGrade), selecteditems[i]);
                }
                if (i == 2) // change is made to unit
                {
                    stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);
                }
            }
            else if (i == 1) // change is made to grade
            {
                stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);
            }


            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }

        private void UpdateUIFromSelectedItems()
        {
            type = (Rebar.RebarMatType)Enum.Parse(typeof(Rebar.RebarMatType), selecteditems[0]);
            if (type == Rebar.RebarMatType.Custom)
            {
                mat = (Rebar.StandardGrade)Enum.Parse(typeof(Rebar.StandardGrade), selecteditems[1]);
                stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[2]);
            }
            else
            {
                stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[1]);
            }

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
        List<string> spacerDescriptions;

        List<string> spacerDescriptionsStandard = new List<string>(new string[]
        {
            "Option",
            "Grade",
            "Unit"
        });
        List<string> spacerDescriptionsOther = new List<string>(new string[]
        {
            "Option",
            "Unit"
        });

        private bool first = true;

        private PressureUnit stressUnit = Units.StressUnit;
        private Rebar.RebarMatType type = Rebar.RebarMatType.Standard;
        private Rebar.StandardGrade mat = Rebar.StandardGrade.EN_500B;

        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity stress = new Pressure(0, stressUnit);
            string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
            pManager.AddGenericParameter("Strength [" + stressunitAbbreviation + "]", "fu", "Characteristic Steel Strength", GH_ParamAccess.item);
            pManager[0].Optional = true;

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stud Dims", "Sdm", "Compos Shear Stud Dimensions", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA) //TODO
        {
            if (type == Rebar.RebarMatType.Standard)
            {
                if (this.Params.Input[0].Sources.Count > 0)
                    DA.SetData(0, new RebarGoo(new Rebar(GetInput.Stress(this, DA, 0, stressUnit))));
                else
                    DA.SetData(0, new RebarGoo(new Rebar(mat)));
            }
            else
            {
                //Pressure fu = GetInput.Stress(this, DA, 0, stressUnit);
                DA.SetData(0, new RebarGoo(new Rebar(GetInput.Stress(this, DA, 0, stressUnit))));
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
        void IGH_VariableParameterComponent.VariableParameterMaintenance() //TODO
        {
            IQuantity stress = new Pressure(0, stressUnit);
            string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
            Params.Input[0].Name = "Strength [" + stressunitAbbreviation + "]";

        }
        #endregion
    }
}
