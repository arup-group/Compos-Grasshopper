using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;

namespace ComposGH.Components
{
  public class CreateSlab : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("7af9eb3d-0868-4476-b31c-87d9eaae5e86");
    public CreateSlab()
      : base("Create" + SlabGoo.Name.Replace(" ", string.Empty),
          SlabGoo.Name.Replace(" ", string.Empty),
          "Create a " + SlabGoo.Description + " for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Resources.CreateSlab;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(ConcreteMaterialGoo.Name, ConcreteMaterialGoo.NickName, ConcreteMaterialGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(SlabDimensionGoo.Name + "(s)", SlabDimensionGoo.NickName, SlabDimensionGoo.Description, GH_ParamAccess.list);
      pManager.AddGenericParameter(TransverseReinforcementGoo.Name, TransverseReinforcementGoo.NickName, TransverseReinforcementGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(MeshReinforcementGoo.Name, MeshReinforcementGoo.NickName, "(Optional) " + MeshReinforcementGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(DeckingGoo.Name, DeckingGoo.NickName, "(Optional) " + DeckingGoo.Description, GH_ParamAccess.item);

      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(SlabGoo.Name, SlabGoo.NickName, SlabGoo.Description + " for a " + MemberGoo.Description, GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      ConcreteMaterialGoo material = (ConcreteMaterialGoo)GetInput.GenericGoo<ConcreteMaterialGoo>(this, DA, 0);
      if (material == null) { return; } // return here on non-optional inputs

      List<SlabDimensionGoo> dimensions = GetInput.GenericGooList<SlabDimensionGoo>(this, DA, 1);
      if (dimensions == null) { return; } // return here on non-optional inputs

      if (dimensions.Count > 1) 
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "There is currently a bug in ComposAPI preventing more than one SlabDimension to be written to a compos file."); 
      }

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
