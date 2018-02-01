This is the repository for testing various speed improvements for Structure.Sketching and other libs. The results can be found below the conclussions section.

# Conclussions

Generally speaking the following seems to hold true at the moment:

1. When you can, you should be using arrays instead of other collections like List, etc.

2. When you can, try not to create large blocks of objects if they are short lived. Instead use an ArrayPool, RecycledMemoryStream, etc. to reduce the number of allocations that you are doing.

3. Use Buffer.MemoryCopy over Array.Copy when dealing with a small number of items. This is less of an issue as the array's size increases.

4. Do not store arrays, lists, etc. as their interfaces when using them. It causes unnecessary overhead.

5. When you can, use the Array.Sort static method or the List.Sort method on the object. They are much faster.

6. Most of the old micro optimization tricks (using >>, &, etc. instead of straight math), don't matter much anymore.

7. When creating large blocks of local objects for a method, check if you can get away with a struct instead of a class. If so there are slight speed improvements to be potentially made.

8. When using a Dictionary<,> the key's type does matter when it comes to speed.

9. ConcurrentDictionary, ConcurrentBag, etc. adds overhead. Make sure you need them before using them.

10. Use dynamic objects only as needed as there is a bunch of overhead involved.

11. Enums, while great, are slower than you would think.

12. Properties do not seem to cause much overhead vs fields in most instances.

13. Hashtables should be looked at as a viable alternative to Dictionary when possible.

14. Don't get fancy when using Linq. Only convert to a list/array when you need to.

15. When possible use a for loop over a foreach.

16. Avoid reflection when possible. Favor normal method calls, then Func/Action, then reflection.

17. When modifying a lot of binary data, favor using [StructLayout(LayoutKind.Explicit)] structs and modify more than one when possible.

18. When reading binary data, once again favor using [StructLayout(LayoutKind.Explicit)] structs when possible.

19. When creating items, just use new when you can. If you can't compiled/cached lambda expressions should be favored over alternatives.

20. When using the TPL. Using partitions may improve speed quite a bit but depends on the work being done.

21. When using reflection, cache the data from the Type object. Grabbing this info is not free.

22. If you need to read in a file whole, do so using a buffer the size of the file. Only do chunks if you need to stream in the file.

23. Don't put a constructor on a static class, if you can help it. This is called every time a method is invoked.

24. Use a StringBuilder when doing a lot of concatenation. Also concatenation is generally cheaper than formatting a string.

25. Using the Vector class can help when dealing with large arrays of data but will potentially lose out to structs.

# Results

**Allocation strategy**

Checks new array vs ArrayPool vs Span to preallocated array vs Pointer to preallocated array.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                                           Method |      Mean |     Error |    StdDev | Scaled | ScaledSD |
|------------------------------------------------- |----------:|----------:|----------:|-------:|---------:|
|                             &#39;Allocate as needed&#39; |  9.019 us | 0.1175 us | 0.1099 us |   1.00 |     0.00 |
|                                        ArrayPool |  6.983 us | 0.1106 us | 0.1034 us |   0.77 |     0.01 |
| &#39;Item from pointer to preallocated array &#39;pool&#39;&#39; | 16.739 us | 0.2232 us | 0.2088 us |   1.86 |     0.03 |
|            &#39;Span from preallocated array &#39;pool&#39;&#39; | 16.703 us | 0.2241 us | 0.2096 us |   1.85 |     0.03 |

			
**Array copy**

Array.Copy() vs Buffer.MemoryCopy().

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                            Method | Count |       Mean |     Error |    StdDev | Scaled |
|---------------------------------- |------ |-----------:|----------:|----------:|-------:|
|         **&#39;Copy using Array.Copy()&#39;** |   **100** |  **30.740 ns** | **0.4038 ns** | **0.3777 ns** |   **1.00** |
| &#39;Copy using Buffer.MemoryCopy&lt;T&gt;&#39; |   100 |   7.941 ns | 0.1330 ns | 0.1244 ns |   0.26 |
|                                   |       |            |           |           |        |
|         **&#39;Copy using Array.Copy()&#39;** |  **1000** |  **56.959 ns** | **0.6771 ns** | **0.6334 ns** |   **1.00** |
| &#39;Copy using Buffer.MemoryCopy&lt;T&gt;&#39; |  1000 |  41.623 ns | 0.6875 ns | 0.6431 ns |   0.73 |
|                                   |       |            |           |           |        |
|         **&#39;Copy using Array.Copy()&#39;** | **10000** | **168.846 ns** | **1.9435 ns** | **1.8179 ns** |   **1.00** |
| &#39;Copy using Buffer.MemoryCopy&lt;T&gt;&#39; | 10000 | 152.139 ns | 1.8735 ns | 1.7525 ns |   0.90 |


**Array interface iteration**

