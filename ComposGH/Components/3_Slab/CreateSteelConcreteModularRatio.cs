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
  public class CreateSteelConcreteModularRatio : GH_Component
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("a49e5e5e-502d-400e-81a2-3644577b3404");
    public CreateSteelConcreteModularRatio()
      : base("Steel/Concrete Modular Ratios", "ModRatios", "Create Steel/Concrete Modular Ratios for Concrete Material",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.RebarMaterial;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddNumberParameter("Short Term", "ST", "Steel/Concrete Modular Ratio for Short Term", GH_ParamAccess.item, 10);
      pManager.AddNumberParameter("Long Term", "LT", "Steel/Concrete Modular Ratio for Long Term", GH_ParamAccess.item, 25);
      pManager.AddNumberParameter("Vibration", "V", "Steel/Concrete Modular Ratio for Vibration", GH_ParamAccess.item, 9.32);
      pManager.AddNumberParameter("Shrinkage", "S", "Steel/Concrete Modular Ratio for Shrinkage", GH_ParamAccess.item, 0);
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("SteelConcreteModularRatio", "SCMR", "Steel/Concrete Modular Ratio for Concrete Material", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      double shortTerm = 0;
      double longTerm = 0;
      double vibration = 0;
      double shrinkage = 0;
      DA.GetData(0, ref shortTerm);
      DA.GetData(1, ref longTerm);
      DA.GetData(2, ref vibration);
      DA.GetData(3, ref shrinkage);

      DA.SetData(0, new SteelConcreteModularRatioGoo(new SteelConcreteModularRatio(shortTerm, longTerm, vibration, shrinkage)));
    }
  }
}
