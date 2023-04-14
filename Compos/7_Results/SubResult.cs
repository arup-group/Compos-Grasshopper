
namespace ComposAPI
{
  public abstract class SubResult
  {
    public SubResult(Member member, int numIntermediatePos)
    {
      Member = member;
      NumIntermediatePos = numIntermediatePos;
    }
    internal Member Member;
    internal int NumIntermediatePos;
  }
}
