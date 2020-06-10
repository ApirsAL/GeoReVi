using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoReVi.Tests
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für LocationTimeValueTest
    /// </summary>
    [TestClass]
    public class LocationTimeValueTest
    {
        public LocationTimeValueTest()
        {
            //
            // TODO: Konstruktorlogik hier hinzufügen
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Ruft den Textkontext mit Informationen über
        ///den aktuellen Testlauf sowie Funktionalität für diesen auf oder legt diese fest.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Zusätzliche Testattribute
        //
        // Sie können beim Schreiben der Tests folgende zusätzliche Attribute verwenden:
        //
        // Verwenden Sie ClassInitialize, um vor Ausführung des ersten Tests in der Klasse Code auszuführen.
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Verwenden Sie ClassCleanup, um nach Ausführung aller Tests in einer Klasse Code auszuführen.
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Mit TestInitialize können Sie vor jedem einzelnen Test Code ausführen. 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Mit TestCleanup können Sie nach jedem Test Code ausführen.
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ToPoint3D_Test()
        {

            // Arrange
            LocationTimeValue loc = new LocationTimeValue(1, 1, 1);
            Point3D expected = new Point3D(1, 1, 1);

            // Act
            Point3D locPt3D = loc.ToPoint3D();

            // Assert
            Assert.AreEqual(locPt3D, expected, "Conversion error.");
            
        }

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

        [TestMethod]
        public void GetMiddlePoint_Test()
        {

            // Arrange
            LocationTimeValue loc1 = new LocationTimeValue(1, 1, 1, "default", 1);
            LocationTimeValue loc2 = new LocationTimeValue(2, 2, 2, "default", 2);

            LocationTimeValue expected = new LocationTimeValue(1.5, 1.5, 1.5, "default", 1.5);

            // Act
            LocationTimeValue middle = loc1.GetMiddlePoint(loc2);

            // Assert
            Assert.AreEqual(expected.X, middle.X, 0.000001, "Calculation x failed");
            Assert.AreEqual(expected.Y, middle.Y, 0.000001, "Calculation y failed");
            Assert.AreEqual(expected.Z, middle.Z, 0.000001, "Calculation z failed");
            Assert.AreEqual(expected.Value[0], middle.Value[0], 0.000001, "Calculation value failed");

        }
    }
}
