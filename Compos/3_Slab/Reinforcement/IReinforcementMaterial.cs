using OasysUnits;

namespace ComposAPI {
  public interface IReinforcementMaterial {
    Pressure Fy { get; }
    RebarGrade Grade { get; }
    bool UserDefined { get; }

    string ToCoaString(string name);
  }
}
