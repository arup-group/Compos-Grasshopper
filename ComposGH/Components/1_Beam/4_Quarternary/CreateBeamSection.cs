using System;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH.Components;
using OasysGH.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Components
{
  public class CreateBeamSection : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("de792051-ae6a-4249-8699-7ea0cfe8c528");
    public CreateBeamSection()
      : base("Create" + BeamSectionGoo.Name.Replace(" ", string.Empty),
          BeamSectionGoo.Name.Replace(" ", string.Empty),
          "Create a " + BeamSectionGoo.Description + " for a " + BeamGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Resources.BeamSection;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter(BeamSectionGoo.Name, BeamSectionGoo.NickName, BeamSectionGoo.Description + " parameter or a text string in the format of either 'CAT IPE IPE200', 'STD I(cm) 20. 19. 8.5 1.27' or 'STD GI 400 300 250 12 25 20'", GH_ParamAccess.item);
      pManager.AddGenericParameter("Start [" + unitAbbreviation + "]", "Px", "(Optional) Start Position of this profile (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Taper Next", "Tp", "Taper to next (default = false)", GH_ParamAccess.item, false);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new BeamSectionParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      string profile = Helpers.Input.BeamSection(this, DA, 0, false);

      IQuantity start = new Ratio(0, RatioUnit.Percent);
      if (this.Params.Input[1].Sources.Count > 0)
       start = Input.LengthOrRatio(this, DA, 1, this.LengthUnit);

      bool taper = false;
      if (DA.GetData(2, ref taper))
      {
        if (taper & profile.StartsWith("CAT"))
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Catalogue profiles cannot taper - use a custom welded section instead");
      }

      BeamSection beamSection = new BeamSection(profile);
      beamSection.StartPosition = start;
      beamSection.TaperedToNext = taper;
      Output.SetItem(this, DA, 0, new BeamSectionGoo(beamSection));
    }

    #region Custom UI
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      Params.Input[1].Name = "Start [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
