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

  public class TransverseReinforcement : Reinforcement, ITransverseReinforcement, ICoaObject
  {
    public IReinforcementMaterial Material { get; set; }
    public LayoutMethod LayoutMethod { get; set; }
    List<ICustomTransverseReinforcementLayout> CustomReinforcementLayouts { get; set; }

    public TransverseReinforcement()
    {
      this.m_type = ReinforcementType.Transverse;
      this.LayoutMethod = LayoutMethod.Automatic;
    }

    public TransverseReinforcement(IReinforcementMaterial material)
    {
      this.Material = material;
      this.m_type = ReinforcementType.Transverse;
      this.LayoutMethod = LayoutMethod.Automatic;
    }

    public TransverseReinforcement(IReinforcementMaterial material, List<ICustomTransverseReinforcementLayout> transverseReinforcmentLayout)
    {
      this.Material = material;
      this.CustomReinforcementLayouts = transverseReinforcmentLayout;
      this.m_type = ReinforcementType.Transverse;
      this.LayoutMethod = LayoutMethod.Custom;
    }

    #region coa interop
    internal TransverseReinforcement(List<string> parameters)
    {

    }

    public new string ToCoaString()
    {
      return String.Empty;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string mat = this.Material.ToString();
      if (this.LayoutMethod == LayoutMethod.Automatic)
      {
        return mat + ", Automatic layout";
      }
      else
      {
        string rebar = string.Join(":", this.CustomReinforcementLayouts.Select(x => x.ToString()).ToList());
        return mat + ", " + rebar;
      }
    }
    #endregion
  }
}
