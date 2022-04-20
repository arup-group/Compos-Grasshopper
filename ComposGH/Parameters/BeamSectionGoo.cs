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
using UnitsNet.Units;

namespace ComposGH.Parameters
{
    /// <summary>
    /// Custom class: this class defines the basic properties and methods for our custom class
    /// </summary>
    public class BeamSection
    {
        // Setting out

        public bool TaperedToNext { get; set; } = true;
        public Length StartPosition { get; set; } = Length.Zero;

        // Dimensions
        public Length Depth { get; set; }
        public Length TopFlangeWidth { get; set; }
        public Length BottomFlangeWidth { get; set; }
        public Length TopFlangeThickness { get; set; }
        public Length BottomFlangeThickness { get; set; }
        public Length WebThickness { get; set; }
        
        public string SectionDescription { get; set; }
        
        #region constructors
        public BeamSection()
        {
            // empty constructor
        }
        public BeamSection(string profile)
        {
            profile = profile.Replace(',', '.');

            if (profile.StartsWith("STD I"))
            {
                // example: STD I 200 190.5 8.5 12.7
                // example: STD I(cm) 20. 19. 8.5 1.27

                string[] parts = profile.Split(' ');

                LengthUnit unit = LengthUnit.Millimeter; // default unit for sections is mm

                string[] type = parts[0].Split('(',')');
                if (type.Length > 0)
                {
                    var parser = UnitParser.Default;
                    unit = parser.Parse<LengthUnit>(type[1]);
                }
                try
                {
                    this.Depth = new Length(double.Parse(parts[1]), unit);
                    this.TopFlangeWidth = new Length(double.Parse(parts[2]), unit);
                    this.BottomFlangeWidth = TopFlangeWidth;
                    this.WebThickness = new Length(double.Parse(parts[3]), unit);
                    this.TopFlangeThickness = new Length(double.Parse(parts[4]), unit);
                    this.BottomFlangeThickness = TopFlangeThickness;
                    this.SectionDescription = profile;
                }
                catch (Exception)
                {
                    throw new Exception("Unrecognisable elements in profile string.");
                }
            }
            else if (profile.StartsWith("STD GI"))
            {
                // example: STD GI 400. 300. 250. 12. 25. 20.
                // example: STD GI(cm) 15. 15. 12. 3. 1. 2.

                string[] parts = profile.Split(' ');

                LengthUnit unit = LengthUnit.Millimeter; // default unit for sections is mm

                string[] type = parts[0].Split('(', ')');
                if (type.Length > 0)
                {
                    var parser = UnitParser.Default;
                    unit = parser.Parse<LengthUnit>(type[1]);
                }
                try
                {
                    this.Depth = new Length(double.Parse(parts[1]), unit);
                    this.TopFlangeWidth = new Length(double.Parse(parts[2]), unit);
                    this.BottomFlangeWidth = new Length(double.Parse(parts[3]), unit);
                    this.WebThickness = new Length(double.Parse(parts[3]), unit);
                    this.TopFlangeThickness = new Length(double.Parse(parts[4]), unit);
                    this.BottomFlangeThickness = new Length(double.Parse(parts[5]), unit);
                    this.SectionDescription = profile;
                }
                catch (Exception)
                {
                    throw new Exception("Unrecognisable elements in profile string.");
                }
            }
            else if (profile.StartsWith("CAT"))
            {
                // to be done
                this.SectionDescription = profile;
            }
            else
                throw new ArgumentException("Unrecognisable profile type. String must start with 'STD I', 'STD GI' or 'CAT'.");
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

        public BeamSection Duplicate()
        {
            if (this == null) { return null; }
            BeamSection dup = (BeamSection)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {
            return (this.SectionDescription == null) ? "Null profile" : this.SectionDescription;
        }
        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class BeamSectionGoo : GH_Goo<BeamSection>
    {
        #region constructors
        public BeamSectionGoo()
        {
            this.Value = new BeamSection();
        }
        public BeamSectionGoo(BeamSection item)
        {
            if (item == null)
                item = new BeamSection();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public BeamSectionGoo DuplicateGoo()
        {
            return new BeamSectionGoo(Value == null ? new BeamSection() : Value.Duplicate());
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

            if (typeof(Q).IsAssignableFrom(typeof(BeamSection)))
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

            //Cast from custom type
            if (typeof(BeamSection).IsAssignableFrom(source.GetType()))
            {
                Value = (BeamSection)source;
                return true;
            }

            //Cast from string
            if (GH_Convert.ToString(source, out string mystring, GH_Conversion.Both))
            {
                Value = new BeamSection(mystring);
                return true;
            }
            return false;
        }
        #endregion
    }
}
