using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class DeckingConfiguration
  {
    public Angle Angle { get; set; }
    public bool IsDiscontinous { get; set; }
    public bool IsWelded { get; set; }
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
    public bool IsValid { get { return true; } }
    public DeckingConfiguration Duplicate()
    {
      if (this == null) { return null; }
      DeckingConfiguration dup = (DeckingConfiguration)this.MemberwiseClone();
      return dup;
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
