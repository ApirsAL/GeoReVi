using System.Collections.Generic;

namespace GeoReVi
{
    /// <summary>
    /// Cell interface
    /// </summary>
    public interface ICell
    {
        /// <summary>
        /// Adjacent cells
        /// </summary>
        List<Cell> Adjacent { get; set; }

        /// <summary>
        /// Children of the cell
        /// </summary>
        List<Cell> Children { get; set; }

        /// <summary>
        /// Checks if the cell has children
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        /// Checks if the cell is active
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Faces of the cell
        /// </summary>
        List<Face> Faces { get; set; }

        /// <summary>
        /// Vertices of the cell
        /// </summary>
        List<LocationTimeValue> Vertices { get; set; }

        /// <summary>
        /// Type of the cell
        /// </summary>
        CellType CellType { get; set; }

    }
}
