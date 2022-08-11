using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ComposAPI.Helpers;
using UnitsNet;

namespace ComposAPI
{
  public enum LayoutMethod
  {
    Automatic,
    Custom
  }

  public class TransverseReinforcement : ITransverseReinforcement, ICoaObject
  {
    public IReinforcementMaterial Material { get; set; } // reinforcement material grade
    public LayoutMethod LayoutMethod { get; set; }
    public IList<ICustomTransverseReinforcementLayout> CustomReinforcementLayouts { get; set; }

    public TransverseReinforcement()
    {
      this.LayoutMethod = LayoutMethod.Automatic;
    }

    public TransverseReinforcement(IReinforcementMaterial material)
    {
      this.Material = material;
      this.LayoutMethod = LayoutMethod.Automatic;
    }

    public TransverseReinforcement(IReinforcementMaterial material, List<ICustomTransverseReinforcementLayout> transverseReinforcmentLayout)
    {
      this.Material = material;
      this.LayoutMethod = LayoutMethod.Custom;
      this.CustomReinforcementLayouts = transverseReinforcmentLayout;
    }

    #region coa interop
    internal static ITransverseReinforcement FromCoaString(string coaString, string name, Code code, ComposUnits units)
    {
      TransverseReinforcement reinforcement = new TransverseReinforcement();
      reinforcement.CustomReinforcementLayouts = new List<ICustomTransverseReinforcementLayout>();

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          return reinforcement;

        if (parameters[0] == CoaIdentifier.UnitData)
          units.FromCoaString(parameters);

        if (parameters[1] != name)
          continue;

        switch (parameters[0])
        {
          case (CoaIdentifier.RebarMaterial):
            reinforcement.Material = ReinforcementMaterial.FromCoaString(parameters, code);
            break;

          case (CoaIdentifier.RebarTransverse):
            if (parameters[2] == "PROGRAM_DESIGNED")
            {
              reinforcement.LayoutMethod = LayoutMethod.Automatic;
            }
            else
            {
              reinforcement.LayoutMethod = LayoutMethod.Custom;
              reinforcement.CustomReinforcementLayouts.Add(CustomTransverseReinforcementLayout.FromCoaString(parameters, units));
            }
            break;

          default:
            // continue;
            break;
        }
      }
      return reinforcement;
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      string str = this.Material.ToCoaString(name);
      
      str += "REBAR_LONGITUDINAL" + '\t' + name + '\t' + "PROGRAM_DESIGNED" + '\n';
      
      if (LayoutMethod == LayoutMethod.Automatic)
      {
        List<string> parameters = new List<string>();
        parameters.Add(CoaIdentifier.RebarTransverse);
        parameters.Add(name);
        parameters.Add("PROGRAM_DESIGNED");
        str += CoaHelper.CreateString(parameters);
      }
      else
      {
        foreach (ICustomTransverseReinforcementLayout layout in this.CustomReinforcementLayouts)
        {
          str += layout.ToCoaString(name, units);
        }
      }
      return str;
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
