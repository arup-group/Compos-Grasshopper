using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OasysUnitsNet;

namespace ComposAPI
{
  public enum NationalAnnex
  {
    Generic,
    United_Kingdom
  }

  /// <summary>
  /// <see cref="DesignCode"/> inherit class specific to EN 1994-1-1:2004
  /// </summary>
  public class EN1994 : DesignCode
  {
    public NationalAnnex NationalAnnex { get; set; } = NationalAnnex.Generic;
    public CodeOptionsEN CodeOptions { get; set; } = new CodeOptionsEN();
    public new ISafetyFactorsEN SafetyFactors { get; set; } = new SafetyFactorsEN();

    public EN1994()
    {
      this.Code = Code.EN1994_1_1_2004;
    }

    #region coa interop
    public override string ToCoaString(string name)
    {
      string str = base.ToCoaString(name);
      str += this.CodeOptions.ToCoaString(name, this.Code, this.NationalAnnex);
      str += this.SafetyFactors.ToCoaString(name);
      return str;
    }
    #endregion
  }
}
