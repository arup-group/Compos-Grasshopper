using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public enum DeckingType
  {
    Custom,
    Catalogue
  }
  public class Decking : IDecking
  {
    public Length b1 { get; set; } // dimensions of decking see decking page of property wizard of the program
    public Length b2 { get; set; }
    public Length b3 { get; set; }
    public Length b4 { get; set; }
    public Length b5 { get; set; }
    public Length Depth { get; set; } // overall depth of decking
    public Length Thickness { get; set; } // 	decking sheet thickness
    public IDeckingConfiguration DeckingConfiguration { get; set; }
    public DeckingType Type { get { return m_type; } }
    internal DeckingType m_type;

    public Decking()
    {
      // empty constructor
    }

    #region coa interop

    internal virtual string ToCoaString(string name, AngleUnit angleUnit, LengthUnit lengthUnit, PressureUnit pressureUnit)
    {
      return String.Empty;
    }

    #endregion
    public Decking(string coaString)
    {
      // to do - implement from coa string method
    }

    public string ToCoaString(string name)
    {
      // to do - implement to coa string method
      return string.Empty;
    }
  }
}
