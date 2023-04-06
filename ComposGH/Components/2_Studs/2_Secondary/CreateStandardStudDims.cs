using System;
using System.Linq;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateStandardStudDimensions : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c97c7e52-7aa3-438f-900a-33f6ca889b3c");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardStudDims;
    public CreateStandardStudDimensions()
      : base("Standard" + StudDimensionsGoo.Name.Replace(" ", string.Empty),
          "StudDimsStandard",
          "Look up a Standard " + StudDimensionsGoo.Description + " for a " + StudGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    { }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new StudDimensionsParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Output.SetItem(this, DA, 0, new StudDimensionsGoo(new StudDimensions(this.StdSize)));
    }

    #region Custom UI
    private StandardStudSize StdSize = StandardStudSize.D19mmH100mm;

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] { "Standard Size" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // spacing
      this._dropDownItems.Add(Enum.GetValues(typeof(StandardStudSize)).Cast<StandardStudSize>()
          .Select(x => x.ToString()).ToList());
      for (int i = 0; i < _dropDownItems[0].Count; i++)
        this._dropDownItems[0][i] = _dropDownItems[0][i].Replace("D", "Ø").Replace("mmH", "/");

      this._selectedItems.Add(this.StdSize.ToString().Replace("D", "Ø").Replace("mmH", "/"));

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0) // change is made to size
      {
        string sz = this._selectedItems[i].Replace("Ø", "D").Replace("/", "mmH");
        this.StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
      }

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      string sz = this._selectedItems[0].Replace("Ø", "D").Replace("/", "mmH");
      this.StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
