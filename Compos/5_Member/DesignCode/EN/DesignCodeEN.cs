namespace ComposAPI {
  /// <summary>
  /// <see cref="DesignCode"/> inherit class specific to EN 1994-1-1:2004
  /// </summary>
  public class EN1994 : DesignCode {
    public CodeOptionsEN CodeOptions { get; set; } = new CodeOptionsEN();
    public NationalAnnex NationalAnnex { get; set; } = NationalAnnex.Generic;
    public new ISafetyFactorsEN SafetyFactors { get; set; } = new SafetyFactorsEN();

    public EN1994() {
      Code = Code.EN1994_1_1_2004;
    }

    public override string ToCoaString(string name) {
      string str = base.ToCoaString(name);
      str += CodeOptions.ToCoaString(name, NationalAnnex);
      str += SafetyFactors.ToCoaString(name);
      return str;
    }
  }

  public enum NationalAnnex {
    Generic,
    United_Kingdom
  }
}
