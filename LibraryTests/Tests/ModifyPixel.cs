using BenchmarkDotNet.Attributes;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace LibraryTests.Tests
{
    public class ModifyPixel
    {
        [Params(1198)]
        public int HeightOfArea { get; set; }

        [Params(804)]
        public int WidthOfArea { get; set; }

        private static string InputPath => "../../../../../../../Formats/Jpg/Calliphora.jpg";

        private static string OutputPath => "../../../../../../../Output/Test.jpg";

        //https://github.com/tunnelvisionlabs/NOpenCL
        //This is potentially where GPU would come in to play.

        private SKBitmap Original { get; set; }

        [GlobalCleanup]
        public void Cleanup()
        {
            Original.UnlockPixels();
            Original.Dispose();
        }

        [Benchmark(Description = "Group SetPixel method on the bitmap")]
        public unsafe void GroupSetPixel()
        {
            var PixelPointer = Original.GetPixels();
            byte* OriginalPointer = (byte*)PixelPointer.ToPointer();
            byte* Pointer = OriginalPointer;
            for (int y = 0; y < HeightOfArea; ++y)
            {
                for (int x = 0; x < WidthOfArea; ++x)
                {
                    *Pointer = 100;
                    ++Pointer;
                    *Pointer = 100;
                    ++Pointer;
                    *Pointer = 0;
                    Pointer += 2;
                }
                Pointer = OriginalPointer + (y * Original.RowBytes);
            }
        }

        [Benchmark(Description = "Parallel Struct SetPixel UInt method on the bitmap")]
        public unsafe void ParallelSetPixelStructUInt()
        {
            var PixelPointer = Original.GetPixels();
            int Width = Original.Width;
            ColorStruct* OriginalPointer = (ColorStruct*)PixelPointer.ToPointer();
            var Value = new ColorStruct { Red = 100, Green = 100 };
            Parallel.For(0, HeightOfArea, y =>
              {
                  ColorStruct* Pointer = OriginalPointer + (y * Width);
                  for (int x = 0; x < WidthOfArea; ++x)
                  {
                      (*Pointer).UIntData = Value.UIntData;
                      ++Pointer;
                  }
              });
        }

        [Benchmark(Description = "Parallel Struct SetPixel UInt Partitioned method on the bitmap")]
        public unsafe void ParallelSetPixelStructUIntPartitioned()
        {
            var PixelPointer = Original.GetPixels();
            int Width = Original.Width;
            ColorStruct* OriginalPointer = (ColorStruct*)PixelPointer.ToPointer();
            var Value = new ColorStruct { Red = 100, Green = 100 };
            var RangePartitioner = Partitioner.Create(0, HeightOfArea);
            Parallel.ForEach(RangePartitioner, (range, state) =>
            {
                for (int y = range.Item1; y < range.Item2; ++y)
                {
                    ColorStruct* Pointer = OriginalPointer + (y * Width);
                    for (int x = 0; x < WidthOfArea; ++x)
                    {
                        (*Pointer).UIntData = Value.UIntData;
                        ++Pointer;
                    }
                }
            });
        }

        [Benchmark(Description = "Parallel Struct SetPixel ULong method on the bitmap")]
        public unsafe void ParallelSetPixelStructULong()
        {
            var PixelPointer = Original.GetPixels();
            int Width = Original.Width / 2;
            ColorStructs* OriginalPointer = (ColorStructs*)PixelPointer.ToPointer();
            var Value = new ColorStructs
            {
                Pixel1 = new ColorStruct { Red = 100, Green = 100 },
                Pixel2 = new ColorStruct { Red = 100, Green = 100 },
            };

            Parallel.For(0, HeightOfArea, y =>
            {
                ColorStructs* Pointer = OriginalPointer + (y * Width);
                for (int x = 0; x < WidthOfArea; x += 2)
                {
                    (*Pointer).ULongData = Value.ULongData;
                    ++Pointer;
                }
            });
        }

        [Benchmark(Baseline = true, Description = "SetPixel method on the bitmap")]
        public void SetPixel()
        {
            for (int y = 0; y < HeightOfArea; ++y)
            {
                for (int x = 0; x < WidthOfArea; ++x)
                {
                    Original.SetPixel(x, y, new SKColor(100, 100, 0));
                }
            }
        }

        [Benchmark(Description = "Struct SetPixel Individual method on the bitmap")]
        public unsafe void SetPixelStruct()
        {
            var PixelPointer = Original.GetPixels();
            int Width = Original.Width;
            ColorStruct* OriginalPointer = (ColorStruct*)PixelPointer.ToPointer();
            ColorStruct* Pointer = OriginalPointer;
            for (int y = 0; y < HeightOfArea; ++y)
            {
                for (int x = 0; x < WidthOfArea; ++x)
                {
                    (*Pointer).Red = 100;
                    (*Pointer).Green = 100;
                    ++Pointer;
                }
                Pointer = OriginalPointer + (y * Width);
            }
        }

        [Benchmark(Description = "Struct SetPixel UInt method on the bitmap")]
        public unsafe void SetPixelStructUInt()
        {
            var PixelPointer = Original.GetPixels();
            int Width = Original.Width;
            ColorStruct* OriginalPointer = (ColorStruct*)PixelPointer.ToPointer();
            ColorStruct* Pointer = OriginalPointer;
            var Value = new ColorStruct { Red = 100, Green = 100 };
            for (int y = 0; y < HeightOfArea; ++y)
            {
                for (int x = 0; x < WidthOfArea; ++x)
                {
                    (*Pointer).UIntData = Value.UIntData;
                    ++Pointer;
                }
                Pointer = OriginalPointer + (y * Width);
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            using (var input = File.OpenRead(InputPath))
            {
                using (var inputStream = new SKManagedStream(input))
                {
                    Original = SKBitmap.Decode(inputStream);
                    Original.LockPixels();
                }
            }
        }

        [Benchmark(Description = "Span SetPixel UInt on the bitmap")]
        public unsafe void SpanSetPixel()
        {
            int Width = Original.Width;
            var ColorPointer = Original.GetPixels().ToPointer();
            Span<ColorStruct> Pointer = new Span<ColorStruct>(ColorPointer, Original.Width * Original.Height);
            var Value = new ColorStruct { Red = 100, Green = 100 };
            for (int y = 0; y < HeightOfArea; ++y)
            {
                Span<ColorStruct> Slice = Pointer.Slice(y * Width, Width);
                for (int x = 0; x < WidthOfArea; ++x)
                {
                    Slice[x].UIntData = Value.UIntData;
                }
            }
        }

        [Benchmark(Description = "Unsafe SetPixel method on the bitmap")]
        public void UnsafeSetPixel()
        {
            var PixelPointer = Original.GetPixels();
            for (int y = 0; y < HeightOfArea; ++y)
            {
                for (int x = 0; x < WidthOfArea; ++x)
                {
                    SetPixel(PixelPointer, x, y, Original.Height, Original.RowBytes);
                }
            }
        }

        private static unsafe void SetPixel(IntPtr pointer, int x, int y, int height, int width)
        {
            byte* ptr = (byte*)pointer.ToPointer();
            ptr[(y * width) + (x * 4)] = 100;
            ptr[(y * width) + (x * 4) + 1] = 100;
            ptr[(y * width) + (x * 4) + 2] = 0;
        }
    }
}