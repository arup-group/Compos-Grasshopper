using System.Collections.Generic;
using OasysUnits;

namespace ComposAPI
{
  internal enum StressOption
  {
    STRESS_CONS_BEAM_BOT, // Maximum stress in steel beam bottom Flange due to Construction loads
    STRESS_CONS_BEAM_WEB, // Maximum stress in steel beam web due to Construction loads
    STRESS_CONS_BEAM_TOP, // Maximum stress in steel beam top Flange due to Construction loads
    STRESS_ADDI_BEAM_BOT, // Maximum stress in steel beam bottom Flange due to additional dead loads
    STRESS_ADDI_BEAM_WEB, // Maximum stress in steel beam web due to additional dead loads
    STRESS_ADDI_BEAM_TOP, // Maximum stress in steel beam top Flange  due to additional dead loads
    STRESS_ADDI_CONC_STRESS, // Maximum stress in concrete slab due to additional dead loads
    STRESS_ADDI_CONC_STRAIN, // Maximum strain in concrete slab due to additional dead loads
    STRESS_FINA_LIVE_BEAM_BOT, // Maximum stress in steel beam bottom Flange due to Final stage live dead loads
    STRESS_FINA_LIVE_BEAM_WEB, // Maximum stress in steel beam web due to Final stage live dead loads
    STRESS_FINA_LIVE_BEAM_TOP, // Maximum stress in steel beam top Flange  due to Final stage live dead loads
    STRESS_FINA_LIVE_CONC_STRESS, // Maximum stress in concrete slab due to Final stage live loads
    STRESS_FINA_LIVE_CONC_STRAIN, // Maximum strain in concrete slab due to Final stage live loads
    STRESS_SHRINK_BEAM_BOT, // Maximum stress in steel beam bottom Flange due to shrinkage
    STRESS_SHRINK_BEAM_WEB, // Maximum stress in steel beam web due to shrinkage
    STRESS_SHRINK_BEAM_TOP, // Maximum stress in steel beam top Flange due to shrinkage
    STRESS_SHRINK_CONC_STRESS, // Maximum stress in concrete slab due to shrinkage
    STRESS_SHRINK_CONC_STRAIN, // Maximum strain in concrete slab due to shrinkage
    STRESS_FINA_TOTL_BEAM_BOT, // Maximum stress in steel beam bottom Flange in Final stage
    STRESS_FINA_TOTL_BEAM_WEB, // Maximum stress in steel beam web in Final stage
    STRESS_FINA_TOTL_BEAM_TOP, // Maximum stress in steel beam top Flange in Final stage
    STRESS_FINA_TOTL_CONC_STRESS, // Maximum stress in concrete slab in Final stage
    STRESS_FINA_TOTL_CONC_STRAIN, // Maximum strain in concrete slab in Final stage
  }

  public interface IBeamStressResult
  {
    #region bottom flange
    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to Construction loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> BottomFlangeConstruction { get; }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to additional dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> BottomFlangeFinalAdditionalDeadLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to Final stage live dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> BottomFlangeFinalLiveLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to shrinkage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> BottomFlangeFinalShrinkage { get; }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> BottomFlangeFinal { get; }
    #endregion
    #region web
    /// <summary>
    /// Maximum stress in steel beam Web due to Construction loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> WebConstruction { get; }

    /// <summary>
    /// Maximum stress in steel beam Web due to additional dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> WebFinalAdditionalDeadLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam Web due to Final stage live dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> WebFinalLiveLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam Web due to shrinkage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> WebFinalShrinkage { get; }

    /// <summary>
    /// Maximum stress in steel beam Web in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> WebFinal { get; }
    #endregion
    #region top flange
    /// <summary>
    /// Maximum stress in steel beam top Flange due to Construction loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> TopFlangeConstruction { get; }

    /// <summary>
    /// Maximum stress in steel beam top Flange due to additional dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> TopFlangeFinalAdditionalDeadLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam top Flange due to Final stage live dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> TopFlangeFinalLiveLoad { get; }

    /// <summary>
    /// Maximum stress in steel beam top Flange due to shrinkage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> TopFlangeFinalShrinkage { get; }

    /// <summary>
    /// Maximum stress in steel beam top Flange in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Pressure> TopFlangeFinal { get; }
    #endregion
  }
}
