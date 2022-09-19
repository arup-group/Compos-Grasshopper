using System.Collections.Generic;
using OasysUnitsNet;

namespace ComposAPI
{
  public interface ICompositeSectionProperties
  {
    /// <summary>
    /// Welding thickness at top - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> GirderWeldThicknessTop { get; }

    /// <summary>
    /// Welding thickness at bottom - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> GirderWeldThicknessBottom { get; }

    /// <summary>
    /// Effective slab width on left side. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> EffectiveSlabWidthLeft { get; }

    /// <summary>
    /// Effective slab width on right side. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> EffectiveSlabWidthRight { get; }

    /// <summary>
    /// Moment of Inertia of steel beam alone. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<AreaMomentOfInertia> BeamMomentOfInertia { get; }

    /// <summary>
    /// Neutral axis position of steel beam alone, measured from the bottom of steel beam. Values given at each <see cref="IResult.Positions"/> 
    /// </summary>
    List<Length> BeamNeutralAxisPosition { get; }

    /// <summary>
    /// Area of steel beam alone. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Area> BeamArea { get; }

    /// <summary>
    /// Moment of Inertia of composite section for long term loading. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<AreaMomentOfInertia> MomentOfInertiaLongTerm { get; }

    /// <summary>
    /// Neutral axis position of composite section for long term loading, measured from the bottom of steel beam. Values given at each <see cref="IResult.Positions"/> 
    /// </summary>
    List<Length> NeutralAxisPositionLongTerm { get; }

    /// <summary>
    /// Area of composite section for long term loading. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Area> AreaLongTerm { get; }

    /// <summary>
    /// Moment of Inertia of composite section for short term loading. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<AreaMomentOfInertia> MomentOfInertiaShortTerm { get; }

    /// <summary>
    /// Neutral axis position of composite section for short term loading, measured from the bottom of steel beam. Values given at each <see cref="IResult.Positions"/> 
    /// </summary>
    List<Length> NeutralAxisPositionShortTerm { get; }

    /// <summary>
    /// Area of composite section for short term loading. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Area> AreaShortTerm { get; }

    /// <summary>
    /// Moment of Inertia of composite section for shrinkage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<AreaMomentOfInertia> MomentOfInertiaShrinkage { get; }

    /// <summary>
    /// Neutral axis position of composite section for shrinkage, measured from the bottom of steel beam. Values given at each <see cref="IResult.Positions"/> 
    /// </summary>
    List<Length> NeutralAxisPositionShrinkage { get; }

    /// <summary>
    /// Area of composite section for shrinkage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Area> AreaShrinkage { get; }

    /// <summary>
    /// Effective Moment of Inertia of composite section under combined loading. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<AreaMomentOfInertia> MomentOfInertiaEffective { get; }

    /// <summary>
    /// Effective Neutral axis position of composite section under combined loading, measured from the bottom of steel beam. Values given at each <see cref="IResult.Positions"/> 
    /// </summary>
    List<Length> NeutralAxisPositionEffective { get; }

    /// <summary>
    /// Effective Area of composite section under combined loading. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Area> AreaEffective { get; }

    /// <summary>
    /// Moment of Inertia of composite section for vibration. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<AreaMomentOfInertia> MomentOfInertiaVibration { get; }

    /// <summary>
    /// Neutral axis position of composite section for vibration, measured from the bottom of steel beam. Values given at each <see cref="IResult.Positions"/> 
    /// </summary>
    List<Length> NeutralAxisPositionVibration { get; }

    /// <summary>
    /// Area of composite section for vibration. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Area> AreaVibration { get; }


    /// <summary>
    /// Natural frequency for composite section
    /// </summary>
    Frequency NaturalFrequency { get; }
  }
}
