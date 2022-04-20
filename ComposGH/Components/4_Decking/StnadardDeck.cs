using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using System.Text.RegularExpressions;
using Grasshopper.Kernel.Parameters;
using System.Linq;
using System.IO;
using System.Reflection;
using UnitsNet;
using ComposGH.Helpers;

namespace ComposGH.Components
{

    public class StandardDeck : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("CD98DB9D-D55B-4994-9535-720F13D6350A");
        public StandardDeck()
          : base("Standard Decking", "StdDeck", "Create Standard Decking for Compos Slab",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat4())
        { this.Hidden = true; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.StandardDeck;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()//TODO
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                //catalogue type
                dropdownitems.Add(typeNames.ToList());
                selecteditems.Add(typeNames.ToString());

                //decking
                //

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }

        public void SetSelected(int i, int j) //TODO
        {
            string cat = "Kingspan"; //TODO
            
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            if (i == 0) // change is made to firstdropdown
            {
                // set types to all
                typeIndex = -1;
                // update typelist with all catalogues
                typedata = SqlReader.GetDeckTypesDataFromSQLite(catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), inclSS);
                typeNames = typedata.Item1;
                typeNumbers = typedata.Item2;

                // update section list to all types
                sectionList = SqlReader.GetDeckingDataFromSQLite(typeNumbers, Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), cat, inclSS);

                // add catalogues (they will always be the same so no need to rerun sql call)
                dropdownitems.Add(typeNames);
            }

                // type list
                // if second list (i.e. catalogue list) is changed, update types list to account for that catalogue
            if (i == 1)
            {
                // update typelist with selected input catalogue
                typedata = SqlReader.GetDeckTypesDataFromSQLite(catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), inclSS);
                typeNames = typedata.Item1;
                typeNumbers = typedata.Item2;

                // update section list from new types (all new types in catalogue)
                List<int> types = typeNumbers.ToList();
                types.RemoveAt(0); // remove -1 from beginning of list
                sectionList = SqlReader.GetDeckingDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), cat, inclSS);

                // update selections to display first item in new list
                selecteditems[0] = typeNames[0];
                selecteditems[1] = filteredlist[0];
            }

            //dropdownitems.Add(typeNames);
            //dropdownitems.Add(filteredlist);
            //profileString = selecteditems[1];


            // update name of inputs (to display unit on sliders) 
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);

        }

        private void UpdateUIFromSelectedItems() //TODO
        {
            typedata = SqlReader.GetDeckTypesDataFromSQLite(catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), inclSS);
            typedata = SqlReader.GetDeckTypesDataFromSQLite(catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), inclSS);
            typeNames = typedata.Item1;
            typeNumbers = typedata.Item2;

            // call graphics update
            comingFromSave = true;
            comingFromSave = false;

            profileString = selecteditems[1];

            CreateAttributes();
            ExpireSolution(true);
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
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
            "Type", "Decking"
        });

        private bool first = true;

        #region catalogue sections

        // Types
        Tuple<List<string>, List<int>> typedata = SqlReader.GetCataloguesDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"));
        List<int> typeNumbers = new List<int>(); //  internal db type numbers
        List<string> typeNames = new List<string>(); // list of displayed types

        // Sections
        bool inclSS;
        List<string> sectionList = SqlReader.GetDeckingDataFromSQLite(new List<int> { -1 }, Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), "Kingspan", false);
        List<string> filteredlist = new List<string>();

        int catalogueIndex = -1; //-1 is all
        int typeIndex = -1;

        // list of sections as outcome from selections
        string profileString = "Multideck 50 (0.8)";
        string search = "";
        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            //empty
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Decking", "Deck", "Decking profile for Compos slab", GH_ParamAccess.item);
        }
        #endregion
        protected override void SolveInstance(IGH_DataAccess DA) //TODO
        {
            this.ClearRuntimeMessages();
            for (int i = 0; i < this.Params.Input.Count; i++)
                this.Params.Input[i].ClearRuntimeMessages();

            #region catalogue
            this.ClearRuntimeMessages();

            // get user input filter search string
            bool incl = false;
            if (DA.GetData(1, ref incl))
            {
                if (inclSS != incl)
                {
                    SetSelected(-1, 0);
                    this.ExpireSolution(true);
                }
            }

            DA.SetData(0, "CAT " + profileString);

            return;

            #endregion

        }

        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer) 
        {
            DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader) 
        {

            DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
            UpdateUIFromSelectedItems();
            first = false;
            return base.Read(reader);
        }
        bool comingFromSave = false;
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
            //empty
        }
        #endregion  
    }
}