using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public interface IInternalForceResult
  {
    #region moment

    /// <summary>
    /// Construction stage ultimate moment
    /// </summary>
    List<Moment> MomentULSConstruction { get; }


    /// <summary>
    /// Final stage ultimate moment
    /// </summary>
    List<Moment> MomentULS { get; }


    /// <summary>
    /// Construction stage working dead load moment
    /// </summary>
    List<Moment> MomentConstructionDeadLoad { get; }


    /// <summary>
    /// Construction stage working live load moment
    /// </summary>
    List<Moment> MomentConstructionLiveLoad { get; }


    /// <summary>
    /// Final stage working additional dead load moment
    /// </summary>
    List<Moment> MomentFinalAdditionalDeadLoad { get; }

    /// <summary>
    /// Final stage working live load moment
    /// </summary>
    List<Moment> MomentFinalLiveLoad { get; }


    /// <summary>
    /// Final stage working shrinkage moment
    /// </summary>
    List<Moment> MomentFinalShrinkage { get; }

    #endregion
    #region shear
    /// <summary>
    /// Construction stage ultimate shear
    /// </summary>
    List<Force> ShearULSConstructionStage { get; }


    /// <summary>
    /// Final stage ultimate shear
    /// </summary>
    List<Force> ShearULS { get; }


    /// <summary>
    /// Construction stage working dead load moment
    /// </summary>
    List<Force> ShearConstructionDeadLoad { get; }


    /// <summary>
    /// Construction stage working live load moment
    /// </summary>
    List<Force> ShearConstructionLiveLoad { get; }


    /// <summary>
    /// Final stage working additional dead load moment
    /// </summary>
    List<Force> ShearFinalAdditionalDeadLoad { get; }


    /// <summary>
    /// Final stage working live load moment
    /// </summary>
    List<Force> ShearFinalLiveLoad { get; }

    #endregion
    #region force
    /// <summary>
    /// Construction stage ultimate shear
    /// </summary>
    List<Force> AxialULSConstructionStage { get; }


    /// <summary>
    /// Final stage ultimate shear
    /// </summary>
    List<Force> AxialULS { get; }


    /// <summary>
    /// Construction stage working dead load moment
    /// </summary>
    List<Force> AxialConstructionDeadLoad { get; }


    /// <summary>
    /// Construction stage working live load moment
    /// </summary>
    List<Force> AxialConstructionLiveLoad { get; }


    /// <summary>
    /// Final stage working additional dead load moment
    /// </summary>
    List<Force> AxialFinalAdditionalDeadLoad { get; }


    /// <summary>
    /// Final stage working live load moment
    /// </summary>
    List<Force> AxialFinalLiveLoad { get; }

    #endregion
  }
}
