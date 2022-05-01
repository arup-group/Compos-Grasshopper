using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComposAPI.SteelBeam;
using ComposAPI.Studs;
using ComposAPI.ConcreteSlab;
using ComposAPI.Loads;

namespace ComposAPI.Member
{
  public class Member
  {
    public Beam Beam { get; set; }
    public Stud Stud { get; set; }
    public Slab Slab { get; set; }
    public List<Load> Loads { get; set; }
    public DesignCode DesignCode { get; set; }

    public string Name { get; set; }
    public string GridReference { get; set; } = "";
    public string Note { get; set; } = "";

    public Member() { }

    public Member(string name, DesignCode designCode, Beam beam, Stud stud, Slab slab, List<Load> loads)
    {
      this.Name = name;
      this.DesignCode = designCode;
      this.Beam = beam;
      this.Stud = stud;
      this.Slab = slab;
      this.Loads = loads;
    }
    public Member(string name, string gridRef, string note, DesignCode designCode, Beam beam, Stud stud, Slab slab, List<Load> loads)
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
    public Member Duplicate()
    {
      if (this == null) { return null; }
      Member dup = (Member)this.MemberwiseClone();
      dup.DesignCode = this.DesignCode.Duplicate();
      dup.Beam = this.Beam.Duplicate();
      dup.Stud = this.Stud.Duplicate();
      dup.Slab = this.Slab.Duplicate();
      dup.Loads = this.Loads.ToList();
      return dup;
    }
  }
}
