﻿namespace ComposAPI {
  /// <summary>
  /// Use this interface to create a DesignCode. Use inheriting <see cref="EN1994"/> or <see cref="ASNZS2327"/> specifically for those codes respectively.
  /// </summary>
  public interface IDesignCode {
    Code Code { get; }
    IDesignOption DesignOption { get; }
    ISafetyFactors SafetyFactors { get; set; }

    string ToCoaString(string name);
  }
}
