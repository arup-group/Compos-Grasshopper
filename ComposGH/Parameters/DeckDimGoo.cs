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
    public class DeckDim
    {
        public Length DistanceB1{ get; set; }
        public Length DistanceB2 { get; set; }
        public Length DistanceB3 { get; set; }
        public Length DistanceB4 { get; set; }
        public Length DistanceB5 { get; set; }
        public Length Depth { get; set; }
        public Length Thickness { get; set; }
        public Pressure Strength { get; set; }

        #region constructors
        public DeckDim()
        {
            // empty constructor
        }

        public DeckDim(Length distanceB1, Length distanceB2, Length distanceB3, Length distanceB4, Length distanceB5, Length depth, Length thickness, Pressure stress)
        {
            this.DistanceB1 = distanceB1;
            this.DistanceB2 = distanceB2;
            this.DistanceB3 = distanceB3;
            this.DistanceB4 = distanceB4;
            this.DistanceB5 = distanceB5;
            this.Depth = depth;
            this.Thickness = thickness;
            this.Strength = stress;

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

        public DeckDim Duplicate()
        {
            if (this == null) { return null; }
            DeckDim dup = (DeckDim)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {
            string distanceB1 = (this.DistanceB1.Value == 0) ? "" : "b1:" + this.DistanceB1.ToString().Replace(" ", string.Empty);
            string distanceB2 = (this.DistanceB2.Value == 0) ? "" : "b2:" + this.DistanceB2.ToString().Replace(" ", string.Empty);
            string distanceB3 = (this.DistanceB3.Value == 0) ? "" : "b3:" + this.DistanceB3.ToString().Replace(" ", string.Empty);
            string distanceB4 = (this.DistanceB4.Value == 0) ? "" : "b4:" + this.DistanceB4.ToString().Replace(" ", string.Empty);
            string distanceB5 = (this.DistanceB5.Value == 0) ? "" : "b5:" + this.DistanceB5.ToString().Replace(" ", string.Empty);
            string depth = (this.Depth.Value == 0) ? "" : "d:" + this.Depth.ToString().Replace(" ", string.Empty);
            string thickness = (this.Thickness.Value == 0) ? "" : "th:" + this.Thickness.ToString().Replace(" ", string.Empty);
            string stress = (this.Strength.Value == 0) ? "" : "stress:" + this.Strength.ToString().Replace(" ", string.Empty);

            string joined = string.Join(" ", new List<string>() { distanceB1, distanceB2, distanceB3, distanceB4, distanceB5, depth, thickness, stress });
            return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
        }
        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class DeckDimGoo : GH_Goo<DeckDim>
    {
        #region constructors
        public DeckDimGoo()
        {
            this.Value = new DeckDim();
        }
        public DeckDimGoo(DeckDim item)
        {
            if (item == null)
                item = new DeckDim();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public DeckDimGoo DuplicateGoo()
        {
            return new DeckDimGoo(Value == null ? new DeckDim() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Deck Dimension";
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

            if (typeof(Q).IsAssignableFrom(typeof(DeckDim)))
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
            if (typeof(DeckDim).IsAssignableFrom(source.GetType()))
            {
                Value = (DeckDim)source;
                return true;
            }

            return false;
        }
        #endregion
    }
}
