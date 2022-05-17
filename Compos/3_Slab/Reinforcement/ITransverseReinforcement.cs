namespace ComposAPI
{
  public interface ITransverseReinforcement
  {
    IReinforcementMaterial Material { get; }
    LayoutMethod LayoutMethod { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
