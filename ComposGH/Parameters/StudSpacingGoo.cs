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
    public class StudGroupSpacing
    {
        public Length DistanceFromStart { get; set; }
        public int NumberOfRows { get; set; } = 2;
        public int NumberOfLines { get; set; } = 1;
        public Length Spacing { get; set; }

        // Stud spacing
        public enum StudSpacingType
        {
            Automatic,
            Partial_Interaction,
            Min_Num_of_Studs,
            Custom
        }
        #region constructors
        public StudGroupSpacing()
        {
            // empty constructor
        }

        public StudGroupSpacing(Length distanceFromStart, int numberOfRows, int numberOfLines, Length spacing)
        {
            this.DistanceFromStart = distanceFromStart;
            if (numberOfRows < 1)
                throw new ArgumentException("Number of rows must be bigger or equal to 1");
            this.NumberOfRows = numberOfRows;
            if (numberOfLines < 1)
                throw new ArgumentException("Number of lines must be bigger or equal to 1");
            this.NumberOfLines = numberOfLines;
            this.Spacing = spacing;
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

        public StudGroupSpacing Duplicate()
        {
            if (this == null) { return null; }
            StudGroupSpacing dup = (StudGroupSpacing)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {
            string start = (this.DistanceFromStart.Value == 0) ? "" : "From:" + this.DistanceFromStart.ToString().Replace(" ", string.Empty);
            string rows = NumberOfRows + "R";
            string lines = NumberOfLines + "L";
            string spacing = "@" + this.Spacing.ToString().Replace(" ", string.Empty);

            string joined = string.Join(" ", new List<string>() { start, rows, lines, spacing });
            return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
        }
        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class StudGroupSpacingGoo : GH_Goo<StudGroupSpacing>
    {
        #region constructors
        public StudGroupSpacingGoo()
        {
            this.Value = new StudGroupSpacing();
        }
        public StudGroupSpacingGoo(StudGroupSpacing item)
        {
            if (item == null)
                item = new StudGroupSpacing();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public StudGroupSpacingGoo DuplicateGoo()
        {
            return new StudGroupSpacingGoo(Value == null ? new StudGroupSpacing() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Stud Spacing";
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

            if (typeof(Q).IsAssignableFrom(typeof(StudGroupSpacing)))
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
            if (typeof(StudGroupSpacing).IsAssignableFrom(source.GetType()))
            {
                Value = (StudGroupSpacing)source;
                return true;
            }

            return false;
        }
        #endregion
    }
}
