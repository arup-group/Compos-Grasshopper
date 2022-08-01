using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  // name linked with coa string naming, do not change!
  public enum OptimiseOption
  {
    MINIMUM_WEIGHT,
    MINIMUM_DEPTH
  }
  public class DesignCriteria : IDesignCriteria
  {
    // beam size
    public IBeamSizeLimits BeamSizeLimits { get; set; }
    public IList<int> CatalogueSectionTypes { get; set; }
    public OptimiseOption OptimiseOption { get; set; } = OptimiseOption.MINIMUM_WEIGHT;

    // deflection limits
    public IDeflectionLimit ConstructionDeadLoad { get; set; } = null;
    public IDeflectionLimit AdditionalDeadLoad { get; set; } = null;
    public IDeflectionLimit FinalLiveLoad { get; set; } = null;
    public IDeflectionLimit TotalLoads { get; set; } = null;
    public IDeflectionLimit PostConstruction { get; set; } = null;

    public IFrequencyLimits FrequencyLimits { get; set; } = null;

    public DesignCriteria() { }

    public DesignCriteria(BeamSizeLimits beamSizeLimits, OptimiseOption optimiseOption, List<int> catalogues, DeflectionLimit constructionDL = null, DeflectionLimit additionalDL = null, DeflectionLimit finalLL = null, DeflectionLimit total = null, DeflectionLimit postConstr = null, FrequencyLimits frequencyLimits = null)
    {
      this.BeamSizeLimits = beamSizeLimits;
      this.OptimiseOption = optimiseOption;
      this.CatalogueSectionTypes = catalogues;
      this.ConstructionDeadLoad = constructionDL;
      this.AdditionalDeadLoad = additionalDL;
      this.FinalLiveLoad = finalLL;
      this.TotalLoads = total;
      this.PostConstruction = postConstr;
      this.FrequencyLimits = frequencyLimits;
    }
    
    #region coa interop
    internal static IDesignCriteria FromCoaString(string coaString, string name, ComposUnits units)
    {
      DesignCriteria designCrit = new DesignCriteria();

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          return designCrit;

        if (parameters[0] == CoaIdentifier.UnitData)
          units.FromCoaString(parameters);

        if (parameters[1] != name)
          continue;

        switch (parameters[0])
        {
          case CoaIdentifier.DesignCriteria.DeflectionLimit:
            DeflectionLimitLoadType type = (DeflectionLimitLoadType)Enum.Parse(typeof(DeflectionLimitLoadType), parameters[2]);
            switch (type)
            {
              case DeflectionLimitLoadType.CONSTRUCTION_DEAD_LOAD:
                designCrit.ConstructionDeadLoad = DeflectionLimit.FromCoaString(coaString, name, type, units);
                break;
              case DeflectionLimitLoadType.ADDITIONAL_DEAD_LOAD:
                designCrit.AdditionalDeadLoad = DeflectionLimit.FromCoaString(coaString, name, type, units);
                break;
              case DeflectionLimitLoadType.FINAL_LIVE_LOAD:
                designCrit.FinalLiveLoad = DeflectionLimit.FromCoaString(coaString, name, type, units);
                break;
              case DeflectionLimitLoadType.TOTAL:
                designCrit.TotalLoads = DeflectionLimit.FromCoaString(coaString, name, type, units);
                break;
              case DeflectionLimitLoadType.POST_CONSTRUCTION:
                designCrit.PostConstruction = DeflectionLimit.FromCoaString(coaString, name, type, units);
                break;
            }
            break;

          case CoaIdentifier.DesignCriteria.BeamSizeLimit:
            designCrit.BeamSizeLimits = ComposAPI.BeamSizeLimits.FromCoaString(parameters, units);
            break;
          
          case CoaIdentifier.DesignCriteria.OptimiseOption:
            designCrit.OptimiseOption = (OptimiseOption)Enum.Parse(typeof(OptimiseOption), parameters[2]);
            break;
          
          case CoaIdentifier.DesignCriteria.SectionType:
            List<string> cats = parameters[2].Split(' ').ToList();
            List<int> catalogueIDs = new List<int>();
            foreach (string cat in cats)
              catalogueIDs.Add(int.Parse(cat));
            designCrit.CatalogueSectionTypes = catalogueIDs;
            break;

          case CoaIdentifier.DesignCriteria.Frequency:
            if (parameters[2] == "CHECK_NATURAL_FREQUENCY")
              designCrit.FrequencyLimits = ComposAPI.FrequencyLimits.FromCoaString(parameters);
            break;
        }
      }
      return designCrit;
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      string coaString = "";

      if (this.ConstructionDeadLoad != null)
        coaString += this.ConstructionDeadLoad.ToCoaString(name, DeflectionLimitLoadType.CONSTRUCTION_DEAD_LOAD, units);
      if (this.AdditionalDeadLoad != null)
        coaString += this.AdditionalDeadLoad.ToCoaString(name, DeflectionLimitLoadType.ADDITIONAL_DEAD_LOAD, units);
      if (this.FinalLiveLoad != null)
        coaString += this.FinalLiveLoad.ToCoaString(name, DeflectionLimitLoadType.FINAL_LIVE_LOAD, units);
      if (this.TotalLoads != null)
        coaString += this.TotalLoads.ToCoaString(name, DeflectionLimitLoadType.TOTAL, units);
      if (this.PostConstruction != null)
        coaString += this.PostConstruction.ToCoaString(name, DeflectionLimitLoadType.POST_CONSTRUCTION, units);

      coaString += this.BeamSizeLimits.ToCoaString(name, units);

      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.DesignCriteria.OptimiseOption);
      parameters.Add(name);
      parameters.Add((this.OptimiseOption == OptimiseOption.MINIMUM_WEIGHT ? "MINIMUM_WEIGHT" : "MINIMUM_DEPTH"));
      coaString += CoaHelper.CreateString(parameters);

      parameters = new List<string>();
      parameters.Add(CoaIdentifier.DesignCriteria.SectionType);
      parameters.Add(name);
      parameters.Add(string.Join(" ", this.CatalogueSectionTypes));
      coaString += CoaHelper.CreateString(parameters);

      if (this.FrequencyLimits != null)
      {
        coaString += this.FrequencyLimits.ToCoaString(name);
      }

      return coaString;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      return "DesignCriteria";
    }
    #endregion
  }
}
