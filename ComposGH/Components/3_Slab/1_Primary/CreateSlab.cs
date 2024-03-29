﻿using System;
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
  public class CreateSlab : GH_OasysComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("7af9eb3d-0868-4476-b31c-87d9eaae5e86");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateSlab;

    public CreateSlab() : base("Create" + SlabGoo.Name.Replace(" ", string.Empty),
      SlabGoo.Name.Replace(" ", string.Empty),
      "Create a " + SlabGoo.Description + " for a " + MemberGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3()) { Hidden = true; } // sets the initial state of the component to hidden

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new ConcreteMaterialParam());
      pManager.AddParameter(new SlabDimensionParam(), SlabDimensionGoo.Name + "(s)", SlabDimensionGoo.NickName, SlabDimensionGoo.Description, GH_ParamAccess.list);
      pManager.AddParameter(new TransverseReinforcementParam());
      pManager.AddParameter(new MeshReinforcementParam());
      pManager.AddParameter(new ComposDeckingParameter(), DeckingGoo.Name, DeckingGoo.NickName, "(Optional) " + DeckingGoo.Description, GH_ParamAccess.item);

      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ComposSlabParameter());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      var material = (ConcreteMaterialGoo)Input.GenericGoo<ConcreteMaterialGoo>(this, DA, 0);
      if (material == null) { return; } // return here on non-optional inputs

      List<SlabDimensionGoo> dimensions = Input.GenericGooList<SlabDimensionGoo>(this, DA, 1);
      if (dimensions == null) { return; } // return here on non-optional inputs

      if (dimensions.Count > 1) {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "There is currently a bug in ComposAPI preventing more than one SlabDimension to be written to a compos file.");
      }

      var transverseReinforcement = (TransverseReinforcementGoo)Input.GenericGoo<TransverseReinforcementGoo>(this, DA, 2);
      if (transverseReinforcement == null) { return; } // return here on non-optional inputs

      var meshReinforcement = (MeshReinforcementGoo)Input.GenericGoo<MeshReinforcementGoo>(this, DA, 3);

      var decking = (DeckingGoo)Input.GenericGoo<DeckingGoo>(this, DA, 4);

      ISlab slab = new Slab(
        material.Value,
        dimensions.Select(x => x.Value as ISlabDimension).ToList(),
        transverseReinforcement.Value,
        meshReinforcement?.Value,
        decking?.Value);

      DA.SetData(0, new SlabGoo(slab));
    }
  }
}
