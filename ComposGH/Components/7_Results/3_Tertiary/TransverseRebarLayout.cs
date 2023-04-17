using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComposGH.Components {
  public class TransverseRebarLayout : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("4ecc12be-e364-40a1-95ec-9a61c7099334");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.TransverseRebarLayout;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public TransverseRebarLayout()
          : base("Transverse Rebar Layout",
      "RebarResults",
      "Get transverse rebar layout automatically designed for a " + MemberGoo.Description,
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat7()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new ComposMemberParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Start Position", "Sta", "Rebar group start position, measured from beam start", GH_ParamAccess.list);
      pManager.AddGenericParameter("End Position", "End", "Rebar group end position, measured from beam start", GH_ParamAccess.list);
      pManager.AddGenericParameter("Diameter", "Dia", "Rebar diameter in group", GH_ParamAccess.list);
      pManager.AddGenericParameter("Spacing", "Spa", "Rebar spacing in group", GH_ParamAccess.list);
      pManager.AddGenericParameter("Cover", "Cov", "Rebar cover in group", GH_ParamAccess.list);
      pManager.AddGenericParameter("Area", "Area", "Rebar area per meter in group", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      ITransverseRebarResult result = res.TransverseRebarResults;

      int i = 0;
      Output.SetList(this, DA, i++,
        result.StartPosition.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.EndPosition.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.Diameter.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.Spacing.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.Cover.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList());
      Output.SetList(this, DA, i,
        result.Area.Select(x => new GH_UnitNumber(x.ToUnit(UnitsHelper.GetAreaUnit(LengthUnit)))).ToList());
    }

    protected override void UpdateUIFromSelectedItems() {
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
