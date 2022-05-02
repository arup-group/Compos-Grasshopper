using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public class TransverseReinforcement : Reinforcement
  {
    public ReinforcementMaterial Material { get; set; }

    public enum LayoutMethod
    {
      Automatic,
      Custom
    }
    public LayoutMethod Layout { get { return m_layout; } }
    internal LayoutMethod m_layout;
    public TransverseReinforcement()
    {
      this.m_type = ReinforcementType.Transverse;
      this.m_layout = LayoutMethod.Automatic;
    }

    public TransverseReinforcement(ReinforcementMaterial material)
    {
      this.Material = material;
      this.m_type = ReinforcementType.Transverse;
      this.m_layout = LayoutMethod.Automatic;
    }

    public override Reinforcement Duplicate()
    {
      if (this == null) { return null; }
      TransverseReinforcement dup = (TransverseReinforcement)this.MemberwiseClone();
      dup.Material = this.Material.Duplicate();
      return dup;
    }
    public override string ToString()
    {
      string mat = this.Material.ToString();
      return mat + ", Automatic layout";
    }
  }
}