Checks converting an array to the various interfaces and doing for/foreach

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|                           Method |       Mean |      Error |     StdDev | Scaled | ScaledSD |
|--------------------------------- |-----------:|-----------:|-----------:|-------:|---------:|
|                    &#39;T[] ForEach&#39; |   6.792 us |  0.0532 us |  0.0498 us |   0.99 |     0.01 |
|                        &#39;T[] For&#39; |   6.856 us |  0.0604 us |  0.0565 us |   1.00 |     0.00 |
|            &#39;ICollection ForEach&#39; | 817.657 us |  5.8472 us |  5.4695 us | 119.27 |     1.22 |
|         &#39;ICollection&lt;T&gt; ForEach&#39; |  66.709 us |  0.3284 us |  0.2564 us |   9.73 |     0.09 |
|            &#39;IEnumerable ForEach&#39; | 850.999 us | 13.9302 us | 13.0303 us | 124.13 |     2.09 |
|         &#39;IEnumerable&lt;T&gt; ForEach&#39; |  67.207 us |  0.5752 us |  0.5380 us |   9.80 |     0.11 |
|                  &#39;IList ForEach&#39; | 842.353 us |  7.5242 us |  6.6700 us | 122.87 |     1.36 |
|               &#39;IList&lt;T&gt; ForEach&#39; |  67.086 us |  0.8508 us |  0.7542 us |   9.79 |     0.13 |
|                   &#39;IList&lt;T&gt; For&#39; |  33.078 us |  0.2274 us |  0.2127 us |   4.83 |     0.05 |
|                      &#39;IList For&#39; | 745.198 us |  9.3907 us |  8.7841 us | 108.70 |     1.51 |
| &#39;IReadOnlyCollection&lt;T&gt; ForEach&#39; |  67.364 us |  0.6311 us |  0.5903 us |   9.83 |     0.11 |
|       &#39;IReadOnlyList&lt;T&gt; ForEach&#39; |  66.522 us |  0.4464 us |  0.4175 us |   9.70 |     0.10 |
|           &#39;IReadOnlyList&lt;T&gt; For&#39; |  33.741 us |  0.5140 us |  0.4808 us |   4.92 |     0.08 |

**Array/List sorting**

Checks a couple of different ways to sort arrays/lists as well as a library called LinqFaster.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                               Method |   Size |             Mean |           Error |          StdDev | Scaled | ScaledSD |
|------------------------------------- |------- |-----------------:|----------------:|----------------:|-------:|---------:|
|               **&#39;Linq Array.OrderBy()&#39;** |     **10** |        **630.35 ns** |       **3.5944 ns** |       **3.1863 ns** |   **1.00** |     **0.00** |
|                     Array.OrderByF() |     10 |        283.28 ns |       0.6358 ns |       0.5309 ns |   0.45 |     0.00 |
| &#39;PLinq Array.AsParallel().OrderBy()&#39; |     10 |     20,940.50 ns |     332.4768 ns |     310.9990 ns |  33.22 |     0.50 |
|                         Array.Sort() |     10 |         66.33 ns |       0.6915 ns |       0.6469 ns |   0.11 |     0.00 |
|                         List.OrderBy |     10 |        600.39 ns |       5.1850 ns |       4.8500 ns |   0.95 |     0.01 |
|                        List.OrderByF |     10 |        557.47 ns |       7.3192 ns |       6.8464 ns |   0.88 |     0.01 |
|                          List.Sort() |     10 |         68.17 ns |       1.1046 ns |       1.0333 ns |   0.11 |     0.00 |
|                                      |        |                  |                 |                 |        |          |
|               **&#39;Linq Array.OrderBy()&#39;** |    **100** |     **10,108.99 ns** |      **80.7705 ns** |      **75.5528 ns** |   **1.00** |     **0.00** |
|                     Array.OrderByF() |    100 |      2,277.26 ns |      16.8069 ns |      15.7212 ns |   0.23 |     0.00 |
| &#39;PLinq Array.AsParallel().OrderBy()&#39; |    100 |     28,518.52 ns |     565.9662 ns |     581.2056 ns |   2.82 |     0.06 |
|                         Array.Sort() |    100 |        512.86 ns |      11.2644 ns |      10.5367 ns |   0.05 |     0.00 |
|                         List.OrderBy |    100 |      8,857.91 ns |      82.5387 ns |      68.9236 ns |   0.88 |     0.01 |
|                        List.OrderByF |    100 |      8,614.45 ns |      59.2271 ns |      52.5032 ns |   0.85 |     0.01 |
|                          List.Sort() |    100 |        508.25 ns |       7.9529 ns |       7.4391 ns |   0.05 |     0.00 |
|                                      |        |                  |                 |                 |        |          |
|               **&#39;Linq Array.OrderBy()&#39;** |   **1000** |    **185,209.87 ns** |   **3,645.4980 ns** |   **3,900.6417 ns** |   **1.00** |     **0.00** |
|                     Array.OrderByF() |   1000 |     47,222.91 ns |     731.2665 ns |     684.0271 ns |   0.26 |     0.01 |
| &#39;PLinq Array.AsParallel().OrderBy()&#39; |   1000 |    151,956.42 ns |   3,814.8214 ns |  11,248.0869 ns |   0.82 |     0.06 |
|                         Array.Sort() |   1000 |      6,042.70 ns |      44.8089 ns |      41.9142 ns |   0.03 |     0.00 |
|                         List.OrderBy |   1000 |    177,483.63 ns |   2,229.2264 ns |   2,085.2196 ns |   0.96 |     0.02 |
|                        List.OrderByF |   1000 |    157,747.04 ns |   2,062.7274 ns |   1,929.4763 ns |   0.85 |     0.02 |
|                          List.Sort() |   1000 |      6,320.13 ns |      82.2953 ns |      76.9791 ns |   0.03 |     0.00 |
|                                      |        |                  |                 |                 |        |          |
|               **&#39;Linq Array.OrderBy()&#39;** |  **10000** |  **2,555,311.55 ns** |  **21,744.4778 ns** |  **20,339.7961 ns** |   **1.00** |     **0.00** |
|                     Array.OrderByF() |  10000 |    768,350.59 ns |   9,214.5936 ns |   8,168.4964 ns |   0.30 |     0.00 |
| &#39;PLinq Array.AsParallel().OrderBy()&#39; |  10000 |    960,198.34 ns |  11,519.6548 ns |  10,775.4912 ns |   0.38 |     0.00 |
|                         Array.Sort() |  10000 |     89,003.07 ns |     923.1031 ns |     863.4712 ns |   0.03 |     0.00 |
|                         List.OrderBy |  10000 |  2,473,553.63 ns |  23,558.4964 ns |  20,883.9914 ns |   0.97 |     0.01 |
|                        List.OrderByF |  10000 |  2,081,163.75 ns |  13,185.8718 ns |  12,334.0714 ns |   0.81 |     0.01 |
|                          List.Sort() |  10000 |     89,209.74 ns |     579.2833 ns |     513.5195 ns |   0.03 |     0.00 |
|                                      |        |                  |                 |                 |        |          |
|               **&#39;Linq Array.OrderBy()&#39;** | **100000** | **33,236,182.12 ns** | **255,992.3357 ns** | **239,455.3664 ns** |   **1.00** |     **0.00** |
|                     Array.OrderByF() | 100000 |  9,687,986.94 ns | 110,652.1353 ns | 103,504.0660 ns |   0.29 |     0.00 |
| &#39;PLinq Array.AsParallel().OrderBy()&#39; | 100000 | 11,246,986.45 ns | 135,822.6156 ns | 120,403.1991 ns |   0.34 |     0.00 |
|                         Array.Sort() | 100000 |    976,876.32 ns |   5,546.5097 ns |   5,188.2081 ns |   0.03 |     0.00 |
|                         List.OrderBy | 100000 | 31,243,391.99 ns | 109,688.5426 ns |  97,236.0264 ns |   0.94 |     0.01 |
|                        List.OrderByF | 100000 | 26,570,180.54 ns | 293,673.5636 ns | 274,702.4069 ns |   0.80 |     0.01 |
|                          List.Sort() | 100000 |    970,925.37 ns |  13,714.5530 ns |  12,828.6002 ns |   0.03 |     0.00 |

