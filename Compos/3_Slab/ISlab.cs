using System.Collections.Generic;
using Oasys.Units;
using UnitsNet.Units;

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
