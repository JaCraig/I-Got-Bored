using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LibraryTests.Tests
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ColorStruct
    {
        [FieldOffset(3)]
        public byte Alpha;

        [FieldOffset(2)]
        public byte Blue;

        [FieldOffset(1)]
        public byte Green;

        [FieldOffset(0)]
        public int IntData;

        [FieldOffset(0)]
        public byte Red;

        [FieldOffset(0)]
        public uint UIntData;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ColorStruct operator +(ColorStruct color1, ColorStruct color2)
        {
            return new ColorStruct
            {
                Red = (byte)(color1.Red + color2.Red).Clamp(0, 255),
                Green = (byte)(color1.Green + color2.Green).Clamp(0, 255),
                Blue = (byte)(color1.Blue + color2.Blue).Clamp(0, 255),
                Alpha = (byte)(color1.Alpha + color2.Alpha).Clamp(0, 255)
            };
        }
    }

    public static class Extensions
    {
        public static int Clamp(this int value, int low, int high)
        {
            return value < low ? low : (value > high ? high : value);
        }
    }
}