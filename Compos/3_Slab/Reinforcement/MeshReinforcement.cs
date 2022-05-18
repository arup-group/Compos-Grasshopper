using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
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

  public class MeshReinforcement : IMeshReinforcement
  {
    public Length Cover { get; set; }
    public bool Rotated { get; set; }
    public ReinforcementMeshType MeshType { get; set; }

    #region constructors
    public MeshReinforcement()   {    }

    public MeshReinforcement(Length cover, ReinforcementMeshType meshType = ReinforcementMeshType.A393, bool rotated = false)
    {
      this.Cover = cover;
      this.MeshType = meshType;
      this.Rotated = rotated;
    }

    #endregion

    #region coa interop
    internal static IMeshReinforcement FromCoaString(List<string> parameters)
    {
      MeshReinforcement reinforcement = new MeshReinforcement();

      return reinforcement;
    }

    public string ToCoaString(string name)
    {
      return String.Empty;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string cov = Cover.ToString("f0");
      string msh = MeshType.ToString();

      string rotated = (this.Rotated == true) ? " (rotated)" : "";

      return msh.Replace(" ", string.Empty) + rotated + ", c:" + cov.Replace(" ", string.Empty);
    }
    #endregion
  }
}
