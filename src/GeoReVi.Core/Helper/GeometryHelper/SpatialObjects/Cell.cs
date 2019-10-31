﻿using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace GeoReVi
{
    /// <summary>
    /// A cell class for meshes
    /// </summary>
    [XmlRoot(Namespace = "GeoReVi")]
    [XmlInclude(typeof(Hexahedron))]
    [XmlInclude(typeof(Tetrahedron))]
    public abstract class Cell : ICell, IEquatable<Cell>
    {
        #region Public properties

        /// <summary>
        /// Adjacent cells
        /// </summary>
        private List<Cell> adjacent = new List<Cell>();
        public List<Cell> Adjacent
        {
            get => this.adjacent;
            set
            {
                this.adjacent = value;
            }
        }

        /// <summary>
        /// Children of the cell
        /// </summary>
        private List<Cell> children = new List<Cell>();
        public List<Cell> Children
        {
            get => this.children;
            set => this.children = value;
        }

        /// <summary>
        /// Checks if the cell has children
        /// </summary>
        private bool hasChildren = false;
        public bool HasChildren
        {
            get
            {
                if (this.Children != null)
                    return this.Children.Count > 0;
                else
                    return false;
            }
        }

        /// <summary>
        /// Checks if the cell is active
        /// </summary>
        private bool isActive = true;
        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }


        /// <summary>
        /// Faces of the grid cell
        /// </summary>
        private List<Face> faces = new List<Face>();
        [XmlIgnore]
        public List<Face> Faces
        {
            get => this.faces;
            set => this.faces = value;
        }

        /// <summary>
        /// Vertices of the grid cell
        /// </summary>
        private List<LocationTimeValue> vertices = new List<LocationTimeValue>();
        public List<LocationTimeValue> Vertices
        {
            get => this.vertices;
            set
            {
                this.vertices = value;
            }
        }

        /// <summary>
        /// Type of the cell
        /// </summary>
        private CellType cellType = CellType.Tetrahedon;
        public CellType CellType
        {
            get => this.cellType;
            set => this.cellType = value;
        }

        #endregion

        #region Constructors

        #endregion

        #region Public methods

        /// <summary>
        /// Equals method
        /// </summary>
        public bool Equals(Cell cell)
        {
            return this.Adjacent == cell.Adjacent &&
                   this.Children == cell.Children &&
                   this.HasChildren == cell.HasChildren &&
                   this.IsActive == cell.IsActive &&
                   this.Faces == cell.Faces &&
                   this.Vertices == cell.Vertices;
        }

        /// <summary>
        /// Helper function to get the position of the i-th vertex.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Position of the i-th vertex</returns>
        public virtual Point3D GetPosition(int i)
        {
            return Vertices[i].ToPoint3D();
        }

        public virtual void MakeFace(int i, int j, int k, Vector3D center, Int32Collection indices)
        {

        }

        /// <summary>
        /// Creates a model of the tetrahedron. Transparency is applied to the color.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <returns>A model representing the tetrahedron</returns>
        public virtual void CreateFaces()
        {

        }

        /// <summary>
        /// Calculates the volume of the element
        /// </summary>
        /// <returns></returns>
        public virtual double GetVolume()
        {
            return 0;
        }

        /// <summary>
        /// Getting the element stiffness matrix necessary for finite element simulations
        /// </summary>
        /// <returns></returns>
        public virtual double[,] GetElementStiffnessMatrix()
        {
            return new double[,] { };
        }
        #endregion
    }
}
