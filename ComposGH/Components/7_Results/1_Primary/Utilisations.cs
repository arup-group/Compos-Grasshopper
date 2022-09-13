using System;
using System.Drawing;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components
{
  public class Utilisations : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("dcbd858c-6077-40a0-b109-ed0a3e2d7217");
    public Utilisations()
      : base("Utilisation Results",
          "Utilisations",
          "Get overall utilisation results for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, MemberGoo.Description, GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Moment", "M", "Final moment utilisation factor.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear", "V", "Final shear utilisation factor.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Moment Construction", "Mc", "Construction stage moment utilisation factor.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Construction", "Vc", "Construction stage shear utilisation factor.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Buckling Construction", "Vc", "Construction stage buckling utilisation factor.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Deflection", "dc", "Construction stage deflection utilisation factor.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Deflection", "d", "Final deflection utilisation factor.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Transverse Shear", "Tv", "Transverse shear utilisation factor.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Web Opening", "Wo", "Web opening (max) utilisation factor.", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      IUtilisation result = res.Utilisations;

      maxUtil = -1;

      int i = 0;
      double output = result.Moment.DecimalFractions;
      if (output > 1)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Moment utilisation is bigger than 100%");
      if (output > maxUtil)
        maxUtil = output;
      DA.SetData(i++, output);

      output = result.Shear.DecimalFractions;
      if (output > 1)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Shear utilisation is bigger than 100%");
      if (output > maxUtil)
        maxUtil = output;
      DA.SetData(i++, output);

      output = result.MomentConstruction.DecimalFractions;
      if (output > 1)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Moment Construction stage utilisation is bigger than 100%");
      if (output > maxUtil)
        maxUtil = output;
      DA.SetData(i++, output);

      output = result.ShearConstruction.DecimalFractions;
      if (output > 1)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Shaer Construction stage utilisation is bigger than 100%");
      if (output > maxUtil)
        maxUtil = output;
      DA.SetData(i++, output);

      output = result.BucklingConstruction.DecimalFractions;
      if (output > 1)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Buckling Construction stage utilisation is bigger than 100%");
      if (output > maxUtil)
        maxUtil = output;
      DA.SetData(i++, output);

      output = result.DeflectionConstruction.DecimalFractions;
      if (output > 1)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Deflection Construction stage utilisation is bigger than 100%");
      if (output > maxUtil)
        maxUtil = output;
      DA.SetData(i++, output);

      output = result.Deflection.DecimalFractions;
      if (output > 1)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Deflection utilisation is bigger than 100%");
      if (output > maxUtil)
        maxUtil = output;
      DA.SetData(i++, output);

      output = result.TransverseShear.DecimalFractions;
      if (output > 1)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Transverse Shear utilisation is bigger than 100%");
      if (output > maxUtil)
        maxUtil = output;
      DA.SetData(i++, output);

      output = result.WebOpening.DecimalFractions;
      if (output > 1)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Web Opening utilisation is bigger than 100%");
      if (output > maxUtil)
        maxUtil = output;
      DA.SetData(i, output);
      
      base.DestroyIconCache();
    }

    double maxUtil = -1;
    protected override Bitmap Icon
    {
      get
      {
        if (maxUtil < 0)
          return Resources.Utilisation;
        else if (maxUtil < 0.50)
          return Resources.UtilisationLow;
        else if (maxUtil < 0.80)
          return Resources.UtilisationMedium;
        else if (maxUtil <= 1)
          return Resources.UtilisationHigh;
        else
          return Resources.UtilisationOver;
      }
    }
    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }

    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
    }
    #endregion
  }
}
