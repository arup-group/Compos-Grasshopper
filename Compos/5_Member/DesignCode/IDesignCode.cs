namespace ComposAPI
{
  /// <summary>
  /// Use this interface to create a DesignCode. Use inheriting <see cref="EN1994"/> or <see cref="ASNZS2327"/> specifically for those codes respectively.
  /// </summary>
  public interface IDesignCode
  {
    Code Code { get; }
    IDesignOptions DesignOptions { get; }
    ISafetyFactors SafetyFactors { get; }

    string ToCoaString(string name);
  }
}
