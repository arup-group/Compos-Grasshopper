using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using Grasshopper.Documentation;
using Rhino.Collections;

namespace ComposGH.Parameters
{
    /// <summary>
    /// Section class, this class defines the basic properties and methods for any Compos Section
    /// </summary>
    public class ComposSteelMaterial
    {

        public enum SteelType
        {
            CUSTOM = 0,
            S235 = 1,
            S275 = 2,
            S355 = 3,
            S450 = 4,
            S460 = 5
        }


        public string WeldMaterial
        {
            get 
            { return wMat; } 
            set
            { wMat = value; }
        }

        public double YeldStrength
        {
            get
            { return fy; }
            set
            { fy = value; }
        }

        public double YoungModulus
        {
            get
            { return E; }
            set
            { E = value; }
        }

        public double Density
        {
            get
            { return ρ; }
            set
            { ρ = value; }
        }


        public SteelType SType;

        
        #region fields
        string wMat = "";
        double fy;
        double E;
        double ρ;

        #endregion

        #region constructors
        public ComposSteelMaterial()
        {
            SType = SteelType.CUSTOM;
        }

        /// <param name="material_id"></param>
        public ComposSteelMaterial(int material_id)
        {
            SType = (SteelType)material_id;
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

        #region methods
        public override string ToString()
        {
            string mate = SType.ToString();
            mate = Char.ToUpper(mate[0]) + mate.Substring(1).ToLower().Replace("_", " ");

            return "Compos Steel Material " + mate;
        }

        #endregion
    }

    /// <summary>
    /// GsaSection Goo wrapper class, makes sure GsaSection can be used in Grasshopper.
    /// </summary>
    public class ComposSteelMaterialGoo : GH_Goo<ComposSteelMaterial>
    {
        #region constructors
        public ComposSteelMaterialGoo()
        {
            this.Value = new ComposSteelMaterial();
        }
        public ComposSteelMaterialGoo(ComposSteelMaterial material)
        {
            if (material == null)
                material = new ComposSteelMaterial();
            this.Value = material; //material.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaSection();
        }
        public ComposSteelMaterialGoo DuplicateGsaSection()
        {
            return new ComposSteelMaterialGoo(Value == null ? new ComposSteelMaterial() : Value); //Value.Duplicate());
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
                //if (Value == null) { return "No internal GsaMember instance"; }
                if (Value.IsValid) { return string.Empty; }
                return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null Compos Steel Material";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("Compos Steel Material"); }
        }
        public override string TypeDescription
        {
            get { return ("Compos Steel Material"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaMaterial into some other type Q.            


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
            // into ComposMaterial.

            if (source == null) { return false; }

            //Cast from GsaMaterial
            if (typeof(ComposSteelMaterial).IsAssignableFrom(source.GetType()))
            {
                Value = (ComposSteelMaterial)source;
                return true;
            }

            //Cast from string
            if (GH_Convert.ToString(source, out string mat, GH_Conversion.Both))
            {
                if (mat.ToUpper() == "CUSTOM")
                {
                    Value.SType = ComposSteelMaterial.SteelType.CUSTOM;
                    return true;
                }

                if (mat.ToUpper() == "S235")
                {
                    Value.SType = ComposSteelMaterial.SteelType.S235;
                    return true;
                }

                if (mat.ToUpper() == "S275")
                {
                    Value.SType = ComposSteelMaterial.SteelType.S275;
                    return true;
                }

                if (mat.ToUpper() == "S355")
                {
                    Value.SType = ComposSteelMaterial.SteelType.S355;
                    return true;
                }

                if (mat.ToUpper() == "S450")
                {
                    Value.SType = ComposSteelMaterial.SteelType.S450;
                    return true;
                }

                if (mat.ToUpper() == "S460")
                {
                    Value.SType = ComposSteelMaterial.SteelType.S460;
                    return true;
                }

                return false;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaSection type.
    /// </summary>
    public class ComposSteelMaterialParameter : GH_PersistentParam<ComposSteelMaterialGoo>
    {
        public ComposSteelMaterialParameter()
          : base(new GH_InstanceDescription("Material", "Ma", "Compos Material", ComposGH.Components.Ribbon.CategoryName.Name(), ComposGH.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("f13d079b-f7d1-4d8a-be7c-3b7e1e59c5ab");

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
