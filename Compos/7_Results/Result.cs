using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComposAPI
{
  public class Result : IResult
  {
    public float MaxResult(string option, short position)
    {
      throw new NotImplementedException();
    }

    public short MaxResultPosition(string option, short position)
    {
      throw new NotImplementedException();
    }

    public string MemberName(int index)
    {
      throw new NotImplementedException();
    }

    public float MinResult(string option, short position)
    {
      throw new NotImplementedException();
    }

    public short MinResultPosition(string option, short position)
    {
      throw new NotImplementedException();
    }

    public short NumIntermediatePos()
    {
      throw new NotImplementedException();
    }

    public short NumTranRebar()
    {
      throw new NotImplementedException();
    }

    public int SaveAs(string fileName)
    {
      throw new NotImplementedException();
    }

    public string ToCoaString()
    {
      throw new NotImplementedException();
    }

    public float TranRebarProp(TransverseRebarOption option, short rebarnum)
    {
      throw new NotImplementedException();
    }

    public float UtilisationFactor(UtilisationFactorOption option)
    {
      throw new NotImplementedException();
    }

    float IResult.Result(string option, short position)
    {
      throw new NotImplementedException();
    }
  }
}
