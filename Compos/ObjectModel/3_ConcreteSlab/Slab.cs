using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;
using System.Drawing;

namespace ComposAPI.ConcreteSlab
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class Slab
  {
    public ConcreteMaterial Material { get; set; }
    public List<SlabDimension> Dimensions { get; set; } = new List<SlabDimension>();
    public TransverseReinforcement TransverseReinforcement { get; set; }
    public MeshReinforcement MeshReinforcement { get; set; } = null;
    public Decking Decking { get; set; } = null;

    #region constructors
    public Slab()
    {
      // empty constructor
    }

    public Slab(ConcreteMaterial material, List<SlabDimension> dimensions, TransverseReinforcement transverseReinforcement, MeshReinforcement meshReinforcement = null, Decking decking = null)
    {
      this.Material = material;
      this.Dimensions = dimensions;
      this.TransverseReinforcement = transverseReinforcement;
      this.MeshReinforcement = meshReinforcement;
      this.Decking = decking;
    }
    public Slab(ConcreteMaterial material, SlabDimension dimensions, TransverseReinforcement transverseReinforcement, MeshReinforcement meshReinforcement = null, Decking decking = null)
    {
      this.Material = material;
      this.Dimensions = new List<SlabDimension> { dimensions };
      this.TransverseReinforcement = transverseReinforcement;
      this.MeshReinforcement = meshReinforcement;
      this.Decking = decking;
    }

    #endregion

    #region coa interop
    internal Slab(string coaString)
    {
      // to do - implement from coa string method
    }

    internal string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
    }
    #endregion

    #region methods

    public Slab Duplicate()
    {
      if (this == null) { return null; }
      Slab dup = (Slab)this.MemberwiseClone();
      dup.Material = this.Material.Duplicate();
      dup.Dimensions = this.Dimensions.ToList();
      dup.TransverseReinforcement = (TransverseReinforcement)this.TransverseReinforcement.Duplicate();
      if (this.MeshReinforcement != null)
        dup.MeshReinforcement = (MeshReinforcement)this.MeshReinforcement.Duplicate();
      if (this.Decking != null)
        dup.Decking = this.Decking.Duplicate();
      return dup;
    }

    public override string ToString()
    {

      string dim = (this.Dimensions.Count > 1) ? string.Join(" : ", this.Dimensions.Select(x => x.ToString()).ToArray()) : this.Dimensions[0].ToString();
      string mat = this.Material.ToString();
      string reinf = this.MeshReinforcement.ToString() + " / " + this.TransverseReinforcement.ToString();
      return dim + ", " + mat + ", " + reinf;
    }
    #endregion

  }
}
