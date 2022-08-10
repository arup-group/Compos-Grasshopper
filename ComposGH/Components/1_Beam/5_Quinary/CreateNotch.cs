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
  public class CreateNotch : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("de802051-ae6a-4249-8699-7ea0cfe8c528");
    public CreateNotch()
      : base("BeamNotch", "Notch", "Create Beam Notch for a " + BeamGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quinary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.Notch;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Web Opening Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Web Opening Height", GH_ParamAccess.item);
      pManager.AddGenericParameter(WebOpeningStiffenersGoo.Name + "(s)", WebOpeningStiffenersGoo.NickName, "(Optional) " + WebOpeningStiffenersGoo.Description, GH_ParamAccess.item);
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(WebOpeningGoo.Name, WebOpeningGoo.NickName, "Notch " + WebOpeningGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length width = GetInput.Length(this, DA, 0, LengthUnit);
      Length height = GetInput.Length(this, DA, 1, LengthUnit);
      WebOpeningStiffenersGoo stiff = (WebOpeningStiffenersGoo)GetInput.GenericGoo<WebOpeningStiffenersGoo>(this, DA, 2);
      if (stiff != null)
      {
        if (stiff.Value.BottomStiffenerWidth != Length.Zero)
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "For Beam Notches only top stiffener(s) will be used.");
      }

      switch (OpeningType)
      {
        case notch_types.Start:
          DA.SetData(0, new WebOpeningGoo(new WebOpening(width, height, NotchPosition.Start, (stiff == null) ? null : stiff.Value)));
          break;

        case notch_types.End:
          DA.SetData(0, new WebOpeningGoo(new WebOpening(width, height, NotchPosition.End, (stiff == null) ? null : stiff.Value)));
          break;

        case notch_types.Both_ends:
          List<WebOpeningGoo> both = new List<WebOpeningGoo>();
          both.Add(new WebOpeningGoo(new WebOpening(width, height, NotchPosition.Start, (stiff == null) ? null : stiff.Value)));
          both.Add(new WebOpeningGoo(new WebOpening(width, height, NotchPosition.End, (stiff == null) ? null : stiff.Value)));
          DA.SetDataList(0, both);
          break;
      }
    }
    
    #region Custom UI
    private enum notch_types
    {
      Both_ends,
      Start,
      End
    }

    private notch_types OpeningType = notch_types.Both_ends;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    internal override void InitialiseDropdowns()
    {
      SpacerDescriptions = new List<string>(new string[]
        {
          "Position",
          "Unit"
        });

      DropdownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // type
      DropdownItems.Add(Enum.GetValues(typeof(notch_types)).Cast<notch_types>()
          .Select(x => x.ToString().Replace('_', ' ')).ToList());
      SelectedItems.Add(notch_types.Both_ends.ToString().Replace('_', ' '));

      // length
      DropdownItems.Add(Units.FilteredLengthUnits);
      SelectedItems.Add(LengthUnit.ToString());

      IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

      if (i == 0)
      {
        if (SelectedItems[i] == OpeningType.ToString().Replace('_', ' '))
          return;
        OpeningType = (notch_types)Enum.Parse(typeof(notch_types), SelectedItems[i].Replace(' ', '_'));
      }
      else if (i == 1) // change is made to length unit
      {
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);
      }

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      OpeningType = (notch_types)Enum.Parse(typeof(notch_types), SelectedItems[0].Replace(' ', '_'));
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      if (OpeningType == notch_types.Both_ends)
        Params.Output[0].Access = GH_ParamAccess.list;
      else
        Params.Output[0].Access = GH_ParamAccess.item;

      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      Params.Input[0].Name = "Width [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Height [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
