using System.Collections.Generic;
using System.Linq;
using ComposAPI.Helpers;

namespace ComposAPI {
  public class Slab : ISlab {
    public IDecking Decking { get; set; } = null;
    public IList<ISlabDimension> Dimensions { get; set; } = new List<ISlabDimension>();
    public IConcreteMaterial Material { get; set; }
    public IMeshReinforcement Mesh { get; set; } = null;
    public ITransverseReinforcement Transverse { get; set; }
    // null, if option "No decking (solid slab)" is selected

    public Slab() {
      // empty constructor
    }

    public Slab(IConcreteMaterial material, List<ISlabDimension> dimensions, ITransverseReinforcement transverseReinforcement, IMeshReinforcement meshReinforcement = null, IDecking decking = null) {
      Material = material;
      Dimensions = dimensions;
      Transverse = transverseReinforcement;
      Mesh = meshReinforcement;
      Decking = decking;
    }

    public string ToCoaString(string name, ComposUnits units) {
      string str = Material.ToCoaString(name, units);
      int num = Dimensions.Count;
      int index = 1;
      foreach (SlabDimension dimension in Dimensions.Cast<SlabDimension>()) {
        str += dimension.ToCoaString(name, num, index, units);
        index++;
      }
      if (Mesh != null && Mesh.MeshType != ReinforcementMeshType.None) {
        str += Mesh.ToCoaString(name, units);
      }
      str += Transverse.ToCoaString(name, units);
      if (Decking != null) {
        str += Decking.ToCoaString(name, units);
      }
      return str;
    }

    public override string ToString() {
      string invalid = "";
      string dim = "";
      if (Dimensions.Count == 0) {
        invalid = "Invalid Slab ";
        dim = "(no dimensions set)";
      } else {
        dim = (Dimensions.Count > 1) ? string.Join(" : ", Dimensions.Select(x => x.ToString()).ToArray()) : Dimensions[0].ToString();
      }

      string mat = "";
      if (Material == null) {
        invalid = "Invalid Slab ";
        mat = "(no material set)";
      } else {
        mat = Material.ToString();
      }
      string reinf = "";
      if (Mesh != null) {
        reinf = Mesh.ToString() + " / ";
      }
      if (Transverse == null) {
        invalid = "Invalid Slab ";
        reinf = "(no reinforcement set)";
      } else {
        reinf += Transverse.ToString();
      }
      return invalid + dim + ", " + mat + ", " + reinf;
    }

    internal static ISlab FromCoaString(string coaString, string name, Code code, ComposUnits units) {
      var slab = new Slab();

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines) {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END") {
          goto transverse;
        }

        if (parameters[0] == CoaIdentifier.UnitData) {
          units.FromCoaString(parameters);
        }

        if (parameters[1] != name) {
          continue;
        }

        switch (parameters[0]) {
          case CoaIdentifier.SlabConcreteMaterial:
            slab.Material = ConcreteMaterial.FromCoaString(parameters, units);
            break;

          case CoaIdentifier.SlabDimension:
            ISlabDimension dimension = SlabDimension.FromCoaString(parameters, units);
            slab.Dimensions.Add(dimension);
            break;

          case CoaIdentifier.RebarMesh:
            slab.Mesh = MeshReinforcement.FromCoaString(parameters, units);
            break;

          case CoaIdentifier.DeckingCatalogue:
            slab.Decking = CatalogueDecking.FromCoaString(parameters);
            break;

          case CoaIdentifier.DeckingUser:
            if (parameters[2] == "USER_DEFINED") {
              slab.Decking = CustomDecking.FromCoaString(parameters, units);
            }
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
  }
}
