namespace ComposAPI
{
  public interface ITransverseReinforcement
  {
    IReinforcementMaterial Material { get; }
    LayoutMethod Layout { get; }

    string ToCoaString();
  }
}
