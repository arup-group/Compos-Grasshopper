using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;
using Rhino.Collections;
using Rhino.Geometry.Collections;
using System.Collections.Concurrent;

namespace ComposGH.Helpers
{
    /// <summary>
    /// Tolerance class
    /// </summary>
    public class Tolerance
    {
        /// <summary>
        /// Method to retrieve active document Rhino units
        /// </summary>
        /// <returns></returns>
        public static double RhinoDocTolerance()
        {
            try
            {
                double tolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
                return tolerance;
            }
            catch (global::System.Exception)
            {
                return 0.001;
            }
        }
    }
    /// <summary>
    /// Helper class to perform some decent geometry approximations from NURBS to poly-geometry
    /// </summary>
    public class Convert
    {
        public static Plane CreateBestFitUnitisedPlaneFromPts(List<Point3d> ctrl_pts, bool round = false)
        {
            Plane pln = Plane.WorldXY;

            // calculate best fit plane:
            Plane.FitPlaneToPoints(ctrl_pts, out pln);

            // change origin to closest point world xyz
            // this will ensure that axes created in same plane will not be duplicated
            pln.Origin = pln.ClosestPoint(new Point3d(0, 0, 0));

            // find significant digits for rounding
            int dig = ComposGH.Parameters.Units.SignificantDigits;

            // unitise the plane normal so we can evaluate if it is XY-type plane
            pln.Normal.Unitize();
            if (Math.Abs(Math.Round(pln.Normal.Z, dig)) == 1) // if normal's z direction is close to vertical
            {
                // set X and Y axis to unit vectors to ensure no funny rotations
                pln.XAxis = Vector3d.XAxis;
                pln.YAxis = Vector3d.YAxis;
            }

            if (round)
            {
                // round origin coordinates
                pln.OriginX = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(pln.OriginX, dig);
                pln.OriginY = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(pln.OriginY, dig);
                pln.OriginZ = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(pln.OriginZ, dig);

                // unitize and round x-axis
                pln.XAxis.Unitize();
                Vector3d xaxis = pln.XAxis;
                xaxis.X = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.X), dig);
                xaxis.Y = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.Y), dig);
                xaxis.Z = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.Z), dig);
                pln.XAxis = xaxis;

                // unitize and round y-axis
                pln.YAxis.Unitize();
                Vector3d yaxis = pln.YAxis;
                yaxis.X = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.X), dig);
                yaxis.Y = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.Y), dig);
                yaxis.Z = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.Z), dig);
                pln.YAxis = yaxis;

                // unitize and round z-axis
                pln.ZAxis.Unitize();
                Vector3d zaxis = pln.ZAxis;
                zaxis.X = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.X), dig);
                zaxis.Y = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.Y), dig);
                zaxis.Z = ComposGH.Helpers.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.Z), dig);
                pln.ZAxis = zaxis;
            }
            else
            {
                // round origin coordinates
                pln.OriginX = Math.Round(pln.OriginX, dig);
                pln.OriginY = Math.Round(pln.OriginY, dig);
                pln.OriginZ = Math.Round(pln.OriginZ, dig);

                // unitize and round x-axis
                pln.XAxis.Unitize();
                Vector3d xaxis = pln.XAxis;
                xaxis.X = Math.Round(Math.Abs(xaxis.X), dig);
                xaxis.Y = Math.Round(Math.Abs(xaxis.Y), dig);
                xaxis.Z = Math.Round(Math.Abs(xaxis.Z), dig);
                pln.XAxis = xaxis;

                // unitize and round y-axis
                pln.YAxis.Unitize();
                Vector3d yaxis = pln.YAxis;
                yaxis.X = Math.Round(Math.Abs(yaxis.X), dig);
                yaxis.Y = Math.Round(Math.Abs(yaxis.Y), dig);
                yaxis.Z = Math.Round(Math.Abs(yaxis.Z), dig);
                pln.YAxis = yaxis;

                // unitize and round z-axis
                pln.ZAxis.Unitize();
                Vector3d zaxis = pln.ZAxis;
                zaxis.X = Math.Round(Math.Abs(zaxis.X), dig);
                zaxis.Y = Math.Round(Math.Abs(zaxis.Y), dig);
                zaxis.Z = Math.Round(Math.Abs(zaxis.Z), dig);
                pln.ZAxis = zaxis;
            }

            return pln;
        }
        
        
        /// <summary>
        /// Method to convert a NURBS curve into a PolyCurve made of lines and arcs.
        /// Automatically uses Rhino document tolerance if tolerance is not inputted
        /// </summary>
        /// <param name="crv"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        
        public static Tuple<PolyCurve, List<Point3d>, List<string>> ConvertMem1dCrv(Curve crv, double tolerance = -1)
        {
            PolyCurve m_crv = null;
            List<string> crv_type = new List<string>();
            List<Point3d> m_topo = new List<Point3d>();

            // arc curve
            if (crv.IsArc())
            {
                crv_type.Add("");
                crv_type.Add("A");
                crv_type.Add("");

                m_topo.Add(crv.PointAtStart);
                m_topo.Add(crv.PointAtNormalizedLength(0.5));
                m_topo.Add(crv.PointAtEnd);

                m_crv = new PolyCurve();
                m_crv.Append(crv);
            }
            else
            {
                if (crv.SpanCount > 1) // polyline (or assumed polyline, we will take controlpoints)
                {
                    m_crv = new PolyCurve();

                    if (tolerance < 0)
                        tolerance = ComposGH.Parameters.Units.Tolerance;

                    crv = crv.ToPolyline(tolerance, 2, 0, 0);
                    if (!crv.IsValid)
                        throw new Exception(" Error converting edge or curve to polyline: please verify input geometry is valid and tolerance is set accordingly with your geometry under GSA Plugin Unit Settings or if unset under Rhino unit settings");

                    Curve[] segments = crv.DuplicateSegments();

                    for (int i = 0; i < segments.Length; i++)
                    {
                        crv_type.Add("");
                        m_topo.Add(segments[i].PointAtStart);

                        m_crv.Append(segments[i]);
                    }
                    crv_type.Add("");
                    m_topo.Add(segments[segments.Length - 1].PointAtEnd);
                }
                else // single line segment
                {
                    crv_type.Add("");
                    crv_type.Add("");

                    m_topo.Add(crv.PointAtStart);
                    m_topo.Add(crv.PointAtEnd);

                    m_crv = new PolyCurve();
                    m_crv.Append(crv);
                }
            }

            return new Tuple<PolyCurve, List<Point3d>, List<string>>(m_crv, m_topo, crv_type);
        }

        public static Tuple<PolyCurve, List<Point3d>, List<string>> ConvertMem2dCrv(Curve crv, double tolerance = -1)
        {
            if (tolerance < 0)
                tolerance = ComposGH.Parameters.Units.Tolerance;

            PolyCurve m_crv = crv.ToArcsAndLines(tolerance, 2, 0, 0);
            Curve[] segments;
            if (m_crv != null)
                segments = m_crv.DuplicateSegments();
            else
                segments = new Curve[] { crv };

            if (segments.Length == 1)
            {
                if (segments[0].IsClosed)
                {
                    segments = segments[0].Split(0.5);
                }
            }

            List<string> crv_type = new List<string>();
            List<Point3d> m_topo = new List<Point3d>();

            for (int i = 0; i < segments.Length; i++)
            {
                m_topo.Add(segments[i].PointAtStart);
                crv_type.Add("");
                if (segments[i].IsArc())
                {
                    m_topo.Add(segments[i].PointAtNormalizedLength(0.5));
                    crv_type.Add("A");
                }
            }
            m_topo.Add(segments[segments.Length - 1].PointAtEnd);
            crv_type.Add("");

            return new Tuple<PolyCurve, List<Point3d>, List<string>>(m_crv, m_topo, crv_type);
        }

        public static Brep ConvertBrep(Brep brep)
        {
            return new Brep();
        }
        
        public static Tuple<PolyCurve, List<Point3d>, List<string>> _notUsedConvertMem2dCrv(Curve crv, double tolerance = -1)
        {
            PolyCurve m_crv = null;
            List<string> crv_type = new List<string>();
            List<Point3d> m_topo = new List<Point3d>();

            if (crv.Degree > 1)
            {
                if (!crv.IsArc() | crv.IsClosed)
                {
                    if (tolerance < 0)
                        tolerance = ComposGH.Parameters.Units.Tolerance;

                    m_crv = crv.ToArcsAndLines(tolerance, 2, 0, 0);
                    Curve[] segments;
                    if (m_crv != null)
                        segments = m_crv.DuplicateSegments();
                    else
                        segments = new Curve[] { crv };

                    for (int i = 0; i < segments.Length; i++)
                    {
                        m_topo.Add(segments[i].PointAtStart);
                        crv_type.Add("");
                        if (segments[i].IsArc())
                        {
                            m_topo.Add(segments[i].PointAtNormalizedLength(0.5));
                            crv_type.Add("A");
                        }
                    }
                    m_topo.Add(segments[segments.Length - 1].PointAtEnd);
                    crv_type.Add("");
                }
                else
                {
                    crv_type.Add("");
                    crv_type.Add("A");
                    crv_type.Add("");

                    m_topo.Add(crv.PointAtStart);
                    m_topo.Add(crv.PointAtNormalizedLength(0.5));
                    m_topo.Add(crv.PointAtEnd);

                    m_crv = new PolyCurve();
                    m_crv.Append(crv);
                }
            }
            else if (crv.Degree == 1)
            {
                if (crv.SpanCount > 1)
                {
                    m_crv = new PolyCurve();
                    Curve[] segments = crv.DuplicateSegments();
                    for (int i = 0; i < segments.Length; i++)
                    {
                        crv_type.Add("");
                        m_topo.Add(segments[i].PointAtStart);

                        m_crv.Append(segments[i]);
                    }
                    crv_type.Add("");
                    m_topo.Add(segments[segments.Length - 1].PointAtEnd);
                }
                else
                {
                    crv_type.Add("");
                    crv_type.Add("");

                    m_topo.Add(crv.PointAtStart);
                    m_topo.Add(crv.PointAtEnd);

                    m_crv = new PolyCurve();
                    m_crv.Append(crv);
                }
            }

            return new Tuple<PolyCurve, List<Point3d>, List<string>>(m_crv, m_topo, crv_type);
        }

        /// <summary>
        /// Method to convert a NURBS Brep into a planar trimmed surface with PolyCurve
        /// internal and external edges of lines and arcs
        /// 
        /// BRep conversion to planar routine first converts the external edge to a PolyCurve
        /// of lines and arcs and uses these controlpoints to fit a plane through points.
        /// 
        /// Will output a Tuple containing:
        /// - PolyCurve
        /// - TopologyList of control points
        /// - TopoTypeList (" " or "a") corrosponding to control points
        /// - List of PolyCurves for internal (void) curves
        /// - Corrosponding list of topology points
        /// - Corrosponding list of topologytypes
        /// </summary>
        /// <param name="brep"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static Tuple<PolyCurve, List<Point3d>, List<string>, List<PolyCurve>, List<List<Point3d>>, List<List<string>>>
            ConvertPolyBrep(Brep brep, double tolerance = -1)
        {
            List<PolyCurve> void_crvs = new List<PolyCurve>();
            List<List<Point3d>> void_topo = new List<List<Point3d>>();
            List<List<string>> void_topoType = new List<List<string>>();

            Curve outer = null;
            List<Curve> inner = new List<Curve>();
            for (int i = 0; i < brep.Loops.Count; i++)
            {
                if (brep.Loops[i].LoopType == BrepLoopType.Outer)
                {
                    outer = brep.Loops[i].To3dCurve();
                }
                else
                {
                    inner.Add(brep.Loops[i].To3dCurve());
                }
            }
            List<Curve> edges = new List<Curve>();
            edges.Add(outer);
            edges.AddRange(inner);

            for (int i = 0; i < edges.Count; i++)
            {
                if (!edges[i].IsPlanar())
                {
                    List<Point3d> ctrl_pts;
                    if (edges[0].TryGetPolyline(out Polyline temp_crv))
                        ctrl_pts = temp_crv.ToList();
                    else
                    {
                        Tuple<PolyCurve, List<Point3d>, List<string>> convertBadSrf = ComposGH.Helpers.Convert.ConvertMem2dCrv(edges[0], tolerance);
                        ctrl_pts = convertBadSrf.Item2;
                    }
                    Plane.FitPlaneToPoints(ctrl_pts, out Plane plane);
                    for (int j = 0; j < edges.Count; j++)
                        edges[j] = Curve.ProjectToPlane(edges[j], plane);
                }
            }

            Tuple<PolyCurve, List<Point3d>, List<string>> convert = ComposGH.Helpers.Convert.ConvertMem2dCrv(edges[0], tolerance);
            PolyCurve edge_crv = convert.Item1;
            List<Point3d>  m_topo = convert.Item2;
            List<string> m_topoType = convert.Item3;

            for (int i = 1; i < edges.Count; i++)
            {
                convert = ComposGH.Helpers.Convert.ConvertMem2dCrv(edges[i], tolerance);
                void_crvs.Add(convert.Item1);
                void_topo.Add(convert.Item2);
                void_topoType.Add(convert.Item3);
            }

            return new Tuple<PolyCurve, List<Point3d>, List<string>, List<PolyCurve>, List<List<Point3d>>, List<List<string>>>
                (edge_crv, m_topo, m_topoType, void_crvs, void_topo, void_topoType);
        }

        /// <summary>
        /// Method to convert a NURBS Brep into a planar trimmed surface with PolyCurve
        /// internal and external edges of lines and arcs. 
        /// 
        /// BRep conversion to planar routine first converts the external edge to a PolyCurve
        /// of lines and arcs and uses these controlpoints to fit a plane through points.
        /// 
        /// Input list of curves and list of points to be included in 2D Member;
        /// lines and curves will automatically be projected onto planar Brep plane
        /// 
        /// Will output 3 Tuples:
        /// (edgeTuple, voidTuple, inclTuple)
        /// 
        /// edgeTuple:
        /// (edge_crv, m_topo, m_topoType)
        /// - PolyCurve
        /// - TopologyList of control points
        /// - TopoTypeList (" " or "a") corrosponding to control points
        /// 
        /// voidTuple:
        /// (void_crvs, void_topo, void_topoType)
        /// - List of PolyCurves for internal (void) curves
        /// - Corrosponding list of topology points
        /// - Corrosponding list of topologytypes
        /// 
        /// inclTuple:
        /// (incl_crvs, incl_topo, incl_topoType, inclPts)
        /// - List of PolyCurves for internal (void) curves
        /// - Corrosponding list of topology points
        /// - Corrosponding list of topologytypes
        /// - List of inclusion points
        /// 
        /// </summary>
        /// <param name="brep"></param>
        /// <param name="inclCrvs"></param>
        /// <param name="inclPts"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static Tuple<Tuple<PolyCurve, List<Point3d>, List<string>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>>
            ConvertPolyBrepInclusion(Brep brep, List<Curve> inclCrvs = null, List<Point3d> inclPts = null, double tolerance = -1)
        {
            List<PolyCurve> void_crvs = new List<PolyCurve>();
            List<List<Point3d>> void_topo = new List<List<Point3d>>();
            List<List<string>> void_topoType = new List<List<string>>();

            List<PolyCurve> incl_crvs = new List<PolyCurve>();
            List<List<Point3d>> incl_topo = new List<List<Point3d>>();
            List<List<string>> incl_topoType = new List<List<string>>();

            Curve outer = null;
            List<Curve> inner = new List<Curve>();
            for (int i = 0; i < brep.Loops.Count; i++)
            {
                if (brep.Loops[i].LoopType == BrepLoopType.Outer)
                {
                    outer = brep.Loops[i].To3dCurve();
                }
                else
                {
                    inner.Add(brep.Loops[i].To3dCurve());
                }
            }
            List<Curve> edges = new List<Curve>();
            edges.Add(outer);
            edges.AddRange(inner);

            List<Point3d> ctrl_pts;
            if (edges[0].TryGetPolyline(out Polyline temp_crv))
                ctrl_pts = temp_crv.ToList();
            else
            {
                Tuple<PolyCurve, List<Point3d>, List<string>> convertBadSrf = ComposGH.Helpers.Convert.ConvertMem2dCrv(edges[0], tolerance);
                ctrl_pts = convertBadSrf.Item2;
            }
            Plane.FitPlaneToPoints(ctrl_pts, out Plane plane);

            for (int i = 0; i < edges.Count; i++)
            {
                if (!edges[i].IsPlanar())
                {
                    for (int j = 0; j < edges.Count; j++)
                        edges[j] = Curve.ProjectToPlane(edges[j], plane);
                }
            }
            Tuple<PolyCurve, List<Point3d>, List<string>> convert = ComposGH.Helpers.Convert.ConvertMem2dCrv(edges[0], tolerance);
            PolyCurve edge_crv = convert.Item1;
            List<Point3d> m_topo = convert.Item2;
            List<string> m_topoType = convert.Item3;

            for (int i = 1; i < edges.Count; i++)
            {
                convert = ComposGH.Helpers.Convert.ConvertMem2dCrv(edges[i], tolerance);
                void_crvs.Add(convert.Item1);
                void_topo.Add(convert.Item2);
                void_topoType.Add(convert.Item3);
            }

            if (inclCrvs != null)
            {
                for (int i = 0; i < inclCrvs.Count; i++)
                {
                    if (!inclCrvs[i].IsInPlane(plane))
                        inclCrvs[i] = Curve.ProjectToPlane(inclCrvs[i], plane);
                    convert = ComposGH.Helpers.Convert.ConvertMem2dCrv(inclCrvs[i], tolerance);
                    incl_crvs.Add(convert.Item1);
                    incl_topo.Add(convert.Item2);
                    incl_topoType.Add(convert.Item3);
                }
            }

            if (inclPts != null)
            {
                for (int i = 0; i < inclPts.Count; i++)
                    inclPts[i] = plane.ClosestPoint(inclPts[i]);
            }

            Tuple<PolyCurve, List<Point3d>, List<string>> edgeTuple = new Tuple<PolyCurve, List<Point3d>, List<string>>(edge_crv, m_topo, m_topoType);
            Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>> voidTuple = new Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>(void_crvs, void_topo, void_topoType);
            Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>> inclTuple = new Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>(incl_crvs, incl_topo, incl_topoType, inclPts);

            return new Tuple<Tuple<PolyCurve, List<Point3d>, List<string>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>>
                (edgeTuple, voidTuple, inclTuple);
        }

    }
}
