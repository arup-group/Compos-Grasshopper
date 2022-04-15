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
    public class ComposBeam
    {
        public Length Length { get; set; }
        public ComposSteelMaterial Material { get; set; }
        public List<BeamSection> BeamSections { get; set; }
        public List<ComposWebOpening> WebOpenings { get; set; }

        #region constructors
        public ComposBeam()
        {
            // empty constructor
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

        #region coa interop
        internal ComposBeam(string coaString)
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

        public ComposBeam Duplicate()
        {
            if (this == null) { return null; }
            ComposBeam dup = (ComposBeam)this.MemberwiseClone();
            return dup;
        }

        public override string ToString()
        {
            // update with better naming
            return "Beam";
        }

        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class ComposBeamGoo : GH_Goo<ComposBeam> // needs to be upgraded to GeometryGoo eventually....
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

            return false;
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the CustomGoo type.
    /// </summary>

    public class ComposBeamParameter : GH_PersistentParam<ComposBeamGoo>
    {
        public ComposBeamParameter()
          : base(new GH_InstanceDescription("Beam", "Bm", "Compos Beam", ComposGH.Components.Ribbon.CategoryName.Name(), ComposGH.Components.Ribbon.SubCategoryName.Cat10()))
        {
        }

        public override Guid ComponentGuid => new Guid("2dc51bc1-9abb-4f26-845f-ca1e66236e9e");

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override System.Drawing.Bitmap Icon => ComposGH.Properties.Resources.SteelMaterialParam;

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
