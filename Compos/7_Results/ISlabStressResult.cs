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
  public interface ISlabStressResult
  {
    #region concrete stress
    /// <summary>
    /// Maximum stress in concrete slab due to additional dead loads
    /// </summary>
    List<Pressure> ConcreteStressAdditionalDeadLoad { get; }

    /// <summary>
    /// Maximum stress in concrete slab due to Final stage live dead loads
    /// </summary>
    List<Pressure> ConcreteStressFinalLiveLoad { get; }

    /// <summary>
    /// Maximum stress in concrete slab due to shrinkage
    /// </summary>
    List<Pressure> ConcreteStressFinalShrinkage { get; }

    /// <summary>
    /// Maximum stress in concrete slab in Final stage
    /// </summary>
    List<Pressure> ConcreteStressFinal { get; }
    #endregion
    #region concrete strain
    /// <summary>
    /// Maximum strain in concrete slab due to additional dead loads
    /// </summary>
    List<Strain> ConcreteStrainAdditionalDeadLoad { get; }

    /// <summary>
    /// Maximum strain in concrete slab due to Final stage live dead loads
    /// </summary>
    List<Strain> ConcreteStrainFinalLiveLoad { get; }

    /// <summary>
    /// Maximum strain in concrete slab due to shrinkage
    /// </summary>
    List<Strain> ConcreteStrainFinalShrinkage { get; }

    /// <summary>
    /// Maximum strain in concrete slab in Final stage
    /// </summary>
    List<Strain> ConcreteStrainFinal { get; }
    #endregion
  }
}
