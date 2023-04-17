using System;
using System.Collections.Generic;

namespace ComposAPI {
  public interface IComposFile {
    Guid Guid { get; }

    void AddMember(IMember member);

    short Analyse(string memberName);

    string BeamSectDesc(string memberName);

    short CodeSatisfied(string memberName);

    short Design(string memberName);

    IMember GetMember(string name);

    IList<IMember> GetMembers();

    string MemberName(int index);

    int SaveAs(string fileName);
  }
}
