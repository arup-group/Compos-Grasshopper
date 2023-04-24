using System.Collections.Generic;

namespace ComposAPI {
  public interface ISlab {
    IDecking Decking { get; }
    IList<ISlabDimension> Dimensions { get; }
    IConcreteMaterial Material { get; }
    IMeshReinforcement Mesh { get; }
    ITransverseReinforcement Transverse { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
