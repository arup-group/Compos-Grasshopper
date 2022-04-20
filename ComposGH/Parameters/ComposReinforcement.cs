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
    public class MeshReinforcement
    {
        public Length Cover { get; set; }
        public ReinforcementMeshType Type { get; set; }
        public bool Rotated { get; set; }

        public enum ReinforcementMeshType
        {
            A393,
            A252,
            A193,
            A142,
            A98,
            B1131,
            B785,
            B503,
            B385,
            B283,
            B196,
            C785,
            C636,
            C503,
            C385,
            C283
        }

        #region constructors
        public MeshReinforcement()
        {
            //empty constructor
        }

        public MeshReinforcement(Length cover, ReinforcementMeshType meshType = ReinforcementMeshType.A393, bool rotated = false)
        {
            this.Cover = cover;
            this.Type = meshType;
            this.Rotated = rotated;
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

        public MeshReinforcement Duplicate()
        {
            if (this == null) { return null; }
            MeshReinforcement dup = (MeshReinforcement)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {
            string cov = Cover.ToString("f0");
            string msh = Type.ToString();

            string rotated = (this.Rotated == true) ? " (rotated)" : "";


            return msh.Replace(" ", string.Empty) + rotated + ", c:" + cov.Replace(" ", string.Empty);
        }
        #endregion
    }
    /// <summary>
    /// Custom class: this class defines the basic properties and methods for our custom class
    /// </summary>
    public class TransverseReinforcement
    {
        public Length DistanceFromStart { get; set; }
        public Length DistanceFromEnd { get; set; }
        public Length Diameter { get; set; }
        public Length Spacing { get; set; }
        public Length Cover { get; set; }
        public RebarMaterial Material { get; set; }

        // Rebar spacing
        public enum RebarSpacingType
        {
            Automatic,
            Custom
        }
        #region constructors
        public TransverseReinforcement()
        {
            // empty constructor
        }

        public TransverseReinforcement(RebarMaterial material, Length distanceFromStart, Length distanceFromEnd, Length diameter, Length spacing, Length cover)
        {
            this.Material = material;
            this.DistanceFromStart = distanceFromStart;
            this.DistanceFromEnd = distanceFromEnd;
            this.Diameter = diameter;
            this.Spacing = spacing;
            this.Cover = cover;
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

        public TransverseReinforcement Duplicate()
        {
            if (this == null) { return null; }
            TransverseReinforcement dup = (TransverseReinforcement)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {
            string start = (this.DistanceFromStart.Value == 0) ? "" : this.DistanceFromStart.ToString().Replace(" ", string.Empty) + "<-";
            string end = (this.DistanceFromEnd.Value == 0) ? "" : "->" + this.DistanceFromEnd.ToString().Replace(" ", string.Empty);
            string startend = start + end;
            startend = startend.Replace("--", "-").Replace(",", string.Empty);
            string mat = this.Material.ToString();
            string dia = "Ø" + this.Diameter.ToString().Replace(" ", string.Empty);
            string spacing = "/" + this.Spacing.ToString().Replace(" ", string.Empty);
            string cov = ", c:" + this.Cover.ToString().Replace(" ", string.Empty);
            string diaspacingcov = dia + spacing + cov;

            string joined = string.Join(" ", new List<string>() { startend, mat, diaspacingcov });
            return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
        }
        #endregion
    }
    /// <summary>
    /// Custom class: this class defines the basic properties and methods for our custom class
    /// </summary>
    public class ComposReinforcement
    {
        public MeshReinforcement Mesh { get; set; }
        public TransverseReinforcement Transverse { get; set; }
        internal enum ReinforcementType
        {
            Mesh,
            Transverse
        }
        internal ReinforcementType Type { get; set; }

        #region constructors
        public ComposReinforcement()
        {
            // empty constructor
        }
        public ComposReinforcement(MeshReinforcement mesh)
        {
            this.Mesh = mesh;
            this.Type = ReinforcementType.Mesh;
        }
        public ComposReinforcement(Length cover, MeshReinforcement.ReinforcementMeshType meshType = MeshReinforcement.ReinforcementMeshType.A393, bool rotated = false)
        {
            this.Mesh = new MeshReinforcement(cover, meshType, rotated);
            this.Type = ReinforcementType.Mesh;
        }
        public ComposReinforcement(TransverseReinforcement transverse)
        {
            this.Transverse = transverse;
            this.Type = ReinforcementType.Transverse;
        }
        public ComposReinforcement(RebarMaterial material, Length distanceFromStart, Length distanceFromEnd, Length diameter, Length spacing, Length cover)
        {
            this.Transverse = new TransverseReinforcement(material, distanceFromStart, distanceFromEnd, diameter, spacing, cover);
            this.Type = ReinforcementType.Transverse;
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
        internal ComposReinforcement(string coaString)
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

        public ComposReinforcement Duplicate()
        {
            if (this == null) { return null; }
            ComposReinforcement dup = (ComposReinforcement)this.MemberwiseClone();
            return dup;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ReinforcementType.Mesh: return this.Mesh.ToString();
                case ReinforcementType.Transverse: return this.Transverse.ToString();
                default: return base.ToString();
            }
        }

        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class ComposReinforcementGoo : GH_Goo<ComposReinforcement>
    {
        #region constructors
        public ComposReinforcementGoo()
        {
            this.Value = new ComposReinforcement();
        }
        public ComposReinforcementGoo(ComposReinforcement item)
        {
            if (item == null)
                item = new ComposReinforcement();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public ComposReinforcementGoo DuplicateGoo()
        {
            return new ComposReinforcementGoo(Value == null ? new ComposReinforcement() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Reinforcement";
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

            if (typeof(Q).IsAssignableFrom(typeof(ComposReinforcement)))
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
            if (typeof(ComposReinforcement).IsAssignableFrom(source.GetType()))
            {
                Value = (ComposReinforcement)source;
                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the CustomGoo type.
    /// </summary>
    public class ComposReinforcementParameter : GH_PersistentParam<ComposReinforcementGoo>
    {
        public ComposReinforcementParameter()
          : base(new GH_InstanceDescription("Reinforcement", "Rb", "Compos Slab Reinforcement", ComposGH.Components.Ribbon.CategoryName.Name(), ComposGH.Components.Ribbon.SubCategoryName.Cat10()))
        {
        }

        public override Guid ComponentGuid => new Guid("c6261340-24f2-4d79-9894-5cd945023163");

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override System.Drawing.Bitmap Icon => ComposGH.Properties.Resources.SteelMaterialParam;

        protected override GH_GetterResult Prompt_Plural(ref List<ComposReinforcementGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref ComposReinforcementGoo value)
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
