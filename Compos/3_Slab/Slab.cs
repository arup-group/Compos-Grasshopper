using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;
using Oasys.Units;

namespace ComposAPI
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
    public Decking Decking { get; set; } = null; // null, if option "No decking (solid slab)" is selected

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
    internal Slab(string coaString, AngleUnit angleUnit, DensityUnit densityUnit, LengthUnit lengthUnit, PressureUnit pressureUnit, StrainUnit strainUnit)
    {
      List<string> lines = CoaHelper.SplitLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);
        switch (parameters[0])
        {
          case (ConcreteMaterial.CoaIdentifier):
            this.Material = new ConcreteMaterial(lines, densityUnit, strainUnit);
            break;

          case (SlabDimension.CoaIdentifier):
            SlabDimension dimension = new SlabDimension(parameters, lengthUnit);
            this.Dimensions.Add(dimension);
            break;

          case (TransverseReinforcement.CoaIdentifier):
            this.TransverseReinforcement = new TransverseReinforcement(parameters);
            break;

          case (MeshReinforcement.CoaIdentifier):
            this.MeshReinforcement = new MeshReinforcement(parameters);
            break;

          case (CatalogueDecking.CoaIdentifier):
            this.Decking = new CatalogueDecking(parameters, angleUnit);
            break;

          case (CustomDecking.CoaIdentifier):
            if (parameters[2] == "USER_DEFINED")
              this.Decking = new CustomDecking(parameters, angleUnit, lengthUnit, pressureUnit);
            //else
              // do nothing
            break;

          default:
            throw new Exception("Unable to convert " + line + " to Compos Slab.");
        }
      }
    }

    internal string ToCoaString(string name, AngleUnit angleUnit, DensityUnit densityUnit, LengthUnit lengthUnit, PressureUnit pressureUnit, StrainUnit strainUnit)
    {
      string str = this.Material.ToCoaString(name, densityUnit, strainUnit);
      int num = 1;
      int index = this.Dimensions.Count + 1;
      foreach (SlabDimension dimension in this.Dimensions)
      {
        str += dimension.ToCoaString(name, num, index, lengthUnit);
        num++;
      }
      str += this.TransverseReinforcement.ToCoaString();
      if (this.MeshReinforcement != null)
        str += this.MeshReinforcement.ToCoaString();
      if (this.Decking != null)
      {
        str += this.Decking.ToCoaString(name, angleUnit, lengthUnit, pressureUnit);
      }
      return str;
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
      string reinf = "";
      if (this.MeshReinforcement != null)
        reinf = this.MeshReinforcement.ToString() + " / ";
      reinf += this.TransverseReinforcement.ToString();
      return dim + ", " + mat + ", " + reinf;
    }
    #endregion

  }
}
