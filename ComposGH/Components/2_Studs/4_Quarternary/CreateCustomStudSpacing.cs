using System;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnits;
using OasysUnits.Units;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateCustomStudSpacing : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("49328e6d-eebe-405c-b58c-060b8bdc1bef");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CustomStudSpacing;
    public CreateCustomStudSpacing()
      : base("Custom" + StudGroupSpacingGoo.Name.Replace(" ", string.Empty),
          StudGroupSpacingGoo.Name.Replace(" ", string.Empty),
          "Create a Custom " + StudGroupSpacingGoo.Description + " for a " + StudGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("Pos x [" + unitAbbreviation + "]", "Px", "Start Position where this Stud Spacing Groups begins on Beam (beam local x-axis)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Rows", "R", "Number of rows (across the top flange)", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Lines", "L", "Number of lines (along the length of the beam", GH_ParamAccess.item);
      pManager.AddGenericParameter("Spacing [" + unitAbbreviation + "]", "S", "Spacing of studs in this group (distance between each line)", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new StudGroupSpacingParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IQuantity start = Input.LengthOrRatio(this, DA, 0, this.LengthUnit);
      int rows = 1;
      DA.GetData(1, ref rows);
      int lines = 1;
      DA.GetData(2, ref lines);
      Length spacing = (Length)Input.UnitNumber(this, DA, 3, this.LengthUnit);

      Output.SetItem(this, DA, 0, new StudGroupSpacingGoo(new StudGroupSpacing(start, rows, lines, spacing)));
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] { "Unit" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // length
      this._dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this._selectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this._selectedItems[i] = this._dropDownItems[i][j];
      if (this.LengthUnit.ToString() == this._selectedItems[i])
        return;

      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this._selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this._selectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      Params.Input[0].Name = "Pos x [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Spacing [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
