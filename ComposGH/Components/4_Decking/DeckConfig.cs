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
using ComposAPI.ConcreteSlab;
using Grasshopper.Kernel.Parameters;

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

    protected override System.Drawing.Bitmap Icon => Properties.Resources.DeckingConfig;
    #endregion

    private AngleUnit angleUnit = AngleUnit.Radian;

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity angle = new Angle(0, angleUnit);
      string unitAbbreviation = string.Concat(angle.ToString().Where(char.IsLetter));

      pManager.AddAngleParameter("Angle", "Angle", "Decking angle", GH_ParamAccess.item, Math.PI/2);
      pManager.AddBooleanParameter("Discontinaus", "Con", "Is decking discontinous (default = true)", GH_ParamAccess.item, false);
      pManager.AddBooleanParameter("Welded", "Wd", "Is decking welded onto steel beam(default = false)", GH_ParamAccess.item, false);
    }
    protected override void BeforeSolveInstance()
    {
      base.BeforeSolveInstance();
      Param_Number angleParameter = Params.Input[0] as Param_Number;
      if (angleParameter != null)
      {
        if (angleParameter.UseDegrees)
          angleUnit = AngleUnit.Degree;
        else
          angleUnit = AngleUnit.Radian;
      }
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Deck Config", "DC", "Compos Deck Configuration", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      double angle = Math.PI / 2;
      DA.GetData(0, ref angle);
      bool isDiscontinous = true;
      bool isWelded = true;
      DA.GetData(1, ref isDiscontinous);
      DA.GetData(2, ref isWelded);

      DA.SetData(0, new DeckingConfigGoo(new DeckingConfiguration(new Angle(angle, angleUnit), isDiscontinous, isWelded)));
    }
  }
}
