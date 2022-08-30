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
  public interface ICapacityResult
  {
    /// <summary>
    /// Sagging moment capacity in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> Moment { get; }

    /// <summary>
    /// Neutral axis depth under Sagging moment in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> NeutralAxis { get; }

    /// <summary>
    /// Sagging moment capacity in Construction stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentConstruction { get; }

    /// <summary>
    /// Neutral axis depth under Sagging moment in Construction stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> NeutralAxisConstruction { get; }

    /// <summary>
    /// Hogging moment capacity in Construction stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentHoggingConstruction { get; }

    /// <summary>
    /// Neutral axis depth under Hogging moment in Construction stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> NeutralAxisHoggingConstruction { get; }

    /// <summary>
    /// Hogging moment capacity in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> MomentHoggingFinal { get; }

    /// <summary>
    /// Neutral axis depth under Hogging moment in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> NeutralAxisHoggingFinal { get; }

    /// <summary>
    /// Shear capacity. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> Shear { get; }

    /// <summary>
    /// Shear capacity with web buckling. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> ShearBuckling { get; }

    /// <summary>
    /// Used shear capacity. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> ShearRequired { get; }

    /// <summary>
    /// Assumed plastic Sagging moment capacity. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> AssumedBeamPlasticMoment { get; }

    /// <summary>
    /// Neutral axis depth under Assumed plastic Sagging moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> AssumedPlasticNeutralAxis { get; }

    /// <summary>
    /// Assumed 100% shear interaction Sagging moment capacity. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> AssumedMomentFullShearInteraction { get; }

    /// <summary>
    /// Neutral axis depth under Assumed 100% shear interaction Sagging moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> AssumedNeutralAxisFullShearInteraction { get; }

    /// <summary>
    /// Assumed plastic Hogging moment capacity. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> AssumedBeamPlasticMomentHogging { get; }

    /// <summary>
    /// Neutral axis depth under Assumed plastic Hogging moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> AssumedPlasticNeutralAxisHogging { get; }

    /// <summary>
    /// Assumed 100% shear interaction Sagging moment capacity. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Moment> AssumedMomentFullShearInteractionHogging { get; }

    /// <summary>
    /// Neutral axis depth under Assumed 100% shear interaction Sagging moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> AssumedNeutralAxisFullShearInteractionHogging { get; }
  }
}
