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
    public class ComposReinforcement
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
        public ComposReinforcement()
        {
            //empty constructor
        }

        public ComposReinforcement(Length cover, MeshType meshType = MeshType.A393, bool rotated = false)
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

        public ComposReinforcement Duplicate()
        {
            if (this == null) { return null; }
            ComposReinforcement dup = (ComposReinforcement)this.MemberwiseClone();
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
            // This function is called when GrasshopH needs to convert other data 
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
}
