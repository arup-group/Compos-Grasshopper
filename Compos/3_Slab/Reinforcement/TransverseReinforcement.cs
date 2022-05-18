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
    internal static ITransverseReinforcement FromCoaString(List<string> parameters, Code code, ComposUnits units)
    {
      TransverseReinforcement reinforcement = new TransverseReinforcement();


      switch (parameters[0])
      {
        //case (CoaIdentifier.RebarMaterial):
        //  reinforcement.Material = ReinforcementMaterial.FromCoaString(line, code);
        //  break;

        //case (CoaIdentifier.RebarTransverse):
        //  reinforcement.CustomReinforcementLayouts.Add(CustomReinforcementLayout.FromCoaString(line, units));
        //  break;

      }


      if (parameters[1] == "PROGRAM_DESIGNED")
      {
        reinforcement.LayoutMethod = LayoutMethod.Automatic;


      }
      else
      {

      }
      return reinforcement;
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      string str = this.Material.ToCoaString(name);

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
