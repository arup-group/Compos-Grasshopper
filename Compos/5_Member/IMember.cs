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
    IResult Result { get; }
    //IComposFile File { get; set; } // the hosting Compos file
    string Name { get; }
    string GridReference { get; }
    string Note { get; }
    string ToCoaString(ComposUnits units);

    short Analyse();
    short CodeSatisfied();
    string GetCodeSatisfiedMessage();
    
    void Register(IComposFile file);
    
  }
}
