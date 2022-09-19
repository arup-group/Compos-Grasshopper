using ComposAPI.Helpers;
using System.Collections.Generic;
using OasysUnitsNet;
using OasysUnitsNet.Units;

namespace ComposAPI
{
  public class BeamSizeLimits : IBeamSizeLimits
  {
    public Length MinDepth { get; set; } = new Length(0.2, LengthUnit.Meter);
    public Length MaxDepth { get; set; } = new Length(1, LengthUnit.Meter);
    public Length MinWidth { get; set; } = new Length(0.1, LengthUnit.Meter);
    public Length MaxWidth { get; set; } = new Length(0.5, LengthUnit.Meter);

    public BeamSizeLimits() { }

    public BeamSizeLimits(double minDepth, double maxDepth, double minWidth, double maxWidth, LengthUnit lengthUnit)
    {
      this.MinDepth = new Length(minDepth, lengthUnit);
      this.MaxDepth = new Length(maxDepth, lengthUnit);
      this.MinWidth = new Length(minWidth, lengthUnit);
      this.MaxWidth = new Length(maxWidth, lengthUnit);
    }

    #region coa interop
    internal static IBeamSizeLimits FromCoaString(List<string> parameters, ComposUnits units)
    {
      LengthUnit unit = units.Section;

      BeamSizeLimits beamSizeLimit = new BeamSizeLimits();
      int i = 2;
      beamSizeLimit.MinDepth = new Length(CoaHelper.ConvertToDouble(parameters[i++]), unit);
      beamSizeLimit.MaxDepth = new Length(CoaHelper.ConvertToDouble(parameters[i++]), unit);
      beamSizeLimit.MinWidth = new Length(CoaHelper.ConvertToDouble(parameters[i++]), unit);
      beamSizeLimit.MaxWidth = new Length(CoaHelper.ConvertToDouble(parameters[i++]), unit);

      return beamSizeLimit;
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.DesignCriteria.BeamSizeLimit);
      parameters.Add(name);
      parameters.Add(CoaHelper.FormatSignificantFigures(this.MinDepth.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.MaxDepth.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.MinWidth.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.MaxWidth.ToUnit(units.Section).Value, 6));

      string coaString = CoaHelper.CreateString(parameters);

      return coaString;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string str = "";
      str += "Dmin:" + this.MinDepth.ToString("g7").Replace(" ", string.Empty);
      str += ", Dmax:" + this.MaxDepth.ToString("g7").Replace(" ", string.Empty);
      str += ", Wmin:" + this.MinWidth.ToString("g7").Replace(" ", string.Empty);
      str += ", Wmax:" + this.MaxWidth.ToString("g7").Replace(" ", string.Empty);
      return str;
    }
    #endregion
  }
}
