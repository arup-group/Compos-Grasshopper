using System;

using ComposGH.Parameters;
using Grasshopper.Kernel;
using ComposAPI;
using System.Collections.Generic;
using UnitsNet.Units;
using UnitsNet;

namespace ComposGH.Components
{
  public class CreateDeflectionLimit : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("02b54c18-a142-4e9c-a2ad-715a71c962f7");
    public CreateDeflectionLimit()
      : base("Create" + DeflectionLimitGoo.Name.Replace(" ", string.Empty),
          DeflectionLimitGoo.Name.Replace(" ", string.Empty),
          "Create a " + DeflectionLimitGoo.Description + " for a " + DesignCriteriaGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.DeflectionLimit;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropDownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Unit",
    });

    private bool First = true;
    private LengthUnit LengthUnit = Units.LengthUnitResult;
    public override void CreateAttributes()
    {
      if (First)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // length
        DropDownItems.Add(Units.FilteredLengthUnits);
        SelectedItems.Add(LengthUnit.ToString());

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[0]);

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
      string unitAbb = Length.GetAbbreviation(this.LengthUnit);
      pManager.AddGenericParameter("Absolute Deflection [" + unitAbb + "]", "Abs", "Absolute Deflection", GH_ParamAccess.item);
      pManager.AddGenericParameter("Span/Deflection Ratio [L/x]", "L/δ", "Span over Deflection ratio, for instance input '500' for 'L/500'", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(DeflectionLimitGoo.Name, DeflectionLimitGoo.NickName, DeflectionLimitGoo.Description + " for a " + DesignCriteriaGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      if (this.Params.Input[0].Sources.Count == 0 &
        this.Params.Input[1].Sources.Count == 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Inputs failed to collect data.");
        return;
      }

      DeflectionLimit deflectionLimit = new DeflectionLimit();

      if (this.Params.Input[0].Sources.Count > 0)
        deflectionLimit.AbsoluteDeflection = GetInput.Length(this, DA, 0, LengthUnit, true);

      if (this.Params.Input[1].Sources.Count > 0)
        deflectionLimit.SpanOverDeflectionRatio = GetInput.Ratio(this, DA, 1, RatioUnit.DecimalFraction);

      DA.SetData(0, new DeflectionLimitGoo(deflectionLimit));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropDownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropDownItems, ref SelectedItems, ref SpacerDescriptions);

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
      string unitAbb = Length.GetAbbreviation(this.LengthUnit);
      Params.Input[0].Name = "Absolute Deflection [" + unitAbb + "]";
    }
    #endregion
  }
}
