namespace ComposAPI
{
  public interface ISafetyFactors
  {
    IMaterialFactors MaterialFactors { get;  }  
    ILoadFactors LoadFactors { get; }

    string ToCoaString(string name);
  }
}
