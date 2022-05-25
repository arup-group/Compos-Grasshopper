namespace ComposAPI
{
  /// <summary>
  /// Interface for custom Load factors. These data can be omitted, if they are omitted, code specified load factor will be used
  /// </summary>
  public interface ILoadFactors
  {
    double ConstantDead { get; }
    double ConstantLive { get; }
    double FinalDead { get; }
    double FinalLive { get; }

    string ToCoaString(string name);
  }
}
