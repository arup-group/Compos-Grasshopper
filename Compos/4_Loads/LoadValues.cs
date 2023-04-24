using System;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public class LoadValues {
    public IQuantity ConstantDead { get; set; }
    public IQuantity ConstantLive { get; set; }
    public IQuantity FinalDead { get; set; }
    public IQuantity FinalLive { get; set; }

    public LoadValues() { }

    public LoadValues(IQuantity consDead, IQuantity consLive, IQuantity finalDead, IQuantity finalLive) {
      ConstantDead = consDead;
      ConstantLive = consLive;
      FinalDead = finalDead;
      FinalLive = finalLive;
    }
  }

  public class NonConstantLoad : LoadValues {
    public IQuantity Position { get; set; }

    public NonConstantLoad(IQuantity consDead, IQuantity consLive, IQuantity finalDead, IQuantity finalLive, IQuantity position)
      : base(consDead, consLive, finalDead, finalLive) {
      if (position.QuantityInfo.UnitType != typeof(LengthUnit) && position.QuantityInfo.UnitType != typeof(RatioUnit)) {
        throw new Exception("Position must be either Length or Ratio");
      }

      Position = position;
    }

    public NonConstantLoad() { }
  }
}
