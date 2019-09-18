using System;

namespace IotaSharp.Utils
{
    /// <summary>
    /// This class provides methods to convert Iota to different units.
    /// </summary>
    public class IotaUnitConverter
    {
        /// <summary>
        /// Convert the iota amount
        /// </summary>
        /// <param name="amount">amount</param>
        /// <param name="fromUnit">the source unit e.g. the unit of amount</param>
        /// <param name="toUnit">the target unit</param>
        /// <returns>the specified amount in the target unit</returns>
        public static double ConvertUnits(long amount, IotaUnits fromUnit, IotaUnits toUnit)
        {
            long amountInSource = amount * fromUnit.Value;
            return (double) amountInSource / toUnit.Value;
        }

        /// <summary>
        /// Finds the optimal unit to display the specified amount in
        /// </summary>
        /// <param name="amount">amount </param>
        /// <returns>the optimal IotaUnit</returns>
        public static IotaUnits FindOptimalIotaUnitToDisplay(long amount)
        {
            amount = Math.Abs(amount);

            if (amount < IotaUnits.KiloIOTA.Value)
                return IotaUnits.IOTA;

            if (amount < IotaUnits.MegaIOTA.Value)
                return IotaUnits.KiloIOTA;

            if (amount < IotaUnits.GigaIOTA.Value)
                return IotaUnits.MegaIOTA;

            if (amount < IotaUnits.TerraIOTA.Value)
                return IotaUnits.GigaIOTA;

            if (amount < IotaUnits.PetaIOTA.Value)
                return IotaUnits.TerraIOTA;

            return IotaUnits.PetaIOTA;

        }

        /// <summary>
        /// Convert the iota amount to text.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="extended"></param>
        /// <returns></returns>
        public static string ConvertRawIotaAmountToDisplayText(long amount, bool extended)
        {
            var unit = FindOptimalIotaUnitToDisplay(amount);
            double amountInDisplayUnit = (double) amount / unit.Value;

            if (extended)
                return $"{amountInDisplayUnit:##0.##################} {unit.Unit}";

            return $"{amountInDisplayUnit:##0.##} {unit.Unit}";
        }
    }
}
