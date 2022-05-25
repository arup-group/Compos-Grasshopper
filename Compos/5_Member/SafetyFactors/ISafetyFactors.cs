namespace ComposAPI
{
  public interface ISafetyFactors
  {
    IMaterialPartialFactors MaterialFactors { get;  }  
    ILoadFactors LoadFactors { get; set; }
  }
}
