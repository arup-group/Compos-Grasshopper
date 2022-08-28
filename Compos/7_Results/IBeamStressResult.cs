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
  public interface IBeamStressResult
  {
    #region bottom flange
    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to Construction loads
    /// </summary>
    List<Pressure> BottomFlangeConstruction { get; }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to additional dead loads
    /// </summary>
    List<Pressure> BottomFlangeFinalAdditionalDeadLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to Final stage live dead loads
    /// </summary>
    List<Pressure> BottomFlangeFinalLiveLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to shrinkage
    /// </summary>
    List<Pressure> BottomFlangeFinalShrinkage { get; }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange in Final stage
    /// </summary>
    List<Pressure> BottomFlangeFinal { get; }
    #endregion
    #region web
    /// <summary>
    /// Maximum stress in steel beam Web due to Construction loads
    /// </summary>
    List<Pressure> WebConstruction { get; }

    /// <summary>
    /// Maximum stress in steel beam Web due to additional dead loads
    /// </summary>
    List<Pressure> WebFinalAdditionalDeadLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam Web due to Final stage live dead loads
    /// </summary>
    List<Pressure> WebFinalLiveLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam Web due to shrinkage
    /// </summary>
    List<Pressure> WebFinalShrinkage { get; }

    /// <summary>
    /// Maximum stress in steel beam Web in Final stage
    /// </summary>
    List<Pressure> WebFinal { get; }
    #endregion
    #region top flange
    /// <summary>
    /// Maximum stress in steel beam top Flange due to Construction loads
    /// </summary>
    List<Pressure> TopFlangeConstruction { get; }

    /// <summary>
    /// Maximum stress in steel beam top Flange due to additional dead loads
    /// </summary>
    List<Pressure> TopFlangeFinalAdditionalDeadLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam top Flange due to Final stage live dead loads
    /// </summary>
    List<Pressure> TopFlangeFinalLiveLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam top Flange due to shrinkage
    /// </summary>
    List<Pressure> TopFlangeFinalShrinkage { get; }

    /// <summary>
    /// Maximum stress in steel beam top Flange in Final stage
    /// </summary>
    List<Pressure> TopFlangeFinal { get; }
    #endregion
  }
}
