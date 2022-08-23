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
  public class CreateStandardStudDimensions : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c97c7e52-7aa3-438f-900a-33f6ca889b3c");
    public CreateStandardStudDimensions()
      : base("Standard" + StudDimensionsGoo.Name.Replace(" ", string.Empty),
          "StudDimsStandard",
          "Look up a Standard " + StudDimensionsGoo.Description + " for a " + StudGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StandardStudDims;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    { }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(StudDimensionsGoo.Name, StudDimensionsGoo.NickName, StudDimensionsGoo.Description + " for a " + StudGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      SetOutput.Item(this, DA, 0, new StudDimensionsGoo(new StudDimensions(this.StdSize)));
    }

    #region Custom UI
    private StandardStudSize StdSize = StandardStudSize.D19mmH100mm;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Standard Size" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // spacing
      this.DropDownItems.Add(Enum.GetValues(typeof(StandardStudSize)).Cast<StandardStudSize>()
          .Select(x => x.ToString()).ToList());
      for (int i = 0; i < DropDownItems[0].Count; i++)
        this.DropDownItems[0][i] = DropDownItems[0][i].Replace("D", "Ø").Replace("mmH", "/");

      this.SelectedItems.Add(this.StdSize.ToString().Replace("D", "Ø").Replace("mmH", "/"));

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to size
      {
        string sz = this.SelectedItems[i].Replace("Ø", "D").Replace("/", "mmH");
        this.StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
      }

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      string sz = this.SelectedItems[0].Replace("Ø", "D").Replace("/", "mmH");
      this.StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