**Bitwise vs Modulo**
Checks bit manipulation vs modulo.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|           Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |
|----------------- |---------:|----------:|----------:|-------:|---------:|
| &#39;Bitwise Modulo&#39; | 4.700 us | 0.0610 us | 0.0571 us |   1.01 |     0.02 |
|  &#39;Normal Modulo&#39; | 4.643 us | 0.0766 us | 0.0717 us |   1.00 |     0.00 |

**Bitwise vs Division**
Checks bit manipulation vs division.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|             Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |
|------------------- |---------:|----------:|----------:|-------:|---------:|
| &#39;Bitwise Division&#39; | 3.886 us | 0.0517 us | 0.0483 us |   0.99 |     0.02 |
|  &#39;Normal Division&#39; | 3.927 us | 0.0433 us | 0.0405 us |   1.00 |     0.00 |


**Bitwise vs Multiplication**
Checks bit manipulation vs multiplication.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                   Method |     Mean |     Error |    StdDev | Scaled |
|------------------------- |---------:|----------:|----------:|-------:|
| &#39;Bitwise Multiplication&#39; | 3.924 us | 0.0342 us | 0.0320 us |   1.01 |
|  &#39;Normal Multiplication&#39; | 3.900 us | 0.0273 us | 0.0256 us |   1.00 |


**Checked vs Unchecked**

Difference between bounds checked multiplication and unchecked.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|           Method |     Mean |     Error |    StdDev | Scaled |
|----------------- |---------:|----------:|----------:|-------:|
|   &#39;Checked math&#39; | 3.891 us | 0.0422 us | 0.0374 us |   1.00 |
| &#39;Unchecked math&#39; | 3.922 us | 0.0258 us | 0.0242 us |   1.01 |


**Class vs Struct Access**
Checks if accessing a field on a class or struct is faster.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|        Method |     Mean |     Error |    StdDev | Scaled |
|-------------- |---------:|----------:|----------:|-------:|
|  &#39;Class test&#39; | 3.842 us | 0.0430 us | 0.0336 us |   1.00 |
| &#39;Struct test&#39; | 3.873 us | 0.0464 us | 0.0434 us |   1.01 |


**Class vs Struct Creation**
Checks the speed difference in creating a class vs a struct.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|        Method |     Mean |     Error |    StdDev | Scaled |
|-------------- |---------:|----------:|----------:|-------:|
|  &#39;Class test&#39; | 3.302 us | 0.0559 us | 0.0523 us |   1.00 |
| &#39;Struct test&#39; | 1.743 us | 0.0343 us | 0.0353 us |   0.53 |


**Class vs Struct Set**

Checks the speed difference in setting a value in a class vs a struct.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|        Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |
|-------------- |---------:|----------:|----------:|-------:|---------:|
|  &#39;Class test&#39; | 3.895 us | 0.0448 us | 0.0419 us |   1.00 |     0.00 |
| &#39;Struct test&#39; | 3.881 us | 0.0542 us | 0.0507 us |   1.00 |     0.02 |


**Dictionary Key Tests**

