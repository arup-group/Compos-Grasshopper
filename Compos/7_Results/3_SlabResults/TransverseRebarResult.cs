﻿using System;
using System.Collections.Generic;
using System.Linq;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public class TransverseRebarResult : SubResult, ITransverseRebarResult {
    /// <summary>
    /// Rebar area per meter
    /// </summary>
    public List<Area> Area {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_AREA;
        return GetResults(resultType).Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Concrete shear resistance at <see cref="Positions"/>
    /// </summary>
    public List<Force> ConcreteShearResistance {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SHEAR_CONC;
        return GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Failure surface at <see cref="Positions"/>, being either a-a section, b-b section, or e-e section
    /// </summary>
    public List<string> ControlSurface {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SURFACE;
        if (m_surface == null) {
          m_surface = new List<string>();
          for (short pos = 0; pos < NumIntermediatePos; pos++) {
            float value = Member.TranRebarProp(resultType, Convert.ToInt16(pos));
            if (value == 1) {
              m_surface.Add("a-a section");
            } else if (value == 2) {
              m_surface.Add("b-b section");
            } else if (value == 5) {
              m_surface.Add("e-e section");
            } else {
              m_surface.Add("unknown");
            }
          }
        }
        return m_surface;
      }
    }

    /// <summary>
    /// Rebar cover
    /// </summary>
    public List<Length> Cover {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_COVER;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Decking shear resistance at <see cref="Positions"/>
    /// </summary>
    public List<Force> DeckingShearResistance {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SHEAR_DECK;
        return GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Rebar diameter
    /// </summary>
    public List<Length> Diameter {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_DIAMETER;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Effective perimeter at <see cref="Positions"/>
    /// </summary>
    public List<Length> EffectiveShearPerimeter {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_PERI;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Rebar ending position, measured from start
    /// </summary>
    public List<Length> EndPosition {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_DIST_RIGHT_SIDE;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Maximum allowable shear resistance at <see cref="Positions"/>
    /// </summary>
    public List<Force> MaxAllowedShearResistance {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SHEAR_MAX_ALLOW;
        return GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Mesh reinforcement bar shear resistance at <see cref="Positions"/>
    /// </summary>
    public List<Force> MeshBarShearResistance {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SHEAR_MESH;
        return GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Critical transverse shear position
    /// </summary>
    public List<Length> Positions {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_DIST;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Rebar shear resistance at <see cref="Positions"/>
    /// </summary>
    public List<Force> RebarShearResistance {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_SHEAR_REBAR;
        return GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Rebar interval
    /// </summary>
    public List<Length> Spacing {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_INTERVAL;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Rebar starting position, measured from start
    /// </summary>
    public List<Length> StartPosition {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_DIST_LEFT_SIDE;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Combined shear resistance [Concrete] + [Decking] + [Mesh bar] + [Rebar] at <see cref="Positions"/>
    /// </summary>
    public List<Force> TotalShearResistance {
      get {
        var total = new List<Force>();
        List<Force> concrete = ConcreteShearResistance;
        List<Force> decking = DeckingShearResistance;
        List<Force> mesh = MeshBarShearResistance;
        List<Force> rebar = RebarShearResistance;
        for (int i = 0; i < Positions.Count; i++) {
          Force tot = concrete[i] + decking[i] + mesh[i] + rebar[i];
          total.Add(tot);
        }
        return total;
      }
    }

    /// <summary>
    /// Actual transverse shear force at <see cref="Positions"/>
    /// </summary>
    public List<Force> TransverseShearForce {
      get {
        TransverseRebarOption resultType = TransverseRebarOption.REBAR_CRITI_ACTUAL_SHEAR;
        return GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    private List<string> m_surface = null;

    private Dictionary<TransverseRebarOption, List<IQuantity>> ResultsCache = new Dictionary<TransverseRebarOption, List<IQuantity>>();

    public TransverseRebarResult(Member member, int numIntermediatePos) : base(member, numIntermediatePos) {
    }

    private List<IQuantity> GetResults(TransverseRebarOption resultType) {
      if (!ResultsCache.ContainsKey(resultType)) {
        var results = new List<IQuantity>();
        for (short pos = 0; pos < NumIntermediatePos; pos++) {
          float value = Member.TranRebarProp(resultType, Convert.ToInt16(pos));

          switch (resultType) {
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
        ResultsCache.Add(resultType, results);
      }
      return ResultsCache[resultType];
    }
  }

  internal enum TransverseRebarOption {
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
}
