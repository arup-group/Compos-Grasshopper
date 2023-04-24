namespace ComposAPI {
  public class SafetyFactors : ISafetyFactors {
    public ILoadFactors LoadFactors { get; set; } = null;
    public IMaterialFactors MaterialFactors { get; set; } = null;

    public SafetyFactors() {
      // default initialiser
    }

    public string ToCoaString(string name) {
      string str = "";
      if (LoadFactors != null) {
        str = LoadFactors.ToCoaString(name);
      }
      if (MaterialFactors != null) {
        str += MaterialFactors.ToCoaString(name);
      }
      return str;
    }
  }
}
