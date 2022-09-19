using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OasysUnitsNet;
using OasysUnitsNet.Units;

namespace ComposAPI
{
  public interface IDecking
  {
    Length b1 { get; }
    Length b2 { get; }
    Length b3 { get; }
    Length b4 { get; }
    Length b5 { get; }
    Length Depth { get; }
    Length Thickness { get; }
    IDeckingConfiguration DeckingConfiguration { get; }
    DeckingType Type { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
