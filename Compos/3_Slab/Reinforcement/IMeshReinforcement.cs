using OasysUnits;

namespace ComposAPI {
  public interface IMeshReinforcement {
    Length Cover { get; }
    ReinforcementMeshType MeshType { get; }
    bool Rotated { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
