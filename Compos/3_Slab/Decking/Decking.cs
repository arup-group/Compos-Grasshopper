﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace ComposAPI
{
  public class Decking
  {
    public enum DeckingType
    {
      Custom,
      Catalogue
    }

    public Length b1 { get; set; }
    public Length b2 { get; set; }
    public Length b3 { get; set; }
    public Length b4 { get; set; }
    public Length b5 { get; set; }
    public Length Depth { get; set; }
    public Length Thickness { get; set; }
    public DeckingConfiguration DeckConfiguration { get; set; }
    public DeckingType Type { get { return m_type; } }
    internal DeckingType m_type;
    public const string CoaIdentifier = "DECKING_CATALOGUE";


    public Decking()
    {
      // empty constructor
    }

    #region coa interop
    internal Decking(List<string> parameters)
    {

    }

    internal string ToCoaString()
    {
      return String.Empty;
    }
    #endregion

    #region methods
    public virtual Decking Duplicate()
    {
      if (this == null) { return null; }
      Decking dup = (Decking)this.MemberwiseClone();
      dup.DeckConfiguration = this.DeckConfiguration.Duplicate();
      return dup;
    }
    #endregion
  }
}
