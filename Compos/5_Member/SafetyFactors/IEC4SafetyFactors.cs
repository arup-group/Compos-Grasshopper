namespace ComposAPI
{
  public interface IEC4SafetyFactors
  {
    IEC4MaterialPartialFactors MaterialFactors { get;  }  
    ILoadCombinationFactors LoadCombinationFactors { get;  }
    LoadCombination LoadCombination { get; }
    string ToCoaString(string name);
  }
}
