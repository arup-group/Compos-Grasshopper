using System;
using System.Collections.Generic;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public class Result : IResult {
    public IBeamClassification BeamClassification {
      get {
        if (m_BeamClassification == null) {
          m_BeamClassification = new BeamClassification(m_Member, m_NumIntermediatePos);
        }
        return m_BeamClassification;
      }
    }
    public IBeamStressResult BeamStresses {
      get {
        if (m_BeamStressResult == null) {
          m_BeamStressResult = new BeamStressResult(m_Member, m_NumIntermediatePos);
        }
        return m_BeamStressResult;
      }
    }
    public ICapacityResult Capacities {
      get {
        if (m_CapacityResult == null) {
          m_CapacityResult = new CapacityResult(m_Member, m_NumIntermediatePos);
        }
        return m_CapacityResult;
      }
    }
    public IDeflectionResult Deflections {
      get {
        if (m_DeflectionResult == null) {
          m_DeflectionResult = new DeflectionResult(m_Member, m_NumIntermediatePos);
        }
        return m_DeflectionResult;
      }
    }
    public IInternalForceResult InternalForces {
      get {
        if (m_ForcesAndMoments == null) {
          m_ForcesAndMoments = new InternalForceResult(m_Member, m_NumIntermediatePos);
        }
        return m_ForcesAndMoments;
      }
    }
    public List<Length> Positions { get; internal set; }
    public ICompositeSectionProperties SectionProperties {
      get {
        if (m_CompositeSectionProperties == null) {
          m_CompositeSectionProperties = new CompositeSectionProperties(m_Member, m_NumIntermediatePos);
        }
        return m_CompositeSectionProperties;
      }
    }
    public ISlabStressResult SlabStresses {
      get {
        if (m_SlabStressResult == null) {
          m_SlabStressResult = new SlabStressResult(m_Member, m_NumIntermediatePos);
        }
        return m_SlabStressResult;
      }
    }
    public IStudResult StudResults {
      get {
        if (m_StudResult == null) {
          m_StudResult = new StudResult(m_Member, m_NumIntermediatePos);
        }
        return m_StudResult;
      }
    }
    public ITransverseRebarResult TransverseRebarResults {
      get {
        if (m_NumTranRebar < 0) {
          m_NumTranRebar = m_Member.NumTranRebar();
        }
        if (m_TransverseRebarResult == null) {
          m_TransverseRebarResult = new TransverseRebarResult(m_Member, m_NumTranRebar);
        }
        return m_TransverseRebarResult;
      }
    }
    public IUtilisation Utilisations {
      get {
        if (m_Utilisation == null) {
          m_Utilisation = new Utilisation(m_Member);
        }
        return m_Utilisation;
      }
    }
    private BeamClassification m_BeamClassification;
    private BeamStressResult m_BeamStressResult;
    private CapacityResult m_CapacityResult;
    private CompositeSectionProperties m_CompositeSectionProperties;
    private DeflectionResult m_DeflectionResult;
    private InternalForceResult m_ForcesAndMoments;
    private Member m_Member;
    private int m_NumIntermediatePos;
    private int m_NumTranRebar = -1;

    private SlabStressResult m_SlabStressResult;

    private StudResult m_StudResult;

    private TransverseRebarResult m_TransverseRebarResult;

    private Utilisation m_Utilisation;

    internal Result(Member member) {
      m_Member = member;

      Positions = new List<Length>();
      m_NumIntermediatePos = member.NumIntermediatePos();
      for (int i = 0; i < m_NumIntermediatePos; i++) {
        float value = member.GetResult(ResultOption.CRITI_SECT_DIST.ToString(), Convert.ToInt16(i));
        var pos = new Length(value, LengthUnit.Meter);
        Positions.Add(pos);
      }
    }
  }

  internal enum ResultOption {
    CRITI_SECT_DIST, // Current position/distance from left end of the member
    CRITI_SECT_ATTRI, // Position attributes (max shear, moment and etc.,)
    SLAB_WIDTH_L_EFFECT, // Effective slab width on left side
    SLAB_WIDTH_R_EFFECT, // Effective slab width on right side
  }
}
