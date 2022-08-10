﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.UI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace ComposGH.Components
{
  /// <summary>
  /// Component to check if a Compos model satisfies the chosen code
  /// </summary>
  public class DesignMember : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c422ae16-b8c0-4203-86c7-43c3f2917075");
    public DesignMember()
      : base("DesignMember", "Design", "Design (size) the Steel Beam of a Compos Member",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override Bitmap Icon => Properties.Resources.DesignMember;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, MemberGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(DesignCriteriaGoo.Name, DesignCriteriaGoo.NickName, DesignCriteriaGoo.Description, GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, "Designed " + MemberGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      MemberGoo memGoo = (MemberGoo)GetInput.GenericGoo<MemberGoo>(this, DA, 0);
      DesignCriteriaGoo critGoo = (DesignCriteriaGoo)GetInput.GenericGoo<DesignCriteriaGoo>(this, DA, 1);
      Message = "";
      if (memGoo.Value != null)
      {
        Member designedMember = (Member)memGoo.Value.Duplicate();
        designedMember.DesignCriteria = critGoo.Value;
        if (!designedMember.Design())
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to design member");
          return;
        }
        string[] profile = designedMember.Beam.Sections[0].SectionDescription.Split(' ');
        Message = profile[2];
        DA.SetData(0, new MemberGoo(designedMember));
      }
    }
  }
}