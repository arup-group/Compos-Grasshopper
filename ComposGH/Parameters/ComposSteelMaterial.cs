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
    public class ComposSteelMaterial
    {
        public Pressure fy { get; set; }
        public Pressure E { get; set; }
        public Density Density { get; set; }
        public bool ReductionFactorMpl { get; set; }

        // add public enum(s) for standard materials here
        // add enum for weld material

        #region constructors
        public ComposSteelMaterial()
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
        internal ComposSteelMaterial(string coaString)
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

        public ComposSteelMaterial Duplicate()
        {
            if (this == null) { return null; }
            ComposSteelMaterial dup = (ComposSteelMaterial)this.MemberwiseClone();
            return dup;
        }

        public override string ToString()
        {
            // update with better naming
            return fy.ToString().Replace(" ", string.Empty);
        }

        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class ComposSteelMaterialGoo : GH_Goo<ComposSteelMaterial>
    {
        #region constructors
        public ComposSteelMaterialGoo()
        {
            this.Value = new ComposSteelMaterial();
        }
        public ComposSteelMaterialGoo(ComposSteelMaterial item)
        {
            if (item == null)
                item = new ComposSteelMaterial();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public ComposSteelMaterialGoo DuplicateGoo()
        {
            return new ComposSteelMaterialGoo(Value == null ? new ComposSteelMaterial() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Steel Material";
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

            if (typeof(Q).IsAssignableFrom(typeof(ComposSteelMaterial)))
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
            if (typeof(ComposSteelMaterial).IsAssignableFrom(source.GetType()))
            {
                Value = (ComposSteelMaterial)source;
                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the CustomGoo type.
    /// </summary>

    public class ComposSteelMaterialParameter: GH_PersistentParam<ComposSteelMaterialGoo>
    {
        public ComposSteelMaterialParameter()
          : base(new GH_InstanceDescription("Stud", "Std", "Compos Stud", ComposGH.Components.Ribbon.CategoryName.Name(), ComposGH.Components.Ribbon.SubCategoryName.Cat10()))
        {
        }

        public override Guid ComponentGuid => new Guid("1245ee2f-3d04-4135-833c-abff82dff85c");

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => ComposGH.Properties.Resources.SteelMaterialParam;

        protected override GH_GetterResult Prompt_Plural(ref List<ComposSteelMaterialGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref ComposSteelMaterialGoo value)
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
