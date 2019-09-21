namespace IotaSharp.Utils
{
    /// <summary>
    /// Table of IOTA units based off of the standard system of Units.
    /// </summary>
    public class IotaUnits
    {
        /// <summary>
        /// 10^0
        /// </summary>
        public static readonly IotaUnits IOTA = new IotaUnits("i", 1L);

        /// <summary>
        /// 10^3
        /// </summary>
        public static readonly IotaUnits KiloIOTA = new IotaUnits("Ki", IOTA.Value * 1000L);

        /// <summary>
        /// 10^6
        /// </summary>
        public static readonly IotaUnits MegaIOTA = new IotaUnits("Mi", KiloIOTA.Value * 1000L);

        /// <summary>
        /// 10^9
        /// </summary>
        public static readonly IotaUnits GigaIOTA = new IotaUnits("Gi", MegaIOTA.Value * 1000L);

        /// <summary>
        /// 10^12
        /// </summary>
        public static readonly IotaUnits TerraIOTA = new IotaUnits("Ti", GigaIOTA.Value * 1000L);

        /// <summary>
        /// 10^15
        /// </summary>
        public static readonly IotaUnits PetaIOTA = new IotaUnits("Pi", TerraIOTA.Value * 1000L);

        private IotaUnits(string unit, long value)
        {
            Unit = unit;
            Value = value;
        }

        /// <summary>
        /// unit
        /// </summary>
        public string Unit { get; }

        /// <summary>
        /// value
        /// </summary>
        public long Value { get; }
    }
}