This tests various types for a dictionary's keys.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                                             Method |     Mean |      Error |     StdDev | Scaled | ScaledSD |
|--------------------------------------------------- |---------:|-----------:|-----------:|-------:|---------:|
|                             Dictionary&lt;int,string&gt; | 113.0 ns |  0.4176 ns |  0.3702 ns |   0.49 |     0.01 |
|                            Dictionary&lt;long,string&gt; | 152.6 ns |  1.1194 ns |  1.0471 ns |   0.66 |     0.01 |
|                           Dictionary&lt;short,string&gt; | 161.5 ns |  1.7224 ns |  1.6111 ns |   0.70 |     0.01 |
| &#39;Dictionary&lt;int,string&gt; Using string key HashCode&#39; | 151.9 ns |  0.7004 ns |  0.6552 ns |   0.66 |     0.01 |
|           &#39;Dictionary&lt;string,string&gt; key interned&#39; | 744.8 ns | 14.8122 ns | 31.5661 ns |   3.25 |     0.14 |
|                          Dictionary&lt;string,string&gt; | 229.5 ns |  3.2704 ns |  3.0591 ns |   1.00 |     0.00 |
|                            Dictionary&lt;uint,string&gt; | 114.4 ns |  0.6714 ns |  0.5952 ns |   0.50 |     0.01 |
|                           Dictionary&lt;ulong,string&gt; | 157.8 ns |  3.1587 ns |  2.4661 ns |   0.69 |     0.01 |
|                          Dictionary&lt;ushort,string&gt; | 149.9 ns |  1.4946 ns |  1.3981 ns |   0.65 |     0.01 |


**Dictionary vs ConcurrentDictionary**

Basic test between a Dictionary and ConcurrentDictionary. No parallelism is involved.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                              Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |
|------------------------------------ |---------:|----------:|----------:|-------:|---------:|
| ConcurrentDictionary&lt;string,string&gt; | 714.1 ns | 12.642 ns | 11.825 ns |   2.28 |     0.04 |
|           Dictionary&lt;string,string&gt; | 312.7 ns |  1.627 ns |  1.271 ns |   1.00 |     0.00 |


**Dynamic Tests**

Checks what overhead is involved with dynamic objects and what can be done to mitigate the issue.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                 Method |        Mean |      Error |     StdDev | Scaled | ScaledSD |
|----------------------- |------------:|-----------:|-----------:|-------:|---------:|
|        &#39;Dynamic value&#39; | 14,216.9 ns | 224.116 ns | 198.673 ns |   1.00 |     0.00 |
| &#39;Dynamic with casting&#39; | 10,470.9 ns | 183.918 ns | 180.632 ns |   0.74 |     0.02 |
|        &#39;Static typing&#39; |    481.4 ns |   3.174 ns |   2.295 ns |   0.03 |     0.00 |

**Enum vs Byte Packing**

Checks which is faster, enums or byte packing when dealing with flags.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
| Method |       Mean |     Error |    StdDev | Scaled |
|------- |-----------:|----------:|----------:|-------:|
|   Byte |   6.516 ns | 0.1093 ns | 0.1023 ns |   0.03 |
|   Enum | 226.112 ns | 1.9302 ns | 1.8055 ns |   1.00 |
|   UInt |   9.389 ns | 0.1632 ns | 0.1526 ns |   0.04 |

**Field vs Property Tests**

Checks if there is any difference between using a field or a property.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|          Method |     Mean |     Error |    StdDev | Scaled |
|---------------- |---------:|----------:|----------:|-------:|
|    &#39;Field test&#39; | 3.898 us | 0.0445 us | 0.0394 us |   1.00 |
|    &#39;Local test&#39; | 3.901 us | 0.0369 us | 0.0288 us |   1.01 |
| &#39;Property test&#39; | 3.881 us | 0.0231 us | 0.0193 us |   1.00 |


**Hashtable vs Dictionary**

Checks Hashtable, StringDictionary, and Dictionary for speed differences.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                      Method |      Mean |     Error |    StdDev | Scaled | ScaledSD |
|---------------------------- |----------:|----------:|----------:|-------:|---------:|
|      Dictionary&lt;int,string&gt; | 117.42 ns | 0.9508 ns | 0.7940 ns |   0.63 |     0.01 |
|   Dictionary&lt;string,string&gt; | 186.35 ns | 3.8625 ns | 4.2932 ns |   1.00 |     0.00 |
|    &#39;Hashtable (int,string)&#39; |  99.99 ns | 0.3033 ns | 0.2837 ns |   0.54 |     0.01 |
| &#39;Hashtable (string,string)&#39; | 146.69 ns | 1.0456 ns | 0.9780 ns |   0.79 |     0.02 |
|            StringDictionary | 340.44 ns | 3.3650 ns | 2.8099 ns |   1.83 |     0.04 |


**IEnumerable Conversion**

Checks if it is faster to convert an IEnumerable before doing Linq queries, or manually doing the query itself.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|           Method |      Mean |     Error |    StdDev | Scaled | ScaledSD |
|----------------- |----------:|----------:|----------:|-------:|---------:|
|            Array | 104.27 us | 0.8782 us | 0.8214 us |   2.39 |     0.03 |
|     &#39;Array Copy&#39; |  52.01 us | 0.5614 us | 0.5251 us |   1.19 |     0.02 |
| &#39;Array for loop&#39; |  58.08 us | 0.6107 us | 0.5713 us |   1.33 |     0.02 |
|      IEnumerable |  43.63 us | 0.5551 us | 0.4635 us |   1.00 |     0.00 |
|             List |  85.03 us | 1.3820 us | 1.2927 us |   1.95 |     0.03 |
|    &#39;List CopyTo&#39; |  56.47 us | 0.4215 us | 0.3943 us |   1.29 |     0.02 |
|  &#39;List for loop&#39; |  60.70 us | 0.9805 us | 0.9172 us |   1.39 |     0.02 |

**IEnumerable Tests**

