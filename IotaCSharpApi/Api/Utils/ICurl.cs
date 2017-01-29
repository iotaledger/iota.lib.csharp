namespace Iota.Lib.CSharp.Api.Utils
{
    public interface ICurl
    {
        ICurl Clone();

        ICurl Absorb(int[] trits, int offset, int length);

        ICurl Absorb(int[] trits);

        int[] Squeeze(int[] trits, int offset, int length);

        int[] Squeeze(int[] trits);

        ICurl Transform();

        ICurl Reset();

        int[] State { get; set; }
    }
}