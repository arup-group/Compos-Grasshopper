using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

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
    IDeckingConfiguration DeckConfiguration { get; }
    DeckingType Type { get; }
  }
}
