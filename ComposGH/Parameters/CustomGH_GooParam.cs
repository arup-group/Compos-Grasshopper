using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace ComposGH.Parameters
{
    public class ComposMaterial
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
        public ComposMaterial()
        {
        }
        

        public ComposMaterial Duplicate()
        {
            if (this == null) { return null; }
            ComposMaterial dup = (ComposMaterial)this.MemberwiseClone();
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
    public class ComposMaterialGoo : GH_Goo<ComposMaterial>
    {
        #region constructors
        public ComposMaterialGoo()
        {
            this.Value = new ComposMaterial();
        }
        public ComposMaterialGoo(ComposMaterial goo)
        {
            if (goo == null)
                goo = new ComposMaterial();
            this.Value = goo.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public ComposMaterialGoo DuplicateGoo()
        {
            return new ComposMaterialGoo(Value == null ? new ComposMaterial() : Value.Duplicate());
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

            if (typeof(Q).IsAssignableFrom(typeof(ComposMaterial)))
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
            if (typeof(ComposMaterial).IsAssignableFrom(source.GetType()))
            {
                Value = (ComposMaterial)source;
                return true;
            }

            return false;
        }
        #endregion

        /// <summary>
        /// This class provides a Parameter interface for the custom GH_Goo type.
        /// </summary>
        public class ComposMaterialParameter : GH_PersistentParam<ComposMaterialGoo>
        {
            public ComposMaterialParameter()
              : base(new GH_InstanceDescription("Material", "Mat", "Compos Material Parameter", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat9()))
            {
            }

            public override Guid ComponentGuid => new Guid("0b357078-43f4-4c72-9184-7296f191fdde");

            public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;

            protected override System.Drawing.Bitmap Icon => Properties.Resources.MaterialParam;

            protected override GH_GetterResult Prompt_Plural(ref List<ComposMaterialGoo> values)
            {
                return GH_GetterResult.cancel;
            }
            protected override GH_GetterResult Prompt_Singular(ref ComposMaterialGoo value)
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
            }
            public bool IsPreviewCapable
            {
                get { return false; }
            }
            #endregion
        }
    }
}
