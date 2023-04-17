using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComposAPI {
  public class DesignCriteria : IDesignCriteria {
    public IDeflectionLimit AdditionalDeadLoad { get; set; } = null;
    // beam size
    public IBeamSizeLimits BeamSizeLimits { get; set; } = new BeamSizeLimits();
    public IList<int> CatalogueSectionTypes {
      get { return m_CatalogueSections; }
      set {
        m_CatalogueSections = value;
        CheckCatalogueTypeIDs();
      }
    }
    // deflection limits
    public IDeflectionLimit ConstructionDeadLoad { get; set; } = null;
    public IDeflectionLimit FinalLiveLoad { get; set; } = null;
    public IFrequencyLimits FrequencyLimits { get; set; } = null;
    public OptimiseOption OptimiseOption { get; set; } = OptimiseOption.MinimumWeight;
    public IDeflectionLimit PostConstruction { get; set; } = null;
    public IDeflectionLimit TotalLoads { get; set; } = null;
    private IList<int> m_CatalogueSections = new List<int>();

    public DesignCriteria() { }

    public DesignCriteria(BeamSizeLimits beamSizeLimits, OptimiseOption optimiseOption, List<int> catalogues, DeflectionLimit constructionDL = null, DeflectionLimit additionalDL = null, DeflectionLimit finalLL = null, DeflectionLimit total = null, DeflectionLimit postConstr = null, FrequencyLimits frequencyLimits = null) {
      BeamSizeLimits = beamSizeLimits;
      OptimiseOption = optimiseOption;
      CatalogueSectionTypes = catalogues;
      CheckCatalogueTypeIDs();
      ConstructionDeadLoad = constructionDL;
      AdditionalDeadLoad = additionalDL;
      FinalLiveLoad = finalLL;
      TotalLoads = total;
      PostConstruction = postConstr;
      FrequencyLimits = frequencyLimits;
    }

    public string ToCoaString(string name, ComposUnits units) {
      string coaString = "";

      if (ConstructionDeadLoad != null)
        coaString += ConstructionDeadLoad.ToCoaString(name, DeflectionLimitLoadType.ConstructionDeadLoad, units);
      if (AdditionalDeadLoad != null)
        coaString += AdditionalDeadLoad.ToCoaString(name, DeflectionLimitLoadType.AdditionalDeadLoad, units);
      if (FinalLiveLoad != null)
        coaString += FinalLiveLoad.ToCoaString(name, DeflectionLimitLoadType.FinalLiveLoad, units);
      if (TotalLoads != null)
        coaString += TotalLoads.ToCoaString(name, DeflectionLimitLoadType.Total, units);
      if (PostConstruction != null)
        coaString += PostConstruction.ToCoaString(name, DeflectionLimitLoadType.PostConstruction, units);

      coaString += BeamSizeLimits.ToCoaString(name, units);

      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.DesignCriteria.OptimiseOption);
      parameters.Add(name);
      parameters.Add(GetOptionCoaString(OptimiseOption));
      coaString += CoaHelper.CreateString(parameters);

      parameters = new List<string>();
      parameters.Add(CoaIdentifier.DesignCriteria.SectionType);
      parameters.Add(name);
      parameters.Add(string.Join(" ", CatalogueSectionTypes));
      coaString += CoaHelper.CreateString(parameters);

      if (FrequencyLimits != null) {
        coaString += FrequencyLimits.ToCoaString(name);
      }

      return coaString;
    }

    public override string ToString() {
      return "DesignCriteria";
    }

    internal static IDesignCriteria FromCoaString(string coaString, string name, ComposUnits units) {
      DesignCriteria designCrit = new DesignCriteria();

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines) {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          return designCrit;

        if (parameters[0] == CoaIdentifier.UnitData)
          units.FromCoaString(parameters);

        if (parameters[1] != name)
          continue;

        switch (parameters[0]) {
          case CoaIdentifier.DesignCriteria.DeflectionLimit:
            DeflectionLimitLoadType type = DeflectionLimit.GetLoadType(parameters[2]);
            switch (type) {
              case DeflectionLimitLoadType.ConstructionDeadLoad:
                designCrit.ConstructionDeadLoad = DeflectionLimit.FromCoaString(coaString, name, type, units);
                break;

              case DeflectionLimitLoadType.AdditionalDeadLoad:
                designCrit.AdditionalDeadLoad = DeflectionLimit.FromCoaString(coaString, name, type, units);
                break;

              case DeflectionLimitLoadType.FinalLiveLoad:
                designCrit.FinalLiveLoad = DeflectionLimit.FromCoaString(coaString, name, type, units);
                break;

              case DeflectionLimitLoadType.Total:
                designCrit.TotalLoads = DeflectionLimit.FromCoaString(coaString, name, type, units);
                break;

              case DeflectionLimitLoadType.PostConstruction:
                designCrit.PostConstruction = DeflectionLimit.FromCoaString(coaString, name, type, units);
                break;
            }
            break;

          case CoaIdentifier.DesignCriteria.BeamSizeLimit:
            designCrit.BeamSizeLimits = ComposAPI.BeamSizeLimits.FromCoaString(parameters, units);
            break;

          case CoaIdentifier.DesignCriteria.OptimiseOption:
            designCrit.OptimiseOption = GetOption(parameters[2]);
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

    internal static OptimiseOption GetOption(string coaString) {
      switch (coaString) {
        case "MINIMUM_DEPTH":
          return OptimiseOption.MinimumHeight;

        case "MINIMUM_WEIGHT":
          return OptimiseOption.MinimumWeight;
      }
      return OptimiseOption.MinimumWeight;
    }

    internal static string GetOptionCoaString(OptimiseOption type) {
      switch (type) {
        case OptimiseOption.MinimumHeight:
          return "MINIMUM_DEPTH";

        case OptimiseOption.MinimumWeight:
          return "MINIMUM_WEIGHT";
      }
      return null;
    }

    internal void CheckCatalogueTypeIDs() {
      foreach (int id in m_CatalogueSections) {
        if (!CatalogueSectionType.CatalogueSectionTypes.ContainsKey(id))
          throw new Exception("Catalogue Section Type of ID: " + id + " does not exist in Compos Catalogue Section Library");
      }
    }
  }

  public enum OptimiseOption {
    MinimumWeight,
    MinimumHeight
  }
}
