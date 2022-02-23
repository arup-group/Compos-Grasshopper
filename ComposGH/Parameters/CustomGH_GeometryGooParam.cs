using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace ComposGH.Parameters
{
    public class ComposSection
    {
        #region fields
        private IMaterial m_material;
        private string m_materialGradeName;
        #endregion

        #region members
        public Length Width
        {
            get { return m_width; }
            set
            {
                m_profile.Width = value.Meters;
                m_width = value;
            }
        }
        public string TypeName
        {
            get { return m_type.ToString(); }
        }
        public string GradeName
        {
            get { return m_materialGradeName; }
            set { m_materialGradeName = value; }
        }
        public AdSecDesignCode DesignCode
        {
            get { return m_designCode; }
            set { m_designCode = value; }
        }
        public string DesignCodeName
        {
            get { if (m_designCode == null) { return null; } return m_designCode.DesignCodeName; }
        }
        #endregion


        #region constructors
        public ComposSection()
        {
        }


        public ComposSection Duplicate()
        {
            if (this == null) { return null; }
            ComposSection dup = (ComposSection)this.MemberwiseClone();
            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (this.Material == null) { return false; }
                return true;
            }
        }
        #endregion

        #region methods
        public override string ToString()
        {
            string grd = "Custom ";
            if (GradeName != null)
                grd = GradeName.Replace("  ", " ") + " ";

            string code = "";
            if (DesignCode != null)
            {
                if (DesignCode.DesignCodeName != null)
                    code = " to " + DesignCodeName.Replace("  ", " ");
            }

            return grd + TypeName.Replace("  ", " ") + code;
        }

        #endregion
    }
    public class ComposSectionGoo : GH_GeometricGoo<ComposSection>, IGH_PreviewData
    {
        #region constructors
        public ComposSectionGoo()
        {
            this.Value = new ComposSection();
        }
        public ComposSectionGoo(ComposSection goo)
        {
            if (goo == null)
                goo = new ComposSection();
            this.Value = goo.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public ComposSectionGoo DuplicateGoo()
        {
            return new ComposSectionGoo(Value == null ? new ComposSection() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return true;
            }
        }
        public override string IsValidWhyNot
        {
            get
            {
                if (Value.IsValid) { return string.Empty; }
                return Value.IsValid.ToString();
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null";
            else
                return "Compos " + TypeName + " {" + Value.ToString() + "}";
        }
        public override string TypeName => "Material";

        public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of this class into some other type Q.            

            if (typeof(Q).IsAssignableFrom(typeof(ComposSection)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Duplicate();
                return true;
            }

            target = default;
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into this parameter.


            if (source == null) { return false; }

            //Cast from own type
            if (typeof(ComposSection).IsAssignableFrom(source.GetType()))
            {
                Value = (ComposSection)source;
                return true;
            }

            return false;
        }
        #endregion

        #region transformation methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            //return new AdSecSectionGoo(Value.Transform(xform));
            return null;
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            //return new AdSecSectionGoo(Value.Morph(xmorph));
            return null;
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
            if (Value.SolidBrep != null)
            {
                // draw profile
                args.Pipeline.DrawBrepShaded(Value.SolidBrep, Value.m_profileColour);
                // draw subcomponents
                for (int i = 0; i < Value.m_subProfiles.Count; i++)
                {
                    args.Pipeline.DrawBrepShaded(Value.m_subProfiles[i], Value.m_subColours[i]);
                }
                // draw rebars
                for (int i = 0; i < Value.m_rebars.Count; i++)
                {
                    args.Pipeline.DrawBrepShaded(Value.m_rebars[i], Value.m_rebarColours[i]);
                }
            }
        }
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }

            if (UI.Colour.isSelected(args.Color))
            if (args.Color.R == defaultCol.R && args.Color.G == defaultCol.G && args.Color.B == defaultCol.B) // not selected
            {
                args.Pipeline.DrawPolyline(Value.m_profileEdge, UI.Colour.OasysBlue, 2);
                if (Value.m_profileVoidEdges != null)
                {
                    foreach (Polyline crv in Value.m_profileVoidEdges)
                    {
                        args.Pipeline.DrawPolyline(crv, UI.Colour.OasysBlue, 1);
                    }
                }
                if (Value.m_subEdges != null)
                {
                    foreach (Polyline crv in Value.m_subEdges)
                    {
                        args.Pipeline.DrawPolyline(crv, UI.Colour.OasysBlue, 1);
                    }
                }
                if (Value.m_subVoidEdges != null)
                {
                    foreach (List<Polyline> crvs in Value.m_subVoidEdges)
                    {
                        foreach (Polyline crv in crvs)
                        {
                            args.Pipeline.DrawPolyline(crv, UI.Colour.OasysBlue, 1);
                        }
                    }
                }
                if (Value.m_rebarEdges != null)
                {
                    foreach (Circle crv in Value.m_rebarEdges)
                    {
                        args.Pipeline.DrawCircle(crv, Color.Black, 1);
                    }
                }
                if (Value.m_linkEdges != null)
                {
                    foreach (Curve crv in Value.m_linkEdges)
                    {
                        args.Pipeline.DrawCurve(crv, Color.Black, 1);
                    }
                }
            }
            else // selected
            {
                args.Pipeline.DrawPolyline(Value.m_profileEdge, UI.Colour.OasysYellow, 3);
                if (Value.m_profileVoidEdges != null)
                {
                    foreach (Polyline crv in Value.m_profileVoidEdges)
                    {
                        args.Pipeline.DrawPolyline(crv, UI.Colour.OasysYellow, 2);
                    }
                }
                if (Value.m_subEdges != null)
                {
                    foreach (Polyline crv in Value.m_subEdges)
                    {
                        args.Pipeline.DrawPolyline(crv, UI.Colour.OasysYellow, 2);
                    }
                }
                if (Value.m_subVoidEdges != null)
                {
                    foreach (List<Polyline> crvs in Value.m_subVoidEdges)
                    {
                        foreach (Polyline crv in crvs)
                        {
                            args.Pipeline.DrawPolyline(crv, UI.Colour.OasysYellow, 2);
                        }
                    }
                }
                if (Value.m_rebarEdges != null)
                {
                    foreach (Circle crv in Value.m_rebarEdges)
                    {
                        args.Pipeline.DrawCircle(crv, UI.Colour.UILightGrey, 2);
                    }
                }
                if (Value.m_linkEdges != null)
                {
                    foreach (Curve crv in Value.m_linkEdges)
                    {
                        args.Pipeline.DrawCurve(crv, UI.Colour.UILightGrey, 2);
                    }
                }
            }
            // local axis
            if (Value.previewXaxis != null)
            {
                args.Pipeline.DrawLine(Value.previewZaxis, Color.FromArgb(255, 244, 96, 96), 1);
                args.Pipeline.DrawLine(Value.previewXaxis, Color.FromArgb(255, 96, 244, 96), 1);
                args.Pipeline.DrawLine(Value.previewYaxis, Color.FromArgb(255, 96, 96, 234), 1);
            }
        }
        #endregion

        /// <summary>
        /// This class provides a Parameter interface for the custom GH_GeometryGoo type.
        /// </summary>
        public class ComposSectionParameter : GH_PersistentGeometryParam<ComposSectionGoo>, IGH_PreviewObject
        {
            public ComposSectionParameter()
              : base(new GH_InstanceDescription("Material", "Mat", "Compos Material Parameter", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat9()))
            {
            }

            public override Guid ComponentGuid => new Guid("1b357078-43f4-4c72-9184-7296f191fdde");

            public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;

            protected override System.Drawing.Bitmap Icon => Properties.Resources.MaterialParam;

            protected override GH_GetterResult Prompt_Plural(ref List<ComposSectionGoo> values)
            {
                return GH_GetterResult.cancel;
            }
            protected override GH_GetterResult Prompt_Singular(ref ComposSectionGoo value)
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
