using System;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Helpers;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components
{
  public class BeamSectionProperties : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("93b27356-f92d-454f-b39d-5c7e2c607391");

    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.ProfileProperties;
    public BeamSectionProperties()
      : base(BeamSectionGoo.Name.Replace(" ", string.Empty)+ "Props",
          BeamSectionGoo.Name.Replace(" ", string.Empty),
          "Get " + BeamSectionGoo.Description + " Properties",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(BeamSectionGoo.Name, BeamSectionGoo.NickName, BeamSectionGoo.Description + " parameter or a text string in the format of either 'CAT IPE IPE200', 'STD I(cm) 20. 19. 8.5 1.27' or 'STD GI 400 300 250 12 25 20'", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      pManager.AddGenericParameter("Depth [" + unitAbbreviation + "]", "D", "Profile Depth", GH_ParamAccess.item);
      pManager.AddGenericParameter("TopWidth [" + unitAbbreviation + "]", "Wt", "Profile's Top Flange Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("BotWidth [" + unitAbbreviation + "]", "Wb", "Profile's Bottom Flange Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("WebTHK [" + unitAbbreviation + "]", "t", "Profile's Web thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("TopFlngTHK [" + unitAbbreviation + "]", "Tt", "Profile's Top Flange thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("BottomFlngTHK [" + unitAbbreviation + "]", "Tb", "Profile's Top Flange thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("RootRadius [" + unitAbbreviation + "]", "r", "Profile's Root radius thickness", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Catalogue", "Cat", "True if profile is Catalogue", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      BeamSection profile = new BeamSection(Input.BeamSection(this, DA, 0, false));

      int i = 0;
      DA.SetData(i++, new GH_UnitNumber(profile.Depth.ToUnit(this.LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.TopFlangeWidth.ToUnit(this.LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.BottomFlangeWidth.ToUnit(this.LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.WebThickness.ToUnit(this.LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.TopFlangeThickness.ToUnit(this.LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.BottomFlangeThickness.ToUnit(this.LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.RootRadius.ToUnit(this.LengthUnit)));
      DA.SetData(i++, profile.isCatalogue);
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    private int ProfileSerialized = 0;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      int i = 0;
      Params.Output[i++].Name = "Depth [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "TopWidth [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "BotWidth [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "WebTHK [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "TopFlngTHK [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "BottomFlngTHK [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "RootRadius [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
