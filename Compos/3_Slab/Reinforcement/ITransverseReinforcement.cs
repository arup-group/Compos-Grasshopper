using System.Collections.Generic;

namespace ComposAPI
{
  public interface ITransverseReinforcement
  {
    IReinforcementMaterial Material { get; }
    LayoutMethod LayoutMethod { get; }
    IList<ICustomTransverseReinforcementLayout> CustomReinforcementLayouts { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
