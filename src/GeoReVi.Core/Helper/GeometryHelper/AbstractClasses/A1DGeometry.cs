
namespace GeoReVi
{
    public abstract class A1DGeometry : BasePropertyChanged, IGeometry
    {
        public A1DGeometry()
        {
        }

        public Dimensionality Dimensionality { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
