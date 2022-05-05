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
using Grasshopper.Kernel.Parameters;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateSlab : GH_Component
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("7af9eb3d-0868-4476-b31c-87d9eaae5e86");
    public CreateSlab()
      : base("Slab", "Slab", "Create concrete slab",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = false; } // sets the initial state of the component to hidden

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
      pManager.AddGenericParameter("Slab", "S", "Concrete slab", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IConcreteMaterial material = GetInput.ConcreteMaterial(this, DA, 0);
      List<ISlabDimension> dimensions = GetInput.SlabDimensions(this, DA, 1);
      ITransverseReinforcement transverseReinforcement = GetInput.TransverseReinforcement(this, DA, 2);
      IMeshReinforcement meshReinforcement = GetInput.MeshReinforcement(this, DA, 3, true);
      IDecking decking = GetInput.Decking(this, DA, 4, true);

      ISlab slab = new Slab(material, dimensions, transverseReinforcement, meshReinforcement, decking);

      DA.SetData(0, new SlabGoo(slab));
    }
  }
}
