namespace CrystalEmuLib
{
    public static class Security
    {
        public static int Hash(short Param, short Salt)
        {
            var A = (uint)(Param >= 0 ? 2 * Param : -2 * Param - 1);
            var B = (uint)(Salt >= 0 ? 2 * Salt : -2 * Salt - 1);
            var C = (int)((A >= B ? A * A + A + B : A + B * B) / 2);
            return Param < 0 && Salt < 0 || Param >= 0 && Salt >= 0 ? C : -C - 1;
        }
    }
}
