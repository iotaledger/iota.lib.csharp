using System;

namespace Iota.Lib.CSharp.Api.Utils
{
    public class IotaUnitConverter
    {
        public static double ConvertUnits(long amount, IotaUnits fromUnit, IotaUnits toUnit)
        {
            long amountInSource = (long) (amount*Math.Pow(10, (int) fromUnit));
            return ConvertUnits(amountInSource, toUnit);
        }

        public static long ConvertUnits(long amount, IotaUnits toUnit)
        {
            int base10NormalizationExponent = (int) toUnit;
            return (long) (amount/Math.Pow(10, base10NormalizationExponent));
        }

        public static IotaUnits findOptimalIotaUnitToDisplay(long amount)
        {
            int length = (amount).ToString().Length;

            if (amount < 0)
            {
// do not count "-" sign
                length -= 1;
            }

            IotaUnits units = IotaUnits.Iota;

            if (length >= 1 && length <= 3)
            {
                units = IotaUnits.Iota;
            }
            else if (length > 3 && length <= 6)
            {
                units = IotaUnits.Kilo;
            }
            else if (length > 6 && length <= 9)
            {
                units = IotaUnits.Mega;
            }
            else if (length > 9 && length <= 12)
            {
                units = IotaUnits.Giga;
            }
            else if (length > 12 && length <= 15)
            {
                units = IotaUnits.Terra;
            }
            else if (length > 15 && length <= 18)
            {
                units = IotaUnits.Peta;
            }
            return units;
        }
    }
}