using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoReVi.Tests
{
    [TestClass]
    public class ShellViewModelTest
    {
        [TestMethod]
        public void ChangeUser_Test()
        {
            // Arrange
            ShellViewModel shell = new ShellViewModel();

            // Act
            shell.ChangeUser(0, "AL");

            // Assert
            int actualId = shell.UserId;
            string actualUserName = shell.UserFullName;

            Assert.AreEqual("AL", actualUserName, "Wrong user name");
            Assert.AreEqual(0, actualId, "Wrong user ID");
        }

        [TestMethod]
        public void Logout_Test()
        {
            // Arrange
            ShellViewModel shell = new ShellViewModel();

            // Act
            shell.ChangeUser(0, "AL");

            shell.Logout();

            // Assert
            int actualId = shell.UserId;
            string actualUserName = shell.UserFullName;
            string actualView = shell.CurrentActiveItem;

            Assert.AreEqual("Logged out", actualUserName, "Wrong user name");
            Assert.AreEqual(0, actualId, "Wrong user ID");
            Assert.AreEqual("LoginView", actualView, "Wrong view");
        }
    }
}
