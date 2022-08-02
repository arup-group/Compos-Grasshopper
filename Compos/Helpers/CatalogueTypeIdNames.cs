﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComposAPI.Helpers
{
  public class CatalogueSectionType
  {
    public static Dictionary<int, string> CatalogueSectionTypes
    {
      get 
      {
        if (m_CatalogueSectionTypesIDs == null)
        {
          m_CatalogueSectionTypesIDs = new Dictionary<int, string>();
          List<string> cats = AcceptedIDs().Split(' ').ToList();
          foreach (string cat in cats)
          {
            int id = int.Parse(cat);
            string name = AllCatalogueSectionTypesIDs[id];
            string[] temp = name.Split(new string[] { " -- " }, StringSplitOptions.None);
            int catID = int.Parse(temp[0]);
            string catName = AllCatalogueNameIDs[catID];
            string catalogueTypeName = 
              catName + ":" + 
              temp[1] + 
              (temp[2] == "1" ? " - s/s" : "");
            m_CatalogueSectionTypesIDs.Add(id, catalogueTypeName);
          }
        }
        return m_CatalogueSectionTypesIDs; 
      }
    }
    private static Dictionary<int, string> m_CatalogueSectionTypesIDs = null;

    // generated by exporting from Compos to .coa selecting all available catalogues in design criteria
    private static string AcceptedIDs()
    {
      return "1 2 3 4 26 27 28 29 30 31 32 33 34 35 36 37 38 50 51 52 53 54 55 56 69 70 71 72 73 91 93 94 95 99 100 101 102 103 112 113 114 126 127 128 129 142 143 144 151 152 153 154 155 166 167 168 174 175 176 177 178 179 202 203 204 210 211 212 213 224 225 226 227 228 235 242 243 244 248 249 250 251 252 253 254 255 256 257 258 264 265 266 267 268 269 270 271 272 273 274 275 286 287 288";
    }

    // generated with SQLite using command:
    // Select '{ ' || CAT_NUM || ', "' || CAT_NAME  || '" },' as CAT_NAME from Catalogues
    private static readonly Dictionary<int, string> AllCatalogueNameIDs = new Dictionary<int, string>()
    {
      { 2, "British" },
{ 3, "Europrofile" },
{ 4, "AISC" },
{ 5, "Australian" },
{ 6, "Chinese" },
{ 7, "Indian" },
{ 8, "Corus Advance (TATA)" },
{ 9, "Russian" },
{ 10, "Korean" },
{ 11, "Japanese" },
{ 12, "EN 10210" },
{ 13, "EN 10219" },
{ 14, "South African" },
{ 15, "ASTM A501" },
{ 16, "CISC" },
{ 17, "British Steel" },
{ 18, "ArcelorMittal" },
    };

    // generated with SQLite using command:
    // Select '{ ' || TYPE_NUM || ', "' || TYPE_CAT_NUM || ' -- ' ||TYPE_NAME || ' -- ' || (TYPE_SUPERSEDED = True or TYPE_SUPERSEDED = TRUE or TYPE_SUPERSEDED = 1) || '" },' as CAT_NAME from Types
    private static readonly Dictionary<int, string> AllCatalogueSectionTypesIDs = new Dictionary<int, string>()
    {
      { 1, "2 -- Universal Beams (BS4-1:1980) -- 1" },
{ 2, "2 -- Universal Columns (BS4-1:1980) -- 1" },
{ 3, "2 -- Joists (BS4-1:1980) -- 1" },
{ 4, "2 -- Univ. Bearing Piles (BS4-1:1980) -- 1" },
{ 5, "2 -- Circ. Hollow Sects (BS4848- 2:1991) -- 1" },
{ 6, "2 -- Sq. Hollow Sections (BS4848-2:1991) -- 1" },
{ 7, "2 -- Rect. Hollow Sects (BS4848-2:1991) -- 1" },
{ 8, "2 -- Channels (BS4-1:1980) -- 1" },
{ 9, "2 -- Equal Angles (BS4848-4:1972) -- 1" },
{ 10, "2 -- Unequal Angles (BS4848-4:1972) -- 1" },
{ 11, "2 -- Castellated Ubs (BS4-1:1980) -- 1" },
{ 12, "2 -- BS Castellated UCs -- 1" },
{ 13, "2 -- BS Castellated Joists -- 1" },
{ 14, "2 -- UB Tee (BS4-1:1980) -- 1" },
{ 15, "2 -- UC Tee (BS4-1:1980) -- 1" },
{ 26, "3 -- EP IPE beams -- 0" },
{ 27, "3 -- EP HE beams -- 0" },
{ 28, "3 -- EP HL beams -- 0" },
{ 29, "3 -- EP HX beams -- 0" },
{ 30, "3 -- EP HD columns - s/s -- 1" },
{ 31, "3 -- EP: US HP bearing piles -- 0" },
{ 32, "3 -- EP: Japanese I shapes -- 0" },
{ 33, "3 -- EP: US I shapes: M -- 0" },
{ 34, "3 -- EP: US I shapes: W -- 0" },
{ 35, "4 -- AISC W shapes -- 1" },
{ 36, "4 -- AISC M shapes -- 1" },
{ 37, "4 -- AISC S shapes -- 1" },
{ 38, "4 -- AISC HP shapes -- 1" },
{ 39, "4 -- AISC Channels -- 1" },
{ 40, "4 -- AISC Misc. Channels -- 1" },
{ 41, "4 -- AISC W Tee -- 1" },
{ 42, "4 -- AISC M Tee -- 1" },
{ 43, "4 -- AISC S Tee -- 1" },
{ 44, "4 -- AISC Angles -- 1" },
{ 45, "4 -- AISC Double Angles -- 1" },
{ 46, "4 -- AISC Structural Tubing -- 1" },
{ 47, "4 -- AISC Pipe (Standard) -- 1" },
{ 48, "4 -- AISC Pipe (Extra str.) -- 1" },
{ 49, "4 -- AISC Pipe (Dbl.Extra) -- 1" },
{ 50, "3 -- EP HD columns -- 0" },
{ 51, "5 -- Universal Beams -- 0" },
{ 52, "5 -- Universal Columns -- 0" },
{ 53, "5 -- Welded Beams -- 0" },
{ 54, "5 -- Welded Columns -- 0" },
{ 55, "5 -- Universal Bearing Piles -- 0" },
{ 56, "5 -- Taper Flange Beams -- 0" },
{ 57, "5 -- Parallel Flange Channels -- 0" },
{ 58, "5 -- Equal Angles -- 0" },
{ 59, "5 -- Unequal Angles -- 0" },
{ 120, "5 -- Circ Hollow (G250) -- 0" },
{ 121, "5 -- Circ Hollow (G350) -- 0" },
{ 122, "5 -- Rect Hollow (G350) -- 0" },
{ 123, "5 -- Rect Hollow (G450) -- 0" },
{ 124, "5 -- Square Hollow (G350) -- 0" },
{ 125, "5 -- Square Hollow (G450) -- 0" },
{ 66, "5 -- Flats -- 0" },
{ 67, "5 -- Rounds -- 0" },
{ 68, "5 -- Squares -- 0" },
{ 69, "5 -- RSJs -- 0" },
{ 70, "2 -- Universal Beams (BS4-1:2005) -- 1" },
{ 71, "2 -- Universal Columns (BS4-1:2005) -- 1" },
{ 72, "2 -- Joists (BS4-1:2005) -- 1" },
{ 73, "2 -- Univ. Bearing Piles (BS4-1:2005) -- 1" },
{ 74, "2 -- Circ. Hollow Sects (EN10210-2:1997) -- 1" },
{ 75, "2 -- Square Hollow Sects (EN10210-2:1997) -- 1" },
{ 76, "2 -- Rect. Hollow Sects (EN10210-2:1997) -- 1" },
{ 77, "2 -- Channels (BS4-1:1993) -- 1" },
{ 78, "2 -- Equal Angles (BS4848-4:1972) -- 1" },
{ 79, "2 -- Unequal Angles (BS4848-4:1972) -- 1" },
{ 80, "2 -- Castellated UBs (BS4-1:1993) -- 1" },
{ 81, "2 -- Castellated UCs (BS4-1:1993) -- 1" },
{ 82, "2 -- Castellated Joists (BS4-1:1993) -- 1" },
{ 83, "2 -- UB Tee (BS4-1:2005) -- 1" },
{ 84, "2 -- UC Tee (BS4-1:2005) -- 1" },
{ 85, "2 -- Two Channels Laced (BS4-1:1993) -- 1" },
{ 86, "2 -- Two Channels Back to Back (BS4-1:1993) -- 1" },
{ 87, "2 -- Equal Angles Back2b (BS4848-4:1972) -- 1" },
{ 88, "2 -- Unequal Angles Short Back2b (BS4848-4:1972) -- 1" },
{ 89, "2 -- Unequal Angles Long Back2b (BS4848-4:1972) -- 1" },
{ 90, "2 -- Parallel Flange Channel (BS4-1:2005) -- 1" },
{ 91, "2 -- Universal Beams (Blue Book, 6th Edition) -- 1" },
{ 92, "2 -- Celsius 355 Ovals (EN10210) -- 1" },
{ 93, "6 -- Wide Flange Beams (GB/T 11263-1998) -- 0" },
{ 94, "6 -- Medium Flange Beams (GB/T 11263-1998) -- 0" },
{ 95, "6 -- Narrow Flange Beams (GB/T 11263-1998) -- 0" },
{ 96, "2 -- Celsius Large Circular Hollow Sects (EN10210) -- 1" },
{ 97, "7 -- Rolled Steel Equal Angles -- 0" },
{ 98, "7 -- Rolled Steel Unequal Angles -- 0" },
{ 99, "7 -- Rolled Steel Beams: ISJB -- 0" },
{ 100, "7 -- Rolled Steel Beams: ISLB -- 0" },
{ 101, "7 -- Rolled Steel Beams: ISMB -- 0" },
{ 102, "7 -- Rolled Steel Beams: ISWB -- 0" },
{ 103, "7 -- Rolled Steel Beams: ISHB -- 0" },
{ 104, "7 -- Rolled Steel Channels: ISJC -- 0" },
{ 105, "7 -- Rolled Steel Channels: ISLC -- 0" },
{ 106, "7 -- Rolled Steel Channels: ISMC -- 0" },
{ 107, "7 -- Rolled Steel Tee Bars: ISNT -- 0" },
{ 108, "7 -- Rolled Steel Tee Bars: ISHT -- 0" },
{ 109, "7 -- Rolled Steel Tee Bars: ISST -- 0" },
{ 110, "7 -- Rolled Steel Tee Bars: ISLT -- 0" },
{ 111, "7 -- Rolled Steel Tee Bars: ISJT -- 0" },
{ 112, "8 -- Advance UKB (DRAFT 2006) -- 1" },
{ 113, "8 -- Advance UKC (DRAFT 2006) -- 1" },
{ 114, "8 -- Advance UKBP (DRAFT 2006) -- 1" },
{ 115, "8 -- Advance UKPFC (DRAFT 2006) -- 1" },
{ 116, "8 -- Advance UKA - Equal (DRAFT 2006) -- 1" },
{ 117, "8 -- Advance UKA - Unequal (DRAFT 2006) -- 1" },
{ 118, "8 -- Advance UKT split from UKB (DRAFT 2006) -- 1" },
{ 119, "8 -- Advance UKT split from UKC (DRAFT 2006) -- 1" },
{ 126, "4 -- AISC W shapes (v13.1: 2005) -- 0" },
{ 127, "4 -- AISC M shapes (v13.1: 2005) -- 0" },
{ 128, "4 -- AISC S shapes (v13.1: 2005) -- 0" },
{ 129, "4 -- AISC HP shapes (v13.1: 2005) -- 0" },
{ 130, "4 -- AISC Channels (v13.1: 2005) -- 0" },
{ 131, "4 -- AISC Misc. Channels (v13.1: 2005) -- 0" },
{ 132, "4 -- AISC Angles (v13.1: 2005) -- 0" },
{ 133, "4 -- AISC W Tee (v13.1: 2005) -- 0" },
{ 134, "4 -- AISC M Tee (v13.1: 2005) -- 0" },
{ 135, "4 -- AISC S Tee (v13.1: 2005) -- 0" },
{ 136, "4 -- AISC Double Angles (v13.1: 2005) -- 0" },
{ 137, "4 -- AISC Rectangular Hollow Sections-ASTM A500 (v13.1: 2005) -- 0" },
{ 138, "4 -- AISC Circular Hollow Sections-ASTM A500 (v13.1: 2005) -- 0" },
{ 139, "4 -- AISC Pipe (Standard)-ASTM A53 (v13.1: 2005) -- 0" },
{ 140, "4 -- AISC Pipe (Extra str.)-ASTM A53 (v13.1: 2005) -- 0" },
{ 141, "4 -- AISC Pipe (Dbl.Extra)-ASTM A53 (v13.1: 2005) -- 0" },
{ 142, "9 -- Regular I-beams (STO ASChM 20-93) -- 0" },
{ 143, "9 -- Broad-flanged I-beams (STO ASChM 20-93) -- 0" },
{ 144, "9 -- Column I-beams (STO ASChM 20-93) -- 0" },
{ 145, "9 -- Channels (GOST 8240-97) -- 0" },
{ 146, "9 -- Equal Angles (GOST 8510-83) -- 0" },
{ 147, "9 -- Unequal Angles (GOST 8510-83) -- 0" },
{ 148, "9 -- Sq. Hollow Sections (GOST 30245-2003) -- 0" },
{ 149, "9 -- Rect. Hollow Sections(GOST 30245-2003) -- 0" },
{ 150, "9 -- Circ. Hollow Sects (GOST 8732-78) -- 0" },
{ 151, "10 -- Wide Flange Shapes -- 0" },
{ 152, "10 -- H-Bearing Piles -- 0" },
{ 153, "10 -- Junior Beams -- 0" },
{ 154, "10 -- I-Beams -- 0" },
{ 155, "10 -- I Beams for Mine Support -- 0" },
{ 156, "10 -- Channel Sections -- 0" },
{ 157, "10 -- Parallel Flange Channel Sections -- 0" },
{ 158, "10 -- Structural Tees -- 0" },
{ 159, "10 -- Equal Angle Sections -- 0" },
{ 160, "10 -- Unequal Angle Sections -- 0" },
{ 161, "10 -- Inverted Angle Sections -- 0" },
{ 162, "9 -- Circ. Hollow Sects (GOST 10704-91) -- 0" },
{ 163, "9 -- Parallel Flange Channels (GOST 8240-97) -- 0" },
{ 164, "9 -- Efficient Parallel Flange Channels (GOST 8240-97) -- 0" },
{ 165, "9 -- Light Parallel Flange Channels (GOST 8240-97) -- 0" },
{ 166, "8 -- Advance UKB -- 1" },
{ 167, "8 -- Advance UKC -- 1" },
{ 168, "8 -- Advance UKBP -- 1" },
{ 169, "8 -- Advance UKPFC -- 1" },
{ 170, "8 -- Advance UKA - Equal -- 1" },
{ 171, "8 -- Advance UKA - Unequal -- 1" },
{ 172, "8 -- Advance UKT split from UKB -- 1" },
{ 173, "8 -- Advance UKT split from UKC -- 1" },
{ 174, "11 -- Hyper Beams -- 0" },
{ 175, "11 -- Conv. H-Shapes (Heavy weight - 500mm Series) -- 0" },
{ 176, "11 -- Conv. H-Shapes (Heavy weight - 400mm Series) -- 0" },
{ 177, "11 -- Conv. H-Shapes (Lightweight - Wide) -- 0" },
{ 178, "11 -- Conv. H-Shapes (Lightweight - Medium) -- 0" },
{ 179, "11 -- Conv. H-Shapes (Lightweight - Narrow) -- 0" },
{ 180, "11 -- Square Hollow Sects (JIS G 3466) -- 0" },
{ 181, "11 -- Rect. Hollow Sects (JIS G 3466) -- 0" },
{ 182, "11 -- Circ. Hollow Sects (JIS G 3444) -- 0" },
{ 183, "11 -- Circ. Hollow Sects (JIS G 3475-1) -- 0" },
{ 184, "11 -- Circ. Hollow Sects (JIS G 3475-2) -- 0" },
{ 185, "11 -- Equal Angles (JIS G 3192) -- 0" },
{ 186, "11 -- Unequal Angles (JIS G 3192) -- 0" },
{ 187, "12 -- CHS (EN10210-2:2006) -- 0" },
{ 188, "12 -- SHS (EN10210-2:2006) -- 0" },
{ 189, "12 -- RHS (EN10210-2:2006) -- 0" },
{ 190, "12 -- EHS (EN10210-2:2006) -- 0" },
{ 191, "12 -- Celsius 355 CHS (EN10210-2:2006) - Corus -- 0" },
{ 192, "12 -- Celsius LCHS (EN10210-2:2006) - Corus -- 0" },
{ 193, "12 -- Celsius OVAL (EN10210-2:2006) - Corus -- 0" },
{ 194, "12 -- Celsius 355 SHS (EN10210-2:2006) - Corus -- 0" },
{ 195, "12 -- Celsius 355 RHS (EN10210-2:2006) - Corus -- 0" },
{ 196, "13 -- Hybox 355 CHS (EN10219:2006) - Corus -- 0" },
{ 197, "13 -- Hybox 355 SHS (EN10219:2006) - Corus -- 0" },
{ 198, "13 -- Hybox 355 RHS (EN10219:2006) - Corus -- 0" },
{ 199, "13 -- Strongbox 235 CHS (EN10219:2006) - Corus -- 0" },
{ 200, "13 -- Strongbox 235 SHS (EN10219:2006) - Corus -- 0" },
{ 201, "13 -- Strongbox 235 RHS (EN10219:2006) - Corus -- 0" },
{ 202, "8 -- Advance UKB -- 1" },
{ 203, "8 -- Advance UKC -- 1" },
{ 204, "8 -- Advance UKBP -- 1" },
{ 205, "8 -- Advance UKPFC -- 1" },
{ 206, "8 -- Advance UKA - Equal -- 1" },
{ 207, "8 -- Advance UKA - Unequal -- 1" },
{ 208, "8 -- Advance UKT split from UKB -- 1" },
{ 209, "8 -- Advance UKT split from UKC -- 1" },
{ 210, "14 -- Universal Beams (BS4: Part 1 : 1993) -- 0" },
{ 211, "14 -- IPE Sections (DIN 1025 : 1965) -- 0" },
{ 212, "14 -- IPE-AA Sections (Internal Spec SPE 230) -- 0" },
{ 213, "14 -- Universal Columns (BS4: Part 1 : 1993) -- 0" },
{ 214, "14 -- Taper Flange Channels (BS4: Part 1 : 1993) -- 0" },
{ 215, "14 -- Parallel Flange Channels (EN 10279) -- 0" },
{ 216, "14 -- Equal Angles (EN 10056) -- 0" },
{ 217, "14 -- Unequal Angles (EN 10056) -- 0" },
{ 218, "14 -- Rectangular Hollow Sections (ISO 657-14) -- 0" },
{ 219, "14 -- Square Hollow Sections (ISO 657-14) -- 0" },
{ 220, "14 -- Circular Hollow Sections (ISO 657-14) -- 0" },
{ 221, "15 -- Celsius Rectangular HSS (EN10210-2:2006)- Corus -- 0" },
{ 222, "15 -- Celsius Square HSS (EN10210-2:2006)- Corus -- 0" },
{ 223, "15 -- Celsius Circular HSS (EN10210-2:2006)- Corus -- 0" },
{ 224, "16 -- CISC W Shapes -- 0" },
{ 225, "16 -- CISC S Shapes -- 0" },
{ 226, "16 -- CISC M Shapes -- 0" },
{ 227, "16 -- CISC HP Shapes -- 0" },
{ 228, "16 -- CISC WWF Shapes  -- 0" },
{ 229, "16 -- CISC Channels Section -- 0" },
{ 230, "16 -- CISC Miscellaneous Channels -- 0" },
{ 231, "16 -- CISC Single Angle -- 0" },
{ 232, "16 -- CISC Angle Back to Back -- 0" },
{ 233, "16 -- CISC WT Shapes -- 0" },
{ 234, "16 -- CISC WWT Shapes -- 0" },
{ 235, "16 -- CISC SLB Section -- 0" },
{ 236, "16 -- CISC Rect HSS (G40) -- 0" },
{ 237, "16 -- CISC Square HSS (G40) -- 0" },
{ 238, "16 -- CISC Circ. HSS (G40) -- 0" },
{ 239, "16 -- CISC Rect HSS (ASTM A500) -- 0" },
{ 240, "16 -- CISC Square HSS (ASTM A500) -- 0" },
{ 241, "16 -- CISC Circ. HSS (ASTM A500) -- 0" },
{ 242, "17 -- Universal Beam(UB) -- 0" },
{ 243, "17 -- Universal Columns (UC) -- 0" },
{ 244, "17 -- Universal Bearing Piles (UBP) -- 0" },
{ 245, "17 -- Parallel Flange Channels (PFC) -- 0" },
{ 246, "17 -- Equal Angles (EA) -- 0" },
{ 247, "17 -- Unequal Angles (UA) -- 0" },
{ 248, "2 -- Parallel flange I sections(IPE) - EN10365:2017 -- 0" },
{ 249, "2 -- Wide flange beams(HE) - EN10365:2017 -- 0" },
{ 250, "2 -- Extra wide flange beams(HL) - EN10365:2017 -- 0" },
{ 251, "2 -- Extra wide flange beams(HLZ) - EN10365:2017 -- 0" },
{ 252, "2 -- Wide flange columns(HD) - EN10365:2017 -- 0" },
{ 253, "2 -- Wide flange bearing piles(HP) - EN10365:2017 -- 0" },
{ 254, "2 -- Wide flange bearing piles(UBP) - EN10365:2017 -- 0" },
{ 255, "2 -- Universal beams(UB) - EN10365:2017 -- 0" },
{ 256, "2 -- Universal columns(UC) - EN10365:2017 -- 0" },
{ 257, "2 -- Taper flange I sections(IPN) - EN10365:2017 -- 0" },
{ 258, "2 -- Taper flange I sections(J) - EN10365:2017 -- 0" },
{ 259, "2 -- Parallel flange channels(UPE) - EN10365:2017 -- 0" },
{ 260, "2 -- Parallel flange channels(PFC) - EN10365:2017 -- 0" },
{ 261, "2 -- Taper flange channels(UPN) - EN10365:2017 -- 0" },
{ 262, "2 -- Taper flange channels(U) - EN10365:2017 -- 0" },
{ 263, "2 -- Taper flange channels(CH) - EN10365:2017 -- 0" },
{ 264, "18 -- AM Parallel flange I section(IPE) -- 0" },
{ 265, "18 -- AM Wide flange beams(HE) -- 0" },
{ 266, "18 -- AM Extra wide flange beams(HL) -- 0" },
{ 267, "18 -- AM Extra wide flange beams-HLZ -- 0" },
{ 268, "18 -- AM Wide flange beams(HD) -- 0" },
{ 269, "18 -- AM Wide flange bearing piles(HP) -- 0" },
{ 270, "18 -- AM Wide flange bearing piles(UBP) -- 0" },
{ 271, "18 -- AM Universal beams(UB) -- 0" },
{ 272, "18 -- AM Universal Columns(UC) -- 0" },
{ 273, "18 -- AM American wide flange beams(W) -- 0" },
{ 274, "18 -- AM Americal wide flange bearing piles(HP) -- 0" },
{ 275, "18 -- AM Russian hot rolled beams(HG) -- 0" },
{ 276, "18 -- AM Parallel flange channels(UPE) -- 0" },
{ 277, "18 -- AM Parallel flange channels(PFC) -- 0" },
{ 278, "18 -- AM Taper flange channels(UPN) -- 0" },
{ 279, "18 -- AM Taper flange channels(UE) -- 0" },
{ 280, "18 -- AM American standard channels(C) -- 0" },
{ 281, "18 -- AM American channels(MC) -- 0" },
{ 282, "18 -- AM Equal leg angles(L) -- 0" },
{ 283, "18 -- AM American equal leg angles(L) -- 0" },
{ 284, "18 -- AM Unequal leg angles(L) -- 0" },
{ 285, "18 -- AM American Unequal leg angles(L) -- 0" },
{ 286, "18 -- AM Taper flange I sections(IPN) -- 0" },
{ 287, "18 -- AM Taper flange I sections(J) -- 0" },
{ 288, "18 -- AM American standard beams(S) -- 0" },
    };
  }
}