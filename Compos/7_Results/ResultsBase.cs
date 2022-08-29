using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Units;
using UnitsNet.Units;
using UnitsNet;

namespace ComposAPI
{
  

  public abstract class ResultsBase
  {
    public ResultsBase(Member member, int numIntermediatePos)
    {
      this.Member = member;
      this.NumIntermediatePos = numIntermediatePos;
    }
    internal Member Member;
    internal int NumIntermediatePos;
  }
}
