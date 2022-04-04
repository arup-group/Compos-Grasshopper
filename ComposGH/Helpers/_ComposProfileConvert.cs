using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Rhino.Geometry;
using System.Windows.Forms;

namespace ComposGH.Helpers
{
    /// <summary>
    /// Profile class holds information about a profile
    /// Type: Standard, Catalogue or Geometric
    /// ShapeOptions for Standard type
    /// Section units
    /// </summary>
    public class ComposProfile
    {
        public enum ProfileTypes
        {
            [Description("Standard")] Standard,
            [Description("Catalogue")] Catalogue,
            [Description("Geometric")] Geometric
        }
        public enum GeoTypes
        {
            [Description("Perimeter")] Perim
            //[Description("Thin Wall")] ThinWall, // removed temporarily as not currently implemented (ADSEC-563)
            //[Description("Point")] Point         // removed temporarily as not currently implemented (ADSEC-563)
        }

        public enum StdShapeOptions
        {
            [Description("Rectangle")] Rectangle,
            [Description("Circle")] Circle,
            [Description("I section")] I_section,
            [Description("Tee")] Tee,
            [Description("Channel")] Channel,
            [Description("Angle")] Angle,
        }

        public enum SectUnitOptions
        {
            [Description("mm")] u_mm,
            [Description("cm")] u_cm,
            [Description("m")] u_m,
            [Description("ft")] u_ft,
            [Description("in")] u_in,
        }

        public ProfileTypes profileType;
        public StdShapeOptions stdShape;
        public GeoTypes geoType;
        public SectUnitOptions sectUnit = SectUnitOptions.u_mm;

        public string catalogueProfileName = "";

        public bool isTapered;
        public bool isHollow;
        public bool isElliptical;
        public bool isGeneral;
        public bool isB2B;

        public double d;
        public double b1;
        public double b2;
        public double tf1;
        public double tf2;
        public double tw1;
        public double tw2;

