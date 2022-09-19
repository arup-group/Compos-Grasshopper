using System.Collections.Generic;
using OasysUnitsNet;

namespace ComposAPI
{
  public interface IDeflectionResult
  {
    /// <summary>
    /// Deflection due to Construction dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> ConstructionDeadLoad { get; }


    /// <summary>
    /// Deflection due to additional dead loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> AdditionalDeadLoad { get; }


    /// <summary>
    /// Deflection due to Final stage live loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> LiveLoad { get; }


    /// <summary>
    /// Deflection due to shrinkage of concrete. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> Shrinkage { get; }


    /// <summary>
    /// Deflection due to post Construction loads. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> PostConstruction { get; }

    /// <summary>
    /// Total Deflection. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Length> Total { get; }


    // bug in COM, ModalShape giving only 0 values
    ///// <summary>
    ///// Mode shape. Values given at each <see cref="IResult.Positions"/>
    ///// </summary>
    //List<Length> ModalShape { get; }
  }
}
