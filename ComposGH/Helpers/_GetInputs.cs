using ComposGH.Parameters;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet.GH;
using UnitsNet;
using UnitsNet.Units;
using Oasys.Units;

namespace ComposGH.Components
{
  class GetInput
  {
    #region UnitsNet
    internal static Length Length(GH_Component owner, IGH_DataAccess DA, int inputid, LengthUnit docLengthUnit, bool isOptional = false)
    {
      GH_UnitNumber unitNumber = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          unitNumber = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(LengthUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be Length");
            return UnitsNet.Length.Zero;
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          unitNumber = new GH_UnitNumber(new Length(val, docLengthUnit));
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return UnitsNet.Length.Zero;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (unitNumber == null)
          return UnitsNet.Length.Zero;
      }

      return (Length)unitNumber.Value;
    }
    internal static List<Length> Lengths(GH_Component owner, IGH_DataAccess DA, int inputid, LengthUnit docLengthUnit, bool isOptional = false)
    {
      List<Length> lengths = new List<Length>();
      List<GH_ObjectWrapper> gh_typs = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(inputid, gh_typs))
      {
        for (int i = 0; i < gh_typs.Count; i++)
        {
          GH_UnitNumber unitNumber = null;
          // try cast directly to quantity type
          if (gh_typs[i].Value is GH_UnitNumber)
          {
            unitNumber = (GH_UnitNumber)gh_typs[i].Value;
            // check that unit is of right type
            if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(LengthUnit)))
            {
              owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Error in " + owner.Params.Input[inputid].NickName + " (item " + i + ") input: Wrong unit type"
                  + Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be Length");
            }
            else
            {
              lengths.Add((Length)unitNumber.Value);
            }
          }
          // try cast to double
          else if (GH_Convert.ToDouble(gh_typs[i].Value, out double val, GH_Conversion.Both))
          {
            // create new quantity from default units
            lengths.Add(new Length(val, docLengthUnit));
          }
          else
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert " + owner.Params.Input[inputid].NickName + " (item " + i + ") to UnitNumber");
            continue;
          }
        }
        return lengths;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return null;
    }
    internal static Pressure Stress(GH_Component owner, IGH_DataAccess DA, int inputid, PressureUnit stressUnit, bool isOptional = false)
    {
      Pressure stressFib = new Pressure();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inStress;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inStress = (GH_UnitNumber)gh_typ.Value;
          if (!inStress.Value.QuantityInfo.UnitType.Equals(typeof(PressureUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + Environment.NewLine + "Unit type is " + inStress.Value.QuantityInfo.Name + " but must be Stress (Pressure)");
            return Pressure.Zero;
          }
          stressFib = (Pressure)inStress.Value.ToUnit(stressUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inStress = new GH_UnitNumber(new Pressure(val, stressUnit));
          stressFib = (Pressure)inStress.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of Stress");
          return Pressure.Zero;
        }
        return stressFib;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return Pressure.Zero;
    }
    internal static Strain Strain(GH_Component owner, IGH_DataAccess DA, int inputid, StrainUnit strainUnit, bool isOptional = false)
    {
      Strain strainFib = new Strain();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inStrain;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inStrain = (GH_UnitNumber)gh_typ.Value;
          if (!inStrain.Value.QuantityInfo.UnitType.Equals(typeof(StrainUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + Environment.NewLine + "Unit type is " + inStrain.Value.QuantityInfo.Name + " but must be Strain");
            return Oasys.Units.Strain.Zero;
          }
          strainFib = (Strain)inStrain.Value.ToUnit(strainUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inStrain = new GH_UnitNumber(new Strain(val, strainUnit));
          strainFib = (Strain)inStrain.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of Strain");
          return Oasys.Units.Strain.Zero;
        }
        return strainFib;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return Oasys.Units.Strain.Zero;
    }
    internal static Curvature Curvature(GH_Component owner, IGH_DataAccess DA, int inputid, CurvatureUnit curvatureUnit, bool isOptional = false)
    {
      Curvature crvature = new Curvature();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inStrain;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inStrain = (GH_UnitNumber)gh_typ.Value;
          if (!inStrain.Value.QuantityInfo.UnitType.Equals(typeof(CurvatureUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + Environment.NewLine + "Unit type is " + inStrain.Value.QuantityInfo.Name + " but must be Curvature");
            return Oasys.Units.Curvature.Zero;
          }
          crvature = (Curvature)inStrain.Value.ToUnit(curvatureUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inStrain = new GH_UnitNumber(new Curvature(val, curvatureUnit));
          crvature = (Curvature)inStrain.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of Curvature");
          return Oasys.Units.Curvature.Zero;
        }
        return crvature;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return Oasys.Units.Curvature.Zero;
    }
    internal static Force Force(GH_Component owner, IGH_DataAccess DA, int inputid, ForceUnit forceUnit, bool isOptional = false)
    {
      Force force = new Force();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inForce;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inForce = (GH_UnitNumber)gh_typ.Value;
          if (!inForce.Value.QuantityInfo.UnitType.Equals(typeof(ForceUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + Environment.NewLine + "Unit type is " + inForce.Value.QuantityInfo.Name + " but must be Force");
            return UnitsNet.Force.Zero;
          }
          force = (Force)inForce.Value.ToUnit(forceUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inForce = new GH_UnitNumber(new Force(val, forceUnit));
          force = (Force)inForce.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of Force");
          return UnitsNet.Force.Zero;
        }
        return force;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return UnitsNet.Force.Zero;
    }
    internal static Moment Moment(GH_Component owner, IGH_DataAccess DA, int inputid, MomentUnit momentUnit, bool isOptional = false)
    {
      Moment moment = new Moment();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inMoment;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inMoment = (GH_UnitNumber)gh_typ.Value;
          if (!inMoment.Value.QuantityInfo.UnitType.Equals(typeof(MomentUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + Environment.NewLine + "Unit type is " + inMoment.Value.QuantityInfo.Name + " but must be Moment");
            return Oasys.Units.Moment.Zero;
          }
          moment = (Moment)inMoment.Value.ToUnit(momentUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inMoment = new GH_UnitNumber(new Moment(val, momentUnit));
          moment = (Moment)inMoment.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of Moment");
          return Oasys.Units.Moment.Zero;
        }
        return moment;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return Oasys.Units.Moment.Zero;
    }
    internal static ForcePerLength ForcePerLength(GH_Component owner, IGH_DataAccess DA, int inputid, ForcePerLengthUnit forceUnit, bool isOptional = false)
    {
      ForcePerLength force = new ForcePerLength();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inForce;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inForce = (GH_UnitNumber)gh_typ.Value;
          if (!inForce.Value.QuantityInfo.UnitType.Equals(typeof(ForceUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + inForce.Value.QuantityInfo.Name + " but must be ForcePerLength");
            return UnitsNet.ForcePerLength.Zero;
          }
          force = (ForcePerLength)inForce.Value.ToUnit(forceUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inForce = new GH_UnitNumber(new ForcePerLength(val, forceUnit));
          force = (ForcePerLength)inForce.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of ForcePerLength");
          return UnitsNet.ForcePerLength.Zero;
        }
        return force;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return UnitsNet.ForcePerLength.Zero;
    }
    internal static Angle Angle(GH_Component owner, IGH_DataAccess DA, int inputid, AngleUnit angleUnit, bool isOptional = false)
    {
      GH_UnitNumber a1 = new GH_UnitNumber(new Angle(0, angleUnit));
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          a1 = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!a1.Value.QuantityInfo.UnitType.Equals(typeof(AngleUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + Environment.NewLine + "Unit type is " + a1.Value.QuantityInfo.Name + " but must be Angle");
            return UnitsNet.Angle.Zero;
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          a1 = new GH_UnitNumber(new Angle(val, angleUnit));
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Angle");
          return UnitsNet.Angle.Zero;
        }
        return (Angle)a1.Value;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return UnitsNet.Angle.Zero;
    }
    #endregion

    #region Studs
    internal static StudDimensions StudDim(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      StudDimensionsGoo goo = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is StudDimensionsGoo)
        {
          goo = (StudDimensionsGoo)gh_typ.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Stud Dimensions");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (goo == null)
          return null;
      }

      return goo.Value;
    }
    internal static StudSpecification StudSpec(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      StudSpecificationGoo goo = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is StudSpecificationGoo)
        {
          goo = (StudSpecificationGoo)gh_typ.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Stud Specification");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (goo == null)
          return null;
      }

      return goo.Value;
    }
    internal static List<StudGroupSpacing> StudSpacings(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      List<StudGroupSpacing> items = new List<StudGroupSpacing>();
      List<GH_ObjectWrapper> gh_typs = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(inputid, gh_typs))
      {
        for (int i = 0; i < gh_typs.Count; i++)
        {
          // try cast directly to quantity type
          if (gh_typs[i].Value is StudGroupSpacingGoo)
          {
            StudGroupSpacingGoo goo = (StudGroupSpacingGoo)gh_typs[i].Value;
            items.Add(goo.Value);
          }
          else
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert " + owner.Params.Input[inputid].NickName + " (item " + i + ") to Stud Spacing");
            continue;
          }
        }
        return items;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return null;
    }
    #endregion

    #region Reinforcement
    internal static ComposReinforcement Reinforcement(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      ComposReinforcementGoo goo = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        if (gh_typ.Value is ComposReinforcementGoo)
        {
          goo = (ComposReinforcementGoo)gh_typ.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Compos Reinforcement");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (goo == null)
          return null;
      }
      return goo.Value;
    }
    internal static List<ComposReinforcement> TransverseReinforcements(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      List<ComposReinforcement> items = new List<ComposReinforcement>();
      List<GH_ObjectWrapper> gh_typs = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(inputid, gh_typs))
      {
        for (int i = 0; i < gh_typs.Count; i++)
        {
          // try cast directly to quantity type
          if (gh_typs[i].Value is ComposReinforcementGoo)
          {
            ComposReinforcementGoo goo = (ComposReinforcementGoo)gh_typs[i].Value;
            items.Add(goo.Value);
          }
          else
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert " + owner.Params.Input[inputid].NickName + " (item " + i + ") to Compos Reinforcement");
            continue;
          }
        }
        return items;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return null;
    }
    internal static RebarMaterial RebarMaterial(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      RebarMaterialGoo goo = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        if (gh_typ.Value is RebarMaterialGoo)
        {
          goo = (RebarMaterialGoo)gh_typ.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Rebar material");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (goo == null)
          return null;
      }
      return goo.Value;
    }
    #endregion

    #region beam
    internal static WebOpeningStiffeners WebOpeningStiffeners(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      WebOpeningStiffenersGoo goo = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        if (gh_typ.Value is WebOpeningStiffenersGoo)
        {
          goo = (WebOpeningStiffenersGoo)gh_typ.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Compos Web Opening Stiffeners");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (goo == null)
          return null;
      }
      return goo.Value;
    }
    internal static List<ComposWebOpening> WebOpenings(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      List<ComposWebOpening> items = new List<ComposWebOpening>();
      List<GH_ObjectWrapper> gh_typs = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(inputid, gh_typs))
      {
        for (int i = 0; i < gh_typs.Count; i++)
        {
          // try cast directly to quantity type
          if (gh_typs[i].Value is ComposWebOpeningGoo)
          {
            ComposWebOpeningGoo goo = (ComposWebOpeningGoo)gh_typs[i].Value;
            items.Add(goo.Value);
          }
          else
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert " + owner.Params.Input[inputid].NickName + " (item " + i + ") to Compos Web Openings");
            continue;
          }
        }
        return items;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return null;
    }

    internal static ComposWebOpening WebOpening(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      ComposWebOpeningGoo goo = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        if (gh_typ.Value is ComposWebOpeningGoo)
        {
          goo = (ComposWebOpeningGoo)gh_typ.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Compos Web Opening");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (goo == null)
          return null;
      }
      return goo.Value;
    }
    internal static List<BeamSection> BeamSections(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      List<BeamSection> items = new List<BeamSection>();
      List<GH_ObjectWrapper> gh_typs = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(inputid, gh_typs))
      {
        for (int i = 0; i < gh_typs.Count; i++)
        {
          string profile = "";
          // try cast directly to quantity type
          if (gh_typs[i].Value is BeamSectionGoo)
          {
            BeamSectionGoo goo = (BeamSectionGoo)gh_typs[i].Value;
            items.Add(goo.Value);
          }
          else if (gh_typs[i].CastTo(ref profile))
          {
            try
            {
              items.Add(new BeamSection(profile));
            }
            catch (Exception e)
            {
              owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert " + owner.Params.Input[inputid].NickName + " (item " + i + ") to Compos Beam Section: " 
                + System.Environment.NewLine + e.Message);
              continue;
            }
          }
          else
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert " + owner.Params.Input[inputid].NickName + " (item " + i + ") to Compos Beam Section");
            continue;
          }
        }
        return items;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return null;
    }
    internal static string BeamSection(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      BeamSectionGoo goo = null;
      string profile = "";
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        if (gh_typ.Value is BeamSectionGoo)
        {
          goo = (BeamSectionGoo)gh_typ.Value;
          return goo.Value.SectionDescription;
        }
        else if (gh_typ.CastTo(ref profile))
          return profile;
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Compos Web Opening");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      
      return String.Empty;
    }

    internal static Supports Supports(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      SupportsGoo goo = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        if (gh_typ.Value is SupportsGoo)
        {
          goo = (SupportsGoo)gh_typ.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Compos Support");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (goo == null)
          return null;
      }
      return goo.Value;
    }

    internal static ComposRestraint Restraint(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      ComposRestraintGoo goo = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        if (gh_typ.Value is ComposRestraintGoo)
        {
          goo = (ComposRestraintGoo)gh_typ.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Compos Restraint");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (goo == null)
          return null;
      }
      return goo.Value;
    }

    internal static ComposSteelMaterial SteelMaterial(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      ComposSteelMaterialGoo goo = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        if (gh_typ.Value is ComposSteelMaterialGoo)
        {
          goo = (ComposSteelMaterialGoo)gh_typ.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Compos Steel Material");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (goo == null)
          return null;
      }
      return goo.Value;
    }
    #endregion
  }
}
