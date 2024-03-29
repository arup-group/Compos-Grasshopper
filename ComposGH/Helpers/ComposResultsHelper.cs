﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ComposGH.Helpers {
  internal static class ComposResultHelper {

    internal static double RoundToSignificantDigits(double d, int digits) {
      if (d == 0.0) {
        return 0.0;
      } else {
        double leftSideNumbers = Math.Floor(Math.Log10(Math.Abs(d))) + 1;
        double scale = Math.Pow(10, leftSideNumbers);
        double result = scale * Math.Round(d / scale, digits, MidpointRounding.AwayFromZero);

        // Clean possible precision error.
        if ((int)leftSideNumbers >= digits) {
          return Math.Round(result, 0, MidpointRounding.AwayFromZero);
        } else {
          if (Math.Abs(digits - (int)leftSideNumbers) > 15) {
            return 0.0;
          }
          return Math.Round(result, digits - (int)leftSideNumbers, MidpointRounding.AwayFromZero);
        }
      }
    }

    internal static List<double> SmartRounder(double max, double min) {
      // list to hold output values
      var roundedvals = new List<double>();

      // check if both are zero then return
      if (max == 0 & min == 0) {
        roundedvals.Add(max);
        roundedvals.Add(min);
        roundedvals.Add(0);
        return roundedvals;
      }
      int signMax = Math.Sign(max);
      int signMin = Math.Sign(min);

      int significantNumbers = 2;

      // find the biggest abs value of max and min
      double val = Math.Max(Math.Abs(max), Math.Abs(min));
      max = Math.Abs(max);
      min = Math.Abs(min);

      // a value for how to round the values on the legend
      int numberOfDigitsOut = significantNumbers;
      //factor for scaling small numbers (0.00012312451)
      double factor = 1;
      if (val < 1) {
        // count the number of zeroes after the decimal point
        string valString = val.ToString().Split('.')[1];
        int digits = 0;
        while (valString[digits] == '0') {
          digits++;
        }
        // create the factor, we want to remove the zeroes as well as making it big enough for rounding
        factor = Math.Pow(10, digits + 1);
        // scale up max/min values
        max *= factor;
        min *= factor;
        max = Math.Ceiling(max);
        min = Math.Floor(min);
        max /= factor;
        min /= factor;
        numberOfDigitsOut = digits + significantNumbers;
      } else {
        string valString = val.ToString();
        // count the number of digits before the decimal point
        int digits = valString.Split('.')[0].Count();
        // create the factor, we want to remove the zeroes as well as making it big enough for rounding
        int power = 10;
        if (val < 500) {
          power = 5;
        }

        factor = Math.Pow(power, digits - 1);
        // scale up max/min values
        max /= factor;
        min /= factor;
        max = Math.Ceiling(max);
        min = Math.Floor(min);
        max *= factor;
        min *= factor;
        numberOfDigitsOut = significantNumbers;
      }

      roundedvals.Add(max * signMax);
      roundedvals.Add(min * signMin);
      roundedvals.Add(numberOfDigitsOut);

      return roundedvals;
    }
  }
}
