using OasysUnits;

namespace ComposAPI
{
  public interface IReinforcementMaterial
  {
    RebarGrade Grade { get; }
    bool UserDefined { get; }
    Pressure Fy { get; }

    string ToCoaString(string name);
  }
}
