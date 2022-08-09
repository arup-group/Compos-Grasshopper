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
    public class WebOpenings : GH_OasysComponent, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("BF28C2ED-FF68-48D9-8070-DCC704B64380");
        public WebOpenings()
          : base("Web Openings", "WebOpng", "Create Web Openings for a Steel Beam",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
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

                // opening
                dropdownitems.Add(Enum.GetValues(typeof(WebOpeningsSpec.Opening)).Cast<WebOpeningsSpec.Opening>().Select(x => x.ToString()).ToList());
                selecteditems.Add(opening.ToString());

                // stiffener
                dropdownitems.Add(Enum.GetValues(typeof(WebOpeningsSpec.Stiffeners)).Cast<WebOpeningsSpec.Stiffeners>().Select(x => x.ToString()).ToList());
                selecteditems.Add(stiffeners.ToString());

                spacerDescriptions = spacerDescription;
                first = true;
            }
            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescription);
        }
        public void SetSelected(int i, int j)
        {

            if (i == 0)
            {
                opening = (WebOpeningsSpec.Opening)Enum.Parse(typeof(WebOpeningsSpec.Opening), selecteditems[i]);

            }

            else if (i == 1)
            {
                stiffeners = (WebOpeningsSpec.Stiffeners)Enum.Parse(typeof(WebOpeningsSpec.Stiffeners), selecteditems[i]);
            }

            if (opening == WebOpeningsSpec.Opening.Rectangular && stiffeners == WebOpeningsSpec.Stiffeners.Without_Stiffeners)
            {
                ModeChangeClicked1();
            }
            else if (opening == WebOpeningsSpec.Opening.Rectangular && stiffeners == WebOpeningsSpec.Stiffeners.With_Stiffeners)
            {
                ModeChangeClicked2();
            }
            else if ((opening == WebOpeningsSpec.Opening.Notch_Left | opening == WebOpeningsSpec.Opening.Notch_Right) && stiffeners == WebOpeningsSpec.Stiffeners.Without_Stiffeners)
            {
                ModeChangeClicked3();
            }
            else if ((opening == WebOpeningsSpec.Opening.Notch_Left | opening == WebOpeningsSpec.Opening.Notch_Right) && stiffeners == WebOpeningsSpec.Stiffeners.With_Stiffeners)
            {
                ModeChangeClicked4();
            }
            else if (opening == WebOpeningsSpec.Opening.Circular && stiffeners == WebOpeningsSpec.Stiffeners.Without_Stiffeners)
            {
                ModeChangeClicked5();
            }
            else if (opening == WebOpeningsSpec.Opening.Circular && stiffeners == WebOpeningsSpec.Stiffeners.With_Stiffeners)
            {
                ModeChangeClicked6();
            }

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);

        }

        private enum FoldMode
        {
            RectNoStiffener,
            RectYesStiffener,
            NotchLRNoStiffener,
            NotchLRYesStiffener,
            CircNoStiffener,
            CircYesStiffener
        }

        private FoldMode _mode = FoldMode.RectNoStiffener; //default FoldMode

        private void UpdateUIFromSelectedItems()
        {
            if (opening == WebOpeningsSpec.Opening.Rectangular && stiffeners == WebOpeningsSpec.Stiffeners.Without_Stiffeners)
            {
                ModeChangeClicked1();
            }
            else if (opening == WebOpeningsSpec.Opening.Rectangular && stiffeners == WebOpeningsSpec.Stiffeners.With_Stiffeners)
            {
                ModeChangeClicked2();
            }
            else if ((opening == WebOpeningsSpec.Opening.Notch_Left | opening == WebOpeningsSpec.Opening.Notch_Right) && stiffeners == WebOpeningsSpec.Stiffeners.Without_Stiffeners)
            {
                ModeChangeClicked3();
            }
            else if ((opening == WebOpeningsSpec.Opening.Notch_Left | opening == WebOpeningsSpec.Opening.Notch_Right) && stiffeners == WebOpeningsSpec.Stiffeners.With_Stiffeners)
            {
                ModeChangeClicked4();
            }
            else if (opening == WebOpeningsSpec.Opening.Circular && stiffeners == WebOpeningsSpec.Stiffeners.Without_Stiffeners)
            {
                ModeChangeClicked5();
            }
            else if (opening == WebOpeningsSpec.Opening.Circular && stiffeners == WebOpeningsSpec.Stiffeners.With_Stiffeners)
            {
                ModeChangeClicked6();
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
        List<string> spacerDescription = new List<string>(new string[]
        {
            "Opening Types",
            "Stiffeners",
        });

        private bool first = true;
        private WebOpeningsSpec.Opening opening = WebOpeningsSpec.Opening.Circular;
        private WebOpeningsSpec.Stiffeners stiffeners = WebOpeningsSpec.Stiffeners.Without_Stiffeners;
        #endregion

        #region Input and output

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Width/Diameter", "W/D", "Width/Diameter of the opening", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("WebOpenings", "WO", "Web Openings for Steel Beam", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double diameter = new double();

            if (opening == WebOpeningsSpec.Opening.Rectangular && stiffeners == WebOpeningsSpec.Stiffeners.Without_Stiffeners)
            {
                double height = new double();
                double centroidPosFromStart = new double();
                double centroidPosFromTop = new double();

                DA.GetData(0, ref diameter);
                DA.GetData(1, ref height);
                DA.GetData(2, ref centroidPosFromStart);
                DA.GetData(3, ref centroidPosFromTop);
            }
            else if (opening == WebOpeningsSpec.Opening.Rectangular && stiffeners == WebOpeningsSpec.Stiffeners.With_Stiffeners)
            {
                double height = new double();
                double centroidPosFromStart = new double();
                double centroidPosFromTop = new double();
                bool bothSides = new bool();
                double stiffenerDistanceFromOpening = new double();
                double topStiffenerWidth = new double();
                double topStiffenerThickness = new double();
                double bottomStiffenerWidth = new double();
                double bottomStiffenerThickness = new double();

                DA.GetData(0, ref diameter);
                DA.GetData(1, ref height);
                DA.GetData(2, ref centroidPosFromStart);
                DA.GetData(3, ref centroidPosFromTop);
                DA.GetData(4, ref bothSides);
                DA.GetData(5, ref stiffenerDistanceFromOpening);
                DA.GetData(6, ref topStiffenerWidth);
                DA.GetData(7, ref topStiffenerThickness);
                DA.GetData(8, ref bottomStiffenerWidth);
                DA.GetData(9, ref bottomStiffenerThickness);
            }
            else if ((opening == WebOpeningsSpec.Opening.Notch_Left | opening == WebOpeningsSpec.Opening.Notch_Left) && stiffeners == WebOpeningsSpec.Stiffeners.Without_Stiffeners)
            {
                double height = new double();

                DA.GetData(0, ref diameter);
                DA.GetData(1, ref height);
            }
            else if ((opening == WebOpeningsSpec.Opening.Notch_Left | opening == WebOpeningsSpec.Opening.Notch_Left) && stiffeners == WebOpeningsSpec.Stiffeners.With_Stiffeners)
            {
                double height = new double();
                bool bothSides = new bool();
                double stiffenerDistanceFromOpening = new double();
                double topStiffenerWidth = new double();
                double topStiffenerThickness = new double();

                DA.GetData(0, ref diameter);
                DA.GetData(1, ref height);
                DA.GetData(2, ref bothSides);
                DA.GetData(3, ref stiffenerDistanceFromOpening);
                DA.GetData(4, ref topStiffenerWidth);
                DA.GetData(5, ref topStiffenerThickness);
            }
            else if (opening == WebOpeningsSpec.Opening.Circular && stiffeners == WebOpeningsSpec.Stiffeners.Without_Stiffeners)
            {
                double centroidPosFromStart = new double();
                double centroidPosFromTop = new double();

                DA.GetData(0, ref diameter);
                DA.GetData(1, ref centroidPosFromStart);
                DA.GetData(2, ref centroidPosFromTop);
            }
            else if (opening == WebOpeningsSpec.Opening.Circular && stiffeners == WebOpeningsSpec.Stiffeners.With_Stiffeners)
            {
                double centroidPosFromStart = new double();
                double centroidPosFromTop = new double();
                bool bothSides = new bool();
                double stiffenerDistanceFromOpening = new double();
                double topStiffenerWidth = new double();
                double topStiffenerThickness = new double();
                double bottomStiffenerWidth = new double();
                double bottomStiffenerThickness = new double();

                DA.GetData(0, ref diameter);
                DA.GetData(1, ref centroidPosFromStart);
                DA.GetData(2, ref centroidPosFromTop);
                DA.GetData(3, ref bothSides);
                DA.GetData(4, ref stiffenerDistanceFromOpening);
                DA.GetData(5, ref topStiffenerWidth);
                DA.GetData(6, ref topStiffenerThickness);
                DA.GetData(7, ref bottomStiffenerWidth);
                DA.GetData(8, ref bottomStiffenerThickness);
            }
        }

        #region menu override
        #endregion


        #region update input params

        private void ModeChangeClicked1() //RectCircNoStiffener
        {
            if (_mode == FoldMode.RectNoStiffener)
                return;

            int numberOfInputs = 3;
            RecordUndoEvent("Changed Parameters");

            _mode = FoldMode.RectNoStiffener;

            while (Params.Input.Count > numberOfInputs)
            {
                Params.UnregisterInputParameter(Params.Input[numberOfInputs], true);
            }

            while (Params.Input.Count < numberOfInputs)
            {
                Params.RegisterInputParam(new Param_GenericObject());
            }

        }

        private void ModeChangeClicked2() //RectCircYesStiffener
        {
            if (_mode == FoldMode.RectYesStiffener)
                return;

            int numberOfInputs = 9;
            RecordUndoEvent("Changed Parameters");

            _mode = FoldMode.RectYesStiffener;

            while (Params.Input.Count > numberOfInputs)
            {
                Params.UnregisterInputParameter(Params.Input[numberOfInputs], true);
            }

            while (Params.Input.Count < numberOfInputs)
            {
                Params.RegisterInputParam(new Param_GenericObject());
            }

        }

        private void ModeChangeClicked3() //NotchLRNoStiffener
        {
            if (_mode == FoldMode.NotchLRNoStiffener)
                return;

            int numberOfInputs = 1;
            RecordUndoEvent("Changed Parameters");

            _mode = FoldMode.NotchLRNoStiffener;

            while (Params.Input.Count > numberOfInputs)
            {
                Params.UnregisterInputParameter(Params.Input[numberOfInputs], true);
            }

            while (Params.Input.Count < numberOfInputs)
            {
                Params.RegisterInputParam(new Param_GenericObject());
            }

        }

        private void ModeChangeClicked4() //NotchLRYesStiffener
        {
            if (_mode == FoldMode.NotchLRYesStiffener)
                return;

            int numberOfInputs = 5;
            RecordUndoEvent("Changed Parameters");

            _mode = FoldMode.NotchLRYesStiffener;

            while (Params.Input.Count > numberOfInputs)
            {
                Params.UnregisterInputParameter(Params.Input[numberOfInputs], true);
            }

            while (Params.Input.Count < numberOfInputs)
            {
                Params.RegisterInputParam(new Param_GenericObject());
            }

        }

        private void ModeChangeClicked5() //CircNoStiffener
        {
            if (_mode == FoldMode.CircNoStiffener)
                return;

            int numberOfInputs = 2;
            RecordUndoEvent("Changed Parameters");

            _mode = FoldMode.CircNoStiffener;

            while (Params.Input.Count > numberOfInputs)
            {
                Params.UnregisterInputParameter(Params.Input[numberOfInputs], true);
            }

            while (Params.Input.Count < numberOfInputs)
            {
                Params.RegisterInputParam(new Param_GenericObject());
            }

        }

        private void ModeChangeClicked6() //CircYesStiffener
        {
            if (_mode == FoldMode.CircYesStiffener)
                return;

            int numberOfInputs = 8;
            RecordUndoEvent("Changed Parameters");

            _mode = FoldMode.CircYesStiffener;

            while (Params.Input.Count > numberOfInputs)
            {
                Params.UnregisterInputParameter(Params.Input[numberOfInputs], true);
            }

            while (Params.Input.Count < numberOfInputs)
            {
                Params.RegisterInputParam(new Param_GenericObject());
            }

        }
        #endregion

        #region (de)serialization
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
            if (_mode == FoldMode.RectNoStiffener)
            {
                Params.Input[1].Name = "Height";
                Params.Input[1].NickName = "H";
                Params.Input[1].Description = "Height of the Opening";
                Params.Input[1].Optional = true;
                Params.Input[2].Name = "CentroidPosFromStart";
                Params.Input[2].NickName = "CPS";
                Params.Input[2].Description = "Centroid Position from Start of the Beam";
                Params.Input[2].Optional = true;
                Params.Input[3].Name = "CentroidPosFromTop";
                Params.Input[3].NickName = "CPT";
                Params.Input[3].Description = "Centroid Position from Top of the Beam";
                Params.Input[3].Optional = true;
            }

            else if (_mode == FoldMode.RectYesStiffener)
            {
                Params.Input[1].Name = "Height";
                Params.Input[1].NickName = "H";
                Params.Input[1].Description = "Height of the Opening";
                Params.Input[1].Optional = true;
                Params.Input[2].Name = "CentroidPosFromStart";
                Params.Input[2].NickName = "CPS";
                Params.Input[2].Description = "Centroid Position from Start of the Beam";
                Params.Input[2].Optional = true;
                Params.Input[3].Name = "CentroidPosFromTop";
                Params.Input[3].NickName = "CPT";
                Params.Input[3].Description = "Centroid Position from Top of the Beam";
                Params.Input[3].Optional = true;
                Params.Input[4].Name = "StiffenerDistanceFromOpening";
                Params.Input[4].NickName = "DO";
                Params.Input[4].Description = "Distance between the Opening and the Stiffener";
                Params.Input[4].Optional = true;
                Params.Input[5].Name = "TopStiffenerWidth";
                Params.Input[5].NickName = "TSW";
                Params.Input[5].Description = "Width of the top Stiffener";
                Params.Input[5].Optional = true;
                Params.Input[6].Name = "TopStiffenerThickness";
                Params.Input[6].NickName = "TST";
                Params.Input[6].Description = "Thickness of the top Stiffener";
                Params.Input[6].Optional = true;
                Params.Input[7].Name = "BottomStiffenerWidth";
                Params.Input[7].NickName = "BSW";
                Params.Input[7].Description = "Width of the bottom Stiffener";
                Params.Input[7].Optional = true;
                Params.Input[8].Name = "BottomStiffenerThickness";
                Params.Input[8].NickName = "BST";
                Params.Input[8].Description = "Thickness of the bottom Stiffener";
                Params.Input[8].Optional = true;
                Params.Input[9].Name = "BothSides";
                Params.Input[9].NickName = "BS";
                Params.Input[9].Description = "Apply Stiffeners at both Sides of the Beam?";
                Params.Input[9].Optional = true;
            }

            else if (_mode == FoldMode.NotchLRNoStiffener)
            {
                Params.Input[1].Name = "Height";
                Params.Input[1].NickName = "H";
                Params.Input[1].Description = "Height of the Opening";
                Params.Input[1].Optional = true;
            }

            else if (_mode == FoldMode.NotchLRYesStiffener)
            {
                Params.Input[1].Name = "Height";
                Params.Input[1].NickName = "H";
                Params.Input[1].Description = "Height of the Opening";
                Params.Input[1].Optional = true;
                Params.Input[2].Name = "StiffenerDistanceFromOpening";
                Params.Input[2].NickName = "DO";
                Params.Input[2].Description = "Distance between the Opening and the Stiffener";
                Params.Input[2].Optional = true;
                Params.Input[3].Name = "TopStiffenerWidth";
                Params.Input[3].NickName = "TSW";
                Params.Input[3].Description = "Width of the top Stiffener";
                Params.Input[3].Optional = true;
                Params.Input[4].Name = "TopStiffenerThickness";
                Params.Input[4].NickName = "TST";
                Params.Input[4].Description = "Thickness of the top Stiffener";
                Params.Input[4].Optional = true;
                Params.Input[5].Name = "BothSides";
                Params.Input[5].NickName = "BS";
                Params.Input[5].Description = "Apply Stiffeners at both Sides of the Beam?";
                Params.Input[5].Optional = true;
            }

            else if (_mode == FoldMode.CircNoStiffener)
            {
                Params.Input[1].Name = "CentroidPosFromStart";
                Params.Input[1].NickName = "CPS";
                Params.Input[1].Description = "Centroid Position from Start of the Beam";
                Params.Input[1].Optional = true;
                Params.Input[2].Name = "CentroidPosFromTop";
                Params.Input[2].NickName = "CPT";
                Params.Input[2].Description = "Centroid Position from Top of the Beam";
                Params.Input[2].Optional = true;
            }

            else if (_mode == FoldMode.CircYesStiffener)
            {
                Params.Input[1].Name = "CentroidPosFromStart";
                Params.Input[1].NickName = "CPS";
                Params.Input[1].Description = "Centroid Position from Start of the Beam";
                Params.Input[1].Optional = true;
                Params.Input[2].Name = "CentroidPosFromTop";
                Params.Input[2].NickName = "CPT";
                Params.Input[2].Description = "Centroid Position from Top of the Beam";
                Params.Input[2].Optional = true;
                Params.Input[3].Name = "StiffenerDistanceFromOpening";
                Params.Input[3].NickName = "DO";
                Params.Input[3].Description = "Distance between the Opening and the Stiffener";
                Params.Input[3].Optional = true;
                Params.Input[4].Name = "TopStiffenerWidth";
                Params.Input[4].NickName = "TSW";
                Params.Input[4].Description = "Width of the top Stiffener";
                Params.Input[4].Optional = true;
                Params.Input[5].Name = "TopStiffenerThickness";
                Params.Input[5].NickName = "TST";
                Params.Input[5].Description = "Thickness of the top Stiffener";
                Params.Input[5].Optional = true;
                Params.Input[6].Name = "BottomStiffenerWidth";
                Params.Input[6].NickName = "BSW";
                Params.Input[6].Description = "Width of the bottom Stiffener";
                Params.Input[6].Optional = true;
                Params.Input[7].Name = "BottomStiffenerThickness";
                Params.Input[7].NickName = "BST";
                Params.Input[7].Description = "Thickness of the bottom Stiffener";
                Params.Input[7].Optional = true;
                Params.Input[8].Name = "BothSides";
                Params.Input[8].NickName = "BS";
                Params.Input[8].Description = "Apply Stiffeners at both Sides of the Beam?";
                Params.Input[8].Optional = true;
            }
            #endregion
        }
    }
}
