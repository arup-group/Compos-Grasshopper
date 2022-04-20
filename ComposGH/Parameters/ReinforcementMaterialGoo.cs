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
    public class RebarMaterial
    {
        public enum RebarMatType
        {
            Standard,
            Custom
        }
        public Pressure Fu { get; set; }

        public enum StandardGrade
        {
            BS_250R,
            BS_460T,
            BS_500X,
            BS_1770,
            EN_500A,
            EN_500B,
            EN_500C,
            HK_250,
            HK_460,
            AS_R250N,
            AS_D500L,
            AS_D500N,
            AS_D500E
        }

        private void SetGradeFromStandard(StandardGrade standardGrade)
        {
            switch (standardGrade)
            {
                case StandardGrade.BS_250R:
                case StandardGrade.HK_250:
                case StandardGrade.AS_R250N:
                    this.Fu = new Pressure(250, UnitsNet.Units.PressureUnit.Megapascal);
                    break;
                case StandardGrade.BS_460T:
                case StandardGrade.HK_460:
                    this.Fu = new Pressure(460, UnitsNet.Units.PressureUnit.Megapascal);
                    break;
                case StandardGrade.BS_500X:
                case StandardGrade.AS_D500L:
                case StandardGrade.AS_D500N:
                case StandardGrade.AS_D500E:
                case StandardGrade.EN_500A:
                case StandardGrade.EN_500B:
                case StandardGrade.EN_500C:
                    this.Fu = new Pressure(500, UnitsNet.Units.PressureUnit.Megapascal);
                    break;
                case StandardGrade.BS_1770:
                    this.Fu = new Pressure(1770, UnitsNet.Units.PressureUnit.Megapascal);
                    break;
            }
        }

        #region constructors
        public RebarMaterial()
        {
            // empty constructor
        }
        public RebarMaterial(Pressure fu)
        {
            this.Fu = fu;
        }
 
        public RebarMaterial(StandardGrade standardGrade)
        {
            SetGradeFromStandard(standardGrade);
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

        public RebarMaterial Duplicate()
        {
            if (this == null) { return null; }
            RebarMaterial dup = (RebarMaterial)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {

            string f = (Fu.Value == 0) ? Fu.ToString() : Fu.ToString();

            return f.Replace(" ", string.Empty);
        }
        #endregion
    }

    /// <summary>
    /// Goo wrapH class, makes sure our custom class can be used in GrasshopH.
    /// </summary>
    public class RebarMaterialGoo : GH_Goo<RebarMaterial>
    {
        #region constructors
        public RebarMaterialGoo()
        {
            this.Value = new RebarMaterial();
        }
        public RebarMaterialGoo(RebarMaterial item)
        {
            if (item == null)
                item = new RebarMaterial();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public RebarMaterialGoo DuplicateGoo()
        {
            return new RebarMaterialGoo(Value == null ? new RebarMaterial() : Value.Duplicate());
        }
        #endregion

        #region propethies
        public override bool IsValid => true;
        public override string TypeName => "Rebar Material";
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

            if (typeof(Q).IsAssignableFrom(typeof(RebarMaterial)))
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
            if (typeof(RebarMaterial).IsAssignableFrom(source.GetType()))
            {
                Value = (RebarMaterial)source;
                return true;
            }

            return false;
        }
        #endregion
    }
}
