namespace ComposAPI
{
  public interface IEC4SafetyFactors
  {
    IEC4MaterialPartialFactors MaterialFactors { get; }
    ILoadCombinationFactors LoadCombinationFactors { get; }
    
    string ToCoaString(string name);
  }
}
