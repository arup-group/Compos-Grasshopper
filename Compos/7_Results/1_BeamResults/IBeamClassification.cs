using System.Collections.Generic;

namespace ComposAPI {
  public interface IBeamClassification {
    /// <summary>
    /// Flange class in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<string> Flange { get; }
    /// <summary>
    /// Flange class in Construction stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<string> FlangeConstruction { get; }

    /// <summary>
    /// Section class in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<string> Section { get; }
    /// <summary>
    /// Section class in Construction stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<string> SectionConstruction { get; }
    /// <summary>
    /// Web class in Final stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<string> Web { get; }
    /// <summary>
    /// Web class in Construction stage. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<string> WebConstruction { get; }
  }
}
