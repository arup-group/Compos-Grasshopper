using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.ConcreteSlab
{
  /// <summary>
  /// Reinforcement object containing either a <see cref="MeshReinforcement"/> or a <see cref="TransverseReinforcement"/>
  /// </summary>
  public class Reinforcement
  {
    public MeshReinforcement Mesh { get; set; }
    public TransverseReinforcement Transverse { get; set; }
    internal enum ReinforcementType
    {
      Mesh,
      Transverse
    }
    internal ReinforcementType Type { get; set; }

    #region constructors
    public Reinforcement()
    {
      // empty constructor
    }
    public Reinforcement(MeshReinforcement mesh)
    {
      this.Mesh = mesh;
      this.Type = ReinforcementType.Mesh;
    }
    public Reinforcement(Length cover, MeshReinforcement.ReinforcementMeshType meshType = MeshReinforcement.ReinforcementMeshType.A393, bool rotated = false)
    {
      this.Mesh = new MeshReinforcement(cover, meshType, rotated);
      this.Type = ReinforcementType.Mesh;
    }
    public Reinforcement(TransverseReinforcement transverse)
    {
      this.Transverse = transverse;
      this.Type = ReinforcementType.Transverse;
    }
    public Reinforcement(ReinforcementMaterial material, Length distanceFromStart, Length distanceFromEnd, Length diameter, Length spacing, Length cover)
    {
      this.Transverse = new TransverseReinforcement(material, distanceFromStart, distanceFromEnd, diameter, spacing, cover);
      this.Type = ReinforcementType.Transverse;
    }

    #endregion

    #region coa interop
    internal Reinforcement(string coaString)
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

    public Reinforcement Duplicate()
    {
      if (this == null) { return null; }
      Reinforcement dup = (Reinforcement)this.MemberwiseClone();
      if (this.Type == ReinforcementType.Mesh)
        dup.Mesh = this.Mesh.Duplicate();
      if (this.Type == ReinforcementType.Transverse)
        dup.Transverse = this.Transverse.Duplicate();
      return dup;
    }

    public override string ToString()
    {
      switch (Type)
      {
        case ReinforcementType.Mesh: return this.Mesh.ToString();
        case ReinforcementType.Transverse: return this.Transverse.ToString();
        default: return base.ToString();
      }
    }

    #endregion
  }
}
