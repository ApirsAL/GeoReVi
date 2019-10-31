using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeoReVi
{
    public static class UnitHelper
    {
        /// <summary>
        /// Returning a label with regard to the property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="additionalParameter"></param>
        /// <returns></returns>
        public static string GetPropertyAndUnit(string property, string additionalParameter = "")
        {
            switch (property)
            {
                case "Apparent permeability":
                    return "Apparent Permeability [mD] " + additionalParameter + "-direction";
                case "Isotope":
                    return additionalParameter;
                case "Intrinsic permeability":
                    return "Intrinsic Permeability [mD]";
                case "Bulk density":
                    return "Bulk density [g cm\x207B\xB3]";
                case "Porosity":
                    return "Porosity[%]";
                case "Grain density":
                    return "Grain density [g cm\x207B\xB3]";
                case "Thermal conductivity":
                    return "Thermal conducticity [W m\x207B\xB9 K\x207B\xB9]";
                case "Thermal diffusivity":
                    return "Thermal diffusivity [10\x207B\x2076 m\xB2 s\x207B\xB9]";
                case "Resistivity":
                    return "Resistivity [Ωm]";
                case "Sonic wave velocity":
                    return "Sonic wave velocity [m s\x207B\xB9] " + additionalParameter;
                case "X-Ray Fluorescence":
                    return additionalParameter;
                default:
                    return "";
            }
        }
    }
}