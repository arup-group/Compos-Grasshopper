using System.Collections.Generic;

namespace ComposAPI {
  public interface ITransverseReinforcement {
    IList<ICustomTransverseReinforcementLayout> CustomReinforcementLayouts { get; }
    LayoutMethod LayoutMethod { get; }
    IReinforcementMaterial Material { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
