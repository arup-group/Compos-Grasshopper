using System.Collections.Generic;

namespace ComposAPI
{
  public interface ISlab
  {
    IConcreteMaterial Material { get; }
    IList<ISlabDimension> Dimensions { get; }
    ITransverseReinforcement Transverse { get; }
    IMeshReinforcement Mesh { get; }
    IDecking Decking { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
