using System.Collections.Generic;

namespace ComposAPI
{
  public interface IFile
  {
    List<IMember> Members { get; }

    string ToCoaString();
  }
}
