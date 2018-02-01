using System.Runtime.InteropServices;

namespace LibraryTests.Tests
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ColorStructs
    {
        [FieldOffset(0)]
        public uint ULongData;

        [FieldOffset(0)]
        public ColorStruct Pixel1;

        [FieldOffset(4)]
        public ColorStruct Pixel2;
    }
}