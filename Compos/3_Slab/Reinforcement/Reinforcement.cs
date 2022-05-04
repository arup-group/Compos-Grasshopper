using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Reinforcement object of either a <see cref="MeshReinforcement"/> or a <see cref="TransverseReinforcement"/>
  /// </summary>
  public class Reinforcement
  {
    public enum ReinforcementType
    {
      Mesh,
      Transverse
    }
    public ReinforcementType Type { get { return m_type; } }
    internal ReinforcementType m_type;

    public Reinforcement()
    {
      // empty constructor
    }

    internal Reinforcement(string coaString)
    {
      // to do - implement from coa string method
    }

    internal string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
    }

  }
}
