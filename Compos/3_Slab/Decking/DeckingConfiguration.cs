using System.Collections.Generic;
using OasysUnits;
using OasysUnits.Units;

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
      Angle = new Angle(90, AngleUnit.Degree);
      IsDiscontinous = false;
      IsWelded = false;
    }

    public DeckingConfiguration(Angle angle, bool isDiscontinous, bool isWelded)
    {
      Angle = angle;
      IsDiscontinous = isDiscontinous;
      IsWelded = isWelded;
    }

    public override string ToString()
    {
      string angle = (Angle.Value == 0) ? "" : Angle.ToUnit(AngleUnit.Degree).ToString().Replace(" ", string.Empty);
      string isDiscontinous = (IsDiscontinous == true) ? "" : IsDiscontinous.ToString().Replace(" ", string.Empty);
      string isWelded = (IsWelded == true) ? "" : IsWelded.ToString().Replace(" ", string.Empty);

      string joined = string.Join(" ", new List<string>() { angle, isDiscontinous, isWelded });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
  }
}
