using OasysUnits;

namespace ComposAPI {
  public interface IDecking {
    Length B1 { get; }
    Length B2 { get; }
    Length B3 { get; }
    Length B4 { get; }
    Length B5 { get; }
    IDeckingConfiguration DeckingConfiguration { get; }
    Length Depth { get; }
    Length Thickness { get; }
    DeckingType Type { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
