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
  public interface IBeamClassification
  {
    /// <summary>
    /// Flange class in Construction stage
    /// </summary>
    List<string> FlangeConstruction { get; }

    /// <summary>
    /// Web class in Construction stage
    /// </summary>
    List<string> WebConstruction { get; }

    /// <summary>
    /// Section class in Construction stage
    /// </summary>
    List<string> SectionConstruction { get; }

    /// <summary>
    /// Flange class in Final stage
    /// </summary>
    List<string> Flange { get; }

    /// <summary>
    /// Web class in Final stage
    /// </summary>
    List<string> Web { get; }

    /// <summary>
    /// Section class in Final stage
    /// </summary>
    List<string> Section { get; }
  }
}
