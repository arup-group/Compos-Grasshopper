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
    public class StandardStudDimensions : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("c97c7e52-7aa3-438f-900a-33f6ca889b3c");
        public StandardStudDimensions()
          : base("Standard Stud Dimensions", "StdStudDim", "Create Standard Stud Dimensions for a Compos Stud",
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

                // spacing
                dropdownitems.Add(standardSize);
                selecteditems.Add(stdSize.ToString().Replace("D", "Ø").Replace("H", "/"));

                // grade
                dropdownitems.Add(Enum.GetValues(typeof(StudDimensions.StandardGrade)).Cast<StudDimensions.StandardGrade>().Select(x => x.ToString()).ToList());
                selecteditems.Add(stdGrd.ToString());

                // strength
                dropdownitems.Add(Units.FilteredStressUnits);
                selecteditems.Add(stressUnit.ToString());

                spacerDescriptions = spacerDescriptionsEN;
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
                    dropdownitems[1] = standardSize;
                    selecteditems[1] = stdSize.ToString();
                    dropdownitems[2] = Enum.GetValues(typeof(StudDimensions.StandardGrade)).Cast<StudDimensions.StandardGrade>().Select(x => x.ToString()).ToList();
                    selecteditems[2] = stdGrd.ToString();
                    dropdownitems.Add(Units.FilteredStressUnits);
                    selecteditems.Add(stressUnit.ToString());
                    spacerDescriptions = spacerDescriptionsEN;
                }
                else
                {
                    switch (code) // change dropdown list
                    {
                        case DesignCode.Code.BS5950_3_1_1990_A1_2010:
                        case DesignCode.Code.BS5950_3_1_1990_Superseeded:
                        case DesignCode.Code.AS_NZS2327_2017:
                            dropdownitems[1] = standardSize;
                            break;
                        case DesignCode.Code.HKSUOS_2005:
                            dropdownitems[1] = standardSizeHK2005;
                            break;
                        case DesignCode.Code.HKSUOS_2011:
                            dropdownitems[1] = standardSizeHK2011;
                            break;
                    }
                    // change selected item, test if currently selected value exist in new list
                    if (!dropdownitems[1].Contains(stdSize.ToString().Replace("D", "Ø").Replace("H", "/")))
                    {
                        selecteditems[1] = dropdownitems[1][0];
                    }
                    else
                        selecteditems[1] = stdSize.ToString().Replace("D", "Ø").Replace("H", "/");

                    // remove additional dropdown lists
                    while (dropdownitems.Count > 2)
                        dropdownitems.RemoveAt(2);
                    while (selecteditems.Count > 2)
                        selecteditems.RemoveAt(2);

                    // re-add dropdown list for force unit
                    dropdownitems.Add(Units.FilteredForceUnits);
                    selecteditems.Add(forceUnit.ToString());

                    // update space descriptions
                    spacerDescriptions = spacerDescriptionsOther;
                }
            }
            if (code == DesignCode.Code.EN1994_1_1_2004) 
            {
                if (i == 1) // change is made to size
                {
                    string sz = selecteditems[i].Replace("Ø", "D").Replace("/", "H");
                    stdSize = (StudDimensions.StandardSize)Enum.Parse(typeof(StudDimensions.StandardSize), sz);
                }
                if (i == 2) // change is made to grade
                {
                    stdGrd = (StudDimensions.StandardGrade)Enum.Parse(typeof(StudDimensions.StandardGrade), selecteditems[i]);
                }
                if (i == 3) // change is made to grade
                {
                    stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);
                }
            }
            else if (i == 1) // change is made to size
            {
                string sz = selecteditems[i].Replace("Ø", "D").Replace("/", "H");
                stdSize = (StudDimensions.StandardSize)Enum.Parse(typeof(StudDimensions.StandardSize), sz);
            }
            else if (i == 2) // change is made to grade
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
            if (code == DesignCode.Code.EN1994_1_1_2004)
            {
                string sz = selecteditems[1].Replace("Ø", "D").Replace("/", "H");
                stdSize = (StudDimensions.StandardSize)Enum.Parse(typeof(StudDimensions.StandardSize), sz);
                stdGrd = (StudDimensions.StandardGrade)Enum.Parse(typeof(StudDimensions.StandardGrade), selecteditems[2]);
                stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[3]);
            }
            else
            {
                string sz = selecteditems[1].Replace("Ø", "D").Replace("/", "H");
                stdSize = (StudDimensions.StandardSize)Enum.Parse(typeof(StudDimensions.StandardSize), sz);
                forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[2]);
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
        List<string> spacerDescriptionsEN = new List<string>(new string[]
        {
            "Design Code",
            "Dimensions",
            "Grade",
            "Unit"
        });
        List<string> spacerDescriptionsOther = new List<string>(new string[]
        {
            "Design Code",
            "Dimensions",
            "Unit"
        });
        List<string> standardSize = new List<string>(new string[]
        {
            "Ø13mm/65mm",
            "Ø16mm/75mm",
            "Ø19mm/75mm",
            "Ø19mm/100mm",
            "Ø19mm/125mm",
            "Ø22mm/100mm",
            "Ø25mm/100mm"
        });
        List<string> standardSizeHK2005 = new List<string>(new string[]
        {
            "Ø16mm/75mm",
            "Ø19mm/100mm",
            "Ø22mm/100mm",
            "Ø25mm/100mm"
        });
        List<string> standardSizeHK2011 = new List<string>(new string[]
        {
            "Ø16mm/70mm",
            "Ø19mm/95mm",
            "Ø22mm/95mm",
            "Ø25mm/95mm",
        });

        private bool first = true;
        private PressureUnit stressUnit = Units.StressUnit;
        private ForceUnit forceUnit = Units.ForceUnit;
        private StudDimensions.StandardGrade stdGrd = StudDimensions.StandardGrade.SD1_EN13918;
        private StudDimensions.StandardSize stdSize = StudDimensions.StandardSize.D19mmH100mm;
        private DesignCode.Code code = DesignCode.Code.EN1994_1_1_2004;
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity stress = new Pressure(0, stressUnit);
            string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("Grade [" + stressunitAbbreviation + "]", "fu", "Stud Steel Grade", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stud Dims", "Sdm", "Compos Shear Stud Dimensions", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            if (code == DesignCode.Code.EN1994_1_1_2004)
            {
                if (this.Params.Input[0].Sources.Count > 0)
                    DA.SetData(0, new StudDimensionsGoo(new StudDimensions(stdSize, GetInput.Stress(this, DA, 0, stressUnit, true))));
                else
                    DA.SetData(0, new StudDimensionsGoo(new StudDimensions(stdSize, stdGrd)));
            }
            else
            {
                DA.SetData(0, new StudDimensionsGoo(new StudDimensions(stdSize, GetInput.Force(this, DA, 0, forceUnit))));
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
            if (code == DesignCode.Code.EN1994_1_1_2004)
            {
                IQuantity stress = new Pressure(0, stressUnit);
                string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

                Params.Input[0].Name = "Grade [" + stressunitAbbreviation + "]";
                Params.Input[0].Description = "Stud Steel Grade";
                Params.Input[0].Optional = true;
            }
            else
            {
                IQuantity force = new Force(0, forceUnit);
                string forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

                Params.Input[0].Name = "Strength [" + forceunitAbbreviation + "]";
                Params.Input[0].Description = "Character strength";
                Params.Input[0].Optional = false;
            }
        }
        #endregion
    }
}
