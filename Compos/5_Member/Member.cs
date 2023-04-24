using System;
using System.Collections.Generic;
using ComposAPI.Helpers;

namespace ComposAPI {
  public class Member : IMember {
    public IBeam Beam { get; set; }
    public IDesignCode DesignCode { get; set; }
    public IDesignCriteria DesignCriteria { get; set; } = null;
    public string GridReference { get; set; } = "";
    public IList<ILoad> Loads { get; set; }
    public string Name { get; set; }
    public string Note { get; set; } = "";
    public IResult Result {
      get {
        if (m_result == null) {
          m_result = new Result(this);
        }
        return m_result;
      }
    }
    public ISlab Slab { get; set; }
    public IStud Stud { get; set; }
    internal static Dictionary<Guid, ComposFile> FileRegister = new Dictionary<Guid, ComposFile>();
    private Guid FileGuid;
    private Result m_result = null;

    public Member() {
      var file = new ComposFile(new List<IMember>() { this });
      Register(file);
    }

    public Member(string name, IDesignCode designCode, IBeam beam, IStud stud, ISlab slab, IList<ILoad> loads, IDesignCriteria designCriteria = null) : this() {
      Name = name;
      DesignCode = designCode;
      Beam = beam;
      Stud = stud;
      Slab = slab;
      Loads = loads;
      DesignCriteria = designCriteria;
    }

    public Member(string name, string gridRef, string note, IDesignCode designCode, IBeam beam, IStud stud, ISlab slab, IList<ILoad> loads, IDesignCriteria designCriteria = null) : this() {
      Name = name;
      GridReference = gridRef;
      Note = note;
      DesignCode = designCode;
      Beam = beam;
      Stud = stud;
      Slab = slab;
      Loads = loads;
      DesignCriteria = designCriteria;
    }

    public short Analyse() {
      return Member.FileRegister[FileGuid].Analyse(Name);
    }

    public short CodeSatisfied() {
      return Member.FileRegister[FileGuid].CodeSatisfied(Name);
    }

    public bool Design() {
      if (Beam.Sections.Count > 1) {
        throw new Exception("Unable to design member with more than one section");
      }
      if (!Beam.Sections[0].IsCatalogue) {
        throw new Exception("Unable to design member, the initial section profile must be a catalogue profile");
      }

      ComposFile file = Member.FileRegister[FileGuid];
      if (file.Design(Name) == 0) {
        var newSection = new BeamSection(file.BeamSectDesc(Name));
        Beam.Sections[0] = newSection;
        file.Update();
        return true;
      }
      return false;
    }

    public string GetCodeSatisfiedMessage() {
      int status = CodeSatisfied();
      switch (status) {
        case 0:
          return "All code requirements are met";

        case 1:
          return "Except natural frequency being lower than required, code requirements are met";

        case 2:
          return "One or more code requirements are not met";

        case 3:
          return "The given member name is not valid";

        case 4:
        default:
          return "There are no results for the given named member";
      }
    }

    public void Register(ComposFile file) {
      FileGuid = file.Guid;
      if (Member.FileRegister.ContainsKey(file.Guid)) {
        Member.FileRegister.Remove(file.Guid);
      }
      Member.FileRegister.Add(file.Guid, file);
    }

    public string ToCoaString(ComposUnits units) {
      var parameters = new List<string> {
        CoaIdentifier.MemberTitle,
        Name,
        GridReference,
        Note
      };
      string coaString = CoaHelper.CreateString(parameters);

      coaString += DesignCode.ToCoaString(Name);
      if (DesignCriteria != null) {
        coaString += DesignCriteria.ToCoaString(Name, units);
      }

      coaString += Beam.ToCoaString(Name, DesignCode.Code, units);
      coaString += Slab.ToCoaString(Name, units);
      coaString += Stud.ToCoaString(Name, units, DesignCode.Code);

      foreach (ILoad load in Loads) {
        coaString += load.ToCoaString(Name, units);
      }

      // EC4_DESIGN_OPTION seems to be part of DesignCode..

      return coaString;
    }

    public override string ToString() {
      return Name + " - Beam:" + Beam.ToString() + ", Stud: " + Stud.ToString() + ", Slab: " + Slab.ToString();
    }

    internal static IMember FromCoaString(List<string> parameters) {
      var member = new Member {
        Name = parameters[1],
        GridReference = (parameters.Count > 1) ? parameters[2] : "",
        Note = (parameters.Count > 2) ? parameters[3] : ""
      };
      return member;
    }

    internal float GetResult(string option, short position) {
      return FileRegister[FileGuid].Result(Name, option, position);
    }

    internal float MaxResult(string option) {
      return Member.FileRegister[FileGuid].MaxResult(Name, option);
    }

    internal short MaxResultPosition(string option) {
      return Member.FileRegister[FileGuid].MaxResultPosition(Name, option);
    }

    internal float MinResult(string option) {
      return Member.FileRegister[FileGuid].MinResult(Name, option);
    }

    internal short MinResultPosition(string option) {
      return Member.FileRegister[FileGuid].MinResultPosition(Name, option);
    }

    internal short NumIntermediatePos() {
      return Member.FileRegister[FileGuid].NumIntermediatePos(Name);
    }

    internal short NumTranRebar() {
      return Member.FileRegister[FileGuid].NumTranRebar(Name);
    }

    internal float TranRebarProp(TransverseRebarOption option, short rebarnum) {
      return Member.FileRegister[FileGuid].TranRebarProp(Name, option, rebarnum);
    }

    internal float UtilisationFactor(UtilisationFactorOption option) {
      return Member.FileRegister[FileGuid].UtilisationFactor(Name, option);
    }
  }
}
