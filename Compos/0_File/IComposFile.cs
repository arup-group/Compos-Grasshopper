using System.Collections.Generic;

namespace ComposAPI
{
  public interface IComposFile
  {
    IList<IMember> Members { get; }
    string FileName { get; }
    string ToCoaString();
  }
}
