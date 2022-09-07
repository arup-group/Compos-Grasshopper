using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  internal enum ResultOption
  {
    CRITI_SECT_DIST, // Current position/distance from left end of the member
    CRITI_SECT_ATTRI, // Position attributes (max shear, moment and etc.,)
    SLAB_WIDTH_L_EFFECT, // Effective slab width on left side
    SLAB_WIDTH_R_EFFECT, // Effective slab width on right side
  }

  public class Result : IResult
  {
    public List<Length> Positions { get; internal set; }
    private Member m_Member;
    private int m_NumIntermediatePos;
    private int m_NumTranRebar = -1;

    internal Result(Member member)
    {
      this.m_Member = member;
      
      this.Positions = new List<Length>();
      this.m_NumIntermediatePos = member.NumIntermediatePos();
      for (int i = 0; i < this.m_NumIntermediatePos; i++)
      {
        float value = member.GetResult(ResultOption.CRITI_SECT_DIST.ToString(), Convert.ToInt16(i));
        Length pos = new Length(value, LengthUnit.Meter);
        this.Positions.Add(pos);
      }
    }

    public IInternalForceResult InternalForces
    {
      get
      {
        if (m_ForcesAndMoments == null)
          m_ForcesAndMoments = new InternalForceResult(m_Member, m_NumIntermediatePos);
        return m_ForcesAndMoments;
      }
    }
    private InternalForceResult m_ForcesAndMoments;

    public ICompositeSectionProperties SectionProperties
    {
      get
      {
        if (m_CompositeSectionProperties == null)
          m_CompositeSectionProperties = new CompositeSectionProperties(m_Member, m_NumIntermediatePos);
        return m_CompositeSectionProperties;
      }
    }
    private CompositeSectionProperties m_CompositeSectionProperties;

    public IBeamClassification BeamClassification
    {
      get
      {
        if (m_BeamClassification == null)
          m_BeamClassification = new BeamClassification(m_Member, m_NumIntermediatePos);
        return m_BeamClassification;
      }
    }
    private BeamClassification m_BeamClassification;

    public ICapacityResult Capacities
    {
      get
      {
        if (m_CapacityResult == null)
          m_CapacityResult = new CapacityResult(m_Member, m_NumIntermediatePos);
        return m_CapacityResult;
      }
    }
    private CapacityResult m_CapacityResult;

    public IUtilisation Utilisations
    {
      get
      {
        if (m_Utilisation == null)
          m_Utilisation = new Utilisation(m_Member);
        return m_Utilisation;
      }
    }
    private Utilisation m_Utilisation;

    public IBeamStressResult BeamStresses
    {
      get
      {
        if (m_BeamStressResult == null)
          m_BeamStressResult = new BeamStressResult(m_Member, m_NumIntermediatePos);
        return m_BeamStressResult;
      }
    }
    private BeamStressResult m_BeamStressResult;

    public ISlabStressResult SlabStresses
    {
      get
      {
        if (m_SlabStressResult == null)
          m_SlabStressResult = new SlabStressResult(m_Member, m_NumIntermediatePos);
        return m_SlabStressResult;
      }
    }
    private SlabStressResult m_SlabStressResult;

    public ITransverseRebarResult TransverseRebarResults
    {
      get
      {
        if (m_NumTranRebar < 0)
          m_NumTranRebar = this.m_Member.NumTranRebar();
        if (m_TransverseRebarResult == null)
          m_TransverseRebarResult = new TransverseRebarResult(m_Member, m_NumTranRebar);
        return m_TransverseRebarResult;
      }
    }
    private TransverseRebarResult m_TransverseRebarResult;

    public IStudResult StudResults
    {
      get
      {
        if (m_StudResult == null)
          m_StudResult = new StudResult(m_Member, m_NumIntermediatePos);
        return m_StudResult;
      }
    }
    private StudResult m_StudResult;

    public IDeflectionResult Deflections
    {
      get
      {
        if (m_DeflectionResult == null)
          m_DeflectionResult = new DeflectionResult(m_Member, m_NumIntermediatePos);
        return m_DeflectionResult;
      }
    }
    private DeflectionResult m_DeflectionResult;
  }
}
