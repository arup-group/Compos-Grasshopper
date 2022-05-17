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
  public class Slab : ISlab
  {
    public IConcreteMaterial Material { get; set; }
    public IList<ISlabDimension> Dimensions { get; set; } = new List<ISlabDimension>();
    public ITransverseReinforcement TransverseReinforcement { get; set; }
    public IMeshReinforcement MeshReinforcement { get; set; } = null;
    public IDecking Decking { get; set; } = null; // null, if option "No decking (solid slab)" is selected

    #region constructors
    public Slab()
    {
      // empty constructor
    }

    public Slab(IConcreteMaterial material, List<ISlabDimension> dimensions, ITransverseReinforcement transverseReinforcement, IMeshReinforcement meshReinforcement = null, IDecking decking = null)
    {
      this.Material = material;
      this.Dimensions = dimensions;
      this.TransverseReinforcement = transverseReinforcement;
      this.MeshReinforcement = meshReinforcement;
      this.Decking = decking;
    }

    public Slab(ConcreteMaterial material, ISlabDimension dimensions, ITransverseReinforcement transverseReinforcement, IMeshReinforcement meshReinforcement = null, Decking decking = null)
    {
      this.Material = material;
      this.Dimensions = new List<ISlabDimension> { dimensions };
      this.TransverseReinforcement = transverseReinforcement;
      this.MeshReinforcement = meshReinforcement;
      this.Decking = decking;
    }
    #endregion

    #region coa interop
    internal Slab(string coaString, ComposUnits units)
    {
      List<string> lines = CoaHelper.SplitLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);
        switch (parameters[0])
        {
          case (CoaIdentifier.SlabConcreteMaterial):
            this.Material = new ConcreteMaterial(lines, units);
            break;

          case (CoaIdentifier.SlabDimension):
            SlabDimension dimension = new SlabDimension(parameters, units);
            this.Dimensions.Add(dimension);
            break;

          case (CoaIdentifier.RebarTransverse):
            this.TransverseReinforcement = new TransverseReinforcement(parameters);
            break;

          case (CoaIdentifier.RebarWesh):
            this.MeshReinforcement = new MeshReinforcement(parameters);
            break;

          case (CoaIdentifier.DeckingCatalogue):
            this.Decking = new CatalogueDecking(parameters, units);
            break;

          case (CoaIdentifier.DeckingUser):
            if (parameters[2] == "USER_DEFINED")
              this.Decking = new CustomDecking(parameters, units);
            //else
              // do nothing
            break;

          default:
            // do we not want to just continue here? or is the incoming coaString pre-filtered for only slab parts?
            throw new Exception("Unable to convert " + line + " to Compos Slab."); 
        }
      }
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      string str = this.Material.ToCoaString(name, units);
      int num = 1;
      int index = this.Dimensions.Count + 1;
      foreach (SlabDimension dimension in this.Dimensions)
      {
        str += dimension.ToCoaString(name, num, index, units);
        num++;
      }
      str += this.TransverseReinforcement.ToCoaString();
      if (this.MeshReinforcement != null)
        str += this.MeshReinforcement.ToCoaString(name);
      if (this.Decking != null)
      {
        str += this.Decking.ToCoaString(name, units);
      }
      return str;
    }
    #endregion

    #region methods
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
