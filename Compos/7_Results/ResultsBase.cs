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
    public ResultsBase(Member member)
    {
      this.Member = member;
      this.NumIntermediatePos = member.NumIntermediatePos();

      this.Positions = new List<Length>();
      for (int i = 0; i < this.NumIntermediatePos; i++)
      {
        float value = this.Member.GetResult(ResultOption.CRITI_SECT_DIST.ToString(), Convert.ToInt16(i));
        Length pos = new Length(value, LengthUnit.Meter);
        this.Positions.Add(pos);
      }
    }

    internal Member Member;
    internal int NumIntermediatePos;
    public List<Length> Positions;
  }
}
