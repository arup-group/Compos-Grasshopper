using System.Collections.Generic;

namespace ComposAPI
{
  public interface IComposFile
  {
    IList<IMember> Members { get; }
    string FileName { get; }

    short Analyze(string memberName);
    short CodeSatisfied(string memberName);
    short Design(string memberName);
    IMember GetMember(string name);
    int SaveAs(string fileName);
    string ToCoaString();
    float UtilisationFactor(string memberName, UtilisationFactorOption option);
  }
}
