using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComposGH.Components
{
    public class CreateMeshReinforcement : GH_OasysDropDownComponent
    {
        #region Name and Ribbon Layout
        public override Guid ComponentGuid => new Guid("17960644-0DFC-4F5D-B17C-45E6FBC3732E");
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
        protected override System.Drawing.Bitmap Icon => Resources.MeshReinforcement;
        public CreateMeshReinforcement()
            : base("Create" + MeshReinforcementGoo.Name.Replace(" ", string.Empty),
              MeshReinforcementGoo.Name.Replace(" ", string.Empty),
              "Create a Standard " + MeshReinforcementGoo.Description + " for a " + SlabGoo.Description,
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { Hidden = true; }
        #endregion

        #region Input and output
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

            pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinforcement cover", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Rotated", "Rot", "If the mesh type is assymetrical, setting 'Rotated' to true will align the stronger direction with the beam's direction", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new MeshReinforcementParam());
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // get default length inputs used for all cases
            Length cov = Length.Zero;
            if (Params.Input[0].Sources.Count > 0)
                cov = (Length)Input.UnitNumber(this, DA, 0, LengthUnit, true);

            bool rotated = false;
            DA.GetData(1, ref rotated);
            Output.SetItem(this, DA, 0, new MeshReinforcementGoo(new MeshReinforcement(cov, Mesh, rotated)));
        }

        #region Custom UI
        private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;
        private ReinforcementMeshType Mesh = ReinforcementMeshType.A393;

        protected override void InitialiseDropdowns()
        {
            _spacerDescriptions = new List<string>(new string[] { "Standard Mesh", "Unit" });

            _dropDownItems = new List<List<string>>();
            _selectedItems = new List<string>();

            // mesh
            _dropDownItems.Add(Enum.GetValues(typeof(ReinforcementMeshType)).Cast<ReinforcementMeshType>().Select(x => x.ToString()).ToList());
            _dropDownItems[0].RemoveAt(0); //
            _selectedItems.Add(Mesh.ToString());

            // length
            _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
            _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

            _isInitialised = true;
        }

        public override void SetSelected(int i, int j)
        {
            _selectedItems[i] = _dropDownItems[i][j];
            if (i == 0)  // change is made to code 
            {
                if (Mesh.ToString() == _selectedItems[i])
                    return; // return if selected value is same as before

                Mesh = (ReinforcementMeshType)Enum.Parse(typeof(ReinforcementMeshType), _selectedItems[i]);
            }
            else
                LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

            base.UpdateUI();
        }

        protected override void UpdateUIFromSelectedItems()
        {
            Mesh = (ReinforcementMeshType)Enum.Parse(typeof(ReinforcementMeshType), _selectedItems[0]);
            LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

            base.UpdateUIFromSelectedItems();
        }

        public override void VariableParameterMaintenance()
        {
            string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
            Params.Input[0].Name = "Cover [" + unitAbbreviation + "]";
        }
        #endregion
    }
}
