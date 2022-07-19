using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public class SafetyFactors : ISafetyFactors
  {
    public IMaterialFactors MaterialFactors { get; set; } = null;
    public ILoadFactors LoadFactors { get; set; } = null;

    public SafetyFactors()
    {
      // default initialiser
    }

    public string ToCoaString(string name)
    {
      string str = "";
      if (this.LoadFactors != null)
        str = this.LoadFactors.ToCoaString(name);
      if (this.MaterialFactors != null)
        str += this.MaterialFactors.ToCoaString(name);
      return str;
    }
  }
}
