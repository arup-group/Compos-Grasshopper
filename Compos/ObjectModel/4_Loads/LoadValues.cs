using System;
using System.Collections.Generic;
using System.Linq;

using UnitsNet;

namespace ComposAPI.Loads
{
  public class LoadValues
  {
    public IQuantity ConstantDead { get; set; }
    public IQuantity ConstantLive { get; set; }
    public IQuantity FinalDead { get; set; }
    public IQuantity FinalLive { get; set; }

    public LoadValues(IQuantity consDead, IQuantity consLive, IQuantity finalDead, IQuantity finalLive)
    {
      this.ConstantDead = consDead;
      this.ConstantLive = consLive;
      this.FinalDead = finalDead;
      this.FinalLive = finalLive;
    }
    public LoadValues Duplicate()
    {
      return (LoadValues)this.MemberwiseClone();
    }
  }

  public class NonConstantLoad : LoadValues
  {
    public Length Position { get; set; }
    public NonConstantLoad(IQuantity consDead, IQuantity consLive, IQuantity finalDead, IQuantity finalLive, Length position)
      : base(consDead, consLive, finalDead, finalLive)
    {
      this.Position = position;
    }
    public new NonConstantLoad Duplicate()
    {
      return (NonConstantLoad)this.MemberwiseClone();
    }
  }
}
