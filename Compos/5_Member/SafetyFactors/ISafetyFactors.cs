namespace ComposAPI
{
  public interface ISafetyFactors
  {
    IMaterialPartialFactors MaterialFactors { get;  }  
    ILoadFactors LoadFactors { get; set; }

    string ToCoaString(string name);
  }
}
