﻿using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components {
  public class CreateStandardStudDimensions : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c97c7e52-7aa3-438f-900a-33f6ca889b3c");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardStudDims;
    private StandardStudSize StdSize = StandardStudSize.D19mmH100mm;

    public CreateStandardStudDimensions() : base("Standard" + StudDimensionsGoo.Name.Replace(" ", string.Empty),
      "StudDimsStandard",
      "Look up a Standard " + StudDimensionsGoo.Description + " for a " + StudGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat2()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) // change is made to size
      {
        string sz = _selectedItems[i].Replace("Ø", "D").Replace("/", "mmH");
        StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
      }

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Standard Size" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // spacing
      _dropDownItems.Add(Enum.GetValues(typeof(StandardStudSize)).Cast<StandardStudSize>()
          .Select(x => x.ToString()).ToList());
      for (int i = 0; i < _dropDownItems[0].Count; i++) {
        _dropDownItems[0][i] = _dropDownItems[0][i].Replace("D", "Ø").Replace("mmH", "/");
      }

      _selectedItems.Add(StdSize.ToString().Replace("D", "Ø").Replace("mmH", "/"));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) { }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new StudDimensionsParam());
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      DA.SetData(0, new StudDimensionsGoo(new StudDimensions(StdSize)));
    }

    protected override void UpdateUIFromSelectedItems() {
      string sz = _selectedItems[0].Replace("Ø", "D").Replace("/", "mmH");
      StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);

      base.UpdateUIFromSelectedItems();
    }
  }
}
