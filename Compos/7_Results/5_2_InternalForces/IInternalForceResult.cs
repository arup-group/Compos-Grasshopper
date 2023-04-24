using System.Collections.Generic;
using OasysUnits;

namespace ComposAPI {
  public interface IInternalForceResult {
    /// <summary>
    /// Construction stage working dead load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> AxialConstructionDeadLoad { get; }
    /// <summary>
    /// Construction stage working live load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> AxialConstructionLiveLoad { get; }
    /// <summary>
    /// Final stage working additional dead load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> AxialFinalAdditionalDeadLoad { get; }
    /// <summary>
    /// Final stage working live load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> AxialFinalLiveLoad { get; }
    /// <summary>
    /// Final stage ultimate shear. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> AxialULS { get; }
    /// <summary>
    /// Construction stage ultimate shear. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> AxialULSConstruction { get; }
    /// <summary>
    /// Construction stage working dead load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentConstructionDeadLoad { get; }
    /// <summary>
    /// Construction stage working live load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentConstructionLiveLoad { get; }
    /// <summary>
    /// Final stage working additional dead load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentFinalAdditionalDeadLoad { get; }
    /// <summary>
    /// Final stage working live load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentFinalLiveLoad { get; }
    /// <summary>
    /// Final stage working shrinkage moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentFinalShrinkage { get; }
    /// <summary>
    /// Final stage ultimate moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentULS { get; }
    /// <summary>
    /// Construction stage ultimate moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentULSConstruction { get; }
    /// <summary>
    /// Construction stage working dead load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> ShearConstructionDeadLoad { get; }
    /// <summary>
    /// Construction stage working live load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> ShearConstructionLiveLoad { get; }
    /// <summary>
    /// Final stage working additional dead load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> ShearFinalAdditionalDeadLoad { get; }
    /// <summary>
    /// Final stage working live load moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> ShearFinalLiveLoad { get; }
    /// <summary>
    /// Final stage ultimate shear. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> ShearULS { get; }
    /// <summary>
    /// Construction stage ultimate shear. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> ShearULSConstruction { get; }
  }
}