Really a test between various IEnumerables to see which is faster to enumerate.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|               Method |       Mean |     Error |    StdDev | Scaled | ScaledSD |
|--------------------- |-----------:|----------:|----------:|-------:|---------:|
|     &#39;ArrayList test&#39; | 103.782 us | 0.8552 us | 0.8000 us |   4.23 |     0.04 |
|         &#39;Array test&#39; |   6.890 us | 0.0449 us | 0.0420 us |   0.28 |     0.00 |
| &#39;ConcurrentBag test&#39; | 102.823 us | 1.3711 us | 2.1748 us |   4.19 |     0.09 |
|       &#39;HashSet test&#39; |  43.991 us | 2.0345 us | 5.9988 us |   1.79 |     0.24 |
|          &#39;List test&#39; |  24.514 us | 0.1753 us | 0.1640 us |   1.00 |     0.00 |

**Increment Tests**

++x vs x++.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
| Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |
|------- |---------:|----------:|----------:|-------:|---------:|
|   x+=1 | 39.05 us | 0.6158 us | 0.5760 us |   1.01 |     0.02 |
|    x++ | 38.77 us | 0.1882 us | 0.1761 us |   1.00 |     0.00 |
|    ++x | 38.73 us | 0.1355 us | 0.1201 us |   1.00 |     0.01 |

**List Interface Iteration**

Checks speed differences between taking a list and casting it as the various interfaces that it implements.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|                           Method |        Mean |      Error |     StdDev | Scaled | ScaledSD |
|--------------------------------- |------------:|-----------:|-----------:|-------:|---------:|
|            &#39;ICollection ForEach&#39; | 1,314.13 us |  8.5560 us |  8.0033 us |  17.95 |     0.16 |
|         &#39;ICollection&lt;T&gt; ForEach&#39; |   850.60 us |  9.2155 us |  8.6201 us |  11.62 |     0.14 |
|            &#39;IEnumerable ForEach&#39; | 1,253.69 us |  6.0994 us |  5.7054 us |  17.13 |     0.14 |
|         &#39;IEnumerable&lt;T&gt; ForEach&#39; |   836.57 us |  1.5995 us |  1.4179 us |  11.43 |     0.08 |
|                  &#39;IList ForEach&#39; | 1,253.78 us |  5.1602 us |  4.8268 us |  17.13 |     0.13 |
|               &#39;IList&lt;T&gt; ForEach&#39; |   860.66 us | 16.7702 us | 15.6868 us |  11.76 |     0.22 |
|                   &#39;IList&lt;T&gt; For&#39; |   331.26 us |  1.7379 us |  1.5406 us |   4.53 |     0.04 |
|                      &#39;IList For&#39; |   669.26 us |  5.6722 us |  5.0283 us |   9.14 |     0.09 |
| &#39;IReadOnlyCollection&lt;T&gt; ForEach&#39; |   836.35 us |  4.4040 us |  3.9041 us |  11.43 |     0.09 |
|       &#39;IReadOnlyList&lt;T&gt; ForEach&#39; |   843.92 us |  9.5209 us |  8.9059 us |  11.53 |     0.14 |
|           &#39;IReadOnlyList&lt;T&gt; For&#39; |   327.89 us |  2.3182 us |  2.1684 us |   4.48 |     0.04 |
|                &#39;List&lt;T&gt; ForEach&#39; |   240.46 us |  1.6983 us |  1.5886 us |   3.29 |     0.03 |
|                    &#39;List&lt;T&gt; For&#39; |    73.20 us |  0.5449 us |  0.5097 us |   1.00 |     0.00 |


**List vs Dictionary**

Checks the difference between using a dictionary, list, collection, hashset, immutable dictionary, immutable list, etc. to find an item.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                          Method |         Mean |      Error |     StdDev | Scaled | ScaledSD |
|-------------------------------- |-------------:|-----------:|-----------:|-------:|---------:|
|            &#39;ArrayList for loop&#39; |    687.45 ns | 13.7627 ns | 18.8386 ns |   0.35 |     0.01 |
|                Collection.First |  1,931.80 ns | 14.5840 ns | 13.6419 ns |   0.99 |     0.02 |
|       Collection.FirstOrDefault |  1,880.47 ns | 19.8301 ns | 18.5491 ns |   0.96 |     0.02 |
|           &#39;Collection for loop&#39; |    869.39 ns |  7.8212 ns |  7.3160 ns |   0.45 |     0.01 |
|                  &#39;Dictionary[]&#39; |     26.09 ns |  0.2168 ns |  0.1922 ns |   0.01 |     0.00 |
|          Dictionary.TryGetValue |     28.76 ns |  0.5766 ns |  0.5663 ns |   0.01 |     0.00 |
|                   HashSet.First |  1,946.85 ns |  9.2551 ns |  8.6572 ns |   1.00 |     0.02 |
|          HashSet.FirstOrDefault |  1,948.84 ns | 13.6800 ns | 12.7963 ns |   1.00 |     0.02 |
|         &#39;ImmutableDictionary[]&#39; |    133.17 ns |  1.4468 ns |  1.3533 ns |   0.07 |     0.00 |
| ImmutableDictionary.TryGetValue |    160.29 ns |  3.1538 ns |  2.9500 ns |   0.08 |     0.00 |
|             ImmutableList.First | 14,870.36 ns | 68.5722 ns | 60.7874 ns |   7.62 |     0.14 |
|    ImmutableList.FirstOrDefault | 14,934.31 ns | 91.8522 ns | 85.9186 ns |   7.66 |     0.14 |
|        &#39;ImmutableList for loop&#39; |  4,440.87 ns | 83.3415 ns | 89.1745 ns |   2.28 |     0.06 |
|                      List.First |  1,951.40 ns | 37.0518 ns | 34.6583 ns |   1.00 |     0.00 |
|             List.FirstOrDefault |  1,955.76 ns | 23.2933 ns | 21.7886 ns |   1.00 |     0.02 |
|                 &#39;List for loop&#39; |    334.62 ns |  3.8666 ns |  3.4277 ns |   0.17 |     0.00 |


