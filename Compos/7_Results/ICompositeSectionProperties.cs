﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public interface ICompositeSectionProperties
  {
    /// <summary>
    /// Welding thickness at top - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web.
    /// </summary>
    List<Length> GirderWeldThicknessTop { get; }

    /// <summary>
    /// Welding thickness at bottom - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web.
    /// </summary>
    List<Length> GirderWeldThicknessBottom { get; }

    /// <summary>
    /// Effective slab width on left side
    /// </summary>
    List<Length> EffectiveSlabWidthLeft { get; }

    /// <summary>
    /// Effective slab width on right side
    /// </summary>
    List<Length> EffectiveSlabWidthRight { get; }

    /// <summary>
    /// Moment of Inertia of steel beam alone
    /// </summary>
    List<AreaMomentOfInertia> BeamMomentOfInertia { get; }

    /// <summary>
    /// Neutral axis position of steel beam alone, measured from the bottom of steel beam 
    /// </summary>
    List<Length> BeamNeutralAxisPosition { get; }

    /// <summary>
    /// Area of steel beam alone
    /// </summary>
    List<Area> BeamArea { get; }

    /// <summary>
    /// Moment of Inertia of composite section for long term loading
    /// </summary>
    List<AreaMomentOfInertia> MomentOfInertiaLongTerm { get; }

    /// <summary>
    /// Neutral axis position of composite section for long term loading, measured from the bottom of steel beam 
    /// </summary>
    List<Length> NeutralAxisPositionLongTerm { get; }

    /// <summary>
    /// Area of composite section for long term loading
    /// </summary>
    List<Area> AreaLongTerm { get; }

    /// <summary>
    /// Moment of Inertia of composite section for short term loading
    /// </summary>
    List<AreaMomentOfInertia> MomentOfInertiaShortTerm { get; }

    /// <summary>
    /// Neutral axis position of composite section for short term loading, measured from the bottom of steel beam 
    /// </summary>
    List<Length> NeutralAxisPositionShortTerm { get; }

    /// <summary>
    /// Area of composite section for short term loading
    /// </summary>
    List<Area> AreaShortTerm { get; }

    /// <summary>
    /// Moment of Inertia of composite section for shrinkage
    /// </summary>
    List<AreaMomentOfInertia> MomentOfInertiaShrinkage { get; }

    /// <summary>
    /// Neutral axis position of composite section for shrinkage, measured from the bottom of steel beam 
    /// </summary>
    List<Length> NeutralAxisPositionShrinkage { get; }

    /// <summary>
    /// Area of composite section for shrinkage
    /// </summary>
    List<Area> AreaShrinkage { get; }

    /// <summary>
    /// Effective Moment of Inertia of composite section under combined loading
    /// </summary>
    List<AreaMomentOfInertia> MomentOfInertiaEffective { get; }

    /// <summary>
    /// Effective Neutral axis position of composite section under combined loading, measured from the bottom of steel beam 
    /// </summary>
    List<Length> NeutralAxisPositionEffective { get; }

    /// <summary>
    /// Effective Area of composite section under combined loading
    /// </summary>
    List<Area> AreaEffective { get; }

    /// <summary>
    /// Moment of Inertia of composite section for vibration
    /// </summary>
    List<AreaMomentOfInertia> MomentOfInertiaVibration { get; }

    /// <summary>
    /// Neutral axis position of composite section for vibration, measured from the bottom of steel beam 
    /// </summary>
    List<Length> NeutralAxisPositionVibration { get; }

    /// <summary>
    /// Area of composite section for vibration
    /// </summary>
    List<Area> AreaVibration { get; }
  }
}
