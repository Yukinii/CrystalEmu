namespace CrystalEmuLib.Extensions
{
    public static class StringExtensions
    {
        public static unsafe void CopyTo(this string Str, void* PointerDestination)
        {
            var Destination = (byte*)PointerDestination;
            for (var I = 0; I < Str.Length; I++)
            {
                Destination[I] = (byte)Str[I];
            }
        }
    }
}
