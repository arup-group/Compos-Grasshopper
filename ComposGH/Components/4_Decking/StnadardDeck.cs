﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using System.Text.RegularExpressions;
using System.IO;
using ComposGH.Parameters;
using Rhino.Geometry;
using UnitsNet;
using UnitsNet.Units;
using ComposGH.Components;
using ComposGH.Helpers;

namespace ComposGH.Components
{
    public class StandardDeck : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public StandardDeck()
            : base("Standard Decking", "StdDeck", "Create Standard Decking for Compos Slab",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat4())
        { this.Hidden = false; }
        public override Guid ComponentGuid => new Guid("6796D3E6-CF84-4AC6-ABB7-012C20E6DB9A");
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.MeshReinforcement;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // catalogue
                dropdownitems.Add(catalogueNames);
                selecteditems.Add(catalogueNames[0]);
                catalogue = selecteditems[0];

                // decking
                sectionList = SqlReader.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), catalogue);
                dropdownitems.Add(sectionList);
                selecteditems.Add(sectionList[0]);
                profile = selecteditems[1];

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }

        public void SetSelected(int i, int j)
        {

            // change selected item
            selecteditems[i] = dropdownitems[i][j];


            if (i == 0)  // change is made to code 
            {
                // update selected section to be all
                dropdownitems[1] = SqlReader.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), selecteditems[0]);
            }
            
            if (i == 1)
            {
                // update displayed selected
                profile = selecteditems[1];
            }
            //profile = selecteditems[1];

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }


        private void UpdateUIFromSelectedItems()
        {
            sectionList = SqlReader.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), catalogue);
            //catalogue = selecteditems[0];
            //profile = selecteditems[1];

            CreateAttributes();
            ExpireSolution(true);
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region Input and output

        // list of lists with all dropdown lists content
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Type", 
            "Decking"
        });
        private bool first = true;
        string catalogue = null;
        string profile = null;
        #endregion


        #region catalogue section
        // Catalogues
        List<string> catalogueNames = SqlReader.GetDeckCataloguesDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"));

        // Sections
        // list of displayed sections
        List<string> sectionList = null;
        List<string> filteredlist = new List<string>();
        //List<string> sectionList = SqlReader.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), catalogue);


        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

            //pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinforcement cover", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Rotated", "Rot", "If the mesh type is assymetrical, setting 'Rotated' to true will align the stronger direction with the beam's direction", GH_ParamAccess.item, false);

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Reinforcement", "Rb", "Mesh Reinforcement for Compos Slab", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, "hi");
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
            //empty
        }

    }
    #endregion
}
