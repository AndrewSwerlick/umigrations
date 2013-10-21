using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cardinal.UmbracoExt.Migrations.Tests
{
    [TestClass]
    public class VersionNumberTests
    {
        [TestMethod]
        public void Ensure_Version_1_0_Is_Less_Than_2_0()
        {
            Assert.IsTrue(new VersionNumber("1.0") < new VersionNumber("2.0"));
        }

        [TestMethod]
        public void Ensure_Version_1_0_Is_Less_Than_1_0()
        {
            Assert.IsTrue(new VersionNumber("1.0") < new VersionNumber("1.1"));
        }

        [TestMethod]
        public void Ensure_Version_1_0_Is_Equal_To_1_0_0()
        {
            Assert.IsTrue(new VersionNumber("1.0") == new VersionNumber("1.0.0"));
        }

        [TestMethod]
        public void Ensure_Version_1_Is_Equal_To_1_0()
        {
            Assert.IsTrue(new VersionNumber("1") == new VersionNumber("1.0.0"));
        }
    }
}
