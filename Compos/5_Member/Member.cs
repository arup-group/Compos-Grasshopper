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
    public List<ILoad> Loads { get; set; }
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
    internal Member(string coaString)
    {
      // to do - implement from coa string method

      
    }

    public string ToCoaString(AngleUnit angleUnit, DensityUnit densityUnit, ForceUnit forceUnit, LengthUnit lengthUnit, PressureUnit pressureUnit, StrainUnit strainUnit)
    {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.MemberTitle);
      parameters.Add(this.Name);
      parameters.Add(this.GridReference);
      parameters.Add(this.Note);
      string coaString = CoaHelper.CreateString(parameters);

      // not sure how DesignCode is organized..
      coaString += this.DesignCode.DesignOptions.ToCoaString(this.Name, this.DesignCode.Code);

      // not yet sure what units are neccessary here
      coaString += this.Beam.ToCoaString(this.Name, this.DesignCode.Code, densityUnit, lengthUnit, pressureUnit);

      coaString += this.Slab.ToCoaString(this.Name, densityUnit, lengthUnit, strainUnit);

      // not yet sure what units are neccessary here
      coaString += this.Stud.ToCoaString();

      foreach (ILoad load in this.Loads)
        coaString += load.ToCoaString(this.Name, forceUnit, lengthUnit);

      // EC4_DESIGN_OPTION seems to be part of DesignCode..

      return coaString;
    }
    #endregion
  }
}
