
namespace ComposAPI
{
  public abstract class SubResult
  {
    public SubResult(Member member, int numIntermediatePos)
    {
      this.Member = member;
      this.NumIntermediatePos = numIntermediatePos;
    }
    internal Member Member;
    internal int NumIntermediatePos;
  }
}
