using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateBeamSection : GH_OasysComponent, IGH_VariableParameterComponent
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

    protected override System.Drawing.Bitmap Icon => Properties.Resources.BeamSection;
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
      "Unit"
    });

    private bool First = true;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

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
      SelectedItems[i] = DropDownItems[i][j];

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
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter(BeamSectionGoo.Name, BeamSectionGoo.NickName, BeamSectionGoo.Description + " or an I Profile string description like 'CAT IPE IPE200', 'STD I(cm) 20. 19. 8.5 1.27' or 'STD GI 400 300 250 12 25 20'", GH_ParamAccess.item);
      pManager.AddGenericParameter("Start [" + unitAbbreviation + "]", "Px", "(Optional) Start Position of this profile (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Taper Next", "Tp", "Taper to next (default = false)", GH_ParamAccess.item, false);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(BeamSectionGoo.Name, BeamSectionGoo.NickName, BeamSectionGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      string profile = GetInput.BeamSection(this, DA, 0, false);

      IQuantity start = new Ratio(0, RatioUnit.Percent);
      if (this.Params.Input[1].Sources.Count > 0)
       start = GetInput.LengthOrRatio(this, DA, 1, LengthUnit);

      bool taper = false;
      if (DA.GetData(2, ref taper))
      {
        if (taper & profile.StartsWith("CAT"))
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Catalogue profiles cannot taper - use a custom welded section instead");
      }

      BeamSection beamSection = new BeamSection(profile);
      beamSection.StartPosition = start;
      beamSection.TaperedToNext = taper;
      DA.SetData(0, new BeamSectionGoo(beamSection));
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
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      Params.Input[1].Name = "Start [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
