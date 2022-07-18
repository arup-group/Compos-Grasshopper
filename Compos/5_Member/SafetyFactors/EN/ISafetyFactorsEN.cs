namespace ComposAPI
{
  public interface ISafetyFactorsEN
  {
    IMaterialPartialFactorsEN MaterialFactors { get; }
    ILoadCombinationFactors LoadCombinationFactors { get; }
    
    string ToCoaString(string name);
  }
}
