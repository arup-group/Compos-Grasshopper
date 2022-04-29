using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using Grasshopper.Documentation;
using Rhino.Collections;
using UnitsNet;
using ComposGH.Converters;
using ComposGH.Helpers;
using UnitsNet.Units;
using System.Drawing;
using Grasshopper;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class ComposBeam
  {
    public LineCurve Line
    {
      get { return (LineCurve)m_line.DuplicateShallow(); }
      set
      {
        m_line = value;
        UpdatePreview();
      }
    }

    LengthUnit LengthUnit { get; set; }

    private LineCurve m_line;
    public Length Length
    {
      get
      {
        if (this.m_line == null)
          throw new Exception("Line has not been set");

        return new Length(m_line.GetLength(), LengthUnit);
      }
    }
    public ComposRestraint Restraint { get; set; }
    public ComposSteelMaterial Material { get; set; }
    public List<BeamSection> BeamSections { get; internal set; } = new List<BeamSection>();
    public List<ComposWebOpening> WebOpenings { get; internal set; } = null;

    #region constructors
    public ComposBeam()
    {
        public Length Length { get; set; }
        public ComposSteelMaterial Material { get; set; }
        public ComposRestraint Restraint { get; set; }
        public List<BeamSection> BeamSections { get; set; }
        public List<ComposWebOpening> WebOpenings { get; set; }

    public ComposBeam(LineCurve line, LengthUnit lengthUnit, ComposRestraint restraint, ComposSteelMaterial material, List<BeamSection> beamSections, List<ComposWebOpening> webOpenings = null)
    {
      this.m_line = line;
      this.LengthUnit = lengthUnit;
      this.Restraint = restraint;
      this.Material = material;
      this.BeamSections = beamSections.ToList();
      if (webOpenings != null)
      {
        this.WebOpenings = webOpenings.ToList();
        this.WebOpenings = this.WebOpenings.OrderByDescending(x => x.CentroidPosFromStart.As(LengthUnit)).Reverse().ToList();
      }



      this.UpdatePreview();
    }

        // add public constructors here

        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                return true;
            }
        }
        #endregion

    public ComposBeam Duplicate()
    {
      if (this == null) { return null; }
      ComposBeam dup = (ComposBeam)this.MemberwiseClone();
      dup.Material = this.Material.Duplicate();
      dup.Restraint = this.Restraint.Duplicate();
      dup.BeamSections = this.BeamSections.ToList();
      if (this.WebOpenings != null)
        dup.WebOpenings = this.WebOpenings.ToList();
      dup.Line = this.Line; // Get the public member will shallow copy the object
      return dup;
    }

    public override string ToString()
    {
      string profile = (this.BeamSections.Count > 1) ? " multiple sections" : this.BeamSections[0].SectionDescription;
      string mat = this.Material.ToString();
      string line = "L:" + this.Length.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      return line + ", " + profile + ", " + mat;
    }
    #endregion

    #region preview geometry
    internal List<PolyCurve> profileOutlines;
    internal List<Curve> profileExtrusionEdges;
    internal List<Brep> profileExtrusions;
    internal List<Brep> stiffenerPlates;
    internal void UpdatePreview()
    {
      profileOutlines = new List<PolyCurve>();
      profileExtrusions = new List<Brep>();
      List<BeamSection> beamSectionsSorted = SortBeamSections(this.BeamSections.ToList());

      for (int i = 0; i < beamSectionsSorted.Count; i++)
      {
        BeamSection beamSection = beamSectionsSorted[i];
        PolyCurve outline = CreateLocalPlaneOutline(beamSection);
        profileOutlines.Add(outline);
        if (i == 0 | i == beamSectionsSorted.Count - 1)
          profileExtrusions.Add(Brep.CreatePlanarBreps(outline, 0.0001)[0]);


        if (i != beamSectionsSorted.Count - 1) // don't extrude last
        {
          if (beamSection.TaperedToNext)
          {
            BeamSection beamSectionNext = new BeamSection();
            if (beamSectionsSorted[i + 1].m_isCatalogue)
            {
              beamSectionNext =
                new BeamSection(
                  beamSectionsSorted[i + 1].Depth,
                  beamSectionsSorted[i + 1].TopFlangeWidth,
                  beamSectionsSorted[i + 1].WebThickness,
                  beamSectionsSorted[i + 1].TopFlangeThickness);
              beamSectionNext.StartPosition = beamSectionsSorted[i + 1].StartPosition;
            }
            else
              beamSectionNext = beamSectionsSorted[i + 1];
            PolyCurve nextOutline = CreateLocalPlaneOutline(beamSectionNext);
            ExtrudeBetweenOutlines(outline, nextOutline);
          }
          else
          {
            BeamSection beamSectionNext = beamSection.Duplicate();
            beamSectionNext.StartPosition = beamSectionsSorted[i + 1].StartPosition;
            PolyCurve nextOutline = CreateLocalPlaneOutline(beamSectionNext);
            profileOutlines.Add(nextOutline);
            ExtrudeBetweenOutlines(outline, nextOutline);
            profileExtrusions.Add(Brep.CreatePlanarBreps(nextOutline, 0.0001)[0]);
            beamSectionNext = beamSectionsSorted[i + 1];
            PolyCurve nextBeamOutline = CreateLocalPlaneOutline(beamSectionNext);
            profileExtrusions.Add(Brep.CreatePlanarBreps(nextBeamOutline, 0.0001)[0]);
          }
        }
      }

      profileExtrusions = Brep.JoinBreps(profileExtrusions, Units.Tolerance.As(LengthUnit)).ToList();
      for (int i = 0; i < profileExtrusions.Count; i++)
      {
        profileExtrusions[i].Compact();
        if (!profileExtrusions[i].IsSolid)
          profileExtrusions[i] = profileExtrusions[i].CapPlanarHoles(Units.Tolerance.As(LengthUnit));
      }
      profileExtrusions = Brep.CreateBooleanUnion(profileExtrusions, Units.Tolerance.As(LengthUnit)).ToList();

      if (this.WebOpenings != null)
      {
        Length maxWebThickness = new Length(this.BeamSections.Max(x => x.WebThickness.As(LengthUnit)), LengthUnit);
        foreach (ComposWebOpening webOpening in this.WebOpenings)
        {
          Brep cutter = OpeningCutter(webOpening, maxWebThickness, beamSectionsSorted);
          bool found = false;
          for (int i = 0; i < profileExtrusions.Count; i++)
          {
            if (webOpening.WebOpeningType == ComposWebOpening.OpeningType.Rectangular |
                webOpening.WebOpeningType == ComposWebOpening.OpeningType.Circular)
            {
              Brep[] cuts = profileExtrusions[i].Split(cutter, Units.Tolerance.As(LengthUnit));
              if (cuts.Length > 0)
              {
                found = true;

                Brep trimmedProfile = cuts.First();
                Brep[] cutEdges = cutter.Split(trimmedProfile, Units.Tolerance.As(LengthUnit));
                Brep cutEdge = (cutEdges.Length == 3) ? cutEdges[0] : cutEdges.First();
                profileExtrusions[i] = Brep.JoinBreps(
                  new List<Brep>() { trimmedProfile, cutEdge },
                  Units.Tolerance.As(LengthUnit)).ToList()[0];
              }

            }
            else // notch
            {
              Brep[] trims = profileExtrusions[i].Trim(cutter, Units.Tolerance.As(LengthUnit));
              if (trims.Length > 0)
              {
                found = true;
                Brep trimmedProfile = trims.First();
                profileExtrusions[i] = trimmedProfile;
              }
            }
          }
          if (!found)
            throw new Exception("One or more Web opening does not intersect the beam's web.");
        }
      }
      profileExtrusions = Brep.CreateBooleanUnion(profileExtrusions, Units.Tolerance.As(LengthUnit)).ToList();

    }
    private Brep OpeningCutter(ComposWebOpening webOpening, Length webThickness, List<BeamSection> beamSectionsSorted)
    {
      List<Brep> parts = new List<Brep>();

      // find local plane on line
      Plane local = new Plane();

      double t = webOpening.CentroidPosFromStart.As(LengthUnit);
      if (webOpening.WebOpeningType == ComposWebOpening.OpeningType.End_notch)
        t = Length.As(LengthUnit);

      if (t > Length.As(LengthUnit))
        throw new Exception("Web Opening Start Position lies outside the Beam's domain");
      this.Line.PerpendicularFrameAt(t, out local);

      // rotate perpendicular plane to be parallel to beam
      local.Rotate(Math.PI / 2, local.YAxis);

      // transform outline to local plane
      Transform maptToLocal = Rhino.Geometry.Transform.PlaneToPlane(Plane.WorldXY, local);

      switch (webOpening.WebOpeningType)
      {
        case ComposWebOpening.OpeningType.Rectangular:
          PolyCurve rect = RectangularOpening(webOpening);
          rect.Transform(maptToLocal);
          // move curve away from web
          rect.Translate(new Vector3d(local.Normal * webThickness.As(LengthUnit)));
          // mirror to other side of web
          PolyCurve rect2 = (PolyCurve)rect.Duplicate();
          rect2.Translate(new Vector3d(local.Normal * webThickness.As(LengthUnit) * -2));
          // connect
          for (int i = 0; i < rect.SegmentCount; i++)
          {
            Curve startSegment = rect.SegmentCurve(i);
            Curve endSegment = rect2.SegmentCurve(i);
            parts.Add(Brep.CreateEdgeSurface(new List<Curve>() { startSegment, endSegment }));
          }

          break;

        case ComposWebOpening.OpeningType.Circular:
          ArcCurve circ = CircularOpening(webOpening);
          circ.Transform(maptToLocal);
          // move curve away from web
          circ.Translate(new Vector3d(local.Normal * webThickness.As(LengthUnit)));
          // mirror to other side of web
          ArcCurve circ2 = (ArcCurve)circ.Duplicate(); // Circle is a struct that copies on assign
          circ2.Translate(new Vector3d(local.Normal * webThickness.As(LengthUnit) * -2));
          // connect
          parts.Add(Brep.CreateFromLoft(new List<Curve>() { circ, circ2 }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]);

          break;

        case ComposWebOpening.OpeningType.Start_notch:
          BeamSection beamstart = beamSectionsSorted.First();
          PolyCurve start = StartNotch(webOpening, beamstart.Depth);
          start.Transform(maptToLocal);
          // move curve away from web
          start.Translate(new Vector3d(local.Normal * beamstart.BottomFlangeWidth.As(LengthUnit) / 2 * 1.01));
          // mirror to other side of web
          PolyCurve start2 = (PolyCurve)start.Duplicate();
          start2.Translate(new Vector3d(local.Normal * beamstart.BottomFlangeWidth.As(LengthUnit) / 2 * 1.01 * -2));
          // connect
          for (int i = 0; i < start.SegmentCount; i++)
          {
            Curve startSegment = start.SegmentCurve(i);
            Curve endSegment = start2.SegmentCurve(i);
            parts.Add(Brep.CreateEdgeSurface(new List<Curve>() { startSegment, endSegment }));
          }
          break;

        case ComposWebOpening.OpeningType.End_notch:
          BeamSection beamend = beamSectionsSorted.Last();
          PolyCurve end = EndNotch(webOpening, beamend.Depth);
          end.Transform(maptToLocal);
          // move curve away from web
          end.Translate(new Vector3d(local.Normal * beamend.BottomFlangeWidth.As(LengthUnit) / 2 * 1.01));
          // mirror to other side of web
          PolyCurve end2 = (PolyCurve)end.Duplicate();
          end2.Translate(new Vector3d(local.Normal * beamend.BottomFlangeWidth.As(LengthUnit) / 2 * 1.01 * -2));
          // connect
          for (int i = 0; i < end.SegmentCount; i++)
          {
            Curve startSegment = end.SegmentCurve(i);
            Curve endSegment = end2.SegmentCurve(i);
            parts.Add(Brep.CreateEdgeSurface(new List<Curve>() { startSegment, endSegment }));
          }
          break;
      }
      return Brep.JoinBreps(parts, Units.Tolerance.As(LengthUnit)).ToList()[0];
    }
    private PolyCurve RectangularOpening(ComposWebOpening webOpening)
    {
      // ## adding segments clockwise, assume xy plane with centre at profile top flange
      //
      //                 Origin
      //                   x
      //
      //    1 ---------------------------- 2
      //     |                            |
      //     |                            |
      //     |                            |
      //     |                            |
      //     |                            |
      //    4 ---------------------------- 3
      //
      // (c) Kr1stj4n 2oo2
      //

      PolyCurve m_crv = null;
      if (webOpening != null)
      {
        LengthUnit unit = LengthUnit;

        if (webOpening.WebOpeningType == ComposWebOpening.OpeningType.Rectangular)
        {
          m_crv = new PolyCurve();
          // top left
          Point3d rec1 = new Point3d(
            webOpening.Width.As(unit) / 2 * -1, // X
            webOpening.CentroidPosFromTop.As(unit) * -1 + webOpening.Height.As(unit) / 2, // Y
            0);
          // top right
          Point3d rec2 = new Point3d(
            webOpening.Width.As(unit) / 2, // X
            webOpening.CentroidPosFromTop.As(unit) * -1 + webOpening.Height.As(unit) / 2, // Y
            0);
          m_crv.Append(new Line(rec1, rec2));
          // bottom right
          Point3d rec3 = new Point3d(
            webOpening.Width.As(unit) / 2, // X
            webOpening.CentroidPosFromTop.As(unit) * -1 - webOpening.Height.As(unit) / 2, // Y
            0);
          m_crv.Append(new Line(rec2, rec3));
          Point3d rec4 = new Point3d(
            webOpening.Width.As(unit) / 2 * -1, // X
            webOpening.CentroidPosFromTop.As(unit) * -1 - webOpening.Height.As(unit) / 2, // Y
            0);
          m_crv.Append(new Line(rec3, rec4));
          m_crv.Append(new Line(rec4, rec1));
          return m_crv;
        }
      }
      return null;
    }
    private PolyCurve EndNotch(ComposWebOpening webOpening, Length beamDepth)
    {
      // ## adding segments clockwise, assume xy plane with centre at profile top flange 
      // Rhino maps x-axis inverse, so this is end even though logically it should be start
      //     Origin
      //     x
      //
      //    1 ---------------------------- 2
      //     |                            |
      //     |                            |
      //     |                            |
      //     |                            |
      //     |                            |
      //    4 ---------------------------- 3
      //
      // (c) Kr1stj4n 2oo2
      //

      PolyCurve m_crv = null;
      if (webOpening != null)
      {
        LengthUnit unit = LengthUnit;

        if (webOpening.WebOpeningType == ComposWebOpening.OpeningType.End_notch)
        {
          m_crv = new PolyCurve();
          // top left
          Point3d rec1 = new Point3d(
            -0.1, // X
            beamDepth.As(unit) * -1 + webOpening.Height.As(unit), // Y
            0);
          // top right
          Point3d rec2 = new Point3d(
            webOpening.Width.As(unit), // X
            beamDepth.As(unit) * -1 + webOpening.Height.As(unit), // Y
            0);
          m_crv.Append(new Line(rec1, rec2));
          // bottom right
          Point3d rec3 = new Point3d(
            webOpening.Width.As(unit), // X
            beamDepth.As(unit) * -1 - 0.1, // Y
            0);
          m_crv.Append(new Line(rec2, rec3));
          // bottom left
          Point3d rec4 = new Point3d(
            -0.1, // X
            beamDepth.As(unit) * -1 - 0.1, // Y
            0);
          m_crv.Append(new Line(rec3, rec4));
          m_crv.Append(new Line(rec4, rec1));
          return m_crv;
        }
      }
      return null;
    }
    private PolyCurve StartNotch(ComposWebOpening webOpening, Length beamDepth)
    {
      // ## adding segments clockwise, assume xy plane with centre at profile top flange
      // Rhino maps x-axis inverse, so this is start even though logically it should be end
      //                             Origin
      //                                  x
      //
      //    1 ---------------------------- 2
      //     |                            |
      //     |                            |
      //     |                            |
      //     |                            |
      //     |                            |
      //    4 ---------------------------- 3
      //
      // (c) Kr1stj4n 2oo2
      //

      PolyCurve m_crv = null;
      if (webOpening != null)
      {
        LengthUnit unit = LengthUnit;

        if (webOpening.WebOpeningType == ComposWebOpening.OpeningType.Start_notch)
        {
          m_crv = new PolyCurve();
          // top left
          Point3d rec1 = new Point3d(
            webOpening.Width.As(unit) * -1, // X
            beamDepth.As(unit) * -1 + webOpening.Height.As(unit), // Y
            0);
          // top right
          Point3d rec2 = new Point3d(
            0.1, // X
            beamDepth.As(unit) * -1 + webOpening.Height.As(unit), // Y
            0);
          m_crv.Append(new Line(rec1, rec2));
          // bottom right
          Point3d rec3 = new Point3d(
            0.1, // X
            beamDepth.As(unit) * -1 - 0.1, // Y
            0);
          m_crv.Append(new Line(rec2, rec3));
          Point3d rec4 = new Point3d(
            webOpening.Width.As(unit) * -1, // X
            beamDepth.As(unit) * -1 - 0.1, // Y
            0);
          m_crv.Append(new Line(rec3, rec4));
          m_crv.Append(new Line(rec4, rec1));
          return m_crv;
        }
      }
      return null;
    }
    private ArcCurve CircularOpening(ComposWebOpening webOpening)
    {
      Circle m_crv = new Circle();
      if (webOpening != null)
      {
        LengthUnit unit = LengthUnit;

        if (webOpening.WebOpeningType == ComposWebOpening.OpeningType.Circular)
        {
          Point3d CP = new Point3d(
            0, // X
            webOpening.CentroidPosFromTop.As(unit) * -1, // Y
            0);
          m_crv = new Circle(CP, webOpening.Diameter.As(unit) / 2);
        }
      }
      return new ArcCurve(m_crv);
    }
    private List<BeamSection> SortBeamSections(List<BeamSection> beamSectionsSorted)
    {
      beamSectionsSorted = beamSectionsSorted.OrderByDescending(x => x.StartPosition.As(LengthUnit)).Reverse().ToList();
      if (beamSectionsSorted.First().StartPosition != Length.Zero)
      {
        BeamSection newStart = beamSectionsSorted.First().Duplicate();
        newStart.StartPosition = Length.Zero;
        newStart.TaperedToNext = true;
        beamSectionsSorted.Insert(0, newStart);
      }
      if (beamSectionsSorted.Last().StartPosition != Length)
      {
        beamSectionsSorted.Last().TaperedToNext = true;
        BeamSection newEnd = beamSectionsSorted.Last().Duplicate();
        newEnd.StartPosition = Length;
        beamSectionsSorted.Add(newEnd);
      }
      return beamSectionsSorted;
    }

    private void ExtrudeBetweenOutlines(PolyCurve start, PolyCurve end)
    {
      for (int j = 0; j < start.SegmentCount; j++)
      {
        Curve startSegment = start.SegmentCurve(j);
        Curve endSegment = end.SegmentCurve(j);
        profileExtrusions.Add(Brep.CreateEdgeSurface(new List<Curve>() { startSegment, endSegment }));
      }
    }
    private PolyCurve CreateLocalPlaneOutline(BeamSection beamSection)
    {
      // construct outline
      PolyCurve outline = CreateProfileOutline(beamSection);

      // find local plane on line
      Plane local = new Plane();

      double t = beamSection.StartPosition.As(LengthUnit);
      if (beamSection.StartPosition == Length.Zero)
        t = 0;

      if (t > Length.As(LengthUnit))
        throw new Exception("Beam Section Start Position lies outside the Beam's domain");
      this.Line.PerpendicularFrameAt(t, out local);

      // transform outline to local plane
      Transform maptToLocal = Rhino.Geometry.Transform.PlaneToPlane(Plane.WorldXY, local);
      outline.Transform(maptToLocal);

      return outline;
    }

    private PolyCurve CreateProfileOutline(BeamSection beamSection)
    {
      // ## adding segments clockwise, assume xy plane with centre at middle of profile top flange
      //
      //                 Origin
      //    1 -------------x------------- 2
      //     |          15    4          |
      //   16 ----------\     /---------- 3
      //                 \   /
      //               14 | | 5
      //                  | |
      //                  | |
      //                  | |
      //                  | |
      //                  | |
      //                  | |
      //               13 | | 6                
      //                 /   \
      //   11 ----------/     \---------- 8
      //     |          12    7          | 
      //   10 --------------------------- 9
      //
      // (c) Kr1stj4n 2oo2
      //

      PolyCurve m_crv = null;
      if (beamSection != null)
      {
        LengthUnit unit = LengthUnit;
        m_crv = new PolyCurve();


        // top flange outside
        Point3d pt1 = new Point3d(
          beamSection.TopFlangeWidth.As(unit) / 2 * -1, // X
          0, // Y
          0);
        Point3d pt2 = new Point3d(beamSection.TopFlangeWidth.As(unit) / 2, //X
          0, // Y
          0);
        m_crv.Append(new Line(pt1, pt2));

        // top flange right edge
        Point3d pt3 = new Point3d(
          beamSection.TopFlangeWidth.As(unit) / 2, // X
          beamSection.TopFlangeThickness.As(unit) * -1, // Y 
          0);
        m_crv.Append(new Line(pt2, pt3));

        // top flange right inside
        Point3d pt4 = new Point3d(
            beamSection.WebThickness.As(unit) / 2 + beamSection.RootRadius.As(unit), // X
            beamSection.TopFlangeThickness.As(unit) * -1, // Y 
            0);
        m_crv.Append(new Line(pt3, pt4));

        Point3d pt8 = new Point3d(
          beamSection.BottomFlangeWidth.As(unit) / 2, // X
          beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit), // Y 
          0);

        if (beamSection.m_isCatalogue)
        {
          // top right root
          Point3d pt5 = new Point3d(
            beamSection.WebThickness.As(unit) / 2, // X
            beamSection.TopFlangeThickness.As(unit) * -1 - beamSection.RootRadius.As(unit), // Y 
            0);
          m_crv.Append(new Arc(pt4, new Vector3d(-1, 0, 0), pt5));

          // web right side
          Point3d pt6 = new Point3d(
            beamSection.WebThickness.As(unit) / 2, // X
            beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit) + beamSection.RootRadius.As(unit), // Y 
            0);
          m_crv.Append(new Line(pt5, pt6));

          // bottom right root
          Point3d pt7 = new Point3d(
            beamSection.WebThickness.As(unit) / 2 + beamSection.RootRadius.As(unit), // X
            beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit), // Y 
            0);
          m_crv.Append(new Arc(pt6, new Vector3d(0, -1, 0), pt7));

          // bottom flange right inside
          m_crv.Append(new Line(pt7, pt8));
        }
        else
        {
          // web right side
          Point3d pt6 = new Point3d(
            beamSection.WebThickness.As(unit) / 2, // X
            beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit), // Y 
            0);
          m_crv.Append(new Line(pt4, pt6));

          // bottom flange right inside
          m_crv.Append(new Line(pt6, pt8));
        }

        // bottom flange right edge
        Point3d pt9 = new Point3d(
         beamSection.BottomFlangeWidth.As(unit) / 2, // X
         beamSection.Depth.As(unit) * -1, // Y 
         0);
        m_crv.Append(new Line(pt8, pt9));

        // bottom flange outside
        Point3d pt10 = new Point3d(
         beamSection.BottomFlangeWidth.As(unit) / 2 * -1, // X
         beamSection.Depth.As(unit) * -1, // Y
         0);
        m_crv.Append(new Line(pt9, pt10));

        // bottom flange left edge
        Point3d pt11 = new Point3d(
          beamSection.BottomFlangeWidth.As(unit) / 2 * -1, // X
          beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit), // Y 
          0);
        m_crv.Append(new Line(pt10, pt11));

        // bottom flange left inside
        Point3d pt12 = new Point3d(
          beamSection.WebThickness.As(unit) / 2 * -1 - beamSection.RootRadius.As(unit), // X
          beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit), // Y 
          0);
        m_crv.Append(new Line(pt11, pt12));

        Point3d pt16 = new Point3d(
          beamSection.TopFlangeWidth.As(unit) / 2 * -1, // X
          beamSection.TopFlangeThickness.As(unit) * -1, // Y 
          0);

        if (beamSection.m_isCatalogue)
        {
          // bottom left root
          Point3d pt13 = new Point3d(
          beamSection.WebThickness.As(unit) / 2 * -1, // X
          beamSection.Depth.As(unit) * -1 + beamSection.BottomFlangeThickness.As(unit) + beamSection.RootRadius.As(unit), // Y 
          0);
          m_crv.Append(new Arc(pt12, new Vector3d(1, 0, 0), pt13));

          // web left side
          Point3d pt14 = new Point3d(
            beamSection.WebThickness.As(unit) / 2 * -1, // X
            beamSection.TopFlangeThickness.As(unit) * -1 - beamSection.RootRadius.As(unit), // Y 
            0);
          m_crv.Append(new Line(pt13, pt14));

          // top left root
          Point3d pt15 = new Point3d(
            beamSection.WebThickness.As(unit) / 2 * -1 - beamSection.RootRadius.As(unit), // X
            beamSection.TopFlangeThickness.As(unit) * -1, // Y 
            0);
          m_crv.Append(new Arc(pt14, new Vector3d(0, 1, 0), pt15));

          // top flange left inside
          m_crv.Append(new Line(pt15, pt16));
        }
        else
        {
          // web left side
          Point3d pt14 = new Point3d(
            beamSection.WebThickness.As(unit) / 2 * -1, // X
            beamSection.TopFlangeThickness.As(unit) * -1, // Y 
            0);
          m_crv.Append(new Line(pt12, pt14));

          // top flange left inside
          m_crv.Append(new Line(pt14, pt16));
        }

        // top flange left edge
        m_crv.Append(new Line(pt16, pt1));
      }
      return m_crv;
    }
    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class ComposBeamGoo : GH_GeometricGoo<ComposBeam>, IGH_PreviewData
  {
    #region constructors
    public ComposBeamGoo()
    {
      this.Value = new ComposBeam();
    }
    public ComposBeamGoo(ComposBeam item)
    {
      if (item == null)
        item = new ComposBeam();
      this.Value = item.Duplicate();
    }

    public override IGH_GeometricGoo DuplicateGeometry()
    {
      return new ComposBeamGoo(Value == null ? new ComposBeam() : Value.Duplicate());
    }
    #endregion

        #endregion
    }

    public override BoundingBox Boundingbox
    {
      get
      {
        if (Value == null) { return BoundingBox.Empty; }
        if (Value.Line == null) { return BoundingBox.Empty; }
        return Value.Line.GetBoundingBox(false);
      }
    }
    public override BoundingBox GetBoundingBox(Transform xform)
    {
      if (Value == null) { return BoundingBox.Empty; }
      if (Value.Line == null) { return BoundingBox.Empty; }
      return Value.Line.GetBoundingBox(xform);
    }
    #endregion

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class ComposBeamGoo : GH_Goo<ComposBeam> // needs to be upgraded to GeometryGoo eventually....
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(ComposBeam)))
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
          target = (Q)(object)Value.Line;
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Line)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Line ghLine = new GH_Line();
          GH_Convert.ToGHLine(Value.Line, GH_Conversion.Both, ref ghLine);
          target = (Q)(object)ghLine;
        }

        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(Curve)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Line;
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Curve(Value.Line);
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

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public ComposBeamGoo DuplicateGoo()
        {
            return new ComposBeamGoo(Value == null ? new ComposBeam() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Beam";
        public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
        public override string IsValidWhyNot
        {
            get
            {
                if (Value.IsValid) { return string.Empty; }
                return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null";
            else
                return "Compos " + TypeName + " {" + Value.ToString() + "}"; ;
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of our custom class into some other type Q.            

            if (typeof(Q).IsAssignableFrom(typeof(ComposBeam)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value;
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
            if (typeof(ComposBeam).IsAssignableFrom(source.GetType()))
            {
                Value = (ComposBeam)source;
                return true;
            }

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
      if (Value.Line == null) { return null; }

      ComposBeam elem = Value.Duplicate();
      LineCurve xLn = elem.Line;
      xLn.Transform(xform);
      elem.Line = xLn;
      elem.UpdatePreview();

      return new ComposBeamGoo(elem);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      if (Value == null) { return null; }
      if (Value.Line == null) { return null; }

      ComposBeam elem = Value.Duplicate();
      LineCurve xLn = Value.Line;
      xmorph.Morph(xLn);
      elem.Line = xLn;
      elem.UpdatePreview();

      return new ComposBeamGoo(elem);
    }

    #endregion

    #region drawing methods
    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //Draw shape.
      if (Value.profileExtrusions != null)
      {
        if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          foreach (Brep brep in Value.profileExtrusions)
            args.Pipeline.DrawBrepShaded(brep, UI.Colour.Steel); //UI.Colour.Member2dFace
        }
        else
        {
          foreach (Brep brep in Value.profileExtrusions)
            args.Pipeline.DrawBrepShaded(brep, UI.Colour.Steel);
        }
      }
    }
    Color defaultCol = Instances.Settings.GetValue("DefaultPreviewColour", Color.White);
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      //Draw lines
      if (Value.Line != null)
      {
        if (args.Color.R == defaultCol.R && args.Color.G == defaultCol.G && args.Color.B == defaultCol.B) // not selected
        {
          // line
          args.Pipeline.DrawCurve(Value.Line, UI.Colour.OasysBlue, 3);

          // profiles
          foreach (PolyCurve crv in Value.profileOutlines)
            args.Pipeline.DrawCurve(crv, UI.Colour.OasysBlue, 2);


        }
        else // selected
        {
          // line
          args.Pipeline.DrawCurve(Value.Line, UI.Colour.ArupRed, 4);

          // profiles
          foreach (PolyCurve crv in Value.profileOutlines)
            args.Pipeline.DrawCurve(crv, UI.Colour.ArupRed, 3);
        }
        // extrusion wirefram
        foreach (Brep brep in Value.profileExtrusions)
          args.Pipeline.DrawBrepWires(brep, UI.Colour.OasysDarkGrey, -1);
      }
      #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the CustomGoo type.
    /// </summary>
    public class ComposBeamParameter : GH_PersistentGeometryParam<ComposBeamGoo>, IGH_PreviewObject
    {
      public ComposBeamParameter()
        : base(new GH_InstanceDescription("Beam", "Bm", "Maintains a collection of Compos Beam data.", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat10()))
      {
      }

      public override Guid ComponentGuid => new Guid("dc61e94b-c326-4789-92f2-e0fe3caea4c7");

      public override GH_Exposure Exposure => GH_Exposure.primary;

      protected override System.Drawing.Bitmap Icon => Properties.Resources.BeamParam;

      //We do not allow users to pick parameter, 
      //therefore the following 4 methods disable all this ui.
      protected override GH_GetterResult Prompt_Plural(ref List<ComposBeamGoo> values)
      {
        return GH_GetterResult.cancel;
      }
      protected override GH_GetterResult Prompt_Singular(ref ComposBeamGoo value)
      {
        return GH_GetterResult.cancel;
      }
      protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
      {
        System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
        {
          Text = "Not available",
          Visible = false
        };
        return item;
      }
      protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
      {
        System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
        {
          Text = "Not available",
          Visible = false
        };
        return item;
      }

      #region preview methods
      public BoundingBox ClippingBox
      {
        get
        {
          return Preview_ComputeClippingBox();
        }
      }
      public void DrawViewportMeshes(IGH_PreviewArgs args)
      {
        //Use a standard method to draw gunk, you don't have to specifically implement this.
        Preview_DrawMeshes(args);
      }
      public void DrawViewportWires(IGH_PreviewArgs args)
      {
        //Use a standard method to draw gunk, you don't have to specifically implement this.
        Preview_DrawWires(args);
      }

      private bool m_hidden = false;
      public bool Hidden
      {
        get { return m_hidden; }
        set { m_hidden = value; }
      }
      public bool IsPreviewCapable
      {
        get { return true; }
      }
      #endregion
    }
  }
}
