﻿
using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Helpers;

namespace ComposGH.Components
{
  public abstract class GH_OasysComponent : GH_Component
  {
    public GH_OasysComponent(string name, string nickname, string description, string category, string subCategory) : base(name, nickname, description, category, subCategory)
    {
    }

    public override void AddedToDocument(GH_Document document)
    {
      PostHog.AddedToDocument(this);
      base.AddedToDocument(document);
    }

    public override void RemovedFromDocument(GH_Document document)
    {
      PostHog.RemovedFromDocument(this);
      base.RemovedFromDocument(document);
    }
  }
}