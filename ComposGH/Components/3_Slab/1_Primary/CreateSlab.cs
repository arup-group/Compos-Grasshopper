using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateSlab : GH_Component
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("7af9eb3d-0868-4476-b31c-87d9eaae5e86");
    public CreateSlab()
      : base("Slab", "Slab", "Create a Compos Concrete Slab",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateSlab;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Material", "CMt", "Concrete material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Dimensions", "SD", "Slab dimensions", GH_ParamAccess.list);
      pManager.AddGenericParameter("TransverseReinforcement", "TRb", "Transverse reinforcement", GH_ParamAccess.item);
      pManager.AddGenericParameter("MeshReinforcement", "MRb", "Mesh reinforcement", GH_ParamAccess.item);
      pManager.AddGenericParameter("Decking", "Dk", "Decking", GH_ParamAccess.item);

      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Slab", "Slb", "Compos Concrete slab", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      ConcreteMaterialGoo material = (ConcreteMaterialGoo)GetInput.GenericGoo<ConcreteMaterialGoo>(this, DA, 0);
      if (material == null) { return; } // return here on non-optional inputs

      List<SlabDimensionGoo> dimensions = GetInput.GenericGooList<SlabDimensionGoo>(this, DA, 1);
      if (dimensions == null) { return; } // return here on non-optional inputs

      TransverseReinforcementGoo transverseReinforcement = (TransverseReinforcementGoo)GetInput.GenericGoo<TransverseReinforcementGoo>(this, DA, 2);
      if (transverseReinforcement == null) { return; } // return here on non-optional inputs

      MeshReinforcementGoo meshReinforcement = (MeshReinforcementGoo)GetInput.GenericGoo<MeshReinforcementGoo>(this, DA, 3);

      DeckingGoo decking = (DeckingGoo)GetInput.GenericGoo<DeckingGoo>(this, DA, 4);

      ISlab slab = new Slab(
        material.Value, 
        dimensions.Select(x => x.Value as ISlabDimension).ToList(), 
        transverseReinforcement.Value, 
        (meshReinforcement == null) ? null : meshReinforcement.Value,
        (decking == null) ? null : decking.Value);

      DA.SetData(0, new SlabGoo(slab));
    }
  }
}
