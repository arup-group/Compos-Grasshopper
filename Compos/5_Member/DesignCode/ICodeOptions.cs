namespace ComposAPI
{
  public interface ICodeOptions
  {
    bool ConsiderShrinkageDeflection { get; }
    ICreepShrinkageParameters LongTerm { get; }
    ICreepShrinkageParameters ShortTerm { get; }
  }
}
