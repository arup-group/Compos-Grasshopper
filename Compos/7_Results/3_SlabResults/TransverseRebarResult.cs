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
  internal enum TransverseRebarOption
  {
    REBAR_DIST_LEFT_SIDE, // Rebar starting position
    REBAR_DIST_RIGHT_SIDE, // Rebar ending position
    REBAR_DIAMETER, // Rebar diameter
    REBAR_INTERVAL, // Rebar interval
    REBAR_COVER, // Rebar cover
    REBAR_AREA, // Rebar area per meter
    REBAR_CRITI_DIST, // Critical transverse shear position
    REBAR_CRITI_SURFACE, // Failure surface, 1: a-a section; 2:b-b section; 5: e-e section
    REBAR_CRITI_PERI, // Effective perimeter
    REBAR_CRITI_ACTUAL_SHEAR, // Transverse shear force
    REBAR_CRITI_SHEAR_CONC, // Concrete shear resistance
    REBAR_CRITI_SHEAR_DECK, // Decking shear resistance
    REBAR_CRITI_SHEAR_MESH, // Mesh bar shear resistance
    REBAR_CRITI_SHEAR_REBAR, // Rebar shear resistance
    REBAR_CRITI_SHEAR_MAX_ALLOW, // Maximum allowable shear resistance
  }

  public class TransverseRebarResult : ResultsBase, ITransverseRebarResult
  {
    internal Dictionary<TransverseRebarOption, List<IQuantity>> ResultsCache = new Dictionary<TransverseRebarOption, List<IQuantity>>();
    public TransverseRebarResult(Member member, int numIntermediatePos) : base(member, numIntermediatePos)
    {
    }


    /// <summary>
    /// Rebar starting position, measured from start
    /// </summary>
    public List<Length> StartPosition
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_DIST_LEFT_SIDE;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Rebar ending position, measured from start
    /// </summary>
    public List<Length> EndPosition
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_DIST_RIGHT_SIDE;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Rebar diameter
    /// </summary>
    public List<Length> Diameter
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_DIAMETER;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Rebar interval
    /// </summary>
    public List<Length> Spacing
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_INTERVAL;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Rebar cover
    /// </summary>
    public List<Length> Cover
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_COVER;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Rebar area per meter
    /// </summary>
    public List<Area> Area
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_AREA;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Critical transverse shear position
    /// </summary>
    public List<Length> Positions
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_DIST;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Failure surface at <see cref="Positions"/>, being either a-a section, b-b section, or e-e section
    /// </summary>
    public List<string> ControlSurface
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SURFACE;
        if (m_surface == null)
        {
          m_surface = new List<string>();
          for (short pos = 0; pos < this.NumIntermediatePos; pos++)
          {
            float value = this.Member.TranRebarProp(resultType, Convert.ToInt16(pos));
            if (value == 1)
              m_surface.Add("a-a section");
            else if (value == 2)
              m_surface.Add("b-b section");
            else if (value == 5)
              m_surface.Add("e-e section");
            else
              m_surface.Add("unknown");
          }
        }
        return m_surface;
      }
    }
    private List<string> m_surface = null;

    /// <summary>
    /// Effective perimeter at <see cref="Positions"/>
    /// </summary>
    public List<Length> EffectiveShearPerimeter
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_PERI;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Actual transverse shear force at <see cref="Positions"/>
    /// </summary>
    public List<Force> TransverseShearForce
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_ACTUAL_SHEAR;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Concrete shear resistance at <see cref="Positions"/>
    /// </summary>
    public List<Force> ConcreteShearResistance
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SHEAR_CONC;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Decking shear resistance at <see cref="Positions"/>
    /// </summary>
    public List<Force> DeckingShearResistance
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SHEAR_DECK;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Mesh reinforcement bar shear resistance at <see cref="Positions"/>
    /// </summary>
    public List<Force> MeshBarShearResistance
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SHEAR_MESH;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Rebar shear resistance at <see cref="Positions"/>
    /// </summary>
    public List<Force> RebarShearResistance
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SHEAR_REBAR;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Combined shear resistance [Concrete] + [Decking] + [Mesh bar] + [Rebar] at <see cref="Positions"/>
    /// </summary>
    public List<Force> TotalShearResistance
    {
      get
      {
        List<Force> total = new List<Force>();
        List<Force> concrete = ConcreteShearResistance;
        List<Force> decking = DeckingShearResistance;
        List<Force> mesh = MeshBarShearResistance;
        List<Force> rebar = RebarShearResistance;
        for (int i = 0; i < Positions.Count; i++)
        {
          Force tot = concrete[i] + decking[i] + mesh[i] + rebar[i];
          total.Add(tot);
        }
        return total;
      }
    }

    /// <summary>
    /// Maximum allowable shear resistance at <see cref="Positions"/>
    /// </summary>
    public List<Force> MaxAllowedShearResistance
    {
      get
      {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SHEAR_MAX_ALLOW;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }


    private void GetResults(TransverseRebarOption resultType)
    {
      List<IQuantity> results = new List<IQuantity>();
      for (short pos = 0; pos < this.NumIntermediatePos; pos++)
      {
        float value = this.Member.TranRebarProp(resultType, Convert.ToInt16(pos));

        switch (resultType)
        {
          case TransverseRebarOption.REBAR_DIST_LEFT_SIDE:
          case TransverseRebarOption.REBAR_DIST_RIGHT_SIDE:
          case TransverseRebarOption.REBAR_DIAMETER:
          case TransverseRebarOption.REBAR_INTERVAL:
          case TransverseRebarOption.REBAR_CRITI_DIST:
          case TransverseRebarOption.REBAR_CRITI_PERI:
          case TransverseRebarOption.REBAR_COVER:
            results.Add(new Length(value, LengthUnit.Meter));
            break;

          case TransverseRebarOption.REBAR_AREA:
            results.Add(new Area(value, AreaUnit.SquareMeter));
            break;

          case TransverseRebarOption.REBAR_CRITI_ACTUAL_SHEAR:
          case TransverseRebarOption.REBAR_CRITI_SHEAR_CONC:
          case TransverseRebarOption.REBAR_CRITI_SHEAR_DECK:
          case TransverseRebarOption.REBAR_CRITI_SHEAR_MESH:
          case TransverseRebarOption.REBAR_CRITI_SHEAR_REBAR:
          case TransverseRebarOption.REBAR_CRITI_SHEAR_MAX_ALLOW:
            results.Add(new Force(value, ForceUnit.Newton));
            break;
        }
      }
      this.ResultsCache.Add(resultType, results);
    }
  }
}
