using System.Collections.Generic;
using Oasys.Units;
using UnitsNet.Units;

namespace ComposAPI
{
  public interface IMember
  {
    IBeam Beam { get; }
    IStud Stud { get; }
    ISlab Slab { get; }
    IList<ILoad> Loads { get; }
    IDesignCode DesignCode { get; }
    IDesignCriteria DesignCriteria { get; }
    //IComposFile File { get; set; } // the hosting Compos file
    string Name { get; }
    string GridReference { get; }
    string Note { get; }
    string ToCoaString(ComposUnits units);

    short Analyse();
    short CodeSatisfied();
    string GetCodeSatisfiedMessage();
    float MaxResult(string option, short position);
    short MaxResultPosition(string option, short position);
    float MinResult(string option, short position);
    short MinResultPosition(string option, short position);
    short NumIntermediatePos();
    short NumTranRebar();
    void Register(IComposFile file);
    float Result(string option, short position);
    float TranRebarProp(TransverseRebarOption option, short rebarnum);
    float UtilisationFactor(UtilisationFactorOption option);
  }
}
