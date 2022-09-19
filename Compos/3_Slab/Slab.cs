﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using ComposAPI.Helpers;

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

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          goto transverse;

        if (parameters[0] == CoaIdentifier.UnitData)
          units.FromCoaString(parameters);

        if (parameters[1] != name)
          continue;

        switch (parameters[0])
        {
          case (CoaIdentifier.SlabConcreteMaterial):
            slab.Material = ConcreteMaterial.FromCoaString(parameters, units);
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
    transverse:
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
      string invalid = "";
      string dim = "";
      if (this.Dimensions.Count == 0)
      {
        invalid = "Invalid Slab ";
        dim = "(no dimensions set)";
      }
      else
        dim = (this.Dimensions.Count > 1) ? string.Join(" : ", this.Dimensions.Select(x => x.ToString()).ToArray()) : this.Dimensions[0].ToString();

      string mat = "";
      if (this.Material == null)
      {
        invalid = "Invalid Slab ";
        mat = "(no material set)";
      }
      else
        mat = this.Material.ToString();
      string reinf = "";
      if (this.Mesh != null)
        reinf = this.Mesh.ToString() + " / ";
      if (this.Transverse == null)
      {
        invalid = "Invalid Slab ";
        reinf = "(no reinforcement set)";
      }
      else
        reinf += this.Transverse.ToString();
      return invalid + dim + ", " + mat + ", " + reinf;
    }
    #endregion
  }
}
