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

namespace ComposGH.Parameters
{
    /// <summary>
    /// Custom class: this class defines the basic properties and methods for our custom class
    /// </summary>
    public class ComposReinf
    {
        public RebarMesh RebarMesh { get; set; }
        public Rebar Rebar { get; set; }

        // Rebar Spacing
        public List<RebarGroupSpacing> CustomSpacing { get; set; } = null;


        #region constructors
        public ComposReinf()
        {
            // empty constructor
        }
        public ComposReinf(RebarMesh mesh, Rebar mat, List<RebarGroupSpacing> spacings)
        {
            this.RebarMesh = mesh;
            this.Rebar = mat;
            this.CustomSpacing = spacings;
            //this.CheckReinfSpacing = checkSpacing;
        }


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

        #region coa interop
        internal ComposReinf(string coaString)
        {
            // to do - implement from coa string method
        }

        internal string ToCoaString()
        {
            // to do - implement to coa string method
            return string.Empty;
        }
        #endregion

        #region methods

        public ComposReinf Duplicate()
        {
            if (this == null) { return null; }
            ComposReinf dup = (ComposReinf)this.MemberwiseClone();
            return dup;
        }

        public override string ToString()
        {
            string size = this.RebarMesh.ToString() + "/" + this.Rebar.ToString();
            return size.Replace(" ", string.Empty);
        }

        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class ComposReinfGoo : GH_Goo<ComposReinf>
    {
        #region constructors
        public ComposReinfGoo()
        {
            this.Value = new ComposReinf();
        }
        public ComposReinfGoo(ComposReinf item)
        {
            if (item == null)
                item = new ComposReinf();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public ComposReinfGoo DuplicateGoo()
        {
            return new ComposReinfGoo(Value == null ? new ComposReinf() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Reinf";
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

            if (typeof(Q).IsAssignableFrom(typeof(ComposReinf)))
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
            if (typeof(ComposReinf).IsAssignableFrom(source.GetType()))
            {
                Value = (ComposReinf)source;
                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the CustomGoo type.
    /// </summary>

    public class ComposReinfParameter : GH_PersistentParam<ComposReinfGoo>
    {
        public ComposReinfParameter()
          : base(new GH_InstanceDescription("Reinf", "Std", "Compos Reinf", ComposGH.Components.Ribbon.CategoryName.Name(), ComposGH.Components.Ribbon.SubCategoryName.Cat10()))
        {
        }

        public override Guid ComponentGuid => new Guid("e0b6cb52-99c8-4b2a-aec1-7f8a2d720daa");

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => ComposGH.Properties.Resources.SteelMaterialParam;

        protected override GH_GetterResult Prompt_Plural(ref List<ComposReinfGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref ComposReinfGoo value)
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

        public bool Hidden
        {
            get { return true; }
            //set { m_hidden = value; }
        }
        public bool IsPreviewCapable
        {
            get { return false; }
        }
        #endregion
    }

}
