namespace ComposGH.Components.Ribbon {
  /// <summary>
  /// Class containing ribbon category names
  ///
  /// Call this class from all components to pick which category component should sit in
  ///
  /// Sorting of categories in the ribbon is controlled with a number of spaces in front of the name
  /// to avoid naming each category with a number in front. Spaces will automatically be removed when displayed
  /// </summary>
  internal class SubCategoryName {
    public static string Cat0() {
      return new string(' ', 10) + "Model";
    }

    public static string Cat1() {
      return new string(' ', 9) + "Beam";
    }

    public static string Cat10() {
      return "Params";
    }

    public static string Cat2() {
      return new string(' ', 8) + "Studs";
    }

    public static string Cat3() {
      return new string(' ', 7) + "Slab";
    }

    public static string Cat4() {
      return new string(' ', 6) + "Loads";
    }

    public static string Cat5() {
      return new string(' ', 5) + "Member";
    }

    public static string Cat6() {
      return new string(' ', 4) + "Analyse";
    }

    public static string Cat7() {
      return new string(' ', 3) + "Results";
    }

    public static string Cat8() {
      return new string(' ', 2) + "Design";
    }

    public static string Cat9() {
      return new string(' ', 1) + "Results";
    }
  }
}
