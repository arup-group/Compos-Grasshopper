using OasysUnits;

namespace ComposAPI {
  public class PointLoad : Load {
    public NonConstantLoad Load { get; set; }

    public PointLoad() { m_type = LoadType.Point; }

    public PointLoad(Force consDead, Force consLive, Force finalDead, Force finalLive, IQuantity position) {
      Load = new NonConstantLoad(consDead, consLive, finalDead, finalLive, position);
      m_type = LoadType.Point;
    }
  }
}
