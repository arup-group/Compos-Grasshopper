using System.Collections.Generic;

namespace ComposAPI
{
  public interface IComposFile
  {
    List<IMember> Members { get; }

    string ToCoaString();
  }
}
