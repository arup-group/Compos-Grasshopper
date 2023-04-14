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

namespace ComposGH.Components {
  public class CreateMeshReinforcement : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("17960644-0DFC-4F5D-B17C-45E6FBC3732E");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.MeshReinforcement;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private ReinforcementMeshType _mesh = ReinforcementMeshType.A393;

    public CreateMeshReinforcement() : base("Create" + MeshReinforcementGoo.Name.Replace(" ", string.Empty), MeshReinforcementGoo.Name.Replace(" ", string.Empty), "Create a Standard " + MeshReinforcementGoo.Description + " for a " + SlabGoo.Description, Ribbon.CategoryName.Name(), Ribbon.SubCategoryName.Cat3()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 0)  // change is made to code
      {
        if (_mesh.ToString() == _selectedItems[i]) {
          return; // return if selected value is same as before
        }

        _mesh = (ReinforcementMeshType)Enum.Parse(typeof(ReinforcementMeshType), _selectedItems[i]);
      }
      else {
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      Params.Input[0].Name = "Cover [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Standard Mesh", "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // mesh
      _dropDownItems.Add(Enum.GetNames(typeof(ReinforcementMeshType)).ToList());

      _selectedItems.Add(_mesh.ToString());

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinforcement cover", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Rotated", "Rot", "If the mesh type is assymetrical, setting 'Rotated' to true will align the stronger direction with the beam's direction", GH_ParamAccess.item, false);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new MeshReinforcementParam());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      // get default length inputs used for all cases
      Length cov = Length.Zero;
      if (Params.Input[0].Sources.Count > 0) {
        cov = (Length)Input.UnitNumber(this, DA, 0, _lengthUnit, true);
      }

      bool rotated = false;
      DA.GetData(1, ref rotated);
      Output.SetItem(this, DA, 0, new MeshReinforcementGoo(new MeshReinforcement(cov, _mesh, rotated)));
    }

    protected override void UpdateUIFromSelectedItems() {
      _mesh = (ReinforcementMeshType)Enum.Parse(typeof(ReinforcementMeshType), _selectedItems[0]);
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
