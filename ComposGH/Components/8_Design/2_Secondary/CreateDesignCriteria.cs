using System;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components {
  public class CreateDesignCriteria : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f99fbf92-b4a2-46d7-8b7a-1e3360ba9f00");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.DesignCriteria;
    private OptimiseOption OptOption = OptimiseOption.MinimumWeight;

    public CreateDesignCriteria() : base("Create" + DesignCriteriaGoo.Name.Replace(" ", string.Empty),
      DesignCriteriaGoo.Name.Replace(" ", string.Empty),
      "Create a " + DesignCriteriaGoo.Description + " for a " + MemberGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat8()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      OptOption = _selectedItems[0] == "Min. Weight" ? OptimiseOption.MinimumWeight : OptimiseOption.MinimumHeight;

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Optimise Option" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(new List<string>() { "Min. Weight", "Min. Height" });
      _selectedItems.Add(_dropDownItems[0][0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new BeamSizeLimitsParam());
      pManager.AddIntegerParameter("Catalogue ID(s)", "CID", "Compos Section Catalogue IDs.", GH_ParamAccess.list);
      pManager.AddParameter(new DeflectionLimitParam(), "Constr. Dead Load Lim.", "DLm", DeflectionLimitGoo.Description + " for Construction Dead Load.", GH_ParamAccess.item);
      pManager.AddParameter(new DeflectionLimitParam(), "Additional Load Lim.", "ALm", DeflectionLimitGoo.Description + " for Additional Dead Load.", GH_ParamAccess.item);
      pManager.AddParameter(new DeflectionLimitParam(), "Live Load Lim.", "LLm", DeflectionLimitGoo.Description + " for Final Live Load.", GH_ParamAccess.item);
      pManager.AddParameter(new DeflectionLimitParam(), "Total Load Lim.", "ALm", DeflectionLimitGoo.Description + " for Total Load.", GH_ParamAccess.item);
      pManager.AddParameter(new DeflectionLimitParam(), "Post Constr. Lim.", "PLm", DeflectionLimitGoo.Description + " for Post Construction Load (Total Load minus Construction Dead Load).", GH_ParamAccess.item);
      pManager.AddParameter(new FrequencyLimitsParam());
      for (int i = 2; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new DesignCriteriaParam());
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      var designCriteria = new DesignCriteria {
        OptimiseOption = OptOption
      };

      int i = 0;
      // 0 beam size limits
      var beamGoo = (BeamSizeLimitsGoo)Input.GenericGoo<BeamSizeLimitsGoo>(this, DA, i++);
      designCriteria.BeamSizeLimits = beamGoo.Value;

      // 1 catalogues
      var cats = new List<int>();
      DA.GetDataList(i++, cats);
      try {
        designCriteria.CatalogueSectionTypes = cats;
      } catch (Exception e) {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
        return;
      }

      // 2 Constr limits
      if (Params.Input[i].Sources.Count > 0) {
        var dlGoo = (DeflectionLimitGoo)Input.GenericGoo<DeflectionLimitGoo>(this, DA, i++);
        designCriteria.ConstructionDeadLoad = dlGoo.Value;
      }

      // 3 Add limits
      if (Params.Input[i].Sources.Count > 0) {
        var dlGoo = (DeflectionLimitGoo)Input.GenericGoo<DeflectionLimitGoo>(this, DA, i++);
        designCriteria.AdditionalDeadLoad = dlGoo.Value;
      }

      // 4 final limits
      if (Params.Input[i].Sources.Count > 0) {
        var dlGoo = (DeflectionLimitGoo)Input.GenericGoo<DeflectionLimitGoo>(this, DA, i++);
        designCriteria.FinalLiveLoad = dlGoo.Value;
      }

      // 5 total limits
      if (Params.Input[i].Sources.Count > 0) {
        var dlGoo = (DeflectionLimitGoo)Input.GenericGoo<DeflectionLimitGoo>(this, DA, i++);
        designCriteria.TotalLoads = dlGoo.Value;
      }

      // 6 post limits
      if (Params.Input[i].Sources.Count > 0) {
        var dlGoo = (DeflectionLimitGoo)Input.GenericGoo<DeflectionLimitGoo>(this, DA, i++);
        designCriteria.PostConstruction = dlGoo.Value;
      }

      // 7 freq limits
      if (Params.Input[i].Sources.Count > 0) {
        var dlGoo = (FrequencyLimitsGoo)Input.GenericGoo<FrequencyLimitsGoo>(this, DA, i++);
        designCriteria.FrequencyLimits = dlGoo.Value;
      }

      DA.SetData(0, new DesignCriteriaGoo(designCriteria));
    }

    protected override void UpdateUIFromSelectedItems() {
      OptOption = _selectedItems[0] == "Min. Weight" ? OptimiseOption.MinimumWeight : OptimiseOption.MinimumHeight;
      base.UpdateUIFromSelectedItems();
    }
  }
}
