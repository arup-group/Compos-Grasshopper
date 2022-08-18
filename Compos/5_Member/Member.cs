using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compos_8_6;
using ComposAPI.Helpers;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class Member : IMember
  {
    internal static Dictionary<Guid, IComposFile> FileRegister = new Dictionary<Guid, IComposFile>();
    public IBeam Beam { get; set; }
    public IStud Stud { get; set; }
    public ISlab Slab { get; set; }
    public IList<ILoad> Loads { get; set; }
    public IDesignCode DesignCode { get; set; }
    public IDesignCriteria DesignCriteria { get; set; } = null;
    private Guid FileGuid;

    public string Name { get; set; }
    public string GridReference { get; set; } = "";
    public string Note { get; set; } = "";

    #region constructors
    public Member()
    {
      ComposFile file = new ComposFile(new List<IMember>() { this });
      this.Register(file);
    }

    public Member(string name, IDesignCode designCode, IBeam beam, IStud stud, ISlab slab, IList<ILoad> loads, IDesignCriteria designCriteria = null) : this()
    {
      this.Name = name;
      this.DesignCode = designCode;
      this.Beam = beam;
      this.Stud = stud;
      this.Slab = slab;
      this.Loads = loads;
      this.DesignCriteria = designCriteria;
    }

    public Member(string name, string gridRef, string note, IDesignCode designCode, IBeam beam, IStud stud, ISlab slab, IList<ILoad> loads, IDesignCriteria designCriteria = null) : this()
    {
      this.Name = name;
      this.GridReference = gridRef;
      this.Note = note;
      this.DesignCode = designCode;
      this.Beam = beam;
      this.Stud = stud;
      this.Slab = slab;
      this.Loads = loads;
      this.DesignCriteria = designCriteria;
    }
    #endregion

    #region methods
    public short Analyse()
    {
      return FileRegister[this.FileGuid].Analyse(this.Name);
    }

    public bool Design()
    {
      if (this.Beam.Sections.Count > 1)
        throw new Exception("Unable to design member with more than one section");
      if (!this.Beam.Sections[0].isCatalogue)
        throw new Exception("Unable to design member, the initial section profile must be a catalogue profile");

      IComposFile file = FileRegister[this.FileGuid];
      if (file.Design(this.Name) == 0)
      {
        BeamSection newSection = new BeamSection(file.BeamSectDesc(this.Name));
        this.Beam.Sections[0] = newSection;
        file.AddMember(this);
        return true;
      }
      return false;
    }

    public short CodeSatisfied()
    {
      return FileRegister[this.FileGuid].CodeSatisfied(this.Name);
    }

    public float MaxResult(string option, short position)
    {
      return FileRegister[this.FileGuid].MaxResult(this.Name, option, position);
    }

    public short MaxResultPosition(string option, short position)
    {
      return FileRegister[this.FileGuid].MaxResultPosition(this.Name, option, position);
    }

    public float MinResult(string option, short position)
    {
      return FileRegister[this.FileGuid].MinResult(this.Name, option, position);
    }

    public short MinResultPosition(string option, short position)
    {
      return FileRegister[this.FileGuid].MinResultPosition(this.Name, option, position);
    }

    public short NumIntermediatePos()
    {
      return FileRegister[this.FileGuid].NumIntermediatePos(this.Name);
    }

    public short NumTranRebar()
    {
      return FileRegister[this.FileGuid].NumTranRebar(this.Name);
    }

    public void Register(IComposFile file)
    {
      this.FileGuid = file.Guid;
      if (FileRegister.ContainsKey(file.Guid))
        FileRegister.Remove(file.Guid);
      FileRegister.Add(file.Guid, file);
    }

    public float Result(string option, short position)
    {
      return FileRegister[this.FileGuid].Result(this.Name, option, position);
    }

    public float TranRebarProp(TransverseRebarOption option, short rebarnum)
    {
      return FileRegister[this.FileGuid].TranRebarProp(this.Name, option, rebarnum);
    }

    public float UtilisationFactor(UtilisationFactorOption option)
    {
      return FileRegister[this.FileGuid].UtilisationFactor(this.Name, option);
    }
    #endregion

    #region coa interop
    internal static IMember FromCoaString(List<string> parameters)
    {
      Member member = new Member();
      member.Name = parameters[1];
      member.GridReference = (parameters.Count > 1) ? parameters[2] : "";
      member.Note = (parameters.Count > 2) ? parameters[3] : "";
      return member;
    }

    public string ToCoaString(ComposUnits units)
    {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.MemberTitle);
      parameters.Add(this.Name);
      parameters.Add(this.GridReference);
      parameters.Add(this.Note);
      string coaString = CoaHelper.CreateString(parameters);

      coaString += this.DesignCode.ToCoaString(this.Name);
      if (this.DesignCriteria != null)
        coaString += this.DesignCriteria.ToCoaString(this.Name, units);

      coaString += this.Beam.ToCoaString(this.Name, this.DesignCode.Code, units);
      coaString += this.Slab.ToCoaString(this.Name, units);
      coaString += this.Stud.ToCoaString(this.Name, units, this.DesignCode.Code);

      foreach (ILoad load in this.Loads)
        coaString += load.ToCoaString(this.Name, units);

      // EC4_DESIGN_OPTION seems to be part of DesignCode..

      return coaString;
    }
    #endregion

    public override string ToString()
    {
      return this.Name + " - Beam:" + this.Beam.ToString() + ", Stud: " + this.Stud.ToString() + ", Slab: " + this.Slab.ToString();
    }
  }
}
