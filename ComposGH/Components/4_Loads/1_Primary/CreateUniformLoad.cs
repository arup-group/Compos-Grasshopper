﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ComposGH.Components {
  public class CreateUniformLoad : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("5dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.UniformLoad;
    private LoadDistribution DistributionType = LoadDistribution.Area;

    private PressureUnit ForcePerAreaUnit = DefaultUnits.ForcePerAreaUnit;

    private ForcePerLengthUnit ForcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;

    public CreateUniformLoad() : base("CreateUniformLoad", "UniformLoad", "Create a uniformly distributed Compos Load.",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat4()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), _selectedItems[i]);
        if (DistributionType == LoadDistribution.Line) {
          _dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength);
          _selectedItems[1] = ForcePerLength.GetAbbreviation(ForcePerLengthUnit);
        } else {
          _dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerArea);
          _selectedItems[1] = Pressure.GetAbbreviation(ForcePerAreaUnit);
        }
      }
      if (i == 1) {
        if (DistributionType == LoadDistribution.Line) {
          ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[i]);
        } else {
          ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[i]);
        }
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation;
      if (DistributionType == LoadDistribution.Line) {
        unitAbbreviation = ForcePerLength.GetAbbreviation(ForcePerLengthUnit);
      } else {
        unitAbbreviation = Pressure.GetAbbreviation(ForcePerAreaUnit);
      }
      int i = 0;
      Params.Input[i++].Name = "Const. Dead [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Distribution", "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // type
      _dropDownItems.Add(Enum.GetValues(typeof(LoadDistribution)).Cast<LoadDistribution>().Select(x => x.ToString()).ToList());
      _selectedItems.Add(LoadDistribution.Area.ToString());

      // force unit
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerArea));
      _selectedItems.Add(Pressure.GetAbbreviation(ForcePerAreaUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Pressure.GetAbbreviation(ForcePerAreaUnit);
      pManager.AddGenericParameter("Const. Dead [" + unitAbbreviation + "]", "dl", "Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live [" + unitAbbreviation + "]", "ll", "Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead [" + unitAbbreviation + "]", "DL", "Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live [" + unitAbbreviation + "]", "LL", "Final Live Load", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ComposLoadParameter());
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      switch (DistributionType) {
        case LoadDistribution.Line:
          var constDeadL = (ForcePerLength)Input.UnitNumber(this, DA, 0, ForcePerLengthUnit);
          var constLiveL = (ForcePerLength)Input.UnitNumber(this, DA, 1, ForcePerLengthUnit);
          var finalDeadL = (ForcePerLength)Input.UnitNumber(this, DA, 2, ForcePerLengthUnit);
          var finalLiveL = (ForcePerLength)Input.UnitNumber(this, DA, 3, ForcePerLengthUnit);
          Load loadL = new UniformLoad(constDeadL, constLiveL, finalDeadL, finalLiveL);
          DA.SetData(0, new LoadGoo(loadL));
          break;

        case LoadDistribution.Area:
          var constDeadA = (Pressure)Input.UnitNumber(this, DA, 0, ForcePerAreaUnit);
          var constLiveA = (Pressure)Input.UnitNumber(this, DA, 1, ForcePerAreaUnit);
          var finalDeadA = (Pressure)Input.UnitNumber(this, DA, 2, ForcePerAreaUnit);
          var finalLiveA = (Pressure)Input.UnitNumber(this, DA, 3, ForcePerAreaUnit);
          Load loadA = new UniformLoad(constDeadA, constLiveA, finalDeadA, finalLiveA);
          DA.SetData(0, new LoadGoo(loadA));
          break;
      }
    }

    protected override void UpdateUIFromSelectedItems() {
      DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), _selectedItems[0]);
      if (DistributionType == LoadDistribution.Line) {
        ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[1]);
      } else {
        ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      }

      base.UpdateUIFromSelectedItems();
    }
  }
}