**Loop Tests**

Checks foreach, for, and an "optimized" for loop.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                   Method |     Mean |     Error |    StdDev | Scaled |
|------------------------- |---------:|----------:|----------:|-------:|
|           &#39;Foreach loop&#39; | 7.044 us | 0.0700 us | 0.0655 us |   1.00 |
|               &#39;For loop&#39; | 3.862 us | 0.0243 us | 0.0216 us |   0.55 |
| &#39;For loop, local length&#39; | 3.889 us | 0.0589 us | 0.0551 us |   0.55 |


**MemoryStream vs RecyclableMemoryStream**

Checks the difference between the two types of streams at different data sizes.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|                 Method |    Size |            Mean |        Error |       StdDev | Scaled | ScaledSD |
|----------------------- |-------- |----------------:|-------------:|-------------:|-------:|---------:|
|           **MemoryStream** |     **100** |        **96.31 ns** |     **1.214 ns** |     **1.136 ns** |   **1.00** |     **0.00** |
| RecyclableMemoryStream |     100 |     2,718.87 ns |    12.659 ns |    11.842 ns |  28.23 |     0.34 |
|                        |         |                 |              |              |        |          |
|           **MemoryStream** |    **1000** |       **249.28 ns** |     **4.390 ns** |     **4.106 ns** |   **1.00** |     **0.00** |
| RecyclableMemoryStream |    1000 |     2,878.97 ns |    34.171 ns |    31.963 ns |  11.55 |     0.22 |
|                        |         |                 |              |              |        |          |
|           **MemoryStream** |   **10000** |     **1,835.50 ns** |    **21.668 ns** |    **19.208 ns** |   **1.00** |     **0.00** |
| RecyclableMemoryStream |   10000 |     4,063.93 ns |    46.971 ns |    43.936 ns |   2.21 |     0.03 |
|                        |         |                 |              |              |        |          |
|           **MemoryStream** |  **100000** |   **103,324.66 ns** | **1,706.558 ns** | **1,596.316 ns** |   **1.00** |     **0.00** |
| RecyclableMemoryStream |  100000 |    59,144.18 ns |   802.453 ns |   750.615 ns |   0.57 |     0.01 |
|                        |         |                 |              |              |        |          |
|           **MemoryStream** | **1000000** | **1,022,879.59 ns** | **6,444.208 ns** | **6,027.916 ns** |   **1.00** |     **0.00** |
| RecyclableMemoryStream | 1000000 |   625,839.68 ns | 5,817.796 ns | 5,441.969 ns |   0.61 |     0.01 |


**Method Call Tests**

Checks different ways to call a method and the speed hits involved.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|                  Method |         Mean |        Error |       StdDev | Scaled | ScaledSD |
|------------------------ |-------------:|-------------:|-------------:|-------:|---------:|
| &#39;Direct call to method&#39; |     391.3 ns |     4.896 ns |     4.579 ns |   1.00 |     0.00 |
|            Func(Method) |  11,434.2 ns |   220.816 ns |   262.866 ns |  29.22 |     0.73 |
|              Func(_=&gt;_) |   3,147.0 ns |    21.787 ns |    20.380 ns |   8.04 |     0.10 |
|          &#39;Local method&#39; |     403.1 ns |     3.030 ns |     2.686 ns |   1.03 |     0.01 |
|    &#39;MethodInfo, cached&#39; | 112,964.6 ns | 2,084.961 ns | 1,950.274 ns | 288.73 |     5.81 |


**Modify Pixel**

More involved test for iterating an array and modifying individual cells. In this case loading an image using Skia.Sharp and changing each pixel's value to (1,1,1,1).

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                                                           Method | HeightOfArea | WidthOfArea |          Mean |      Error |     StdDev | Scaled |
|----------------------------------------------------------------- |------------- |------------ |--------------:|-----------:|-----------:|-------:|
|                            &#39;Group SetPixel method on the bitmap&#39; |         1198 |         804 |   1,288.38 us |   6.676 us |   6.245 us |  0.012 |
|             &#39;Parallel Struct SetPixel UInt method on the bitmap&#39; |         1198 |         804 |     166.57 us |   3.182 us |   3.788 us |  0.002 |
| &#39;Parallel Struct SetPixel UInt Partitioned method on the bitmap&#39; |         1198 |         804 |     177.72 us |   3.482 us |   4.010 us |  0.002 |
|            &#39;Parallel Struct SetPixel ULong method on the bitmap&#39; |         1198 |         804 |      96.70 us |   1.817 us |   1.699 us |  0.001 |
|                                  &#39;SetPixel method on the bitmap&#39; |         1198 |         804 | 110,074.59 us | 597.110 us | 529.322 us |  1.000 |
|                &#39;Struct SetPixel Individual method on the bitmap&#39; |         1198 |         804 |     896.20 us |   7.251 us |   6.428 us |  0.008 |
|                      &#39;Struct SetPixel UInt method on the bitmap&#39; |         1198 |         804 |     528.19 us |   4.285 us |   4.008 us |  0.005 |
|                               &#39;Span SetPixel UInt on the bitmap&#39; |         1198 |         804 |     673.07 us |  11.282 us |  10.553 us |  0.006 |
|                           &#39;Unsafe SetPixel method on the bitmap&#39; |         1198 |         804 |  71,452.81 us | 320.131 us | 267.324 us |  0.649 |



