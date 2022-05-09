using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace ComposAPI
{
  public class DeckingConfiguration : IDeckingConfiguration
  {
    public Angle Angle { get; set; } // decking angle relative to steel beam in degrees
    public bool IsDiscontinous { get; set; } // is decking jointed
    public bool IsWelded { get; set; } // 	is joint welded
    public bool IsValid { get { return true; } }

    public DeckingConfiguration()
    {
      // default values:
      this.Angle = new Angle(90, UnitsNet.Units.AngleUnit.Degree);
      this.IsDiscontinous = false;
      this.IsWelded = false;
    }

    public DeckingConfiguration(Angle angle, bool isDiscontinous, bool isWelded)
    {
      this.Angle = angle;
      this.IsDiscontinous = isDiscontinous;
      this.IsWelded = isWelded;
    }

    public override string ToString()
    {
      string angle = (this.Angle.Value == 0) ? "" : this.Angle.ToUnit(UnitsNet.Units.AngleUnit.Degree).ToString().Replace(" ", string.Empty);
      string isDiscontinous = (this.IsDiscontinous == true) ? "" : this.IsDiscontinous.ToString().Replace(" ", string.Empty);
      string isWelded = (this.IsWelded == true) ? "" : this.IsWelded.ToString().Replace(" ", string.Empty);

      string joined = string.Join(" ", new List<string>() { angle, isDiscontinous, isWelded });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
  }
}
