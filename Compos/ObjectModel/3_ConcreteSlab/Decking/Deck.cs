using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace ComposAPI.Decking
{
  public class Deck
  {
    public Length b1 { get; set; }
    public Length b2 { get; set; }
    public Length b3 { get; set; }
    public Length b4 { get; set; }
    public Length b5 { get; set; }
    public Length Depth { get; set; }
    public Length Thickness { get; set; }
    public DeckingConfiguration DeckConfiguration { get; set; }
    public enum DeckingType
    {
      Custom,
      Catalogue
    }
    public DeckingType Type { get { return m_type; } }
    internal DeckingType m_type;

    public Deck()
    {
      // empty constructor
    }

    internal Deck(string coaString)
    {
      // to do - implement from coa string method
    }

    internal string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
    }

    public virtual Deck Duplicate()
    {
      if (this == null) { return null; }
      Deck dup = (Deck)this.MemberwiseClone();
      dup.DeckConfiguration = this.DeckConfiguration.Duplicate();
      return dup;
    }
  }
}
