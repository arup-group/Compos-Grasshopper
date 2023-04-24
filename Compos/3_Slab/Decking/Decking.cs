using OasysUnits;

namespace ComposAPI {
  public class Decking : IDecking {
    public Length B1 { get; set; } // dimensions of decking see decking page of property wizard of the program
    public Length B2 { get; set; }
    public Length B3 { get; set; }
    public Length B4 { get; set; }
    public Length B5 { get; set; }
    public IDeckingConfiguration DeckingConfiguration { get; set; }
    public Length Depth { get; set; } // overall depth of decking
    public Length Thickness { get; set; } // 	decking sheet thickness
    public DeckingType Type => m_type;
    internal DeckingType m_type;

    public Decking() {
      // empty constructor
    }

    public virtual string ToCoaString(string name, ComposUnits units) {
      return string.Empty;
    }
  }

  public enum DeckingType {
    Custom,
    Catalogue
  }
}
