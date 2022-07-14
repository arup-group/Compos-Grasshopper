using System;
using System.Collections.Generic;
using System.Linq;

using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class LoadValues
  {
    public IQuantity ConstantDead { get; set; }
    public IQuantity ConstantLive { get; set; }
    public IQuantity FinalDead { get; set; }
    public IQuantity FinalLive { get; set; }

    public LoadValues() { }

    public LoadValues(IQuantity consDead, IQuantity consLive, IQuantity finalDead, IQuantity finalLive)
    {
      this.ConstantDead = consDead;
      this.ConstantLive = consLive;
      this.FinalDead = finalDead;
      this.FinalLive = finalLive;
    }
  }

  public class NonConstantLoad : LoadValues
  {
    public IQuantity Position { get; set; }

    public NonConstantLoad(IQuantity consDead, IQuantity consLive, IQuantity finalDead, IQuantity finalLive, IQuantity position)
      : base(consDead, consLive, finalDead, finalLive)
    {
      if (position.QuantityInfo.UnitType != typeof(LengthUnit) &&
        position.QuantityInfo.UnitType != typeof(RatioUnit))
        throw new Exception("Position must be either Length or Ratio");
      
      this.Position = position;
    }

    public NonConstantLoad() { }
  }
}
