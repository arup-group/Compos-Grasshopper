using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;

namespace ComposGH.Components
{
  public class DeckConfig : GH_Component
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("85E6A4A4-DD97-4780-A679-B733C4B4FE01");
    public DeckConfig()
      : base("Deck Config", "DeckConf", "Create Decking configuration for a Compos Slab",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    //protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateStudZoneLength;
    #endregion

    private AngleUnit angleUnit = AngleUnit.Radian;

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity angle = new Angle(0, angleUnit);
      string unitAbbreviation = string.Concat(angle.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Angle [" + unitAbbreviation + "]", "Angle", "Decking angle", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Discontinaus", "Con", "Is decking discontinous (default = true)", GH_ParamAccess.item, false);
      pManager.AddBooleanParameter("Welded", "Wd", "Is decking welded onto steel beam(default = false)", GH_ParamAccess.item, false);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Deck Config", "DeckConfig", "Compos Deck Configuration setup", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Angle angle = GetInput.Angle(this, DA, 0, angleUnit);
      bool isDiscontinous = true;
      bool isWelded = true;
      DA.GetData(1, ref isDiscontinous);
      DA.GetData(2, ref isWelded);

      DA.SetData(0, new DeckConfigurationGoo(new DeckConfiguration(angle, isDiscontinous, isWelded)));
    }
  }
}
