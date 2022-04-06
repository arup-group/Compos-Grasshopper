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
    public class StudSpecification
    {
        // Stud Specifications
        public bool Welding { get; set; }
        public bool NCCI { get; set; }
        public bool EC4_Limit { get; set; }
        public Length NoStudZoneStart { get; set; }
        public Length NoStudZoneEnd { get; set; }
        public Length ReinforcementPosition { get; set; }
        #region constructors
        public StudSpecification()
        {
            // empty constructor
        }
        /// <summary>
        /// for EC4 code
        /// </summary>
        /// <param name="noStudZoneStart"></param>
        /// <param name="noStudZoneEnd"></param>
        /// <param name="reinforcementPosition"></param>
        /// <param name="welding"></param>
        /// <param name="ncci"></param>
        public StudSpecification(Length noStudZoneStart, Length noStudZoneEnd, Length reinforcementPosition, bool welding, bool ncci)
        {
            this.NoStudZoneStart = noStudZoneStart;
            this.NoStudZoneEnd = noStudZoneEnd;
            this.ReinforcementPosition = reinforcementPosition;
            this.Welding = welding;
            this.NCCI = ncci;
        }
        /// <summary>
        /// for BS5950 code
        /// </summary>
        /// <param name="useEC4Limit"></param>
        /// <param name="noStudZoneStart"></param>
        /// <param name="noStudZoneEnd"></param>
        /// <param name=""></param>
        public StudSpecification(bool useEC4Limit, Length noStudZoneStart, Length noStudZoneEnd)
        {
            this.NoStudZoneStart = noStudZoneStart;
            this.NoStudZoneEnd = noStudZoneEnd;
            this.EC4_Limit = useEC4Limit;
        }
        /// <summary>
        /// for codes: AS/NZ, HK, 
        /// </summary>
        /// <param name="noStudZoneStart"></param>
        /// <param name="noStudZoneEnd"></param>
        /// <param name="welding"></param>
        public StudSpecification(Length noStudZoneStart, Length noStudZoneEnd, bool welding)
        {
            this.NoStudZoneStart = noStudZoneStart;
            this.NoStudZoneEnd = noStudZoneEnd;
            this.Welding = welding;
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

        public StudSpecification Duplicate()
        {
            if (this == null) { return null; }
            StudSpecification dup = (StudSpecification)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {
            string noStudStart = (this.NoStudZoneStart.Value == 0) ? "" : "NSZS:" + this.NoStudZoneStart.ToString().Replace(" ", string.Empty);
            string noStudEnd = (this.NoStudZoneEnd.Value == 0) ? "" : "NSZE:" + this.NoStudZoneEnd.ToString().Replace(" ", string.Empty);
            string rebarPos = (this.ReinforcementPosition.Value == 0) ? "" : "RbP:" + this.ReinforcementPosition.ToString().Replace(" ", string.Empty);
            string welding = (this.Welding == true) ? "Welded" : "";
            string ncci = (this.NCCI == true) ? "NCCI Limit" : "";
            string ec4 = (this.EC4_Limit == true) ? "EC4 Limit" : "";
            string joined = string.Join(" ", new List<string>() { noStudStart, noStudEnd, rebarPos, welding, ncci, ec4 });
            return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
        }
        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class StudSpecificationGoo : GH_Goo<StudSpecification>
    {
        #region constructors
        public StudSpecificationGoo()
        {
            this.Value = new StudSpecification();
        }
        public StudSpecificationGoo(StudSpecification item)
        {
            if (item == null)
                item = new StudSpecification();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public StudSpecificationGoo DuplicateGoo()
        {
            return new StudSpecificationGoo(Value == null ? new StudSpecification() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Stud Spec";
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

            if (typeof(Q).IsAssignableFrom(typeof(StudSpecification)))
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
            if (typeof(StudSpecification).IsAssignableFrom(source.GetType()))
            {
                Value = (StudSpecification)source;
                return true;
            }

            return false;
        }
        #endregion
    }
}
