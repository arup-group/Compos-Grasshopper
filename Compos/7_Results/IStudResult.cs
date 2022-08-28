using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public interface IStudResult
  {
    /// <summary>
    /// Actual stud capacity, as [number of studs] x [single stud capacity]
    /// </summary>
    List<Force> StudCapacity { get; }


    /// <summary>
    /// Capacity of a single stud
    /// </summary>
    Force SingleStudCapacity { get; }


    /// <summary>
    /// Required stud capacity for given moment
    /// </summary>
    List<Force> StudCapacityRequired { get; }


    /// <summary>
    /// Required stud capacity for 100% shear interaction
    /// </summary>
    List<Force> StudCapacityRequiredForFullShearInteraction { get; }


    /// <summary>
    /// Actual shear capacity from left end
    /// </summary>
    List<Force> StudCapacityLeft { get; }


    /// <summary>
    /// Actual shear capacity from right end
    /// </summary>
    List<Force> StudCapacityRight { get; }


    /// <summary>
    /// Actual shear interaction
    /// </summary>
    List<Ratio> ShearInteraction { get; }


    /// <summary>
    /// Required shear interaction for given moment
    /// </summary>
    List<Ratio> ShearInteractionRequired { get; }


    /// <summary>
    /// Actual number of studs provided from left end
    /// </summary>
    List<int> NumberOfStudsLeft { get; }

    /// <summary>
    /// Used number of studs provided from left end
    /// </summary>
    List<int> NumberOfStudsRequiredLeft { get; }

    /// <summary>
    /// Actual number of studs provided from right end
    /// </summary>
    List<int> NumberOfStudsRight { get; }

    /// <summary>
    /// Used number of studs provided from right end
    /// </summary>
    List<int> NumberOfStudsRequiredRight { get; }
  }
}
