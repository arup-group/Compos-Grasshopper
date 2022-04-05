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
    public class DesignCode
    {
        public enum Code
        {
            BS5950_3_1_1990_Superseeded,
            BS5950_3_1_1990_A1_2010,
            EN1994_1_1_2004,
            HKSUOS_2005,
            HKSUOS_2011,
            AS_NZS2327_2017
        }
        public enum NationalAnnex
        {
            Generic,
            United_Kingdom
        }
        
        public Code Design_Code { get; set; }
        public NationalAnnex National_Annex { get; set; }

        #region constructors
        public DesignCode()
        {
            this.Design_Code = Code.EN1994_1_1_2004;
            this.National_Annex = NationalAnnex.Generic;
        }
        public DesignCode(Code designcode, NationalAnnex nationalAnnex = NationalAnnex.Generic)
        {
            this.Design_Code = designcode;
            this.National_Annex = nationalAnnex;
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
        internal DesignCode(string coaString)
        {
            switch (coaString)
            {
                case "BS5950-3.1:1990 (superseded)":
                    this.Design_Code = Code.BS5950_3_1_1990_Superseeded;
                    break;
                case "BS5950-3.1:1990+A1:2010":
                    this.Design_Code = Code.BS5950_3_1_1990_A1_2010;
                    break;
                case "EN1994-1-1:2004":
                    this.Design_Code = Code.EN1994_1_1_2004;
                    break;
                case "HKSUOS:2005":
                    this.Design_Code = Code.HKSUOS_2005;
                    break;
                case "HKSUOS:2011":
                    this.Design_Code = Code.HKSUOS_2011;
                    break;
                case "AS/NZS2327:2017":
                    this.Design_Code = Code.AS_NZS2327_2017;
                    break;
            }
        }

        internal string Coa()
        {
            switch (this.Design_Code)
            {
                case Code.BS5950_3_1_1990_Superseeded:
                    return "BS5950-3.1:1990 (superseded)";
                case Code.BS5950_3_1_1990_A1_2010:
                    return "BS5950-3.1:1990+A1:2010";
                case Code.EN1994_1_1_2004:
                    return "EN1994-1-1:2004";
                case Code.HKSUOS_2005:
                    return "HKSUOS:2005";
                case Code.HKSUOS_2011:
                    return "HKSUOS:2011";
                case Code.AS_NZS2327_2017:
                    return "AS/NZS2327:2017";
            }
            return "";
        }
        #endregion

        #region methods

        public DesignCode Duplicate()
        {
            if (this == null) { return null; }
            DesignCode dup = (DesignCode)this.MemberwiseClone();
            return dup;
        }

        public override string ToString()
        {
            return Coa();
        }

        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class DesignCodeGoo : GH_Goo<DesignCode>
    {
        #region constructors
        public DesignCodeGoo()
        {
            this.Value = new DesignCode();
        }
        public DesignCodeGoo(DesignCode item)
        {
            if (item == null)
                item = new DesignCode();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public DesignCodeGoo DuplicateGoo()
        {
            return new DesignCodeGoo(Value == null ? new DesignCode() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "DesignCode";
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

            if (typeof(Q).IsAssignableFrom(typeof(ComposStud)))
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
            if (typeof(DesignCode).IsAssignableFrom(source.GetType()))
            {
                Value = (DesignCode)source;
                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the CustomGoo type.
    /// </summary>
    public class DesignCodeParameter : GH_PersistentParam<DesignCodeGoo>
    {
        public DesignCodeParameter()
          : base(new GH_InstanceDescription("DesignCode", "DC", "Compos DesignCode", ComposGH.Components.Ribbon.CategoryName.Name(), ComposGH.Components.Ribbon.SubCategoryName.Cat10()))
        {
        }
        public override Guid ComponentGuid => new Guid("fb4d79ea-1c30-4e86-9654-a55ef42fd8e2");

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => ComposGH.Properties.Resources.SteelMaterialParam;

        protected override GH_GetterResult Prompt_Plural(ref List<DesignCodeGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref DesignCodeGoo value)
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
