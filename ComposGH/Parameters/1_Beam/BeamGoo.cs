using System;
using System.Collections.Generic;
using System.Drawing;
using ComposAPI;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using Rhino.Geometry;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Parameters
{
  /// <summary>
  /// GeometryGoo wrapper class, makes sure our custom class can be used and displayed in Grasshopper.
  /// </summary>
  public class BeamGoo : GH_GeometricGoo<IBeam>, IGH_PreviewData
  {
    public LineCurve Line { get; set; }
    LengthUnit LengthUnit { get; set; }

    public static string Name => "Beam";
    public static string NickName => "Bm";
    public static string Description => "Compos Steel Beam";

    #region constructors
    public BeamGoo(LineCurve line, LengthUnit lengthUnit, IRestraint restraint, ISteelMaterial material, List<IBeamSection> beamSections, List<IWebOpening> webOpenings = null)
    {
      this.Line = line;
      this.LengthUnit = lengthUnit;
      Length length = new Length(line.GetLength(), lengthUnit);
      this.Value = new Beam(length, restraint, material, beamSections, webOpenings);
      UpdatePreview();
    }

    public BeamGoo()
    {
      this.Value = new Beam();
    }

    public BeamGoo(LineCurve line, LengthUnit lengthUnit, IBeam item)
    {
      if (item == null)
        item = new Beam();
      this.Line = (LineCurve)line.DuplicateShallow();
      this.LengthUnit = lengthUnit;
      this.Value = item.Duplicate() as IBeam;
      UpdatePreview();
    }

    private BeamGoo(BeamGoo goo)
    {
      this.Line = (LineCurve)goo.Line.Duplicate();
      this.LengthUnit = goo.LengthUnit;
      this.Value = goo.Value.Duplicate() as IBeam;
    }

    public override IGH_Goo Duplicate()
    {
      BeamGoo dup = new BeamGoo();
      dup.Line = (LineCurve)this.Line.DuplicateShallow();
      dup.LengthUnit = this.LengthUnit;
      dup.Value = this.Value.Duplicate() as IBeam;
      dup.UpdatePreview();
      return dup;
    }

    public override IGH_GeometricGoo DuplicateGeometry()
    {
      if (Value == null)
        return null;
      else
        return (IGH_GeometricGoo)this.Duplicate();
    }
    #endregion

    #region properties
    public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Beam";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (IsValid) { return string.Empty; }
        return IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }
    public override string ToString()
    {
      if (Value == null)
        return "Null";
      else
        return "Compos " + TypeName + " {" + Value.ToString() + "}"; ;
    }

    public override BoundingBox Boundingbox
    {
      get
      {
        if (Value == null) { return BoundingBox.Empty; }
        if (Line == null) { return BoundingBox.Empty; }
        return Line.GetBoundingBox(false);
      }
    }
    public override BoundingBox GetBoundingBox(Transform xform)
    {
      if (Value == null) { return BoundingBox.Empty; }
      if (Line == null) { return BoundingBox.Empty; }
      return Line.GetBoundingBox(xform);
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(Beam)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      //Cast to Curve
      if (typeof(Q).IsAssignableFrom(typeof(Line)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)this.Line;
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Line)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Line ghLine = new GH_Line();
          GH_Convert.ToGHLine(this.Line, GH_Conversion.Both, ref ghLine);
          target = (Q)(object)ghLine;
        }

        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(Curve)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)this.Line;
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Curve(this.Line);
        }
        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into our custom class.

      if (source == null) { return false; }

      //Cast from GsaMaterial
      if (typeof(Beam).IsAssignableFrom(source.GetType()))
      {
        Value = (Beam)source;
        return true;
      }

      try
      {
        // Cast from GsaGH
        //if (GsaGHConverter.IsPresent())
        //{
        //  Type type = GsaGHConverter.GetTypeFor(typeof(IComposBeam));
        //  if (type.IsAssignableFrom(source.GetType()))
        //  {
        //    Value = (Beam)GsaGHConverter.CastToComposBeam(source);
        //    return true;
        //  }
        //}
        //// Cast from AdSecGH
        //if (AdSecGHConverter.IsPresent())
        //{
        //  Type type = AdSecGHConverter.GetTypeFor(typeof(IComposBeam));
        //  if (type.IsAssignableFrom(source.GetType()))
        //  {
        //    Value = (Beam)AdSecGHConverter.CastToComposBeam(source);
        //    return true;
        //  }
        //}
        //// Cast from Speckle
        //if (SpeckleConverter.IsPresent())
        //{
        //  // todo: implement
        //}

      }
      catch (Exception)
      {
        return false;
      }

      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      if (Value == null) { return null; }
      if (Line == null) { return null; }

      BeamGoo dup = new BeamGoo(this);
      LineCurve xLn = dup.Line;
      xLn.Transform(xform);
      dup.Line = xLn;
      dup.UpdatePreview();

      return dup;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      if (Value == null) { return null; }
      if (Line == null) { return null; }

      BeamGoo dup = new BeamGoo(this);
      LineCurve xLn = dup.Line;
      xmorph.Morph(xLn);
      dup.Line = xLn;
      dup.LengthUnit = this.LengthUnit;
      dup.UpdatePreview();

      return dup;
    }

    #endregion

    #region preview geometry
    //List<PolyCurve> profileOutlines;
    //List<Brep> profileExtrusions;
    //List<Brep> stiffenerPlates;
    void UpdatePreview()
    {
      return;
    //  profileOutlines = new List<PolyCurve>();
    //  profileExtrusions = new List<Brep>();
    //  List<IBeamSection> beamSectionsSorted = SortBeamSections(this.Value.Sections.ToList());

    //  for (int i = 0; i < beamSectionsSorted.Count; i++)
    //  {
    //    IBeamSection beamSection = beamSectionsSorted[i];
    //    PolyCurve outline = CreateLocalPlaneOutline(beamSection);
    //    profileOutlines.Add(outline);
    //    if (i == 0 | i == beamSectionsSorted.Count - 1)
    //      profileExtrusions.Add(Brep.CreatePlanarBreps(outline, 0.0001)[0]);


    //    if (i != beamSectionsSorted.Count - 1) // don't extrude last
    //    {
    //      //  if (beamSection.TaperedToNext)
    //      //  {
    //      //    IBeamSection beamSectionNext = new BeamSection();
    //      //    if (beamSectionsSorted[i + 1].isCatalogue)
    //      //    {
    //      //      beamSectionNext =
    //      //        new BeamSection(
    //      //          beamSectionsSorted[i + 1].Depth,
    //      //          beamSectionsSorted[i + 1].TopFlangeWidth,
    //      //          beamSectionsSorted[i + 1].WebThickness,
    //      //          beamSectionsSorted[i + 1].TopFlangeThickness);
    //      //      beamSectionNext.StartPosition = beamSectionsSorted[i + 1].StartPosition;
    //      //    }
    //      //    else
    //      //      beamSectionNext = beamSectionsSorted[i + 1];
    //      //    PolyCurve nextOutline = CreateLocalPlaneOutline(beamSectionNext);
    //      //    ExtrudeBetweenOutlines(outline, nextOutline);
    //      //  }
    //      //  else
    //      //  {
    //      //    IBeamSection beamSectionNext = beamSection.Duplicate() as BeamSection;
    //      //    beamSectionNext.StartPosition = beamSectionsSorted[i + 1].StartPosition;
    //      //    PolyCurve nextOutline = CreateLocalPlaneOutline(beamSectionNext);
    //      //    profileOutlines.Add(nextOutline);
    //      //    ExtrudeBetweenOutlines(outline, nextOutline);
    //      //    profileExtrusions.Add(Brep.CreatePlanarBreps(nextOutline, 0.0001)[0]);
    //      //    beamSectionNext = beamSectionsSorted[i + 1];
    //      //    PolyCurve nextBeamOutline = CreateLocalPlaneOutline(beamSectionNext);
    //      //    profileExtrusions.Add(Brep.CreatePlanarBreps(nextBeamOutline, 0.0001)[0]);
    //      //  }
    //    }
    //  }

    //  profileExtrusions = Brep.JoinBreps(profileExtrusions, Units.Tolerance.As(LengthUnit)).ToList();
    //  for (int i = 0; i < profileExtrusions.Count; i++)
    //  {
    //    profileExtrusions[i].Compact();
    //    if (!profileExtrusions[i].IsSolid)
    //      profileExtrusions[i] = profileExtrusions[i].CapPlanarHoles(Units.Tolerance.As(LengthUnit));
    //  }
    //  profileExtrusions = Brep.CreateBooleanUnion(profileExtrusions, Units.Tolerance.As(LengthUnit)).ToList();

    //  if (this.Value.WebOpenings != null)
    //  {
    //    Length maxWebThickness = new Length(this.Value.Sections.Max(x => x.WebThickness.As(LengthUnit)), LengthUnit);
    //    foreach (WebOpening webOpening in this.Value.WebOpenings)
    //    {
    //      Brep cutter = OpeningCutter(webOpening, maxWebThickness, beamSectionsSorted);
    //      bool found = false;
    //      for (int i = 0; i < profileExtrusions.Count; i++)
    //      {
    //        if (webOpening.WebOpeningType == OpeningType.Rectangular |
    //            webOpening.WebOpeningType == OpeningType.Circular)
    //        {
    //          Brep[] cuts = profileExtrusions[i].Split(cutter, Units.Tolerance.As(LengthUnit));
    //          if (cuts.Length > 0)
    //          {
    //            found = true;

    //            Brep trimmedProfile = cuts.First();
    //            Brep[] cutEdges = cutter.Split(trimmedProfile, Units.Tolerance.As(LengthUnit));
    //            Brep cutEdge = cutEdges.First();
    //            profileExtrusions[i] = Brep.JoinBreps(
    //              new List<Brep>() { trimmedProfile, cutEdge },
    //              Units.Tolerance.As(LengthUnit)).ToList()[0];
    //          }

    //        }
    //        else // notch
    //        {
    //          Brep[] trims = profileExtrusions[i].Trim(cutter, Units.Tolerance.As(LengthUnit));
    //          if (trims.Length > 0)
    //          {
    //            found = true;
    //            Brep trimmedProfile = trims.First();
    //            List<Brep> cutEdges = cutter.Trim(trimmedProfile, Units.Tolerance.As(LengthUnit)).ToList();
    //            cutEdges.Add(trimmedProfile);
    //            profileExtrusions[i] = Brep.JoinBreps(cutEdges, Units.Tolerance.As(LengthUnit)).ToList()[0];
    //          }
    //        }
    //      }
    //      if (!found)
    //        throw new Exception("One or more Web opening does not intersect the beam's web.");
    //    }
    //  }
    //  profileExtrusions = Brep.CreateBooleanUnion(profileExtrusions, Units.Tolerance.As(LengthUnit)).ToList();

    //}
    //private Brep OpeningCutter(IWebOpening webOpening, Length webThickness, List<IBeamSection> beamSectionsSorted)
    //{
    //  List<Brep> parts = new List<Brep>();

    //  // find local plane on line
    //  Plane local = new Plane();

    //  double t = webOpening.CentroidPosFromStart.As(LengthUnit);
    //  if (webOpening.WebOpeningType == OpeningType.End_notch)
    //    t = this.Value.Length.As(LengthUnit);

    //  if (t > this.Value.Length.As(LengthUnit))
    //    throw new Exception("Web Opening Start Position lies outside the Beam's domain");
    //  this.Line.PerpendicularFrameAt(t, out local);

    //  // rotate perpendicular plane to be parallel to beam
    //  local.Rotate(Math.PI / 2, local.YAxis);

    //  // transform outline to local plane
    //  Transform maptToLocal = Rhino.Geometry.Transform.PlaneToPlane(Plane.WorldXY, local);

    //  switch (webOpening.WebOpeningType)
    //  {
    //    case OpeningType.Rectangular:
    //      PolyCurve rect = RectangularOpening(webOpening);
    //      rect.Transform(maptToLocal);
    //      // move curve away from web
    //      rect.Translate(new Vector3d(local.Normal * webThickness.As(LengthUnit)));
    //      // mirror to other side of web
    //      PolyCurve rect2 = (PolyCurve)rect.Duplicate();
    //      rect2.Translate(new Vector3d(local.Normal * webThickness.As(LengthUnit) * -2));
    //      // connect
    //      for (int i = 0; i < rect.SegmentCount; i++)
    //      {
    //        Curve startSegment = rect.SegmentCurve(i);
    //        Curve endSegment = rect2.SegmentCurve(i);
    //        parts.Add(Brep.CreateEdgeSurface(new List<Curve>() { startSegment, endSegment }));
    //      }

    //      break;

    //    case OpeningType.Circular:
    //      ArcCurve circ = CircularOpening(webOpening);
    //      circ.Transform(maptToLocal);
    //      // move curve away from web
    //      circ.Translate(new Vector3d(local.Normal * webThickness.As(LengthUnit)));
    //      // mirror to other side of web
    //      ArcCurve circ2 = (ArcCurve)circ.Duplicate(); // Circle is a struct that copies on assign
    //      circ2.Translate(new Vector3d(local.Normal * webThickness.As(LengthUnit) * -2));
    //      // connect
    //      parts.Add(Brep.CreateFromLoft(new List<Curve>() { circ, circ2 }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]);

    //      break;

    //    case OpeningType.Start_notch:
    //      IBeamSection beamstart = beamSectionsSorted.First();
    //      PolyCurve start = StartNotch(webOpening, beamstart.Depth);
    //      start.Transform(maptToLocal);
    //      // move curve away from web
    //      start.Translate(new Vector3d(local.Normal * beamstart.BottomFlangeWidth.As(LengthUnit) / 2 * 1.01));
    //      // mirror to other side of web
    //      PolyCurve start2 = (PolyCurve)start.Duplicate();
    //      start2.Translate(new Vector3d(local.Normal * beamstart.BottomFlangeWidth.As(LengthUnit) / 2 * 1.01 * -2));
    //      // connect
    //      for (int i = 0; i < start.SegmentCount; i++)
    //      {
    //        Curve startSegment = start.SegmentCurve(i);
    //        Curve endSegment = start2.SegmentCurve(i);
    //        parts.Add(Brep.CreateEdgeSurface(new List<Curve>() { startSegment, endSegment }));
    //      }
    //      break;

    //    case OpeningType.End_notch:
    //      IBeamSection beamend = beamSectionsSorted.Last();
    //      PolyCurve end = EndNotch(webOpening, beamend.Depth);
    //      end.Transform(maptToLocal);
    //      // move curve away from web
    //      end.Translate(new Vector3d(local.Normal * beamend.BottomFlangeWidth.As(LengthUnit) / 2 * 1.01));
    //      // mirror to other side of web
    //      PolyCurve end2 = (PolyCurve)end.Duplicate();
    //      end2.Translate(new Vector3d(local.Normal * beamend.BottomFlangeWidth.As(LengthUnit) / 2 * 1.01 * -2));
    //      // connect
    //      for (int i = 0; i < end.SegmentCount; i++)
    //      {
    //        Curve startSegment = end.SegmentCurve(i);
    //        Curve endSegment = end2.SegmentCurve(i);
    //        parts.Add(Brep.CreateEdgeSurface(new List<Curve>() { startSegment, endSegment }));
    //      }
    //      break;
    //  }
    //  return Brep.JoinBreps(parts, Units.Tolerance.As(LengthUnit)).ToList()[0];
    }
    //private PolyCurve RectangularOpening(IWebOpening webOpening)
    //{
    //  // ## adding segments clockwise, assume xy plane with centre at profile top flange
    //  //
    //  //                 Origin
    //  //                   x
    //  //
    //  //    1 ---------------------------- 2
    //  //     |                            |
    //  //     |                            |
    //  //     |                            |
    //  //     |                            |
    //  //     |                            |
    //  //    4 ---------------------------- 3
    //  //
    //  // (c) Kr1stj4n 2oo2
    //  //

    //  PolyCurve m_crv = null;
    //  if (webOpening != null)
    //  {
    //    LengthUnit unit = LengthUnit;

    //    if (webOpening.WebOpeningType == OpeningType.Rectangular)
    //    {
    //      m_crv = new PolyCurve();
    //      // top left
    //      Point3d rec1 = new Point3d(
    //        webOpening.Width.As(unit) / 2 * -1, // X
    //        webOpening.CentroidPosFromTop.As(unit) * -1 + webOpening.Height.As(unit) / 2, // Y
    //        0);
    //      // top right
    //      Point3d rec2 = new Point3d(
    //        webOpening.Width.As(unit) / 2, // X
    //        webOpening.CentroidPosFromTop.As(unit) * -1 + webOpening.Height.As(unit) / 2, // Y
    //        0);
    //      m_crv.Append(new Line(rec1, rec2));
    //      // bottom right
    //      Point3d rec3 = new Point3d(
    //        webOpening.Width.As(unit) / 2, // X
    //        webOpening.CentroidPosFromTop.As(unit) * -1 - webOpening.Height.As(unit) / 2, // Y
    //        0);
    //      m_crv.Append(new Line(rec2, rec3));
    //      Point3d rec4 = new Point3d(
    //        webOpening.Width.As(unit) / 2 * -1, // X
    //        webOpening.CentroidPosFromTop.As(unit) * -1 - webOpening.Height.As(unit) / 2, // Y
    //        0);
    //      m_crv.Append(new Line(rec3, rec4));
    //      m_crv.Append(new Line(rec4, rec1));
    //      return m_crv;
    //    }
    //  }
    //  return null;
    //}
    //private PolyCurve EndNotch(IWebOpening webOpening, Length beamDepth)
    //{
    //  // ## adding segments clockwise, assume xy plane with centre at profile top flange 
    //  // Rhino maps x-axis inverse, so this is end even though logically it should be start
    //  //     Origin
    //  //     x
    //  //
    //  //    1 ---------------------------- 2
    //  //     |                            |
    //  //     |                            |
    //  //     |                            |
    //  //     |                            |
    //  //     |                            |
    //  //    4 ---------------------------- 3
    //  //
    //  // (c) Kr1stj4n 2oo2
    //  //

    //  PolyCurve m_crv = null;
    //  if (webOpening != null)
    //  {
    //    LengthUnit unit = LengthUnit;

    //    if (webOpening.WebOpeningType == OpeningType.End_notch)
    //    {
    //      m_crv = new PolyCurve();
    //      // top left
    //      Point3d rec1 = new Point3d(
    //        -0.1, // X
    //        beamDepth.As(unit) * -1 + webOpening.Height.As(unit), // Y
    //        0);
    //      // top right
    //      Point3d rec2 = new Point3d(
    //        webOpening.Width.As(unit), // X
    //        beamDepth.As(unit) * -1 + webOpening.Height.As(unit), // Y
    //        0);
    //      m_crv.Append(new Line(rec1, rec2));
    //      // bottom right
    //      Point3d rec3 = new Point3d(
    //        webOpening.Width.As(unit), // X
    //        beamDepth.As(unit) * -1 - 0.1, // Y
    //        0);
    //      m_crv.Append(new Line(rec2, rec3));
    //      // bottom left
    //      Point3d rec4 = new Point3d(
    //        -0.1, // X
    //        beamDepth.As(unit) * -1 - 0.1, // Y
    //        0);
    //      m_crv.Append(new Line(rec3, rec4));
    //      m_crv.Append(new Line(rec4, rec1));
    //      return m_crv;
    //    }
    //  }
    //  return null;
    //}
    //private PolyCurve StartNotch(IWebOpening webOpening, Length beamDepth)
    //{
    //  // ## adding segments clockwise, assume xy plane with centre at profile top flange
    //  // Rhino maps x-axis inverse, so this is start even though logically it should be end
    //  //                             Origin
    //  //                                  x
    //  //
    //  //    1 ---------------------------- 2
    //  //     |                            |
    //  //     |                            |
    //  //     |                            |
    //  //     |                            |
    //  //     |                            |
    //  //    4 ---------------------------- 3
    //  //
    //  // (c) Kr1stj4n 2oo2
    //  //

    //  PolyCurve m_crv = null;
    //  if (webOpening != null)
    //  {
    //    LengthUnit unit = LengthUnit;

    //    if (webOpening.WebOpeningType == OpeningType.Start_notch)
    //    {
    //      m_crv = new PolyCurve();
    //      // top left
    //      Point3d rec1 = new Point3d(
    //        webOpening.Width.As(unit) * -1, // X
    //        beamDepth.As(unit) * -1 + webOpening.Height.As(unit), // Y
    //        0);
    //      // top right
    //      Point3d rec2 = new Point3d(
    //        0.1, // X
    //        beamDepth.As(unit) * -1 + webOpening.Height.As(unit), // Y
    //        0);
    //      m_crv.Append(new Line(rec1, rec2));
    //      // bottom right
    //      Point3d rec3 = new Point3d(
    //        0.1, // X
    //        beamDepth.As(unit) * -1 - 0.1, // Y
    //        0);
    //      m_crv.Append(new Line(rec2, rec3));
    //      Point3d rec4 = new Point3d(
    //        webOpening.Width.As(unit) * -1, // X
    //        beamDepth.As(unit) * -1 - 0.1, // Y
    //        0);
    //      m_crv.Append(new Line(rec3, rec4));
    //      m_crv.Append(new Line(rec4, rec1));
    //      return m_crv;
    //    }
    //  }
    //  return null;
    //}
    //private ArcCurve CircularOpening(IWebOpening webOpening)
    //{
    //  Circle m_crv = new Circle();
    //  if (webOpening != null)
    //  {
    //    LengthUnit unit = this.LengthUnit;

    //    if (webOpening.WebOpeningType == OpeningType.Circular)
    //    {
    //      Point3d CP = new Point3d(
    //        0, // X
    //        webOpening.CentroidPosFromTop.As(unit) * -1, // Y
    //        0);
    //      m_crv = new Circle(CP, webOpening.Diameter.As(unit) / 2);
    //    }
    //  }
    //  return new ArcCurve(m_crv);
    //}
    //private List<IBeamSection> SortBeamSections(List<IBeamSection> beamSectionsSorted)
    //{
    //  beamSectionsSorted = beamSectionsSorted.OrderByDescending(x => x.StartPosition.As(LengthUnit)).Reverse().ToList();
    //  //if (beamSectionsSorted.First().StartPosition != Length.Zero)
    //  //{
    //  //  IBeamSection newStart = beamSectionsSorted.First().Duplicate() as IBeamSection;
    //  //  newStart.StartPosition = Length.Zero;
    //  //  newStart.TaperedToNext = true;
    //  //  beamSectionsSorted.Insert(0, newStart);
    //  //}
    //  //if (beamSectionsSorted.Last().StartPosition != this.Value.Length)
    //  //{
    //  //  beamSectionsSorted.Last().TaperedToNext = true;
    //  //  IBeamSection newEnd = beamSectionsSorted.Last().Duplicate() as IBeamSection;
    //  //  newEnd.StartPosition = this.Value.Length;
    //  //  beamSectionsSorted.Add(newEnd);
    //  //}
    //  return beamSectionsSorted;
    //}

    //private void ExtrudeBetweenOutlines(PolyCurve start, PolyCurve end)
    //{
    //  for (int j = 0; j < start.SegmentCount; j++)
    //  {
    //    Curve startSegment = start.SegmentCurve(j);
    //    Curve endSegment = end.SegmentCurve(j);
    //    profileExtrusions.Add(Brep.CreateEdgeSurface(new List<Curve>() { startSegment, endSegment }));
    //  }
    //}
    //private PolyCurve CreateLocalPlaneOutline(IBeamSection beamSection)
    //{
    //  // construct outline
    //  PolyCurve outline = CreateProfileOutline(beamSection);

    //  // find local plane on line
    //  Plane local = new Plane();

    //  double t = 0;
    //  if (beamSection.StartPosition.QuantityInfo.UnitType == typeof(LengthUnit))
    //    t = beamSection.StartPosition.As(LengthUnit);
    //  else
    //    t = beamSection.StartPosition.As(RatioUnit.DecimalFraction) * this.Value.Length.As(LengthUnit);

    //  if (t > this.Value.Length.As(LengthUnit))
    //    throw new Exception("Beam Section Start Position lies outside the Beam's domain");
    //  this.Line.PerpendicularFrameAt(t, out local);

    //  // transform outline to local plane
    //  Transform maptToLocal = Rhino.Geometry.Transform.PlaneToPlane(Plane.WorldXY, local);
    //  outline.Transform(maptToLocal);

    //  return outline;
    //}

    //private PolyCurve CreateProfileOutline(IBeamSection beamSection)
    //{
    //  // ## adding segments clockwise, assume xy plane with centre at middle of profile top flange
    //  //
    //  //                 Origin
    //  //    1 -------------x------------- 2
    //  //     |          15    4          |
    //  //   16 ----------\     /---------- 3
    //  //                 \   /
    //  //               14 | | 5
    //  //                  | |
    //  //                  | |
    //  //                  | |
    //  //                  | |
    //  //                  | |
    //  //                  | |
    //  //               13 | | 6                
    //  //                 /   \
    //  //   11 ----------/     \---------- 8
    //  //     |          12    7          | 
    //  //   10 --------------------------- 9
    //  //
    //  // (c) Kr1stj4n 2oo2
    //  //

    //  PolyCurve m_crv = null;
    //  if (beamSection != null)
    //  {
    //    LengthUnit unit = LengthUnit;
    //    m_crv = new PolyCurve();


    //    // top flange outside
    //    Point3d pt1 = new Point3d(
    //      beamSection.TopFlangeWidth.As(unit) / 2 * -1, // X
    //      0, // Y
    //      0);
    //    Point3d pt2 = new Point3d(beamSection.TopFlangeWidth.As(unit) / 2, //X
    //      0, // Y
    //      0);
    //    m_crv.Append(new Line(pt1, pt2));

    //    // top flange right edge
    //    Point3d pt3 = new Point3d(
    //      beamSection.TopFlangeWidth.As(unit) / 2, // X
    //      beamSection.TopFlangeThickness.As(unit) * -1, // Y 
    //      0);
    //    m_crv.Append(new Line(pt2, pt3));

    //    // top flange right inside
    //    Point3d pt4 = new Point3d(
    //        beamSection.WebThickness.As(unit) / 2 + beamSection.RootRadius.As(unit), // X
    //        beamSection.TopFlangeThickness.As(unit) * -1, // Y 
    //        0);
    //    m_crv.Append(new Line(pt3, pt4));

    //    Point3d pt8 = new Point3d(
    //      beamSection.BottomFlangeWidth.As(unit) / 2, // X
    //      beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit), // Y 
    //      0);

    //    if (beamSection.isCatalogue)
    //    {
    //      // top right root
    //      Point3d pt5 = new Point3d(
    //        beamSection.WebThickness.As(unit) / 2, // X
    //        beamSection.TopFlangeThickness.As(unit) * -1 - beamSection.RootRadius.As(unit), // Y 
    //        0);
    //      m_crv.Append(new Arc(pt4, new Vector3d(-1, 0, 0), pt5));

    //      // web right side
    //      Point3d pt6 = new Point3d(
    //        beamSection.WebThickness.As(unit) / 2, // X
    //        beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit) + beamSection.RootRadius.As(unit), // Y 
    //        0);
    //      m_crv.Append(new Line(pt5, pt6));

    //      // bottom right root
    //      Point3d pt7 = new Point3d(
    //        beamSection.WebThickness.As(unit) / 2 + beamSection.RootRadius.As(unit), // X
    //        beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit), // Y 
    //        0);
    //      m_crv.Append(new Arc(pt6, new Vector3d(0, -1, 0), pt7));

    //      // bottom flange right inside
    //      m_crv.Append(new Line(pt7, pt8));
    //    }
    //    else
    //    {
    //      // web right side
    //      Point3d pt6 = new Point3d(
    //        beamSection.WebThickness.As(unit) / 2, // X
    //        beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit), // Y 
    //        0);
    //      m_crv.Append(new Line(pt4, pt6));

    //      // bottom flange right inside
    //      m_crv.Append(new Line(pt6, pt8));
    //    }

    //    // bottom flange right edge
    //    Point3d pt9 = new Point3d(
    //     beamSection.BottomFlangeWidth.As(unit) / 2, // X
    //     beamSection.Depth.As(unit) * -1, // Y 
    //     0);
    //    m_crv.Append(new Line(pt8, pt9));

    //    // bottom flange outside
    //    Point3d pt10 = new Point3d(
    //     beamSection.BottomFlangeWidth.As(unit) / 2 * -1, // X
    //     beamSection.Depth.As(unit) * -1, // Y
    //     0);
    //    m_crv.Append(new Line(pt9, pt10));

    //    // bottom flange left edge
    //    Point3d pt11 = new Point3d(
    //      beamSection.BottomFlangeWidth.As(unit) / 2 * -1, // X
    //      beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit), // Y 
    //      0);
    //    m_crv.Append(new Line(pt10, pt11));

    //    // bottom flange left inside
    //    Point3d pt12 = new Point3d(
    //      beamSection.WebThickness.As(unit) / 2 * -1 - beamSection.RootRadius.As(unit), // X
    //      beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit), // Y 
    //      0);
    //    m_crv.Append(new Line(pt11, pt12));

    //    Point3d pt16 = new Point3d(
    //      beamSection.TopFlangeWidth.As(unit) / 2 * -1, // X
    //      beamSection.TopFlangeThickness.As(unit) * -1, // Y 
    //      0);

    //    if (beamSection.isCatalogue)
    //    {
    //      // bottom left root
    //      Point3d pt13 = new Point3d(
    //      beamSection.WebThickness.As(unit) / 2 * -1, // X
    //      beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit) + beamSection.RootRadius.As(unit), // Y 
    //      0);
    //      m_crv.Append(new Arc(pt12, new Vector3d(1, 0, 0), pt13));

    //      // web left side
    //      Point3d pt14 = new Point3d(
    //        beamSection.WebThickness.As(unit) / 2 * -1, // X
    //        beamSection.TopFlangeThickness.As(unit) * -1 - beamSection.RootRadius.As(unit), // Y 
    //        0);
    //      m_crv.Append(new Line(pt13, pt14));

    //      // top left root
    //      Point3d pt15 = new Point3d(
    //        beamSection.WebThickness.As(unit) / 2 * -1 - beamSection.RootRadius.As(unit), // X
    //        beamSection.TopFlangeThickness.As(unit) * -1, // Y 
    //        0);
    //      m_crv.Append(new Arc(pt14, new Vector3d(0, 1, 0), pt15));

    //      // top flange left inside
    //      m_crv.Append(new Line(pt15, pt16));
    //    }
    //    else
    //    {
    //      // web left side
    //      Point3d pt14 = new Point3d(
    //        beamSection.WebThickness.As(unit) / 2 * -1, // X
    //        beamSection.TopFlangeThickness.As(unit) * -1, // Y 
    //        0);
    //      m_crv.Append(new Line(pt12, pt14));

    //      // top flange left inside
    //      m_crv.Append(new Line(pt14, pt16));
    //    }

    //    // top flange left edge
    //    m_crv.Append(new Line(pt16, pt1));
    //  }
    //  return m_crv;
    //}
    #endregion

    #region drawing methods
    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      ////Draw shape.
      //if (profileExtrusions != null)
      //{
      //  if (args.Material.Diffuse == Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
      //  {
      //    foreach (Brep brep in profileExtrusions)
      //      args.Pipeline.DrawBrepShaded(brep, UI.Colour.Steel); //UI.Colour.Member2dFace
      //  }
      //  else
      //  {
      //    foreach (Brep brep in profileExtrusions)
      //      args.Pipeline.DrawBrepShaded(brep, UI.Colour.Steel);
      //  }
      //}
    }
    Color defaultCol = Instances.Settings.GetValue("DefaultPreviewColour", Color.White);
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      //Draw lines
      if (Line != null)
      {
        //if (args.Color.R == defaultCol.R && args.Color.G == defaultCol.G && args.Color.B == defaultCol.B) // not selected
        //{
        //  // line
        //  args.Pipeline.DrawCurve(Line, UI.Colour.OasysBlue, 3);

        //  // profiles
        //  foreach (PolyCurve crv in profileOutlines)
        //    args.Pipeline.DrawCurve(crv, UI.Colour.OasysBlue, 2);


        //}
        //else // selected
        //{
        //  // line
        //  args.Pipeline.DrawCurve(Line, UI.Colour.ArupRed, 4);

        //  // profiles
        //  foreach (PolyCurve crv in profileOutlines)
        //    args.Pipeline.DrawCurve(crv, UI.Colour.ArupRed, 3);
        //}
        //// extrusion wirefram
        //foreach (Brep brep in profileExtrusions)
        //  args.Pipeline.DrawBrepWires(brep, UI.Colour.OasysDarkGrey, -1);
      }
      #endregion
    }
  }
}
