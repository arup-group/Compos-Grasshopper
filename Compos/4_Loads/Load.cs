using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public enum LoadType
  {
    Point,
    Uniform,
    Linear,
    TriLinear,
    Patch,
    MemberLoad,
    Axial
  }

  public enum LoadDistribution
  {
    Line,
    Area
  }

  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class Load : ILoad
  {
    public LoadType Type { get { return m_type; } }
    internal LoadType m_type;

    #region constructors
    public Load()
    {
      // empty constructor
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region coa interop
    internal Load(string coaString)
    {
      // to do - implement from coa string method
    }

    internal string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      // update with better naming
      return this.Type.ToString() + " Load";
    }
    #endregion
  }
}
