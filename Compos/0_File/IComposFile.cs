using System;
using System.Collections.Generic;

namespace ComposAPI
{
  public interface IComposFile
  {
    Guid Guid { get; }
    void AddMember(IMember member);
    short Analyse(string memberName);
    short CodeSatisfied(string memberName);
    IList<IMember> GetMembers();
    short Design(string memberName);
    string BeamSectDesc(string memberName);
    IMember GetMember(string name);
    string MemberName(int index);
    int SaveAs(string fileName);
  }
}
