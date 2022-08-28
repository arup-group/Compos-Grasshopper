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
  public class CompositeSectionProperties : ResultsBase, ICompositeSectionProperties
  {
    internal Dictionary<Enum, List<IQuantity>> ResultsCache = new Dictionary<Enum, List<IQuantity>>();
    public CompositeSectionProperties(Member member) : base(member)
    {
    }

    /// <summary>
    /// Welding thickness at top - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web.
    /// </summary>
    public List<Length> GirderWeldThicknessTop
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.GIRDER_WELD_THICK_T;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Welding thickness at bottom - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web.
    /// </summary>
    public List<Length> GirderWeldThicknessBottom
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.GIRDER_WELD_THICK_B;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Effective slab width on left side
    /// </summary>
    public List<Length> EffectiveSlabWidthLeft
    {
      get
      {
        ResultOption resultType = ResultOption.SLAB_WIDTH_L_EFFECT;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Effective slab width on right side
    /// </summary>
    public List<Length> EffectiveSlabWidthRight
    {
      get
      {
        ResultOption resultType = ResultOption.SLAB_WIDTH_R_EFFECT;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Moment of Inertia of steel beam alone
    /// </summary>
    public List<AreaMomentOfInertia> BeamMomentOfInertia
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.I_STEEL_BEAM;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis position of steel beam alone, measured from the bottom of steel beam 
    /// </summary>
    public List<Length> BeamNeutralAxisPosition
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.X_STEEL_BEAM;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Area of steel beam alone
    /// </summary>
    public List<Area> BeamArea
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.AREA_STEEL_BEAM;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Moment of Inertia of composite section for long term loading
    /// </summary>
    public List<AreaMomentOfInertia> MomentOfInertiaLongTerm
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.I_LONG_TERM;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis position of composite section for long term loading, measured from the bottom of steel beam 
    /// </summary>
    public List<Length> NeutralAxisPositionLongTerm
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.X_LONG_TERM;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Area of composite section for long term loading
    /// </summary>
    public List<Area> AreaLongTerm
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.AREA_LONG_TERM;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Moment of Inertia of composite section for short term loading
    /// </summary>
    public List<AreaMomentOfInertia> MomentOfInertiaShortTerm
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.I_SHORT_TERM;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis position of composite section for short term loading, measured from the bottom of steel beam 
    /// </summary>
    public List<Length> NeutralAxisPositionShortTerm
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.X_SHORT_TERM;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Area of composite section for short term loading
    /// </summary>
    public List<Area> AreaShortTerm
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.AREA_SHORT_TERM;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Moment of Inertia of composite section for shrinkage
    /// </summary>
    public List<AreaMomentOfInertia> MomentOfInertiaShrinkage
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.I_SHRINK;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis position of composite section for shrinkage, measured from the bottom of steel beam 
    /// </summary>
    public List<Length> NeutralAxisPositionShrinkage
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.X_SHRINK;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Area of composite section for shrinkage
    /// </summary>
    public List<Area> AreaShrinkage
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.AREA_SHRINK;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Effective Moment of Inertia of composite section under combined loading
    /// </summary>
    public List<AreaMomentOfInertia> MomentOfInertiaEffective
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.I_EFFECTIVE;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Effective Neutral axis position of composite section under combined loading, measured from the bottom of steel beam 
    /// </summary>
    public List<Length> NeutralAxisPositionEffective
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.X_EFFECTIVE;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Effective Area of composite section under combined loading
    /// </summary>
    public List<Area> AreaEffective
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.AREA_EFFECT;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Moment of Inertia of composite section for vibration
    /// </summary>
    public List<AreaMomentOfInertia> MomentOfInertiaVibration
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.I_VIBRATION;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis position of composite section for vibration, measured from the bottom of steel beam 
    /// </summary>
    public List<Length> NeutralAxisPositionVibration
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.X_VIBRATION;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Area of composite section for vibration
    /// </summary>
    public List<Area> AreaVibration
    {
      get
      {
        BeamResultOption resultType = BeamResultOption.AREA_VIBRATION;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Area)x).ToList();
      }
    }
 

    private void GetResults(BeamResultOption resultType)
    {
      List<IQuantity> results = new List<IQuantity>();
      for (short pos = 0; pos < this.NumIntermediatePos; pos++)
      {
        float value = this.Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));

        switch (resultType)
        {
          case BeamResultOption.GIRDER_WELD_THICK_T:
          case BeamResultOption.GIRDER_WELD_THICK_B:
          case BeamResultOption.X_STEEL_BEAM:
          case BeamResultOption.X_LONG_TERM:
          case BeamResultOption.X_SHORT_TERM:
          case BeamResultOption.X_SHRINK:
          case BeamResultOption.X_EFFECTIVE:
          case BeamResultOption.X_VIBRATION:
            results.Add(new Length(value, LengthUnit.Meter));
            break;
          case BeamResultOption.I_STEEL_BEAM:
          case BeamResultOption.I_LONG_TERM:
          case BeamResultOption.I_SHORT_TERM:
          case BeamResultOption.I_SHRINK:
          case BeamResultOption.I_EFFECTIVE:
          case BeamResultOption.I_VIBRATION:
            results.Add(new AreaMomentOfInertia(value, AreaMomentOfInertiaUnit.MeterToTheFourth));
            break;
          case BeamResultOption.AREA_STEEL_BEAM:
          case BeamResultOption.AREA_LONG_TERM:
          case BeamResultOption.AREA_SHORT_TERM:
          case BeamResultOption.AREA_SHRINK:
          case BeamResultOption.AREA_EFFECT:
          case BeamResultOption.AREA_VIBRATION:
            results.Add(new Area(value, AreaUnit.SquareMeter));
            break;
        }
      }
      ResultsCache.Add(resultType, results);
    }

    private void GetResults(ResultOption resultType)
    {
      List<IQuantity> results = new List<IQuantity>();
      for (short pos = 0; pos < this.NumIntermediatePos; pos++)
      {
        float value = this.Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));

        results.Add(new Length(value, LengthUnit.Meter));
      }
      ResultsCache.Add(resultType, results);
    }
  }
}
