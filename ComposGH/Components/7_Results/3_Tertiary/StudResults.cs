﻿using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components {
  public class StudResults : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("75fbd8a9-eb55-443e-88c8-353307c96097");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StudResults;
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;

    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public StudResults() : base("Stud Results",
      "StudResults",
      "Get stud results for a " + MemberGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat7()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[i]);
      } else if (i == 1) {
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      }

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Force Unit", "Length Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // force
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _selectedItems.Add(Force.GetAbbreviation(ForceUnit));

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new ComposMemberParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Capacity", "Rd", "Actual stud capacity, as [number of studs] x [single stud capacity]. Values given at each position", GH_ParamAccess.list);
      pManager.AddIntegerParameter("StartCount", "Sp", "Actual number of studs provided from start. Values given at each position", GH_ParamAccess.list);
      pManager.AddIntegerParameter("EndCount", "Ep", "Actual number of studs provided from end. Values given at each position", GH_ParamAccess.list);
      pManager.AddIntegerParameter("StartRequired", "Sr", "Required number of studs provided from start. Values given at each position", GH_ParamAccess.list);
      pManager.AddIntegerParameter("EndRequired", "Er", "Required number of studs provided from end. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Req.Full IA", "RFI", "Required stud capacity for 100% shear interaction. Values given at each position", GH_ParamAccess.list);
      pManager.AddNumberParameter("Req. Interaction", "RIA", "Required shear interaction for given moment. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Single Capacity", "1Rd", "Capacity of a single stud", GH_ParamAccess.item);
      pManager.AddNumberParameter("Interaction", "IA", "Actual shear interaction for given moment. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Stud Capacity Start", "SRd", "Actual shear capacity from start. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Stud Capacity Start", "ERd", "Actual shear capacity from end. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      var positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
      IStudResult result = res.StudResults;

      int i = 0;
      DA.SetDataList(i++,
        result.StudCapacity.Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList());

      DA.SetDataList(i++,
        result.NumberOfStudsStart.Select(x => new GH_Integer(x)).ToList());

      DA.SetDataList(i++,
        result.NumberOfStudsEnd.Select(x => new GH_Integer(x)).ToList());

      DA.SetDataList(i++,
        result.NumberOfStudsRequiredStart.Select(x => new GH_Integer(x)).ToList());

      DA.SetDataList(i++,
        result.NumberOfStudsRequiredEnd.Select(x => new GH_Integer(x)).ToList());

      DA.SetDataList(i++,
        result.StudCapacityRequiredForFullShearInteraction.Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList());

      DA.SetDataList(i++,
        result.ShearInteractionRequired.Select(x => new GH_UnitNumber(x.ToUnit(RatioUnit.DecimalFraction))).ToList());

      DA.SetData(i++, new GH_UnitNumber(
        result.SingleStudCapacity.ToUnit(ForceUnit)));

      DA.SetDataList(i++,
        result.ShearInteraction.Select(x => new GH_UnitNumber(x.ToUnit(RatioUnit.DecimalFraction))).ToList());

      DA.SetDataList(i++,
        result.StudCapacityStart.Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList());

      DA.SetDataList(i++,
        result.StudCapacityEnd.Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList());

      DA.SetDataList(i, positions);
    }

    protected override void UpdateUIFromSelectedItems() {
      ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[0]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
