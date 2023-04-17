namespace ComposAPI {
  public interface ISafetyFactorsEN {
    ILoadCombinationFactors LoadCombinationFactors { get; }
    IMaterialPartialFactors MaterialFactors { get; }

    string ToCoaString(string name);
  }
}
