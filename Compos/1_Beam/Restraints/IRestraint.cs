namespace ComposAPI
{

  /// <summary>
  /// Restraint interface that provides two <see cref="ISupports"/> objects for 'Construction Stage Support' and 'Final Stage Support', and if top flange is laterally restrained in at construction stage.
  /// </summary>
  public interface IRestraint
  {
    ISupports ConstructionStageSupports { get; }
    ISupports FinalStageSupports { get; }
    bool TopFlangeRestrained { get; }
  }
}
