namespace ComposAPI {
  public interface ILoad {
    LoadType Type { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
