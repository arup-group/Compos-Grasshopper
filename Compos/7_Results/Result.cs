using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComposAPI
{
  public class Result : IResult
  {
    internal Result(Member member)
    {
      this.m_Member = member;
    }
    private Member m_Member;

    public IInternalForceResult InternalForces
    {
      get
      {
        if (m_ForcesAndMoments == null)
          m_ForcesAndMoments = new InternalForceResult(m_Member);
        return m_ForcesAndMoments;
      }
    }
    private InternalForceResult m_ForcesAndMoments;
    
  }
}
