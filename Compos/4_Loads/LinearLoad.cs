using System;
using System.Collections.Generic;
using System.Linq;

using UnitsNet;

namespace ComposAPI
{
  public class LinearLoad : Load
  {
    public LoadValues LoadW1 { get; set; }
    public LoadValues LoadW2 { get; set; }
    public LoadDistribution Distribution { get; set; }

    public LinearLoad() { this.m_type = LoadType.Linear; }

    public LinearLoad(
      ForcePerLength consDeadW1, ForcePerLength consLiveW1, ForcePerLength finalDeadW1, ForcePerLength finalLiveW1,
      ForcePerLength consDeadW2, ForcePerLength consLiveW2, ForcePerLength finalDeadW2, ForcePerLength finalLiveW2)
    {
      this.LoadW1 = new LoadValues(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1);
      this.LoadW2 = new LoadValues(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2);
      this.m_type = LoadType.Linear;
      this.Distribution = LoadDistribution.Line;
    }

    public LinearLoad(
      Pressure consDeadW1, Pressure consLiveW1, Pressure finalDeadW1, Pressure finalLiveW1,
      Pressure consDeadW2, Pressure consLiveW2, Pressure finalDeadW2, Pressure finalLiveW2)
    {
      this.LoadW1 = new LoadValues(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1);
      this.LoadW2 = new LoadValues(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2);
      this.m_type = LoadType.Linear;
      this.Distribution = LoadDistribution.Area;
    }
  }
}
