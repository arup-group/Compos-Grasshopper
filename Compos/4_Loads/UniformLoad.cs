using OasysUnits;

namespace ComposAPI
{
  public class UniformLoad : Load
  {
    public LoadValues Load { get; set; }
    public LoadDistribution Distribution { get; set; }

    public UniformLoad() { m_type = LoadType.Uniform; }

    public UniformLoad(ForcePerLength consDead, ForcePerLength consLive, ForcePerLength finalDead, ForcePerLength finalLive)
    {
      Load = new LoadValues(consDead, consLive, finalDead, finalLive);
      m_type = LoadType.Uniform;
      Distribution = LoadDistribution.Line;
    }

    public UniformLoad(Pressure consDead, Pressure consLive, Pressure finalDead, Pressure finalLive)
    {
      Load = new LoadValues(consDead, consLive, finalDead, finalLive);
      m_type = LoadType.Uniform;
      Distribution = LoadDistribution.Area;
    }
  }
}