**Object Creation Tests**

Checks different ways to create an object.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                                                   Method |            Mean |         Error |        StdDev | Scaled | ScaledSD |
|--------------------------------------------------------- |----------------:|--------------:|--------------:|-------:|---------:|
|      &#39;Create object using Activator.CreateInstance&lt;T&gt;()&#39; |      96.2011 ns |     0.5472 ns |     0.4851 ns |      ? |        ? |
|         &#39;Create object using Activator.CreateInstance()&#39; |      82.7740 ns |     1.1506 ns |     1.0763 ns |      ? |        ? |
|   &#39;Create object using ConstructorInfo, cached in field&#39; |     188.8986 ns |     1.6115 ns |     1.5074 ns |      ? |        ? |
| &#39;Create object using compiled lambda expression, cached&#39; |       6.7007 ns |     0.0488 ns |     0.0456 ns |      ? |        ? |
|             &#39;Create object using ConstructorInfo cached&#39; |     205.6568 ns |     1.8556 ns |     1.7357 ns |      ? |        ? |
|                    &#39;Create object using ConstructorInfo&#39; |     310.8729 ns |     2.3737 ns |     1.9821 ns |      ? |        ? |
|                  &#39;Create object using FormatterServices&#39; |      83.5138 ns |     1.0812 ns |     0.9585 ns |      ? |        ? |
|         &#39;Create object using compiled lambda expression&#39; | 139,253.1234 ns | 1,103.8442 ns | 1,032.5365 ns |      ? |        ? |
|                                &#39;Create object using new&#39; |       0.0275 ns |     0.0219 ns |     0.0205 ns |      ? |        ? |

**Partition Parallel**

Checks the difference between using partitions and not in basic TPL use.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                         Method |      Mean |     Error |   StdDev | Scaled | ScaledSD |
|------------------------------- |----------:|----------:|---------:|-------:|---------:|
|                   Parallel.For | 132.99 us | 2.6262 us | 6.343 us |   1.00 |     0.00 |
|               Parallel.ForEach | 237.49 us | 3.2358 us | 3.726 us |   1.79 |     0.09 |
| &#39;Parallel.ForEach partitioned&#39; |  29.31 us | 0.5849 us | 1.154 us |   0.22 |     0.01 |


**Pattern Matching Tests**

Checks if there is any overhead in using pattern matching.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|             Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |
|------------------- |---------:|----------:|----------:|-------:|---------:|
|                 As | 5.493 ns | 0.1548 ns | 0.1958 ns |   1.02 |     0.04 |
|                 Is | 5.780 ns | 0.1454 ns | 0.1360 ns |   1.07 |     0.03 |
| &#39;Pattern matching&#39; | 5.403 ns | 0.1262 ns | 0.1180 ns |   1.00 |     0.00 |


**Read File**

Checks a couple of ways to read a file. In this case the file was a 10MB text file.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                           Method |       Mean |     Error |    StdDev | Scaled | ScaledSD |
|--------------------------------- |-----------:|----------:|----------:|-------:|---------:|
|                        File.Read |   7.767 ms | 0.1355 ms | 0.1267 ms |   1.00 |     0.00 |
|                   File.ReadAsync |   7.654 ms | 0.0557 ms | 0.0521 ms |   0.99 |     0.02 |
|        &#39;File.Read in loop, 1024&#39; |  10.988 ms | 0.0466 ms | 0.0436 ms |   1.42 |     0.02 |
| &#39;File.Read in loop, 1024, async&#39; |  50.142 ms | 0.1867 ms | 0.1559 ms |   6.46 |     0.10 |
|        &#39;File.Read in loop, 2048&#39; |  10.841 ms | 0.0504 ms | 0.0472 ms |   1.40 |     0.02 |
| &#39;File.Read in loop, 2048, async&#39; |  33.197 ms | 0.3362 ms | 0.3145 ms |   4.28 |     0.08 |
|        &#39;File.Read in loop, 4096&#39; |  10.447 ms | 0.0682 ms | 0.0638 ms |   1.35 |     0.02 |
| &#39;File.Read in loop, 4096, async&#39; |  22.586 ms | 0.1578 ms | 0.1399 ms |   2.91 |     0.05 |
|                    File.OpenRead |   7.529 ms | 0.0466 ms | 0.0436 ms |   0.97 |     0.02 |
|                File.ReadAllBytes |   7.680 ms | 0.0970 ms | 0.0907 ms |   0.99 |     0.02 |
|           File.ReadAllBytesAsync |   7.684 ms | 0.0736 ms | 0.0689 ms |   0.99 |     0.02 |
|       &#39;File.ReadAllBytes static&#39; |   7.733 ms | 0.0358 ms | 0.0335 ms |   1.00 |     0.02 |
|                File.ReadAllLines | 198.606 ms | 0.9428 ms | 0.8819 ms |  25.58 |     0.41 |
|           File.ReadAllLinesAsync | 562.203 ms | 2.4535 ms | 2.2950 ms |  72.41 |     1.16 |
|                 File.ReadAllText |  47.746 ms | 0.3481 ms | 0.2907 ms |   6.15 |     0.10 |
|            File.ReadAllTextAsync |  97.800 ms | 1.1389 ms | 0.9511 ms |  12.60 |     0.23 |

