﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComposAPI
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
  }
}
