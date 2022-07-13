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
    public ITransverseReinforcement Transverse { get; set; }
    public IMeshReinforcement Mesh { get; set; } = null;
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
      this.Transverse = transverseReinforcement;
      this.Mesh = meshReinforcement;
      this.Decking = decking;
    }
    #endregion

    #region coa interop
    internal static ISlab FromCoaString(string coaString, string name, Code code, ComposUnits units)
    {
      Slab slab = new Slab();

      List<string> lines = CoaHelper.SplitLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          return slab;

        if (parameters[0] == CoaIdentifier.UnitData)
          units.FromCoaString(parameters);

        if (parameters[1] != name)
          continue;

        switch (parameters[0])
        {
          case (CoaIdentifier.SlabConcreteMaterial):
            slab.Material = ConcreteMaterial.FromCoaString(lines, units);
            break;

          case (CoaIdentifier.SlabDimension):
            ISlabDimension dimension = SlabDimension.FromCoaString(parameters, units);
            slab.Dimensions.Add(dimension);
            break;

          case (CoaIdentifier.RebarMesh):
            slab.Mesh = MeshReinforcement.FromCoaString(parameters, units);
            break;

          case (CoaIdentifier.DeckingCatalogue):
            slab.Decking = CatalogueDecking.FromCoaString(parameters, units);
            break;

          case (CoaIdentifier.DeckingUser):
            if (parameters[2] == "USER_DEFINED")
              slab.Decking = CustomDecking.FromCoaString(parameters, units);
            // else
            // do nothing
            break;

          default:
            // continue;
            break;
        }
      }
      slab.Transverse = TransverseReinforcement.FromCoaString(coaString, name, code, units);

      return slab;
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      string str = this.Material.ToCoaString(name, units);
      int num = this.Dimensions.Count;
      int index = 1;
      foreach (SlabDimension dimension in this.Dimensions)
      {
        str += dimension.ToCoaString(name, num, index, units);
        index++;
      }
      if (this.Mesh != null)
        str += this.Mesh.ToCoaString(name, units);
      str += this.Transverse.ToCoaString(name, units);
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
      if (this.Mesh != null)
        reinf = this.Mesh.ToString() + " / ";
      reinf += this.Transverse.ToString();
      return dim + ", " + mat + ", " + reinf;
    }
    #endregion
  }
}
