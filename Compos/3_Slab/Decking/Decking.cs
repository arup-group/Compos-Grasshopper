using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class Decking
  {
    public enum DeckingType
    {
      Custom,
      Catalogue
    }

    public Length b1 { get; set; } // dimensions of decking see decking page of property wizard of the program
    public Length b2 { get; set; }
    public Length b3 { get; set; }
    public Length b4 { get; set; }
    public Length b5 { get; set; }
    public Length Depth { get; set; } // overall depth of decking
    public Length Thickness { get; set; } // 	decking sheet thickness
    public DeckingConfiguration DeckingConfiguration { get; set; }
    public DeckingType Type { get { return m_type; } }
    internal DeckingType m_type;

    public Decking()
    {
      // empty constructor
    }

    #region methods
    public virtual Decking Duplicate()
    {
      if (this == null) { return null; }
      Decking dup = (Decking)this.MemberwiseClone();
      dup.DeckingConfiguration = this.DeckingConfiguration.Duplicate();
      return dup;
    }

    internal virtual string ToCoaString(string name, AngleUnit angleUnit, LengthUnit lengthUnit, PressureUnit pressureUnit)
    {
      return String.Empty;
    }

    #endregion
  }
}
