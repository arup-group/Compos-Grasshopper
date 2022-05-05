using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class CustomDecking : Decking
  {
    public Pressure Strength { get; set; } // decking material characteristic strength
    public CustomDecking()
    {
      this.m_type = DeckingType.Custom;
    }

    public CustomDecking(Length distanceB1, Length distanceB2, Length distanceB3, Length distanceB4, Length distanceB5, Length depth, Length thickness, Pressure stress, IDeckingConfiguration dconf)
    {
      this.b1 = distanceB1;
      this.b2 = distanceB2;
      this.b3 = distanceB3;
      this.b4 = distanceB4;
      this.b5 = distanceB5;
      this.Depth = depth;
      this.Thickness = thickness;
      this.Strength = stress;
      this.DeckingConfiguration = dconf;
      this.m_type = DeckingType.Custom;
    }

    #region coa interop
    internal CustomDecking(List<string> parameters, AngleUnit angleUnit, LengthUnit lengthUnit, PressureUnit pressureUnit)
    {
      this.Strength = new Pressure(Convert.ToDouble(parameters[3]), pressureUnit);
      DeckingConfiguration deckingConfiguration = new DeckingConfiguration();
      deckingConfiguration.Angle = new Angle(Convert.ToDouble(parameters[4]), angleUnit);
      this.b1 = new Length(Convert.ToDouble(parameters[5]), lengthUnit);
      this.b2 = new Length(Convert.ToDouble(parameters[6]), lengthUnit);
      this.b3 = new Length(Convert.ToDouble(parameters[7]), lengthUnit);
      this.Depth =  new Length(Convert.ToDouble(parameters[8]), lengthUnit);
      this.Thickness  = new Length(Convert.ToDouble(parameters[9]), lengthUnit);
      this.b4 = new Length(Convert.ToDouble(parameters[10]), lengthUnit);
      this.b5 = new Length(Convert.ToDouble(parameters[11]), lengthUnit);

      if (parameters[12] == "DECKING_JOINTED")
        deckingConfiguration.IsDiscontinous = true;
      else
        deckingConfiguration.IsDiscontinous = false;

      if (parameters[12] == "JOINT_WELDED")
        deckingConfiguration.IsWelded = true;
      else
        deckingConfiguration.IsWelded = false;
      this.DeckingConfiguration = deckingConfiguration;
    }

    internal override string ToCoaString(string name, AngleUnit angleUnit, LengthUnit lengthUnit, PressureUnit pressureUnit)
    {
      List<string> parameters = new List<string>();
      parameters.Add("DECKING_USER");
      parameters.Add(name);

      // NO_DECKING ??
      parameters.Add("USER_DEFINED");
      parameters.Add(this.Strength.ToUnit(pressureUnit).ToString());
      parameters.Add(this.DeckingConfiguration.Angle.ToUnit(angleUnit).ToString());
      parameters.Add(this.b1.ToUnit(lengthUnit).ToString());
      parameters.Add(this.b2.ToUnit(lengthUnit).ToString());
      parameters.Add(this.b3.ToUnit(lengthUnit).ToString());
      parameters.Add(this.Depth.ToUnit(lengthUnit).ToString());
      parameters.Add(this.Thickness.ToUnit(lengthUnit).ToString());
      parameters.Add(this.b4.ToUnit(lengthUnit).ToString());
      parameters.Add(this.b5.ToUnit(lengthUnit).ToString());

      if (this.DeckingConfiguration.IsDiscontinous)
        parameters.Add("DECKING_JOINTED");
      else
        parameters.Add("DECKING_CONTINUED");

      if (this.DeckingConfiguration.IsWelded)
        parameters.Add("JOINT_WELDED");
      else
        parameters.Add("JOINT_NOT_WELD");

      return CoaHelper.CreateString(parameters);
    }
    #endregion

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
