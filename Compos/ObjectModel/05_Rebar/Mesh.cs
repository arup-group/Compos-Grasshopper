using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.ConcreteSlab
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class MeshReinforcement
  {
    public Length Cover { get; set; }
    public ReinforcementMeshType Type { get; set; }
    public bool Rotated { get; set; }

    public enum ReinforcementMeshType
    {
      A393,
      A252,
      A193,
      A142,
      A98,
      B1131,
      B785,
      B503,
      B385,
      B283,
      B196,
      C785,
      C636,
      C503,
      C385,
      C283
    }

    #region constructors
    public MeshReinforcement()
    {
      //empty constructor
    }

    public MeshReinforcement(Length cover, ReinforcementMeshType meshType = ReinforcementMeshType.A393, bool rotated = false)
    {
      this.Cover = cover;
      this.Type = meshType;
      this.Rotated = rotated;
    }

    #endregion

    #region methods

    public MeshReinforcement Duplicate()
    {
      if (this == null) { return null; }
      MeshReinforcement dup = (MeshReinforcement)this.MemberwiseClone();
      return dup;
    }
    public override string ToString()
    {
      string cov = Cover.ToString("f0");
      string msh = Type.ToString();

      string rotated = (this.Rotated == true) ? " (rotated)" : "";


      return msh.Replace(" ", string.Empty) + rotated + ", c:" + cov.Replace(" ", string.Empty);
    }
    #endregion
  }
}
