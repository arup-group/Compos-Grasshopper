using OasysUnits;

namespace ComposAPI
{
  public class TriLinearLoad : Load
  {
    public NonConstantLoad LoadW1 { get; set; }
    public NonConstantLoad LoadW2 { get; set; }
    public LoadDistribution Distribution { get; set; }
    public TriLinearLoad() { this.m_type = LoadType.TriLinear; }

    public TriLinearLoad(
      ForcePerLength consDeadW1, ForcePerLength consLiveW1, ForcePerLength finalDeadW1, ForcePerLength finalLiveW1, IQuantity positionW1,
      ForcePerLength consDeadW2, ForcePerLength consLiveW2, ForcePerLength finalDeadW2, ForcePerLength finalLiveW2, IQuantity positionW2)
    {
      this.LoadW1 = new NonConstantLoad(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1, positionW1);
      this.LoadW2 = new NonConstantLoad(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2, positionW2);
      this.m_type = LoadType.TriLinear;
      this.Distribution = LoadDistribution.Line;
    }

    public TriLinearLoad(
      Pressure consDeadW1, Pressure consLiveW1, Pressure finalDeadW1, Pressure finalLiveW1, IQuantity positionW1,
      Pressure consDeadW2, Pressure consLiveW2, Pressure finalDeadW2, Pressure finalLiveW2, IQuantity positionW2)
    {
      this.LoadW1 = new NonConstantLoad(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1, positionW1);
      this.LoadW2 = new NonConstantLoad(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2, positionW2);
      this.m_type = LoadType.TriLinear;
      this.Distribution = LoadDistribution.Area;
    }
  }
}
