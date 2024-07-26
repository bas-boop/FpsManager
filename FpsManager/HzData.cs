namespace FpsManager
{
    public struct HzData(int a, int b)
    {
        public readonly int LowestHz = a;
        public readonly int StandaloneHz = b;
    }
}