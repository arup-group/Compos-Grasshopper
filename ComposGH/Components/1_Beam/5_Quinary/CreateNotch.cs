using System;
using System.Linq;
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
  public class CreateNotch : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("de802051-ae6a-4249-8699-7ea0cfe8c528");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.Notch;
    public CreateNotch()
      : base("BeamNotch", "Notch", "Create Beam Notch for a " + BeamGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Web Opening Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Web Opening Height", GH_ParamAccess.item);
      pManager.AddParameter(new WebOpeningStiffenersParam(), WebOpeningStiffenersGoo.Name + "(s)", WebOpeningStiffenersGoo.NickName, "(Optional) " + WebOpeningStiffenersGoo.Description, GH_ParamAccess.item);
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ComposWebOpeningParameter(), WebOpeningGoo.Name, WebOpeningGoo.NickName, "Notch " + WebOpeningGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length width = (Length)Input.UnitNumber(this, DA, 0, LengthUnit);
      Length height = (Length)Input.UnitNumber(this, DA, 1, LengthUnit);
      WebOpeningStiffenersGoo stiff = (WebOpeningStiffenersGoo)Input.GenericGoo<WebOpeningStiffenersGoo>(this, DA, 2);
      if (stiff != null)
      {
        if (stiff.Value.BottomStiffenerWidth != Length.Zero)
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "For Beam Notches only top stiffener(s) will be used.");
      }

      switch (OpeningType)
      {
        case NotchTypes.Start:
          Output.SetItem(this, DA, 0, new WebOpeningGoo(new WebOpening(width, height, NotchPosition.Start, stiff?.Value)));
          break;

        case NotchTypes.End:
          Output.SetItem(this, DA, 0, new WebOpeningGoo(new WebOpening(width, height, NotchPosition.End, stiff?.Value)));
          break;

        case NotchTypes.Both_ends:
          List<WebOpeningGoo> both = new List<WebOpeningGoo>();
          both.Add(new WebOpeningGoo(new WebOpening(width, height, NotchPosition.Start, (stiff == null) ? null : stiff.Value)));
          both.Add(new WebOpeningGoo(new WebOpening(width, height, NotchPosition.End, (stiff == null) ? null : stiff.Value)));
          Output.SetList(this, DA, 0, both);
          break;
      }
    }
    
    #region Custom UI
    private enum NotchTypes
    {
      Both_ends,
      Start,
      End
    }

    private NotchTypes OpeningType = NotchTypes.Both_ends;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Position", "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // type
      this.DropDownItems.Add(Enum.GetValues(typeof(NotchTypes)).Cast<NotchTypes>()
          .Select(x => x.ToString().Replace('_', ' ')).ToList());
      this.SelectedItems.Add(NotchTypes.Both_ends.ToString().Replace('_', ' '));

      // length
      this.DropDownItems.Add(FilteredUnits.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
      {
        if (this.SelectedItems[i] == this.OpeningType.ToString().Replace('_', ' '))
          return;
        this.OpeningType = (NotchTypes)Enum.Parse(typeof(NotchTypes), this.SelectedItems[i].Replace(' ', '_'));
      }
      else if (i == 1) // change is made to length unit
      {
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      }

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.OpeningType = (NotchTypes)Enum.Parse(typeof(NotchTypes), this.SelectedItems[0].Replace(' ', '_'));
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      if (this.OpeningType == NotchTypes.Both_ends)
        Params.Output[0].Access = GH_ParamAccess.list;
      else
        Params.Output[0].Access = GH_ParamAccess.item;

      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      Params.Input[0].Name = "Width [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Height [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
