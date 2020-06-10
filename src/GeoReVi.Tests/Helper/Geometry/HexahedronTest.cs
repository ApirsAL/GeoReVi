using System;
using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoReVi.Tests
{
    [TestClass]
    public class HexahedronTest
    {
        [TestMethod]
        public void ToVector3D_Test()
        {

            // Arrange
            LocationTimeValue loc = new LocationTimeValue(1, 1, 1);
            Vector3D expected = new Vector3D(1, 1, 1);

            // Act
            Vector3D locPt3D = loc.ToVector3D();

            // Assert
            Assert.AreEqual(locPt3D, expected, "Calculation error.");

        }
    }
}
