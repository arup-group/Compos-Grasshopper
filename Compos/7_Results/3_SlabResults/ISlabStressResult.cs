using OasysUnits;
using System.Collections.Generic;

namespace ComposAPI {
  public interface ISlabStressResult {
    /// <summary>
    /// Maximum strain in concrete slab due to additional dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Strain> ConcreteStrainAdditionalDeadLoad { get; }
    /// <summary>
    /// Maximum strain in concrete slab in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Strain> ConcreteStrainFinal { get; }
    /// <summary>
    /// Maximum strain in concrete slab due to Final stage live dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Strain> ConcreteStrainFinalLiveLoad { get; }
    /// <summary>
    /// Maximum strain in concrete slab due to shrinkage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Strain> ConcreteStrainFinalShrinkage { get; }
    /// <summary>
    /// Maximum stress in concrete slab due to additional dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> ConcreteStressAdditionalDeadLoad { get; }

    /// <summary>
    /// Maximum stress in concrete slab in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> ConcreteStressFinal { get; }
    /// <summary>
    /// Maximum stress in concrete slab due to Final stage live dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> ConcreteStressFinalLiveLoad { get; }

    /// <summary>
    /// Maximum stress in concrete slab due to shrinkage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> ConcreteStressFinalShrinkage { get; }
  }
}
