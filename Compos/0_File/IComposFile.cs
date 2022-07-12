using System.Collections.Generic;

namespace ComposAPI
{
  public interface IComposFile
  {
    //string FileName { get; }

    void AddMember(IMember member);
    //short Analyse();
    short Analyse(string memberName);
    short CodeSatisfied(string memberName);
    //short Design();
    short Design(string memberName);
    IMember GetMember(string name);
    float MaxResult(string memberName, string option, short position);
    short MaxResultPosition(string memberName, string option, short position);
    string MemberName(int index);
    float MinResult(string memberName, string option, short position);
    short MinResultPosition(string memberName, string option, short position);
    short NumIntermediatePos(string memberName);
    short NumTranRebar(string memberName);
    float Result(string memberName, string option, short position);
    int SaveAs(string fileName);
    string ToCoaString();
    float TranRebarProp(string memberName, TransverseRebarOption option, short rebarnum);
    float UtilisationFactor(string memberName, UtilisationFactorOption option);
  }
}
