using System.Collections.Generic;
using OasysUnitsNet;

namespace ComposAPI
{
  public interface IInternalForceResult
  {
    #region moment

    /// <summary>
    /// Construction stage ultimate moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentULSConstruction { get; }


    /// <summary>
    /// Final stage ultimate moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentULS { get; }


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

    #endregion
    #region shear
    /// <summary>
    /// Construction stage ultimate shear. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> ShearULSConstruction { get; }


    /// <summary>
    /// Final stage ultimate shear. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> ShearULS { get; }


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

    #endregion
    #region force
    /// <summary>
    /// Construction stage ultimate shear. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> AxialULSConstruction { get; }


    /// <summary>
    /// Final stage ultimate shear. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> AxialULS { get; }


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

    #endregion
  }
}
