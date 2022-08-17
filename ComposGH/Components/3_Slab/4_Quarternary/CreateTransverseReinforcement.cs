using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateTransverseReinforcement : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public CreateTransverseReinforcement()
      : base("Create" + TransverseReinforcementGoo.Name.Replace(" ", string.Empty),
          TransverseReinforcementGoo.Name.Replace(" ", string.Empty),
          "Create a " + TransverseReinforcementGoo.Description + " for a " + SlabGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override Guid ComponentGuid => new Guid("E832E3E8-1EF9-4F31-BC2A-683881E4BAC3");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.TransverseReinforcement;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(ReinforcementMaterialGoo.Name, ReinforcementMaterialGoo.NickName, ReinforcementMaterialGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(CustomTransverseReinforcementLayoutGoo.Name + "(s)", CustomTransverseReinforcementLayoutGoo.NickName, "(Optional) " + CustomTransverseReinforcementLayoutGoo.Description + " for a " + TransverseReinforcementGoo.Description + " - if left empty, Compos will create the layout automatically", GH_ParamAccess.list);
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(TransverseReinforcementGoo.Name, TransverseReinforcementGoo.NickName, TransverseReinforcementGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      ReinforcementMaterialGoo mat = (ReinforcementMaterialGoo)GetInput.GenericGoo<ReinforcementMaterialGoo>(this, DA, 0);
      if (mat == null) { return; }

      if (this.Params.Input[1].Sources.Count > 0)
      {
        List<CustomTransverseReinforcementLayoutGoo> transverseReinforcmentLayouts = GetInput.GenericGooList<CustomTransverseReinforcementLayoutGoo>(this, DA, 1);
        DA.SetData(0, new TransverseReinforcementGoo(new TransverseReinforcement(mat.Value, transverseReinforcmentLayouts.Select(x => x.Value).ToList())));
      }
      else
      {
        DA.SetData(0, new TransverseReinforcementGoo(new TransverseReinforcement(mat.Value)));
      }
    }
  }
}
