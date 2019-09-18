using IotaSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IotaSharp.Tests.Utils
{
    [TestClass]
    public class IotaUnitConverterTest
    {
        [TestMethod]
        public void ShouldConvertUnitItoKi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.IOTA, IotaUnits.KiloIOTA), 1);
        }

        [TestMethod]
        public void ShouldConvertUnitKiToMi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.KiloIOTA, IotaUnits.MegaIOTA), 1);
        }

        [TestMethod]
        public void ShouldConvertUnitMiToGi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.MegaIOTA, IotaUnits.GigaIOTA), 1);
        }

        [TestMethod]
        public void ShouldConvertUnitGiToTi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.GigaIOTA, IotaUnits.TerraIOTA), 1);
        }

        [TestMethod]
        public void ShouldConvertUnitTiToPi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.TerraIOTA, IotaUnits.PetaIOTA), 1);
        }

        [TestMethod]
        public void ShouldFindOptimizeUnitToDisplay()
        {
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1), IotaUnits.IOTA);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000), IotaUnits.KiloIOTA);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000), IotaUnits.MegaIOTA);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000), IotaUnits.GigaIOTA);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000000L), IotaUnits.TerraIOTA);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000000000L), IotaUnits.PetaIOTA);
        }

        [TestMethod]
        public void ShouldConvertRawIotaAmountToDisplayText()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertRawIotaAmountToDisplayText(1, false), "1 i");
            Assert.AreEqual(IotaUnitConverter.ConvertRawIotaAmountToDisplayText(1000, false), "1 Ki");
            Assert.AreEqual(IotaUnitConverter.ConvertRawIotaAmountToDisplayText(1000000, false), "1 Mi");
            Assert.AreEqual(IotaUnitConverter.ConvertRawIotaAmountToDisplayText(1000000000, false), "1 Gi");
            Assert.AreEqual(IotaUnitConverter.ConvertRawIotaAmountToDisplayText(1000000000000L, false), "1 Ti");
            Assert.AreEqual(IotaUnitConverter.ConvertRawIotaAmountToDisplayText(1000000000000000L, false), "1 Pi");
        }
    }
}
