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
    public class RebarGroupSpacing
    {
        public Length DistanceFromStart { get; set; }
        public Length DistanceFromEnd { get; set; }
        public Length Diameter { get; set; }
        public Length Spacing { get; set; }
        public Length Cover { get; set; }

        // Rebar spacing
        public enum RebarSpacingType
        {
            Automatic,
            Custom
        }
        #region constructors
        public RebarGroupSpacing()
        {
            // empty constructor
        }

        public RebarGroupSpacing(Length distanceFromStart, Length distanceFromEnd, Length diameter, Length spacing, Length cover)
        {
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

        public RebarGroupSpacing Duplicate()
        {
            if (this == null) { return null; }
            RebarGroupSpacing dup = (RebarGroupSpacing)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {
            string start = (this.DistanceFromStart.Value == 0) ? "" : "From:" + this.DistanceFromStart.ToString().Replace(" ", string.Empty);
            string end = (this.DistanceFromEnd.Value == 0) ? "" : "To:" + this.DistanceFromEnd.ToString().Replace(" ", string.Empty);
            string dia = "Φ" + this.Diameter.ToString().Replace(" ", string.Empty);
            string spacing = "@" + this.Spacing.ToString().Replace(" ", string.Empty);
            string cov =  this.Cover.ToString().Replace(" ", string.Empty);

            string joined = string.Join(" ", new List<string>() { start, end, dia, spacing, cov });
            return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
        }
        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class RebarGroupSpacingGoo : GH_Goo<RebarGroupSpacing>
    {
        #region constructors
        public RebarGroupSpacingGoo()
        {
            this.Value = new RebarGroupSpacing();
        }
        public RebarGroupSpacingGoo(RebarGroupSpacing item)
        {
            if (item == null)
                item = new RebarGroupSpacing();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public RebarGroupSpacingGoo DuplicateGoo()
        {
            return new RebarGroupSpacingGoo(Value == null ? new RebarGroupSpacing() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Rebar Spacing";
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

            if (typeof(Q).IsAssignableFrom(typeof(RebarGroupSpacing)))
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
            if (typeof(RebarGroupSpacing).IsAssignableFrom(source.GetType()))
            {
                Value = (RebarGroupSpacing)source;
                return true;
            }

            return false;
        }
        #endregion
    }
}
