using System;
using System.Collections.Generic;
using MIConvexHull;

namespace GeoReVi
{
    /// <summary>
    /// An interface for a location with a generic value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILocationTimeValue : IVertex, IEquatable<LocationTimeValue>, ICloneable, IComparable<LocationTimeValue>
    {
        double X { get; set; }

        double Y { get; set; }

        double Z { get; set; }

        DateTime Date { get; set; }

        bool Geographic { get; set; }

        bool IsDiscretized { get; set; }

        List<double> Value { get; set; }

        string Name { get; set; }

        bool IsExterior { get; set; }

        List<LocationTimeValue> Neighbors { get; set; }
    }
}
