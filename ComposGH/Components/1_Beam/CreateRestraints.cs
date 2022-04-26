﻿using System;
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
using Grasshopper.Kernel.Parameters;

namespace ComposGH.Components
{
  public class CreateRestraints : GH_Component
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("71c87cde-f442-475b-9131-8f2974c42499");
    public CreateRestraints()
      : base("Restraints", "Restraints", "Create Restraints for a Compos Beam",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    //protected override System.Drawing.Bitmap Icon => Properties.Resources.BeamSection;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddBooleanParameter("Top flng. lat. res. constr.stg.", "TFLR", "Top flange laterally restrained continuously at construction stage (default = true)", GH_ParamAccess.item, true);
      pManager.AddGenericParameter("Construction Stage Support", "sup", "Support(s) at construction stage", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Stage Support", "SUP", "(Optional) Support(s) at final stage", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Restraints", "Res", "Restraints for a Compos Beam", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      bool tflr = true;
      DA.GetData(0, ref tflr);

      Supports construction = GetInput.Supports(this, DA, 1);

      if (this.Params.Input[2].Sources.Count > 0)
      {
        Supports final = GetInput.Supports(this, DA, 2);
        ComposRestraint res = new ComposRestraint(tflr, construction, final);
        DA.SetData(0, new ComposRestraintGoo(res));
      }
      else
      {
        ComposRestraint res = new ComposRestraint(tflr, construction);
        DA.SetData(0, new ComposRestraintGoo(res));
      }
    }
  }
}
