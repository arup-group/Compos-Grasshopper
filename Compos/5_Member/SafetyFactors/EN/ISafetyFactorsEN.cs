namespace ComposAPI
{
  public interface ISafetyFactorsEN
  {
    IMaterialPartialFactors MaterialFactors { get; }
    ILoadCombinationFactors LoadCombinationFactors { get; }
    
    string ToCoaString(string name);
  }
}
