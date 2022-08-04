﻿using System;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateRestraint : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("82c87cde-f442-475b-9131-8f2974c42499");
    public CreateRestraint()
      : base("Create" + RestraintGoo.Name.Replace(" ", string.Empty), 
          RestraintGoo.Name.Replace(" ", string.Empty), 
          "Create a " + RestraintGoo.Description + " for a " + BeamGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateRestraint;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddBooleanParameter("Top flng. lat. res. constr.stg.", "TFLR", "Top flange laterally restrained continuously at construction stage (default = true)", GH_ParamAccess.item, true);
      pManager.AddGenericParameter("Construction Stage " + SupportsGoo.Name, SupportsGoo.NickName.ToLower(), "Construction stage " + SupportsGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Stage " + SupportsGoo.Name, SupportsGoo.NickName.ToUpper(), "(Optional) Final stage " + SupportsGoo.Description, GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(RestraintGoo.Name, RestraintGoo.NickName, RestraintGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      bool tflr = true;
      DA.GetData(0, ref tflr);

      SupportsGoo construction = (SupportsGoo)GetInput.GenericGoo<SupportsGoo>(this, DA, 1);
      if (construction == null) { return; } // return here on non-optional inputs

      if (this.Params.Input[2].Sources.Count > 0)
      {
        SupportsGoo final = (SupportsGoo)GetInput.GenericGoo<SupportsGoo>(this, DA, 2);
        if (final == null) { return; }
        IRestraint res = new Restraint(tflr, construction.Value, final.Value);
        DA.SetData(0, new RestraintGoo(res));
      }
      else
      {
        IRestraint res = new Restraint(tflr, construction.Value);
        DA.SetData(0, new RestraintGoo(res));
      }
    }
  }
}
