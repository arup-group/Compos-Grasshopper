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
  public interface IDeflectionResult
  {
    /// <summary>
    /// Deflection due to Construction dead loads
    /// </summary>
    List<Length> ConstructionDeadLoad { get; }


    /// <summary>
    /// Deflection due to additional dead loads
    /// </summary>
    List<Length> AdditionalDeadLoad { get; }


    /// <summary>
    /// Deflection due to Final stage live loads
    /// </summary>
    List<Length> LiveLoad { get; }


    /// <summary>
    /// Deflection due to shrinkage of concrete
    /// </summary>
    List<Length> Shrinkage { get; }


    /// <summary>
    /// Deflection due to post Construction loads
    /// </summary>
    List<Length> PostConstruction { get; }

    /// <summary>
    /// Total Deflection
    /// </summary>
    List<Length> Total { get; }


    /// <summary>
    /// Mode shape
    /// </summary>
    List<Length> ModeShape { get; }
  }
}
