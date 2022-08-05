using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;
using UnitsNet.GH;

namespace ComposGH.Components
{
  public class BeamSectionProperties : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("93b27356-f92d-454f-b39d-5c7e2c607391");
    public BeamSectionProperties()
      : base(BeamSectionGoo.Name.Replace(" ", string.Empty)+ "Props",
          BeamSectionGoo.Name.Replace(" ", string.Empty),
          "Get " + BeamSectionGoo.Description + " Properties",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.ProfileProperties;
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
      "Unit"
    });

    private bool First = true;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // length
        DropdownItems.Add(Units.FilteredLengthUnits);
        SelectedItems.Add(LengthUnit.ToString());

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropdownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

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
      

      pManager.AddGenericParameter(BeamSectionGoo.Name, BeamSectionGoo.NickName, BeamSectionGoo.Description + " or an I Profile string description like 'CAT IPE IPE200', 'STD I(cm) 20. 19. 8.5 1.27' or 'STD GI 400 300 250 12 25 20'", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string unitAbbreviation = new Length(0, LengthUnit).ToString("a");
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
      BeamSection profile = new BeamSection(GetInput.BeamSection(this, DA, 0, false));
      int i = 0;
      DA.SetData(i++, new GH_UnitNumber(profile.Depth.ToUnit(LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.TopFlangeWidth.ToUnit(LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.BottomFlangeWidth.ToUnit(LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.WebThickness.ToUnit(LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.TopFlangeThickness.ToUnit(LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.BottomFlangeThickness.ToUnit(LengthUnit)));
      DA.SetData(i++, new GH_UnitNumber(profile.RootRadius.ToUnit(LengthUnit)));
      DA.SetData(i++, profile.isCatalogue);
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
      string unitAbbreviation = new Length(0, LengthUnit).ToString("a");
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
