using System.Collections.Generic;

namespace ComposAPI {
  public interface IMember {
    IBeam Beam { get; }
    IDesignCode DesignCode { get; }
    IDesignCriteria DesignCriteria { get; }
    string GridReference { get; }
    IList<ILoad> Loads { get; }
    //IComposFile File { get; set; } // the hosting Compos file
    string Name { get; }
    string Note { get; }
    IResult Result { get; }
    ISlab Slab { get; }
    IStud Stud { get; }

    short Analyse();

    short CodeSatisfied();

    string GetCodeSatisfiedMessage();

    string ToCoaString(ComposUnits units);

    //void Register(ComposFile file);
  }
}
