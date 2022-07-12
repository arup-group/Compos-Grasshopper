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
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();

    public IBeam Beam { get; set; }
    public IStud Stud { get; set; }
    public ISlab Slab { get; set; }
    public IList<ILoad> Loads { get; set; }
    public IDesignCode DesignCode { get; set; }
    private IComposFile File { get; set; }

    public string Name { get; set; }
    public string GridReference { get; set; } = "";
    public string Note { get; set; } = "";

    internal IAutomation ComposCOM { get; set; }

    #region constructors
    public Member()
    {
      this.File = new ComposFile(new List<IMember>() { this });
    }

    public Member(string name, IDesignCode designCode, IBeam beam, IStud stud, ISlab slab, IList<ILoad> loads) : this()
    {
      this.Name = name;
      this.DesignCode = designCode;
      this.Beam = beam;
      this.Stud = stud;
      this.Slab = slab;
      this.Loads = loads;
    }

    public Member(string name, string gridRef, string note, IDesignCode designCode, IBeam beam, IStud stud, ISlab slab, IList<ILoad> loads) : this()
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

    #region methods
    public short Analyse()
    {
      return this.File.Analyse(this.Name);
    }

    public short CodeSatisfied()
    {
      return this.File.CodeSatisfied(this.Name);
    }

    public float MaxResult(string option, short position)
    {
      return this.File.MaxResult(this.Name, option, position);
    }

    public short MaxResultPosition(string option, short position)
    {
      return this.File.MaxResultPosition(this.Name, option, position);
    }

    public float MinResult(string option, short position)
    {
      return this.File.MinResult(this.Name, option, position);
    }

    public short MinResultPosition(string option, short position)
    {
      return this.File.MinResultPosition(this.Name, option, position);
    }

    public short NumIntermediatePos()
    {
      return this.File.NumIntermediatePos(this.Name);
    }

    public short NumTranRebar()
    {
      return this.File.NumTranRebar(this.Name);
    }

    public void Register(IComposFile file)
    {
      this.File = file;
    }
    public float Result(string option, short position)
    {
      return this.File.Result(this.Name, option, position); 
    }

    public float TranRebarProp(TransverseRebarOption option, short rebarnum)
    {
      return this.File.TranRebarProp(this.Name, option, rebarnum);
    }

    public float UtilisationFactor(UtilisationFactorOption option)
    {
      return this.File.UtilisationFactor(this.Name, option);
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

      coaString += this.Beam.ToCoaString(this.Name, this.DesignCode.Code, units);
      coaString += this.Slab.ToCoaString(this.Name, units);
      coaString += this.Stud.ToCoaString(this.Name, units, this.DesignCode.Code);

      foreach (ILoad load in this.Loads)
        coaString += load.ToCoaString(this.Name, units);

      // EC4_DESIGN_OPTION seems to be part of DesignCode..

      return coaString;
    }
    #endregion
  }
}
