using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;
using System.Collections.Generic;

namespace ComposAPI {
  public class BeamSizeLimits : IBeamSizeLimits {
    public Length MaxDepth { get; set; } = new Length(1, LengthUnit.Meter);
    public Length MaxWidth { get; set; } = new Length(0.5, LengthUnit.Meter);
    public Length MinDepth { get; set; } = new Length(0.2, LengthUnit.Meter);
    public Length MinWidth { get; set; } = new Length(0.1, LengthUnit.Meter);

    public BeamSizeLimits() { }

    public BeamSizeLimits(double minDepth, double maxDepth, double minWidth, double maxWidth, LengthUnit lengthUnit) {
      MinDepth = new Length(minDepth, lengthUnit);
      MaxDepth = new Length(maxDepth, lengthUnit);
      MinWidth = new Length(minWidth, lengthUnit);
      MaxWidth = new Length(maxWidth, lengthUnit);
    }

    public string ToCoaString(string name, ComposUnits units) {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.DesignCriteria.BeamSizeLimit);
      parameters.Add(name);
      parameters.Add(CoaHelper.FormatSignificantFigures(MinDepth.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(MaxDepth.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(MinWidth.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(MaxWidth.ToUnit(units.Section).Value, 6));

      string coaString = CoaHelper.CreateString(parameters);

      return coaString;
    }

    public override string ToString() {
      string str = "";
      str += "Dmin:" + MinDepth.ToString("g7").Replace(" ", string.Empty);
      str += ", Dmax:" + MaxDepth.ToString("g7").Replace(" ", string.Empty);
      str += ", Wmin:" + MinWidth.ToString("g7").Replace(" ", string.Empty);
      str += ", Wmax:" + MaxWidth.ToString("g7").Replace(" ", string.Empty);
      return str;
    }

    internal static IBeamSizeLimits FromCoaString(List<string> parameters, ComposUnits units) {
      LengthUnit unit = units.Section;

      BeamSizeLimits beamSizeLimit = new BeamSizeLimits();
      int i = 2;
      beamSizeLimit.MinDepth = new Length(CoaHelper.ConvertToDouble(parameters[i++]), unit);
      beamSizeLimit.MaxDepth = new Length(CoaHelper.ConvertToDouble(parameters[i++]), unit);
      beamSizeLimit.MinWidth = new Length(CoaHelper.ConvertToDouble(parameters[i++]), unit);
      beamSizeLimit.MaxWidth = new Length(CoaHelper.ConvertToDouble(parameters[i++]), unit);

      return beamSizeLimit;
    }
  }
}
