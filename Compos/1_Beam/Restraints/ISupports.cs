using System.Collections.Generic;
using OasysUnits;

namespace ComposAPI {
  /// <summary>
  /// Interface with information about support conditions. The Supports interface is required for input(s) when creating a <see cref="IRestraint"/> object.
  /// </summary>
  public interface ISupports {
    bool BothFlangesFreeToRotateOnPlanAtEnds { get; }
    IList<IQuantity> CustomIntermediateRestraintPositions { get; }
    IntermediateRestraint IntermediateRestraintPositions { get; }
    bool SecondaryMemberAsIntermediateRestraint { get; }
  }
}
