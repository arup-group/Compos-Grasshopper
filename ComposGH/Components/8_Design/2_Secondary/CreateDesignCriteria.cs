using System;

using ComposGH.Parameters;
using Grasshopper.Kernel;
using ComposAPI;
using System.Collections.Generic;
using UnitsNet.Units;
using UnitsNet;

namespace ComposGH.Components
{
  public class CreateDesignCriteria : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f99fbf92-b4a2-46d7-8b7a-1e3360ba9f00");
    public CreateDesignCriteria()
      : base("DesignCriteria", "Crt", "Create Compos Design Criteria for a Member",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    //protected override System.Drawing.Bitmap Icon => Properties.Resources.CreepShrinkageParams;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropdownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Optimise Option",
    });

    private bool First = true;
    private OptimiseOption OptOption = OptimiseOption.MinimumWeight;
    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // length
        DropdownItems.Add(new List<string>() { "Min. Weight", "Min. Height"});
        SelectedItems.Add(DropdownItems[0][0]);

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropdownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

      OptOption = (SelectedItems[0] == "Min. Weight" ? OptimiseOption.MinimumWeight : OptimiseOption.MinimumHeight);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      OptOption = (SelectedItems[0] == "Min. Weight" ? OptimiseOption.MinimumWeight : OptimiseOption.MinimumHeight);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Beam Size Limit", "BLm", "Compos Beam Size Limit.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Catalogue ID(s)", "CID", "Compos Section Catalogue IDs.", GH_ParamAccess.list);
      pManager.AddGenericParameter("Constr. Dead Load Lim.", "DLm", "Deflection Limit for Construction Dead Load.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Additional Load Lim.", "ALm", "Deflection Limit for Additional Dead Load.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Live Load Lim.", "LLm", "Deflection Limit for Final Live Load.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Load Lim.", "ALm", "Deflection Limit for Total Load.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Post Constr. Lim.", "PLm", "Deflection Limit for Post Construction Load (Total Load minus Construction Dead Load).", GH_ParamAccess.item);
      pManager.AddGenericParameter("Frequency Lim.", "fLm", "Frequncy Limit.", GH_ParamAccess.item);
      for (int i = 2; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Design Criteria", "Crt", "Compos Design Criteria for a Member", GH_ParamAccess.item);
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

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropdownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropdownItems, ref SelectedItems, ref SpacerDescriptions);

      UpdateUIFromSelectedItems();

      First = false;

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
      
    }
    #endregion
  }
}
