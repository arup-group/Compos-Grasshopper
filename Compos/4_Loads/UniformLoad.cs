using OasysUnitsNet;

namespace ComposAPI
{
  public class UniformLoad : Load
  {
    public LoadValues Load { get; set; }
    public LoadDistribution Distribution { get; set; }

    public UniformLoad() { this.m_type = LoadType.Uniform; }

    public UniformLoad(ForcePerLength consDead, ForcePerLength consLive, ForcePerLength finalDead, ForcePerLength finalLive)
    {
      this.Load = new LoadValues(consDead, consLive, finalDead, finalLive);
      this.m_type = LoadType.Uniform;
      this.Distribution = LoadDistribution.Line;
    }

    public UniformLoad(Pressure consDead, Pressure consLive, Pressure finalDead, Pressure finalLive)
    {
      this.Load = new LoadValues(consDead, consLive, finalDead, finalLive);
      this.m_type = LoadType.Uniform;
      this.Distribution = LoadDistribution.Area;
    }
  }
}
