namespace ComposAPI
{
  interface IResult
  {
    float MaxResult(string option, short position);
    short MaxResultPosition( string option, short position);
    string MemberName(int index);
    float MinResult(string option, short position);
    short MinResultPosition(string option, short position);
    short NumIntermediatePos();
    short NumTranRebar();
    float Result(string option, short position);
    int SaveAs(string fileName);
    string ToCoaString();
    float TranRebarProp(TransverseRebarOption option, short rebarnum);
    float UtilisationFactor(UtilisationFactorOption option);
  }
}
