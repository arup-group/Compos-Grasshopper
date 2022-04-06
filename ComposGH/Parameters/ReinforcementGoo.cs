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
    public class Reinforcement
    {
        public Length Cover { get; set; }
        public MeshType Mesh_Type { get; set; }

        public enum MeshType
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
        public Reinforcement()
        {
            // empty constructor
        }

        public Reinforcement(Length cover, MeshType meshType = MeshType.A393)
        {
            this.Cover = cover;
            this.Mesh_Type = meshType;

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

        public Reinforcement Duplicate()
        {
            if (this == null) { return null; }
            Reinforcement dup = (Reinforcement)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {
            string cov = Cover.ToString("f0");

            return "Ø" + cov.Replace(" ", string.Empty);
        }
        #endregion
    }

    /// <summary>
    /// Goo wrapH class, makes sure our custom class can be used in GrasshopH.
    /// </summary>
    public class ReinforcementGoo : GH_Goo<Reinforcement>
    {
        #region constructors
        public ReinforcementGoo()
        {
            this.Value = new Reinforcement();
        }
        public ReinforcementGoo(Reinforcement item)
        {
            if (item == null)
                item = new Reinforcement();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public ReinforcementGoo DuplicateGoo()
        {
            return new ReinforcementGoo(Value == null ? new Reinforcement() : Value.Duplicate());
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

            if (typeof(Q).IsAssignableFrom(typeof(Reinforcement)))
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
            if (typeof(Reinforcement).IsAssignableFrom(source.GetType()))
            {
                Value = (Reinforcement)source;
                return true;
            }

            return false;
        }
        #endregion
    }
}
