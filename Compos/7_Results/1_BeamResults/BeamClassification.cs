using System;
using System.Collections.Generic;

namespace ComposAPI {
  public class BeamClassification : SubResult, IBeamClassification {
    /// <summary>
    /// Flange class in Final stage
    /// </summary>
    public List<string> Flange {
      get {
        ClassResultOption resultType = ClassResultOption.CLAS_FINA_FLAN_CLASS;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Flange class in Construction stage
    /// </summary>
    public List<string> FlangeConstruction {
      get {
        ClassResultOption resultType = ClassResultOption.CLAS_CONS_FLAN_CLASS;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Section class in Final stage
    /// </summary>
    public List<string> Section {
      get {
        ClassResultOption resultType = ClassResultOption.CLAS_FINA_SECTION;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Section class in Construction stage
    /// </summary>
    public List<string> SectionConstruction {
      get {
        ClassResultOption resultType = ClassResultOption.CLAS_CONS_SECTION;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Web class in Final stage
    /// </summary>
    public List<string> Web {
      get {
        ClassResultOption resultType = ClassResultOption.CLAS_FINA_WEB_CLASS;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Web class in Construction stage
    /// </summary>
    public List<string> WebConstruction {
      get {
        ClassResultOption resultType = ClassResultOption.CLAS_CONS_WEB_CLASS;
        return GetResults(resultType);
      }
    }

    private Dictionary<ClassResultOption, List<string>> ResultsCache = new Dictionary<ClassResultOption, List<string>>();

    public BeamClassification(Member member, int numIntermediatePos) : base(member, numIntermediatePos) {
    }

    private List<string> GetResults(ClassResultOption resultType) {
      if (!ResultsCache.ContainsKey(resultType)) {
        var results = new List<string>();
        for (short pos = 0; pos < NumIntermediatePos; pos++) {
          float value = Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));
          switch (Member.DesignCode.Code) {
            case Code.BS5950_3_1_1990_Superseded:
            case Code.BS5950_3_1_1990_A1_2010:
            case Code.HKSUOS_2005:
            case Code.HKSUOS_2011:
              if (resultType == ClassResultOption.CLAS_CONS_SECTION || resultType == ClassResultOption.CLAS_FINA_SECTION) {
                if (value == 1) {
                  results.Add("Plastic");
                } else if (value == 2) {
                  results.Add("Plastic reduced");
                } else if (value == 3) {
                  results.Add("Elastic");
                } else if (value == 4) {
                  results.Add("Elastic reduced");
                } else {
                  results.Add("Unknown");
                }
              } else {
                if (value == 1) {
                  results.Add("Plastic");
                } else if (value == 2) {
                  results.Add("Compact");
                } else if (value == 3) {
                  results.Add("Semi-compact");
                } else if (value == 4) {
                  results.Add("Slender");
                } else {
                  results.Add("Unknown");
                }
              }
              break;

            case Code.EN1994_1_1_2004:
              results.Add("Class " + Math.Round(value));
              break;

            case Code.AS_NZS2327_2017:
              if (value == 1) {
                results.Add("Compact");
              } else if (value == 2) {
                results.Add("Non compact");
              } else if (value == 3) {
                results.Add("Slender");
              } else if (value == 4) {
                results.Add("Deform");
              } else {
                results.Add("Unknown");
              }
              break;
          }
        }
        ResultsCache.Add(resultType, results);
      }
      return ResultsCache[resultType];
    }
  }

  internal enum ClassResultOption {
    CLAS_CONS_FLAN_CLASS, // Flange class in Construction stage
    CLAS_CONS_WEB_CLASS, // web class in Construction stage
    CLAS_CONS_SECTION, // Section class in Construction stage
    CLAS_FINA_FLAN_CLASS, // Flange class in Final stage
    CLAS_FINA_WEB_CLASS, // web class in Final stage
    CLAS_FINA_SECTION, // Section class in Final stage
  }
}
