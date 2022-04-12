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
    public class RebarMesh
    {
        public Length Cover { get; set; }
        public MeshType Mesh_Type { get; set; }
        public bool Rotated { get; set; }

        public enum MeshType
        {
            None,
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
        public RebarMesh()
        {
            //empty constructor
        }

        public RebarMesh(Length cover, MeshType meshType = MeshType.A393, bool rotated = false)
        {
            this.Cover = cover;
            this.Mesh_Type = meshType;
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

        public RebarMesh Duplicate()
        {
            if (this == null) { return null; }
            RebarMesh dup = (RebarMesh)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {
            string cov = Cover.ToString("f0");
            string msh = Mesh_Type.ToString();
            
            string rotated = (this.Rotated == true) ? " Rotated" : "";
            

            return msh.Replace(" ", string.Empty) + " " +  cov.Replace(" ", string.Empty) + rotated;
        }
        #endregion
    }

    /// <summary>
    /// Goo wrapH class, makes sure our custom class can be used in GrasshopH.
    /// </summary>
    public class RebarMeshGoo : GH_Goo<RebarMesh>
    {
        #region constructors
        public RebarMeshGoo()
        {
            this.Value = new RebarMesh();
        }
        public RebarMeshGoo(RebarMesh item)
        {
            if (item == null)
                item = new RebarMesh();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public RebarMeshGoo DuplicateGoo()
        {
            return new RebarMeshGoo(Value == null ? new RebarMesh() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Reinforcement Type";
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
            // This function is called when GrasshopH needs to convert this 
            // instance of our custom class into some other type Q.            

            if (typeof(Q).IsAssignableFrom(typeof(RebarMesh)))
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
            // This function is called when GrasshopH needs to convert other data 
            // into our custom class.

            if (source == null) { return false; }

            //Cast from GsaMaterial
            if (typeof(RebarMesh).IsAssignableFrom(source.GetType()))
            {
                Value = (RebarMesh)source;
                return true;
            }

            return false;
        }
        #endregion

    }
}
