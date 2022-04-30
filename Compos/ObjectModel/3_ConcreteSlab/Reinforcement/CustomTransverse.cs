using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.ConcreteSlab
{
  public class TransverseReinforcmentLayout
  {
    public Length DistanceFromStart { get; set; }
    public Length DistanceFromEnd { get; set; }
    public Length Diameter { get; set; }
    public Length Spacing { get; set; }
    public Length Cover { get; set; }
    public TransverseReinforcmentLayout() { }
    public TransverseReinforcmentLayout(Length distanceFromStart, Length distanceFromEnd, Length diameter, Length spacing, Length cover)
    {
      this.DistanceFromStart = distanceFromStart;
      this.DistanceFromEnd = distanceFromEnd;
      this.Diameter = diameter;
      this.Spacing = spacing;
      this.Cover = cover;
    }
    public TransverseReinforcmentLayout Duplicate()
    {
      return (TransverseReinforcmentLayout)this.MemberwiseClone();
    }
    public override string ToString()
    {
      string start = (this.DistanceFromStart.Value == 0) ? "" : this.DistanceFromStart.ToUnit(Helpers.Units.FileUnits.LengthUnitGeometry).ToString("f2").Replace(" ", string.Empty) + "<-";
      string end = (this.DistanceFromEnd.Value == 0) ? "" : "->" + this.DistanceFromEnd.ToUnit(Helpers.Units.FileUnits.LengthUnitGeometry).ToString("f2").Replace(" ", string.Empty);
      string startend = start + end;
      startend = startend.Replace("--", "-").Replace(",", string.Empty);
      string dia = "Ø" + this.Diameter.ToUnit(Helpers.Units.FileUnits.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string spacing = "/" + this.Spacing.ToUnit(Helpers.Units.FileUnits.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string cov = ", c:" + this.Cover.ToUnit(Helpers.Units.FileUnits.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string diaspacingcov = dia + spacing + cov;
      string joined = string.Join(" ", new List<string>() { startend, diaspacingcov });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
  }
  public class CustomTransverseReinforcement : TransverseReinforcement
  {
    List<TransverseReinforcmentLayout> CustomReinforcementLayouts { get; set; }
    
    public CustomTransverseReinforcement()
    {
      this.m_type = ReinforcementType.Transverse;
      this.m_layout = LayoutMethod.Custom;
    }

    public CustomTransverseReinforcement(ReinforcementMaterial material, List<TransverseReinforcmentLayout> transverseReinforcmentLayout)
    {
      this.Material = material;
      this.CustomReinforcementLayouts = transverseReinforcmentLayout;
      this.m_type = ReinforcementType.Transverse;
      this.m_layout = LayoutMethod.Custom;
    }

    public override Reinforcement Duplicate()
    {
      if (this == null) { return null; }
      CustomTransverseReinforcement dup = (CustomTransverseReinforcement)this.MemberwiseClone();
      dup.Material = this.Material.Duplicate();
      dup.CustomReinforcementLayouts = this.CustomReinforcementLayouts.ToList();
      return dup;
    }
    public override string ToString()
    {
      string rebar = string.Join(":", this.CustomReinforcementLayouts.Select(x => x.ToString()).ToList());
      string mat = this.Material.ToString();
      return mat + ", " + rebar;
    }
  }
}
