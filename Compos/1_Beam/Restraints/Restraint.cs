﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{

  /// <summary>
  /// Restraint object that contains two <see cref="Supports"/> objects for 'Construction Stage Support' and 'Final Stage Support', and if top flange is laterally restrained in at construction stage.
  /// </summary>
  public class Restraint : IRestraint
  {
    public ISupports ConstructionStageSupports { get; set; }
    public ISupports FinalStageSupports { get; set; }
    public bool TopFlangeRestrained { get; set; }
    private bool finalSupportsSet;

    #region constructors
    public Restraint()
    {
      // empty constructore
    }
    public Restraint(bool topFlangeRestrained, ISupports constructionStageSupports, ISupports finalStageSupports)
    {
      this.TopFlangeRestrained = topFlangeRestrained;
      this.ConstructionStageSupports = constructionStageSupports;
      this.FinalStageSupports = finalStageSupports;
      this.finalSupportsSet = true;
    }
    public Restraint(bool topFlangeRestrained, Supports constructionStageSupports)
    {
      this.TopFlangeRestrained = topFlangeRestrained;
      this.ConstructionStageSupports = constructionStageSupports;
      this.finalSupportsSet = false;
    }
    #endregion

    #region coa interop
    internal Restraint(string coaString)
    {
      // to be done
    }

    internal string Coa()
    {
      // to be done
      return "";
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string top = (TopFlangeRestrained) ? "TFLR, " : "";
      string con = "Constr.: " + this.ConstructionStageSupports.ToString();
      string fin = ", Final: None";
      if (this.FinalStageSupports != null)
        fin = ", Final: " + this.FinalStageSupports.ToString();
      return top + con + fin;
    }

    #endregion
  }
}
