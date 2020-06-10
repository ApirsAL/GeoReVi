using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoReVi.Tests.Helper.Geography
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für GeographyHelperTest
    /// </summary>
    [TestClass]
    public class GeographyHelperTest
    {
        public GeographyHelperTest()
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
        public void EuclideanDistancePoint_Test()
        {
            // Arrange
            LocationTimeValue loc1 = new LocationTimeValue(0, 0, 0);
            LocationTimeValue loc2 = new LocationTimeValue(1, 1, 1);

            double expected = 1.732050808;

            // Act
            double dist = GeographyHelper.EuclideanDistance(loc1, loc2);


            // Assert
            Assert.AreEqual(dist, expected, 0.002, "Calculation error.");
        }

        [TestMethod]
        public void EuclideanDistancePointExtensionMethod_Test()
        {
            // Arrange
            LocationTimeValue loc1 = new LocationTimeValue(0, 0, 0);
            LocationTimeValue loc2 = new LocationTimeValue(1, 1, 1);

            double expected = 1.732050808;

            // Act
            double dist = loc1.GetEuclideanDistance(loc2);

            // Assert
            Assert.AreEqual(dist, expected, 0.002, "Calculation error.");
        }
    }
}
