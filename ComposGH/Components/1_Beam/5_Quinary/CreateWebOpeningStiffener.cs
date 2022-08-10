using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using Grasshopper.Kernel.Parameters;
using ComposAPI;

namespace ComposGH.Components
{
  public class WebOpeningStiffener : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("4e7a2c23-0504-46d2-8fe1-846bf4ef6a37");
    public WebOpeningStiffener()
      : base("Create" + WebOpeningStiffenersGoo.Name.Replace(" ", string.Empty),
          WebOpeningStiffenersGoo.Name.Replace(" ", string.Empty),
          "Create a " + WebOpeningStiffenersGoo.Description + " for a " + WebOpeningGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.Stiffener;
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
            "Type",
            "Unit"
    });
    private enum Stiff_types
    {
      Web_Opening,
      Notch,
    }
    private bool First = true;
    private Stiff_types OpeningType = Stiff_types.Web_Opening;
    private LengthUnit LengthUnit = Units.LengthUnitSection;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // type
        DropDownItems.Add(Enum.GetValues(typeof(Stiff_types)).Cast<Stiff_types>()
            .Select(x => x.ToString().Replace('_', ' ')).ToList());
        SelectedItems.Add(Stiff_types.Web_Opening.ToString().Replace('_', ' '));

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
      SelectedItems[i] = DropDownItems[i][j];

      if (i == 0)
      {
        if (SelectedItems[i] == OpeningType.ToString().Replace('_', ' '))
          return;
        OpeningType = (Stiff_types)Enum.Parse(typeof(Stiff_types), SelectedItems[i].Replace(' ', '_'));
        ModeChangeClicked();
      }
      else if (i == 1) // change is made to length unit
      {
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);
      }

        // update name of inputs (to display unit on sliders)
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      OpeningType = (Stiff_types)Enum.Parse(typeof(Stiff_types), SelectedItems[0].Replace(' ', '_'));
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[1]);

      CreateAttributes();
      ModeChangeClicked();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddBooleanParameter("Both Sides", "BS", "Set to true to apply horizontal stiffeners on both sides of web", GH_ParamAccess.item);
      pManager.AddGenericParameter("Dist. z [" + unitAbbreviation + "]", "Dz", "Vertical distance above/below opening edge to centre of stiffener (beam local z-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Top Width [" + unitAbbreviation + "]", "Wt", "Top Stiffener Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Top Thickness [" + unitAbbreviation + "]", "Tt", "Top Stiffener Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Bottom Width [" + unitAbbreviation + "]", "Wb", "Bottom Stiffener Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Bottom Thickness [" + unitAbbreviation + "]", "Tb", "Bottom Stiffener Thickness", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(WebOpeningStiffenersGoo.Name, WebOpeningStiffenersGoo.NickName, WebOpeningStiffenersGoo.Description + " for a " + WebOpeningGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      bool bothSides = false;
      DA.GetData(0, ref bothSides);
      Length start = GetInput.Length(this, DA, 1, LengthUnit);
      Length topWidth = GetInput.Length(this, DA, 2, LengthUnit);
      Length topTHK = GetInput.Length(this, DA, 3, LengthUnit);
      if (OpeningType == Stiff_types.Web_Opening)
      {
        Length bottomWidth = GetInput.Length(this, DA, 4, LengthUnit);
        Length bottomTHK = GetInput.Length(this, DA, 5, LengthUnit);
        DA.SetData(0, new WebOpeningStiffenersGoo(new WebOpeningStiffeners(
            start, topWidth, topTHK, bottomWidth, bottomTHK, bothSides)));
      }
      else
      {
        DA.SetData(0, new WebOpeningStiffenersGoo(new WebOpeningStiffeners(
            start, topWidth, topTHK, bothSides)));
      }
    }
    #region update input params
    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      if (OpeningType == Stiff_types.Web_Opening)
      {
        if (this.Params.Input.Count == 6)
          return;

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
      }
      if (OpeningType == Stiff_types.Notch)
      {
        if (this.Params.Input.Count == 4)
          return;

        Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
        Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
      }
    }
    #endregion

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
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      Params.Input[1].Name = "Dist. z [" + unitAbbreviation + "]";
      Params.Input[2].Name = "Top Width [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Top Thickness [" + unitAbbreviation + "]";

      if (OpeningType == Stiff_types.Web_Opening)
      {
        int i = 4;
        Params.Input[i].Name = "Bottom Width [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "Wb";
        Params.Input[i].Description = "Bottom Stiffener Width";
        Params.Input[i].Optional = false;
        i++;
        Params.Input[i].Name = "Bottom Thickness [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "Tb";
        Params.Input[i].Description = "Bottom Stiffener Thickness";
        Params.Input[i].Optional = false;
      }
    }
    #endregion
  }
}
