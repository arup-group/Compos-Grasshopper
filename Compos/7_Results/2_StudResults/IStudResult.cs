using System.Collections.Generic;
using OasysUnits;

namespace ComposAPI {
  public interface IStudResult {
    /// <summary>
    /// Actual number of studs provided from end. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<int> NumberOfStudsEnd { get; }
    /// <summary>
    /// Used number of studs provided from end. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<int> NumberOfStudsRequiredEnd { get; }
    /// <summary>
    /// Used number of studs provided from start. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<int> NumberOfStudsRequiredStart { get; }
    /// <summary>
    /// Actual number of studs provided from start. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<int> NumberOfStudsStart { get; }
    /// <summary>
    /// Actual shear interaction. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Ratio> ShearInteraction { get; }
    /// <summary>
    /// Required shear interaction for given moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Ratio> ShearInteractionRequired { get; }
    /// <summary>
    /// Capacity of a single stud. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    Force SingleStudCapacity { get; }
    /// <summary>
    /// Actual stud capacity, as [number of studs] x [single stud capacity]. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> StudCapacity { get; }
    /// <summary>
    /// Actual shear capacity from right end. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> StudCapacityEnd { get; }
    /// <summary>
    /// Required stud capacity for given moment. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> StudCapacityRequired { get; }

    /// <summary>
    /// Required stud capacity for 100% shear interaction. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> StudCapacityRequiredForFullShearInteraction { get; }

    /// <summary>
    /// Actual shear capacity from left end. Values given at each <see cref="IResult.Positions"/>
    /// </summary>
    List<Force> StudCapacityStart { get; }
  }
}
