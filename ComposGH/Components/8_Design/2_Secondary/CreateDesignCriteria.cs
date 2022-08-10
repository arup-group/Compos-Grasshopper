using System;

using ComposGH.Parameters;
using Grasshopper.Kernel;
using ComposAPI;
using System.Collections.Generic;
using UnitsNet.Units;
using UnitsNet;

namespace ComposGH.Components
{
  public class CreateDesignCriteria : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f99fbf92-b4a2-46d7-8b7a-1e3360ba9f00");
    public CreateDesignCriteria()
      : base("Create" + DesignCriteriaGoo.Name.Replace(" ", string.Empty),
          DesignCriteriaGoo.Name.Replace(" ", string.Empty),
          "Create a " + DesignCriteriaGoo.Description + " for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.DesignCriteria;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(BeamSizeLimitsGoo.Name, BeamSizeLimitsGoo.NickName, BeamSizeLimitsGoo.Description, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Catalogue ID(s)", "CID", "Compos Section Catalogue IDs.", GH_ParamAccess.list);
      pManager.AddGenericParameter("Constr. Dead Load Lim.", "DLm", DeflectionLimitGoo.Description + " for Construction Dead Load.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Additional Load Lim.", "ALm", DeflectionLimitGoo.Description + " for Additional Dead Load.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Live Load Lim.", "LLm", DeflectionLimitGoo.Description + " for Final Live Load.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Load Lim.", "ALm", DeflectionLimitGoo.Description + " for Total Load.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Post Constr. Lim.", "PLm", DeflectionLimitGoo.Description + " for Post Construction Load (Total Load minus Construction Dead Load).", GH_ParamAccess.item);
      pManager.AddGenericParameter(FrequencyLimitsGoo.Name, FrequencyLimitsGoo.NickName, FrequencyLimitsGoo.Description, GH_ParamAccess.item);
      for (int i = 2; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(DesignCriteriaGoo.Name, DesignCriteriaGoo.NickName, DesignCriteriaGoo.Description + " for a " + MemberGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      DesignCriteria designCriteria = new DesignCriteria();
      designCriteria.OptimiseOption = OptOption;
      
      int i = 0;
      // 0 beam size limits
      BeamSizeLimitsGoo beamGoo = (BeamSizeLimitsGoo)GetInput.GenericGoo<BeamSizeLimitsGoo>(this, DA, i++);
      designCriteria.BeamSizeLimits = beamGoo.Value;
      
      // 1 catalogues
      List<int> cats = new List<int>();
      DA.GetDataList(i++, cats);
      try
      {
        designCriteria.CatalogueSectionTypes = cats;
      }
      catch (Exception e)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
        return;
      }

      // 2 Constr limits
      if (this.Params.Input[i].Sources.Count > 0)
      {
        DeflectionLimitGoo dlGoo = (DeflectionLimitGoo)GetInput.GenericGoo<DeflectionLimitGoo>(this, DA, i++);
        designCriteria.ConstructionDeadLoad = dlGoo.Value;
      }

      // 3 Add limits
      if (this.Params.Input[i].Sources.Count > 0)
      {
        DeflectionLimitGoo dlGoo = (DeflectionLimitGoo)GetInput.GenericGoo<DeflectionLimitGoo>(this, DA, i++);
        designCriteria.AdditionalDeadLoad = dlGoo.Value;
      }

      // 4 final limits
      if (this.Params.Input[i].Sources.Count > 0)
      {
        DeflectionLimitGoo dlGoo = (DeflectionLimitGoo)GetInput.GenericGoo<DeflectionLimitGoo>(this, DA, i++);
        designCriteria.FinalLiveLoad = dlGoo.Value;
      }

      // 5 total limits
      if (this.Params.Input[i].Sources.Count > 0)
      {
        DeflectionLimitGoo dlGoo = (DeflectionLimitGoo)GetInput.GenericGoo<DeflectionLimitGoo>(this, DA, i++);
        designCriteria.TotalLoads = dlGoo.Value;
      }

      // 6 post limits
      if (this.Params.Input[i].Sources.Count > 0)
      {
        DeflectionLimitGoo dlGoo = (DeflectionLimitGoo)GetInput.GenericGoo<DeflectionLimitGoo>(this, DA, i++);
        designCriteria.PostConstruction = dlGoo.Value;
      }

      // 7 freq limits
      if (this.Params.Input[i].Sources.Count > 0)
      {
        FrequencyLimitsGoo dlGoo = (FrequencyLimitsGoo)GetInput.GenericGoo<FrequencyLimitsGoo>(this, DA, i++);
        designCriteria.FrequencyLimits = dlGoo.Value;
      }

      DA.SetData(0, new DesignCriteriaGoo(designCriteria));
    }


    #region Custom UI
    private OptimiseOption OptOption = OptimiseOption.MinimumWeight;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Optimise Option" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      DropDownItems.Add(new List<string>() { "Min. Weight", "Min. Height" });
      SelectedItems.Add(DropDownItems[0][0]);

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      this.OptOption = this.SelectedItems[0] == "Min. Weight" ? OptimiseOption.MinimumWeight : OptimiseOption.MinimumHeight;

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.OptOption = this.SelectedItems[0] == "Min. Weight" ? OptimiseOption.MinimumWeight : OptimiseOption.MinimumHeight;

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
