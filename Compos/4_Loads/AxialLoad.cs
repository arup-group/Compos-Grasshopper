using System;
using System.Collections.Generic;
using System.Linq;

using UnitsNet;

namespace ComposAPI
{
  public class AxialLoad : Load
  {
    public LoadValues LoadW1 { get; set; }
    public LoadValues LoadW2 { get; set; }
    public AxialLoad() { this.m_type = LoadType.Axial; }
    public Length Depth1 { get; set; }
    public Length Depth2 { get; set; }

    public AxialLoad(
      Force consDeadW1, Force consLiveW1, Force finalDeadW1, Force finalLiveW1, Length depth1,
      Force consDeadW2, Force consLiveW2, Force finalDeadW2, Force finalLiveW2, Length depth2)
    {
      this.LoadW1 = new LoadValues(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1);
      this.LoadW2 = new LoadValues(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2);
      this.Depth1 = depth1;
      this.Depth2 = depth2;
      this.m_type = LoadType.Axial;
    }
  }
}
