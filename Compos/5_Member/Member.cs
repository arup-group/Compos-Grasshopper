using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComposAPI.Helpers;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class Member : IMember
  {
    public IBeam Beam { get; set; }
    public IStud Stud { get; set; }
    public ISlab Slab { get; set; }
    public IList<ILoad> Loads { get; set; }
    public IDesignCode DesignCode { get; set; }

    public string Name { get; set; }
    public string GridReference { get; set; } = "";
    public string Note { get; set; } = "";

    #region constructors
    public Member() { }

    public Member(string name, IDesignCode designCode, IBeam beam, IStud stud, ISlab slab, List<ILoad> loads)
    {
      this.Name = name;
      this.DesignCode = designCode;
      this.Beam = beam;
      this.Stud = stud;
      this.Slab = slab;
      this.Loads = loads;
    }

    public Member(string name, string gridRef, string note, IDesignCode designCode, IBeam beam, IStud stud, ISlab slab, List<ILoad> loads)
    {
      this.Name = name;
      this.GridReference = gridRef;
      this.Note = note;
      this.DesignCode = designCode;
      this.Beam = beam;
      this.Stud = stud;
      this.Slab = slab;
      this.Loads = loads;
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

      // not sure how DesignCode is organized..
      coaString += this.DesignCode.DesignOptions.ToCoaString(this.Name, this.DesignCode.Code);

      coaString += this.Beam.ToCoaString(this.Name, this.DesignCode.Code, units);
      coaString += this.Stud.ToCoaString(this.Name, units, this.DesignCode.Code);
      coaString += this.Slab.ToCoaString(this.Name, units);

      foreach (ILoad load in this.Loads)
        coaString += load.ToCoaString(this.Name, units);

      // EC4_DESIGN_OPTION seems to be part of DesignCode..

      return coaString;
    }
    #endregion
  }
}
