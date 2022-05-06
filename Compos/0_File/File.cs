using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class File : IFile
  {
    public List<IMember> Members { get; set; }

    #region constructors
    public File()
    {
      // empty constructor
    }

    public File(List<IMember> members)
    {
      this.Members = members;
    }
    #endregion

    #region coa interop
    internal File(string coaString)
    {
      // to do - implement from coa string method
    }

    public string ToCoaString()
    {
      string coaString = "";
      foreach (IMember member in Members)
        coaString += member.ToCoaString(AngleUnit.Radian, Units.DensityUnit, Units.ForceUnit, Units.LengthUnitGeometry, Units.StressUnit, Units.StrainUnit);
      
      return coaString;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string str = "";
      foreach(IMember member in Members)  
        str += member.ToString();
      return str;
    }
    #endregion
  }
}
