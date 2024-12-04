using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using ComposAPI.Helpers;

namespace ComposGH.Components {
  public class CreateNotch : GH_OasysDropDownComponent {
    private enum NotchTypes {
      Both_ends,
      Start,
      End
    }

    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("de802051-ae6a-4249-8699-7ea0cfe8c528");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.Notch;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    private NotchTypes OpeningType = NotchTypes.Both_ends;

    public CreateNotch() : base("BeamNotch", "Notch", "Create Beam Notch for a " + BeamGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        if (_selectedItems[i] == OpeningType.ToString().Replace('_', ' ')) {
          return;
        }
        OpeningType = (NotchTypes)Enum.Parse(typeof(NotchTypes), _selectedItems[i].Replace(' ', '_'));
      }
      else if (i == 1) // change is made to length unit
        {
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      if (OpeningType == NotchTypes.Both_ends) {
        Params.Output[0].Access = GH_ParamAccess.list;
      }
      else {
        Params.Output[0].Access = GH_ParamAccess.item;
      }

      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      Params.Input[0].Name = "Width [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Height [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Position", "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // type
      _dropDownItems.Add(Enum.GetValues(typeof(NotchTypes)).Cast<NotchTypes>()
          .Select(x => x.ToString().Replace('_', ' ')).ToList());
      _selectedItems.Add(NotchTypes.Both_ends.ToString().Replace('_', ' '));

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Web Opening Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Web Opening Height", GH_ParamAccess.item);
      pManager.AddParameter(new WebOpeningStiffenersParam(), WebOpeningStiffenersGoo.Name + "(s)", WebOpeningStiffenersGoo.NickName, "(Optional) " + WebOpeningStiffenersGoo.Description, GH_ParamAccess.item);
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ComposWebOpeningParameter(), WebOpeningGoo.Name, WebOpeningGoo.NickName, "Notch " + WebOpeningGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      var width = (Length)Input.UnitNumber(this, DA, 0, LengthUnit);
      var height = (Length)Input.UnitNumber(this, DA, 1, LengthUnit);
      var stiff = (WebOpeningStiffenersGoo)Input.GenericGoo<WebOpeningStiffenersGoo>(this, DA, 2);
      if (stiff != null) {
        if (!ComposUnitsHelper.IsEqual(stiff.Value.BottomStiffenerWidth, Length.Zero)) {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "For Beam Notches only top stiffener(s) will be used.");
        }
      }

      switch (OpeningType) {
        case NotchTypes.Start:
          DA.SetData(0, new WebOpeningGoo(new WebOpening(width, height, NotchPosition.Start, stiff?.Value)));
          break;

        case NotchTypes.End:
          DA.SetData(0, new WebOpeningGoo(new WebOpening(width, height, NotchPosition.End, stiff?.Value)));
          break;

        case NotchTypes.Both_ends:
          var both = new List<WebOpeningGoo> {
            new WebOpeningGoo(new WebOpening(width, height, NotchPosition.Start, stiff?.Value)),
            new WebOpeningGoo(new WebOpening(width, height, NotchPosition.End, stiff?.Value))
          };
          DA.SetDataList(0, both);
          break;
      }
    }

    protected override void UpdateUIFromSelectedItems() {
      OpeningType = (NotchTypes)Enum.Parse(typeof(NotchTypes), _selectedItems[0].Replace(' ', '_'));
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