**Reflection Caching Tests**

Checks the difference between caching reflection info vs not.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|           Method |      Mean |     Error |    StdDev |
|----------------- |----------:|----------:|----------:|
| CachedReflection |  3.528 ns | 0.0701 ns | 0.0655 ns |
| NormalReflection | 96.448 ns | 0.9920 ns | 0.9279 ns |


**Span vs Array**

The difference between an array and a span in basic speed tests.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|                Method |      Mean |     Error |    StdDev | Scaled |
|---------------------- |----------:|----------:|----------:|-------:|
|                 Array | 10.029 us | 0.1051 us | 0.0932 us |   1.00 |
| &#39;Array With Pointers&#39; |  7.008 us | 0.0319 us | 0.0267 us |   0.70 |
|                  Span | 10.208 us | 0.0886 us | 0.0829 us |   1.02 |
|     &#39;Span With Slice&#39; |  9.516 us | 0.1074 us | 0.0952 us |   0.95 |

**StackAlloc vs New**

Tests a couple of different strategies for creating objects to see if there is much of a difference.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|              Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |
|-------------------- |---------:|----------:|----------:|-------:|---------:|
|                 New | 44.85 us | 0.3669 us | 0.3432 us |   0.98 |     0.02 |
| &#39;Stackalloc struct&#39; | 44.74 us | 0.6166 us | 0.5767 us |   0.97 |     0.02 |
|          Stackalloc | 45.97 us | 0.8195 us | 0.7665 us |   1.00 |     0.00 |

**Static Contructor Tests**

Checks if there is a speed difference between having a static constructor or not when calling a static class method.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|             Method |       Mean |     Error |   StdDev | Scaled | ScaledSD |
|------------------- |-----------:|----------:|---------:|-------:|---------:|
|   &#39;No constructor&#39; |   454.9 ns |  8.884 ns | 11.86 ns |   1.00 |     0.00 |
| &#39;With constructor&#39; | 2,261.7 ns | 36.324 ns | 33.98 ns |   4.97 |     0.15 |

**String Concat Tests**

Tests a couple of different ways to concat a string.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|             Method |       Mean |     Error |    StdDev | Scaled |
|------------------- |-----------:|----------:|----------:|-------:|
| &#39;char list concat&#39; |  11.763 us | 0.1895 us | 0.1772 us |   0.03 |
|      StringBuilder |   9.008 us | 0.0812 us | 0.0720 us |   0.03 |
|    &#39;string concat&#39; | 356.834 us | 4.2936 us | 3.3522 us |   1.00 |

**String Formatting**

Tests a couple of different ways to format a string.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|                 Method |       Mean |     Error |    StdDev | Scaled | ScaledSD |
|----------------------- |-----------:|----------:|----------:|-------:|---------:|
|          StringBuilder | 1,120.9 ns | 15.325 ns | 14.335 ns |   0.98 |     0.02 |
|        &#39;string concat&#39; |   692.4 ns |  5.512 ns |  5.156 ns |   0.61 |     0.01 |
|          string.format | 1,139.8 ns | 12.257 ns | 11.465 ns |   1.00 |     0.00 |
| &#39;string interpolation&#39; | 1,131.4 ns |  6.352 ns |  4.959 ns |   0.99 |     0.01 |
|         string.replace | 1,137.5 ns | 11.852 ns | 11.087 ns |   1.00 |     0.01 |


**String Substring**

Checks how to get a substring of a string.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|           Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |
|----------------- |---------:|----------:|----------:|-------:|---------:|
|       Span.Slice | 35.59 ns | 0.5586 ns | 0.5225 ns |   1.91 |     0.03 |
| String.Substring | 18.67 ns | 0.2402 ns | 0.2129 ns |   1.00 |     0.00 |

**String Trim Tests**

Checks a couple of ways to trim a string.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                       Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |
|----------------------------- |---------:|----------:|----------:|-------:|---------:|
|      &#39;String substring trim&#39; | 41.58 ns | 0.3471 ns | 0.3077 ns |   1.11 |     0.01 |
|                String.Trim() | 37.59 ns | 0.2604 ns | 0.2309 ns |   1.00 |     0.00 |
| String.TrimStart().TrimEnd() | 60.85 ns | 1.0463 ns | 0.9788 ns |   1.62 |     0.03 |

**Vector vs Byte Math**

Basic tests for SIMD backed Vector vs basic integer based math.

``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-6300HQ CPU 2.30GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2250004 Hz, Resolution=444.4437 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|               Method |     Mean |    Error |    StdDev |   Median | Scaled | ScaledSD |
|--------------------- |---------:|---------:|----------:|---------:|-------:|---------:|
|        &#39;byte[] test&#39; | 480.8 us | 7.887 us |  7.377 us | 483.2 us |   1.00 |     0.00 |
|        &#39;Struct test&#39; | 369.9 us | 4.004 us |  3.549 us | 370.5 us |   0.77 |     0.01 |
|   &#39;Struct UInt test&#39; | 231.9 us | 4.594 us |  9.790 us | 228.8 us |   0.48 |     0.02 |
|  &#39;Vector&lt;byte&gt; test&#39; | 353.5 us | 7.845 us | 22.636 us | 345.1 us |   0.74 |     0.05 |
| &#39;Vector&lt;float&gt; test&#39; | 343.0 us | 5.328 us |  4.723 us | 343.1 us |   0.71 |     0.01 |
|  &#39;Vector&lt;uint&gt; test&#39; | 341.5 us | 6.601 us |  7.337 us | 341.4 us |   0.71 |     0.02 |
