using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OasysUnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// <see cref="DesignCode"/> inherit class specific to AS/NZS2327:2017
  /// </summary>
  public class ASNZS2327 : DesignCode
  {
    public CodeOptionsASNZ CodeOptions { get; set; } = new CodeOptionsASNZ();

    public ASNZS2327()
    {
      this.Code = Code.AS_NZS2327_2017;
    }
  }
}
