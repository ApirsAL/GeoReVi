
namespace GeoReVi
{
    /// <summary>
    /// An interface for a class that can povide a secure password
    /// </summary>
    public interface IHavePassword
    {
        System.Security.SecureString Password { get; }
    }
}