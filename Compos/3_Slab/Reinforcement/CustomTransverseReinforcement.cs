using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public class CustomTransverseReinforcementLayout : ICustomTransverseReinforcementLayout
  {
    public Length DistanceFromStart { get; set; }
    public Length DistanceFromEnd { get; set; }
    public Length Diameter { get; set; }
    public Length Spacing { get; set; }
    public Length Cover { get; set; }

    public CustomTransverseReinforcementLayout() { }

    public CustomTransverseReinforcementLayout(Length distanceFromStart, Length distanceFromEnd, Length diameter, Length spacing, Length cover)
    {
      this.DistanceFromStart = distanceFromStart;
      this.DistanceFromEnd = distanceFromEnd;
      this.Diameter = diameter;
      this.Spacing = spacing;
      this.Cover = cover;
    }

    public override string ToString()
    {
      string start = (this.DistanceFromStart.Value == 0) ? "" : this.DistanceFromStart.ToUnit(Units.LengthUnitGeometry).ToString("f2").Replace(" ", string.Empty) + "<-";
      string end = (this.DistanceFromEnd.Value == 0) ? "" : "->" + this.DistanceFromEnd.ToUnit(Units.LengthUnitGeometry).ToString("f2").Replace(" ", string.Empty);
      string startend = start + end;
      startend = startend.Replace("--", "-").Replace(",", string.Empty);
      string dia = "Ø" + this.Diameter.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string spacing = "/" + this.Spacing.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string cov = ", c:" + this.Cover.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string diaspacingcov = dia + spacing + cov;
      string joined = string.Join(" ", new List<string>() { startend, diaspacingcov });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
  }

  public class CustomTransverseReinforcement : TransverseReinforcement
  {
    List<ICustomTransverseReinforcementLayout> CustomReinforcementLayouts { get; set; }
    
    public CustomTransverseReinforcement()
    {
      this.m_type = ReinforcementType.Transverse;
      this.m_layout = LayoutMethod.Custom;
    }

    public CustomTransverseReinforcement(IReinforcementMaterial material, List<ICustomTransverseReinforcementLayout> transverseReinforcmentLayout)
    {
      this.Material = material;
      this.CustomReinforcementLayouts = transverseReinforcmentLayout;
      this.m_type = ReinforcementType.Transverse;
      this.m_layout = LayoutMethod.Custom;
    }

    public override string ToString()
    {
      string rebar = string.Join(":", this.CustomReinforcementLayouts.Select(x => x.ToString()).ToList());
      string mat = this.Material.ToString();
      return mat + ", " + rebar;
    }
  }
}
