using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public enum LayoutMethod
  {
    Automatic,
    Custom
  }

  public class TransverseReinforcement : Reinforcement, ITransverseReinforcement
  {
    public IReinforcementMaterial Material { get; set; }

    public LayoutMethod Layout { get { return m_layout; } }
    internal LayoutMethod m_layout;
    public TransverseReinforcement()
    {
      this.m_type = ReinforcementType.Transverse;
      this.m_layout = LayoutMethod.Automatic;
    }

    public TransverseReinforcement(IReinforcementMaterial material)
    {
      this.Material = material;
      this.m_type = ReinforcementType.Transverse;
      this.m_layout = LayoutMethod.Automatic;
    }

    public override string ToString()
    {
      string mat = this.Material.ToString();
      return mat + ", Automatic layout";
    }
  }
}
