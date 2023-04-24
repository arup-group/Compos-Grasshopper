namespace ComposAPI {
  public class CodeOptionsASNZ : ICodeOptions {
    public bool ConsiderShrinkageDeflection { get; set; } = false;
    public ICreepShrinkageParameters LongTerm { get; set; } = new CreepShrinkageParametersASNZ() { CreepCoefficient = 2.0 };
    public ICreepShrinkageParameters ShortTerm { get; set; } = new CreepShrinkageParametersASNZ() { CreepCoefficient = 2.0 };

    /// <summary>
    /// Deafult constructor with AS/NZ values and members
    /// </summary>
    public CodeOptionsASNZ() {
      // default initialiser
    }
  }
}
