using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace ComposAPI.Decking
{
  public class CustomDeck : Deck
  {
    public Pressure Strength { get; set; }

    public CustomDeck()
    {
      this.m_type = DeckingType.Custom;
    }
    public CustomDeck(Length distanceB1, Length distanceB2, Length distanceB3, Length distanceB4, Length distanceB5, Length depth, Length thickness, Pressure stress, DeckingConfiguration dconf)
    {
      this.b1 = distanceB1;
      this.b2 = distanceB2;
      this.b3 = distanceB3;
      this.b4 = distanceB4;
      this.b5 = distanceB5;
      this.Depth = depth;
      this.Thickness = thickness;
      this.Strength = stress;
      this.DeckConfiguration = dconf;
      this.m_type = DeckingType.Custom;
    }

    public override Deck Duplicate()
    {
      if (this == null) { return null; }
      CustomDeck dup = (CustomDeck)this.MemberwiseClone();
      dup.DeckConfiguration = this.DeckConfiguration.Duplicate();
      return dup;
    }
    public override string ToString()
    {
      string distanceB1 = (this.b1.Value == 0) ? "" : "b1:" + this.b1.ToString().Replace(" ", string.Empty);
      string distanceB2 = (this.b2.Value == 0) ? "" : "b2:" + this.b2.ToString().Replace(" ", string.Empty);
      string distanceB3 = (this.b3.Value == 0) ? "" : "b3:" + this.b3.ToString().Replace(" ", string.Empty);
      string distanceB4 = (this.b4.Value == 0) ? "" : "b4:" + this.b4.ToString().Replace(" ", string.Empty);
      string distanceB5 = (this.b5.Value == 0) ? "" : "b5:" + this.b5.ToString().Replace(" ", string.Empty);
      string depth = (this.Depth.Value == 0) ? "" : "d:" + this.Depth.ToString().Replace(" ", string.Empty);
      string thickness = (this.Thickness.Value == 0) ? "" : "th:" + this.Thickness.ToString().Replace(" ", string.Empty);
      string stress = (this.Strength.Value == 0) ? "" : "stress:" + this.Strength.ToString().Replace(" ", string.Empty);

      string joined = string.Join(" ", new List<string>() { distanceB1, distanceB2, distanceB3, distanceB4, distanceB5, depth, thickness, stress });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
  }
}
