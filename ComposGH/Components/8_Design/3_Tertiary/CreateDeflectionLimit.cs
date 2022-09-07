using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using UnitsNet.Units;
using UnitsNet;

namespace ComposGH.Components
{
  public class CreateDeflectionLimit : GH_OasysDropDownComponent
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

    protected override System.Drawing.Bitmap Icon => Resources.DeflectionLimit;
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
        deflectionLimit.AbsoluteDeflection = GetInput.Length(this, DA, 0, this.LengthUnit, true);

      if (this.Params.Input[1].Sources.Count > 0)
        deflectionLimit.SpanOverDeflectionRatio = GetInput.Ratio(this, DA, 1, RatioUnit.DecimalFraction);

      SetOutput.Item(this, DA, 0, new DeflectionLimitGoo(deflectionLimit));
    }

    #region Custom UI
    private LengthUnit LengthUnit = Units.LengthUnitResult;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (this.LengthUnit.ToString() == this.SelectedItems[i])
        return;

      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbb = Length.GetAbbreviation(this.LengthUnit);
      Params.Input[0].Name = "Absolute Deflection [" + unitAbb + "]";
    }
    #endregion
  }
}