        public List<Point2d> perimeterPoints;
        public List<List<Point2d>> voidPoints;
    }
    /// <summary>
    /// Helper class for Profile/Section conversions
    /// </summary>
    public class ConvertSection
    {
        /// <summary>
        /// Method to update section units. 
        /// Use "factorValues" to automatically factor existing values to match new unit.
        /// </summary>
        /// <param name="composProfile"></param>
        /// <param name="factorValues"></param>
        /// <returns></returns>
        public static ComposProfile UpdateSectUnit(ComposProfile composProfile, bool factorValues)
        {
            if (composProfile.sectUnit.ToString() != ComposGH.Parameters.Units.LengthSection)
            {
                if (ComposGH.Parameters.Units.LengthSection == "mm" || ComposGH.Parameters.Units.LengthSection == "cm" || ComposGH.Parameters.Units.LengthSection == "m" ||
                ComposGH.Parameters.Units.LengthSection == "ft" || ComposGH.Parameters.Units.LengthSection == "in")
                {
                    
                    if (factorValues)
                    {
                        double conversionFactor = 1;
                        // convert current unit back to meters, I know that one
                        double toMeters = 1;
                        switch (ComposGH.Parameters.Units.LengthSection)
                        {
                            case "mm":
                                toMeters = 1/1000;
                                break;
                            case "cm":
                                toMeters = 1/100;
                                break;
                            case "m":
                                toMeters = 1;
                                break;
                            case "in":
                                toMeters = 1/(1000 / 25.4);
                                break;
                            case "ft":
                                toMeters = 1/(1000 / (12 * 25.4));
                                break;
                        }
                        // convert to new unit
                        switch (composProfile.sectUnit.ToString())
                        {
                            case "mm":
                                conversionFactor = toMeters * 1000;
                                break;
                            case "cm":
                                conversionFactor = toMeters * 100;
                                break;
                            case "m":
                                conversionFactor = toMeters * 1;
                                break;
                            case "in":
                                conversionFactor = toMeters * 1000 / 25.4;
                                break;
                            case "ft":
                                conversionFactor = toMeters * 1000 / (12 * 25.4);
                                break;
                        }
                        composProfile.d *= conversionFactor;
                        composProfile.b1 *= conversionFactor;
                        composProfile.b2 *= conversionFactor;
                        composProfile.tf1 *= conversionFactor;
                        composProfile.tf2 *= conversionFactor;
                        composProfile.tw1 *= conversionFactor;
                        composProfile.tw2 *= conversionFactor;
                    }
                        
                    switch (ComposGH.Parameters.Units.LengthSection)
                    {
                        case "mm":
                            composProfile.sectUnit = ComposProfile.SectUnitOptions.u_mm;
                            break;
                        case "cm":
                            composProfile.sectUnit = ComposProfile.SectUnitOptions.u_cm;
                            break;
                        case "m":
                            composProfile.sectUnit = ComposProfile.SectUnitOptions.u_m;
                            break;
                        case "in":
                            composProfile.sectUnit = ComposProfile.SectUnitOptions.u_in;
                            break;
                        case "ft":
                            composProfile.sectUnit = ComposProfile.SectUnitOptions.u_ft;
                            break;
                    }
                }
            }
            return composProfile;
        }
        /// <summary>
        /// Method to convert a GsaProfile to a string that can be read by GSA
        /// (in GsaAPI.Section.Profile or GhSA.Parameters.GsaSection.Section.Profile)
        /// 
        /// NOTE: 
        /// - Does not cover all profile types available in GSA (but all available in GsaProfile)
        /// - Geometric can handle custom profiles with voids. Origin/anchor to be implemented.
        /// - Catalogue profiles yet to be implemented
        /// </summary>
        /// <param name="composProfile"></param>
        /// <returns></returns>
        public static string ProfileConversion(ComposProfile composProfile)
        {
            if (composProfile.profileType == ComposProfile.ProfileTypes.Standard)
            {
                string unit = " ";

                switch (composProfile.sectUnit)
                {
                    case ComposProfile.SectUnitOptions.u_cm:
                        unit = "(cm) ";
                        break;
                    case ComposProfile.SectUnitOptions.u_m:
                        unit = "(m) ";
                        break;
                    case ComposProfile.SectUnitOptions.u_in:
                        unit = "(in) ";
                        break;
                    case ComposProfile.SectUnitOptions.u_ft:
                        unit = "(ft) ";
                        break;
                }


                if (composProfile.stdShape == ComposProfile.StdShapeOptions.Rectangle)
                {
                    if (composProfile.isTapered)
                    {
                        return "STD TR" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.b2.ToString("0.############");
                    }
                    else
                    {
                        if (composProfile.isHollow)
                        {
                            return "STD RHS" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.tw1.ToString("0.############") + " " + composProfile.tf1.ToString("0.############");
                        }
                        else
                        {
                            return "STD R" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############");
                        }
                    }
                }
                else if (composProfile.stdShape == ComposProfile.StdShapeOptions.Circle)
                {
                    if (composProfile.isHollow)
                    {
                        if (composProfile.isElliptical)
                        {
                            return "STD OVAL" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.tw1.ToString("0.############");
                        }
                        else
                        {
                            return "STD CHS" + unit + composProfile.d.ToString("0.############") + " " + composProfile.tw1.ToString("0.############");
                        }
                    }
                    else
                    {
                        if (composProfile.isElliptical)
                        {
                            return "STD E" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " 2";
                        }
                        else
                        {
                            return "STD C" + unit + composProfile.d.ToString("0.############");
                        }
                    }
                }
                else if (composProfile.stdShape == ComposProfile.StdShapeOptions.I_section)
                {
                    if (composProfile.isGeneral)
                    {
                        if (composProfile.isTapered)
                        {
                            return "STD TI" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.b2.ToString("0.############") + " "
                                + composProfile.tw1.ToString("0.############") + " " + composProfile.tw2.ToString("0.############") + " " + composProfile.tf1.ToString("0.############") + " " + composProfile.tf2.ToString("0.############");
                        }
                        else
                        {
                            return "STD GI" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.b2.ToString("0.############") + " "
                                + composProfile.tw1.ToString("0.############") + " " + composProfile.tf1.ToString("0.############") + " " + composProfile.tf2.ToString("0.############");
                        }
                    }
                    else
                    {
                        return "STD I" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.tw1.ToString("0.############") + " " + composProfile.tf1.ToString("0.############");
                    }
                }
                else if (composProfile.stdShape == ComposProfile.StdShapeOptions.Tee)
                {
                    if (composProfile.isTapered)
                    {
                        return "STD TT" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.tw1.ToString("0.############") + " "
                            + composProfile.tw2.ToString("0.############") + " " + composProfile.tf1.ToString("0.############");
                    }
                    else
                    {
                        return "STD T" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.tw1.ToString("0.############") + " " + composProfile.tf1.ToString("0.############");
                    }
                }

                else if (composProfile.stdShape == ComposProfile.StdShapeOptions.Channel)
                {
                    if (composProfile.isB2B)
                    {
                        return "STD DCH" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.tw1.ToString("0.############") + " " + composProfile.tf1.ToString("0.############");
                    }
                    else
                    {
                        return "STD CH" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.tw1.ToString("0.############") + " " + composProfile.tf1.ToString("0.############");
                    }
                }

                else if (composProfile.stdShape == ComposProfile.StdShapeOptions.Angle)
                {
                    if (composProfile.isB2B)
                    {
                        return "STD D" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.tw1.ToString("0.############") + " " + composProfile.tf1.ToString("0.############");
                    }
                    else
                    {
                        return "STD A" + unit + composProfile.d.ToString("0.############") + " " + composProfile.b1.ToString("0.############") + " " + composProfile.tw1.ToString("0.############") + " " + composProfile.tf1.ToString("0.############");
                    }
                }
                else
                {
                    return "STD something else";
                }
            }
            else if (composProfile.profileType == ComposProfile.ProfileTypes.Catalogue)
            {
                string outputSectionString = "";

                if(composProfile.catalogueProfileName !=null)
                {
                    outputSectionString = composProfile.catalogueProfileName.ToString();
                }
                
                return $"CAT {outputSectionString}"; 
            }
            else if (composProfile.profileType == ComposProfile.ProfileTypes.Geometric)
            {
                if (composProfile.geoType == ComposProfile.GeoTypes.Perim)
                {
                    string unit = "";

                    switch (composProfile.sectUnit)
                    {
                        case ComposProfile.SectUnitOptions.u_cm:
                            unit = "(cm)";
                            break;
                        case ComposProfile.SectUnitOptions.u_m:
                            unit = "(m)";
                            break;
                        case ComposProfile.SectUnitOptions.u_in:
                            unit = "(in)";
                            break;
                        case ComposProfile.SectUnitOptions.u_ft:
                            unit = "(ft)";
                            break;
                    }

                    var profile = "GEO P" + unit;
                    var iPoint = 0;
                    foreach (Point2d point in composProfile.perimeterPoints)
                    {
                        if ((iPoint > 0))
                            profile += " L";
                        else
                            profile += " M";

                        profile += ("("
                                    + (point.X + ("|"
                                    + (point.Y + ")"))));
                        iPoint++;
                    }
                    if (!(composProfile.voidPoints == null || !(composProfile.voidPoints.Count > 0)))
                    {
                        for (int i = 0; i < composProfile.voidPoints.Count; i++)
                        {
                            iPoint = 0;
                            foreach (Point2d point in composProfile.voidPoints[i])
                            {
                                if ((iPoint > 0))
                                    profile += " L";
                                else
                                    profile += " M";

                                profile += ("("
                                            + (point.X + ("|"
                                            + (point.Y + ")"))));
                                iPoint++;
                            }
                        }
                    }

                    return profile;
                }
                return "GEO";
            }
            else
                return null;
        }
    }
}
