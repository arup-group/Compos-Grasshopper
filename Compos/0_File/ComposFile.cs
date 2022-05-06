using Compos_8_6;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class ComposFile : IComposFile
  {
    public List<IMember> Members { get; set; }

    #region constructors
    public ComposFile()
    {
      // empty constructor
    }

    public ComposFile(List<IMember> members)
    {
      this.Members = members;
    }
    #endregion

    #region coa interop
    internal ComposFile(string coaString)
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
    public IAutomation Open(string pathName)
    {
      IAutomation automation = new Automation();
      automation.Open(pathName);

      return automation;
    }

    public IAutomation SaveAs(string pathName, string coaString)
    {
      File.WriteAllLines(pathName, new string[] { coaString });
      return Open(pathName);
    }

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
