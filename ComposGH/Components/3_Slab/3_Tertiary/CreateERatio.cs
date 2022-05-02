using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using ComposGH.Parameters;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Rhino.Geometry;
using UnitsNet;
using UnitsNet.Units;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateERatio : GH_Component
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("a49e5e5e-502d-400e-81a2-3644577b3404");
    public CreateERatio()
      : base("Steel/Concrete Modular Ratio", "ERatio", "Create steel/concrete Young´s modulus ratios for concrete material",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.ERatio;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddNumberParameter("Short Term", "ST", "Steel/concrete Young´s modulus ratio for short term", GH_ParamAccess.item, 6.24304);
      pManager.AddNumberParameter("Long Term", "LT", "Steel/concrete Young´s modulus ratio for long term", GH_ParamAccess.item, 23.5531);
      pManager.AddNumberParameter("Vibration", "V", "Steel/concrete Young´s modulus ratio for vibration", GH_ParamAccess.item, 5.526);
      pManager.AddNumberParameter("Shrinkage", "S", "Steel/concrete Young´s modulus ratio for shrinkage", GH_ParamAccess.item, 22.3517);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("E Ratios", "ER", "Steel/concrete Young´s modulus ratios for concrete material", GH_ParamAccess.item);
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
      if (this.Params.Input[0].Sources.Count == 0
        & this.Params.Input[1].Sources.Count == 0
        & this.Params.Input[2].Sources.Count == 0
        & this.Params.Input[3].Sources.Count == 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Default values for EC4 C30/37 concrete");
      }

      DA.SetData(0, new ERatioGoo(new ERatio(shortTerm, longTerm, vibration, shrinkage)));
    }
  }
}
