using Iota.Api.Standard.Utils;
using Xunit;

namespace Iota.Api.Standard.UnitTests
{
    public class IotaUnitConverterTest
    {
        [Fact]
        public void ShouldConvertUnitItoKi()
        {
            Assert.Equal(1, IotaUnitConverter.ConvertUnits(1000, IotaUnits.Iota, IotaUnits.Kilo));
        }

        [Fact]
        public void ShouldConvertUnitKiToMi()
        {
            Assert.Equal(1, IotaUnitConverter.ConvertUnits(1000, IotaUnits.Kilo, IotaUnits.Mega));
        }

        [Fact]
        public void ShouldConvertUnitMiToGi()
        {
            Assert.Equal(1, IotaUnitConverter.ConvertUnits(1000, IotaUnits.Mega, IotaUnits.Giga));
        }

        [Fact]
        public void ShouldConvertUnitGiToTi()
        {
            Assert.Equal(1, IotaUnitConverter.ConvertUnits(1000, IotaUnits.Giga, IotaUnits.Terra));
        }

        [Fact]
        public void ShouldConvertUnitTiToPi()
        {
            Assert.Equal(1, IotaUnitConverter.ConvertUnits(1000, IotaUnits.Terra, IotaUnits.Peta));
        }

        [Fact]
        public void ShouldFindBestUnitToDisplay()
        {
            Assert.Equal(IotaUnits.Iota, IotaUnitConverter.FindOptimalIotaUnitToDisplay(1));
            Assert.Equal(IotaUnits.Kilo, IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000));
            Assert.Equal(IotaUnits.Mega, IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000));
            Assert.Equal(IotaUnits.Giga, IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000));
            Assert.Equal(IotaUnits.Terra, IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000000L));
            Assert.Equal(IotaUnits.Peta, IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000000000L));
        }
    }
}