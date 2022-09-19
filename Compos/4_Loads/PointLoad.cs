using System;
using System.Collections.Generic;
using System.Linq;

using OasysUnitsNet;

namespace ComposAPI
{
  public class PointLoad : Load
  {
    public NonConstantLoad Load { get; set; }
    public PointLoad() { this.m_type = LoadType.Point; }

    public PointLoad(Force consDead, Force consLive, Force finalDead, Force finalLive, IQuantity position)
    {
      this.Load = new NonConstantLoad(consDead, consLive, finalDead, finalLive, position);
      this.m_type = LoadType.Point;
    }
  }
}
