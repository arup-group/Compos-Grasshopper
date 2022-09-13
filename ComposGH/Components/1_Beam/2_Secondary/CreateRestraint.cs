using System;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH.Components;
using OasysGH.Helpers;

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

    protected override System.Drawing.Bitmap Icon => Resources.CreateRestraint;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddBooleanParameter("Top flng. lat. res. constr.stg.", "TFLR", "Top flange laterally restrained continuously at construction stage (default = true)", GH_ParamAccess.item, true);
      pManager.AddParameter(new SupportsParam(), "Construction Stage " + SupportsGoo.Name, SupportsGoo.NickName.ToLower(), "Construction stage " + SupportsGoo.Description, GH_ParamAccess.item);
      pManager.AddParameter(new SupportsParam(), "Final Stage " + SupportsGoo.Name, SupportsGoo.NickName.ToUpper(), "(Optional) Final stage " + SupportsGoo.Description, GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new RestraintParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      bool tflr = true;
      DA.GetData(0, ref tflr);
      ISupports construction;
      IRestraint res;

      if (this.Params.Input[1].Sources.Count > 0)
      {
        SupportsGoo constructionGoo = (SupportsGoo)Input.GenericGoo<SupportsGoo>(this, DA, 1);
        construction = constructionGoo.Value;

        if (this.Params.Input[0].Sources.Count > 0)
        {
          AddRuntimeMessage(tflr ? GH_RuntimeMessageLevel.Warning : GH_RuntimeMessageLevel.Remark, "When setting Construction Stage supports it is assumed Top Flange is not laterally restrained");
        }
        tflr = false;
      }
      else
        construction = new Supports();

      if (this.Params.Input[2].Sources.Count > 0)
      {
        SupportsGoo final = (SupportsGoo)Input.GenericGoo<SupportsGoo>(this, DA, 2);
        if (final == null) { return; }
        res = new Restraint(tflr, construction, final.Value);
      }
      else
        res = new Restraint(tflr, construction);

      DA.SetData(0, new RestraintGoo(res));
    }
  }
}
