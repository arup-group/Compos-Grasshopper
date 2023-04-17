using OasysUnits;

namespace ComposAPI {
  public interface IDecking {
    Length b1 { get; }
    Length b2 { get; }
    Length b3 { get; }
    Length b4 { get; }
    Length b5 { get; }
    IDeckingConfiguration DeckingConfiguration { get; }
    Length Depth { get; }
    Length Thickness { get; }
    DeckingType Type { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
