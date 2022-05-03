using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public class TransverseReinforcement : Reinforcement
  {
    public enum LayoutMethod
    {
      Automatic,
      Custom
    }

    public ReinforcementMaterial Material { get; set; }
    public const string CoaIdentifier = "REBAR_TRANSVERSE";
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

    #region coa interop
    internal TransverseReinforcement(List<string> parameters)
    {
    
    }

    internal string ToCoaString(string name)
    {
      return String.Empty;
    }
    #endregion

    #region methods
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
    #endregion
  }
}
