using System.Collections.Generic;
using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Interface with information about support conditions. The Supports interface is required for input(s) when creating a <see cref="IRestraint"/> object.
  /// </summary>
  public interface ISupports
  {
    bool SecondaryMemberIntermediateRestraint { get; }
    bool BothFlangesFreeToRotateOnPlanAtEnds { get; }
    IList<IQuantity> CustomIntermediateRestraintPositions { get; }
    IntermediateRestraint IntermediateRestraintPositions { get; }
  }
}
