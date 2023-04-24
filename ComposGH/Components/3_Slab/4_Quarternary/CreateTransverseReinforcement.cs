using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components {
  public class CreateTransverseReinforcement : GH_OasysComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("E832E3E8-1EF9-4F31-BC2A-683881E4BAC3");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.TransverseReinforcement;

    public CreateTransverseReinforcement() : base("Create" + TransverseReinforcementGoo.Name.Replace(" ", string.Empty),
      TransverseReinforcementGoo.Name.Replace(" ", string.Empty),
      "Create a " + TransverseReinforcementGoo.Description + " for a " + SlabGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3()) { Hidden = true; } // sets the initial state of the component to hidden

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new ReinforcementMaterialParam());
      pManager.AddParameter(new CustomTransverseReinforcementParam(), CustomTransverseReinforcementLayoutGoo.Name + "(s)", CustomTransverseReinforcementLayoutGoo.NickName, "(Optional) " + CustomTransverseReinforcementLayoutGoo.Description + " for a " + TransverseReinforcementGoo.Description + " - if left empty, Compos will create the layout automatically", GH_ParamAccess.list);
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new TransverseReinforcementParam());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      var mat = (ReinforcementMaterialGoo)Input.GenericGoo<ReinforcementMaterialGoo>(this, DA, 0);
      if (mat == null) { return; }

      if (Params.Input[1].Sources.Count > 0) {
        List<CustomTransverseReinforcementLayoutGoo> transverseReinforcmentLayouts = Input.GenericGooList<CustomTransverseReinforcementLayoutGoo>(this, DA, 1);
        DA.SetData(0, new TransverseReinforcementGoo(new TransverseReinforcement(mat.Value, transverseReinforcmentLayouts.Select(x => x.Value).ToList())));
      } else {
        DA.SetData(0, new TransverseReinforcementGoo(new TransverseReinforcement(mat.Value)));
      }
    }
  }
}
