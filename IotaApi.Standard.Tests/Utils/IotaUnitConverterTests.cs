using Iota.Api.Standard.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Api.Standard.Tests.Utils
{
    [TestClass]
    public class IotaUnitConverterTest
    {
        [TestMethod]
        public void ShouldConvertUnitItoKi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.Iota, IotaUnits.Kilo), 1);
        }

        [TestMethod]
        public void ShouldConvertUnitKiToMi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.Kilo, IotaUnits.Mega), 1);
        }

        [TestMethod]
        public void ShouldConvertUnitMiToGi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.Mega, IotaUnits.Giga), 1);
        }

        [TestMethod]
        public void ShouldConvertUnitGiToTi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.Giga, IotaUnits.Terra), 1);
        }

        [TestMethod]
        public void ShouldConvertUnitTiToPi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.Terra, IotaUnits.Peta), 1);
        }

        [TestMethod]
        public void ShouldFindOptimizeUnitToDisplay()
        {
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1), IotaUnits.Iota);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000), IotaUnits.Kilo);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000), IotaUnits.Mega);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000), IotaUnits.Giga);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000000L), IotaUnits.Terra);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000000000L), IotaUnits.Peta);
        }

        /*[TestMethod]
        public void shouldConvertRawIotaAmountToDisplayText()
        {
            Assert.AreEqual(IotaUnitConverter.convertRawIotaAmountToDisplayText(1, false), "1 i");
            Assert.AreEqual(IotaUnitConverter.convertRawIotaAmountToDisplayText(1000, false), "1 Ki");
            Assert.AreEqual(IotaUnitConverter.convertRawIotaAmountToDisplayText(1000000, false), "1 Mi");
            Assert.AreEqual(IotaUnitConverter.convertRawIotaAmountToDisplayText(1000000000, false), "1 Gi");
            Assert.AreEqual(IotaUnitConverter.convertRawIotaAmountToDisplayText(1000000000000L, false), "1 Ti");
            Assert.AreEqual(IotaUnitConverter.convertRawIotaAmountToDisplayText(1000000000000000L, false), "1 Pi");
        }*/
    }
}