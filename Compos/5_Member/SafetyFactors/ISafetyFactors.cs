namespace ComposAPI {
  public interface ISafetyFactors {
    ILoadFactors LoadFactors { get; }
    IMaterialFactors MaterialFactors { get; }

    string ToCoaString(string name);
  }
}
