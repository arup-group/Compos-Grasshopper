namespace ComposAPI {
  public abstract class SubResult {
    internal Member Member;

    internal int NumIntermediatePos;

    public SubResult(Member member, int numIntermediatePos) {
      Member = member;
      NumIntermediatePos = numIntermediatePos;
    }
  }
}
