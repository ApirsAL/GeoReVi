namespace GeoReVi
{
    /// <summary>
    /// Types of boundary conditions for solving DEs
    /// </summary>
    public enum BoundaryConditionType
    {
        None = 1,
        Dirichlet = 2,
        Neumann = 3,
        Robin = 4,
        Source = 5
    }
}
