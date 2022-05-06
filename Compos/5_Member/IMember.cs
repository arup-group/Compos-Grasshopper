using System.Collections.Generic;

namespace ComposAPI
{
  public interface IMember
  {
    IBeam Beam { get; }
    IStud Stud { get; }
    ISlab Slab { get; }
    List<ILoad> Loads { get; }
    IDesignCode DesignCode { get; }
    string Name { get; }
    string GridReference { get; }
    string Note { get; }
  }
}
