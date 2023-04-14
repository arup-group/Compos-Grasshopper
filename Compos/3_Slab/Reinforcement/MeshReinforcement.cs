using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using OasysUnits;

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
    public Length Cover { get; set; } // cover of mesh reinforcement
    public ReinforcementMeshType MeshType { get; set; } // name of mesh reinforcement
    public bool Rotated { get; set; } // direction of mesh reinforcement

    #region constructors
    public MeshReinforcement() { }

    public MeshReinforcement(Length cover, ReinforcementMeshType meshType = ReinforcementMeshType.A393, bool rotated = false)
    {
      Cover = cover;
      MeshType = meshType;
      Rotated = rotated;
    }
    #endregion

    #region coa interop
    internal static IMeshReinforcement FromCoaString(List<string> parameters, ComposUnits units)
    {
      MeshReinforcement reinforcement = new MeshReinforcement();

      reinforcement.MeshType = (ReinforcementMeshType)Enum.Parse(typeof(ReinforcementMeshType), parameters[2]);
      reinforcement.Cover = CoaHelper.ConvertToLength(parameters[3], units.Length);
      if (parameters[4] == "PARALLEL")
        reinforcement.Rotated = false;
      else
        reinforcement.Rotated = true;

      return reinforcement;
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.RebarMesh);
      parameters.Add(name);
      parameters.Add(MeshType.ToString());
      parameters.Add(CoaHelper.FormatSignificantFigures(Cover.ToUnit(units.Length).Value, 6));
      if (Rotated)
        parameters.Add("PERPENDICULAR");
      else
        parameters.Add("PARALLEL");

      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string cov = Cover.ToString("g4");
      string msh = MeshType.ToString();

      string rotated = (Rotated == true) ? " (rotated)" : "";

      return msh.Replace(" ", string.Empty) + rotated + ", c:" + cov.Replace(" ", string.Empty);
    }
    #endregion
  }
}
