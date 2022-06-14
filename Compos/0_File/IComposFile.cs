using System.Collections.Generic;

namespace ComposAPI
{
  public interface IComposFile
  {
    IList<IMember> Members { get; }
    //string FileName { get; }

    short Analyse();
    short Analyse(string memberName);
    short CodeSatisfied(string memberName);
    short Design(string memberName);
    IMember GetMember(string name);
    float MaxResult(string memberName, ResultOption option, short position);
    short MaxResultPosition(string memberName, ResultOption option, short position);
    string MemberName(int index);
    float MinResult(string memberName, ResultOption option, short position);
    short MinResultPosition(string memberName, ResultOption option, short position);
    short NumIntermediatePos(string memberName);
    short NumTranRebar(string memberName);
    float Result(string memberName, ResultOption option, short position);
    int SaveAs(string fileName);
    string ToCoaString();
    float TranRebarProp(string memberName, TransverseRebarOption option, short rebarnum);
    float UtilisationFactor(string memberName, UtilisationFactorOption option);
  }
}
