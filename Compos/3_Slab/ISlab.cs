using System.Collections.Generic;
using Oasys.Units;
using UnitsNet.Units;

namespace ComposAPI
{
  public interface ISlab
  {
    IConcreteMaterial Material { get; }
    IList<ISlabDimension> Dimensions { get; }
    ITransverseReinforcement TransverseReinforcement { get; }
    IMeshReinforcement MeshReinforcement { get; }
    IDecking Decking { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
