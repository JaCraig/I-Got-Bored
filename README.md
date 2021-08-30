This is the repository for testing various speed improvements for Structure.Sketching and other libs. The results can be found below the conclussions section.

# Conclussions

1. When dealing with arrays of data inside of a method, use ArrayPool to allocate.

2. Array.Copy is generally better for smaller arrays. Larger arrays (100,000+), Buffer.MemoryCopy seems to be a better option.

3. For arrays, just treat them as arrays when iterating over them. IEnumerable, etc. slow them down.

4. For arrays, foreach and for have no difference at this point in time.

5. If you can, use Array.Sort and List.Sort. They're much faster than the alternatives.

6. The various old optimizations like bitwise math are no longer effective.

7. Unchecked math no longer seems to be substantially faster.

8. Structs are much faster to create than classes. Setting or getting values is about the same though.

9. For ConcurrentDictionary, use TryGetValue over an index or key lookup.

10. For Dictionary, the type of the key matters. Long, uint, ulong, etc. are much faster than string.

11. Dictionary is much faster than ConcurrentDictionary. Only use ConcurrentDictionary when you need to.

12. Dynamic vs static typing, static is about 10x faster.

13. Byte packing is faster than using an enum but it's about 4ns vs 2ns...

14. Fields are slightly faster than properties in certain circumstances.

15. Dictionary is generally faster than Hashtable if using an int as the key. Hashtable seems slightly faster when dealing with string as the key.

16. Array.Copy and List.CopyTo are much faster at copying data over than other options.

17. Arrays are much faster than alternatives for iteration.

18. ++x or x++ makes no difference.

19. Use 'is' instead of any of the clever other options if you can for determining the type of an object.

20. If you can, use List.AddRange instead of List.Add.

21. Using a for loop with Lists is the fastest approach.

22. If you need something that looks like a Dictionary, then just use a Dictionary...

23. If you're going to create a list of items, use AddRange instead of feeding them into the constructor.

24. For small items, MemoryStream is better than RecyclableMemoryStream. For larger items, RecyclableMemoryStream is much better though.

25. In terms of what is better (best to worst): Direct Method Call > Func(=>) > Func(Method) > Cached MethodInfo > new Func(=>) > new Func(Method).

26. For null equality, use either is or ReferenceEquals.

27. If you need to create an object, just use new. If you can't use new, compile/cache a lambda expression.

28. Use partitioning when doing Parallel.ForEach if you need to worry about speed but note there will be a slight memory increase.

29. If you're reading in a whole file, create the whole file in one go. This does not apply for instances where you are streaming the file or modifying portions of it.

30. When you can, cache type info as it's not free.

31. When you can, use a simple Contains instead of using Regex.

32. using pointers into an array is slightly faster than a Span.

33. stackalloc seems to be a speed boost at larger number of items.

34. Do not use a statuc constructor if you don't need to. It gets called every time you call a method on the class.

35. Use a StringBuilder instead of string concat. Memory and speed improvements for anything 100+ concatenations.

36. If you're formatting a string, string concat is usually faster than things like string interpolation.

37. If you can use a StringBuilderPool. Faster and memory improvements to be had.

38. If you are going to replace a value in a string, use string.Replace instead of StringBuilder.Replace.

39. String.Substring works slightly better than Span.Slice.

40. Using string.Substring works better than Trim for small strings. Larger strings, Trim works better.

41. Vector works better than dealing with straight struct, byte array, etc. in terms of speed. Memory increases though.

# Results

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.1165 (20H2/October2020Update)
Intel Core i7-9850H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.303
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  DefaultJob : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT


```
**Allocation Strategy Tests**


|                                           Method |  Count |          Mean |         Error |        StdDev |       StdErr |        Median |           Min |            Q1 |            Q3 |           Max |         Op/s | Ratio | RatioSD | Rank |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|------------------------------------------------- |------- |--------------:|--------------:|--------------:|-------------:|--------------:|--------------:|--------------:|--------------:|--------------:|-------------:|------:|--------:|-----:|---------:|---------:|---------:|----------:|
|                             **&#39;Allocate as needed&#39;** |    **100** |      **56.12 ns** |      **1.134 ns** |      **1.662 ns** |     **0.309 ns** |      **55.71 ns** |      **53.39 ns** |      **54.98 ns** |      **57.10 ns** |      **60.07 ns** | **17,817,409.3** |  **1.00** |    **0.00** |    **2** |   **0.0675** |        **-** |        **-** |     **424 B** |
|                                        ArrayPool |    100 |      52.99 ns |      1.049 ns |      0.981 ns |     0.253 ns |      53.08 ns |      51.72 ns |      52.13 ns |      53.51 ns |      55.33 ns | 18,870,851.6 |  0.93 |    0.03 |    1 |        - |        - |        - |         - |
| &#39;Item from pointer to preallocated array &#39;pool&#39;&#39; |    100 |     102.99 ns |      1.567 ns |      1.389 ns |     0.371 ns |     103.37 ns |     101.09 ns |     101.57 ns |     103.84 ns |     105.31 ns |  9,709,521.8 |  1.80 |    0.05 |    4 |        - |        - |        - |         - |
|            &#39;Span from preallocated array &#39;pool&#39;&#39; |    100 |      99.15 ns |      1.173 ns |      1.040 ns |     0.278 ns |      98.87 ns |      97.79 ns |      98.55 ns |      99.43 ns |     101.05 ns | 10,085,768.4 |  1.73 |    0.04 |    3 |        - |        - |        - |         - |
|                                                  |        |               |               |               |              |               |               |               |               |               |              |       |         |      |          |          |          |           |
|                             **&#39;Allocate as needed&#39;** |   **1000** |     **534.01 ns** |     **52.968 ns** |    **154.511 ns** |    **15.608 ns** |     **482.06 ns** |     **377.06 ns** |     **402.84 ns** |     **642.89 ns** |     **939.27 ns** |  **1,872,612.8** |  **1.00** |    **0.00** |    **2** |   **0.6413** |        **-** |        **-** |   **4,024 B** |
|                                        ArrayPool |   1000 |     285.37 ns |      4.622 ns |      8.682 ns |     1.309 ns |     283.82 ns |     276.96 ns |     279.07 ns |     286.53 ns |     317.50 ns |  3,504,181.7 |  0.61 |    0.13 |    1 |        - |        - |        - |         - |
| &#39;Item from pointer to preallocated array &#39;pool&#39;&#39; |   1000 |   1,006.20 ns |      7.074 ns |      6.271 ns |     1.676 ns |   1,005.33 ns |     996.64 ns |   1,003.08 ns |   1,009.33 ns |   1,018.39 ns |    993,841.9 |  1.67 |    0.26 |    3 |        - |        - |        - |         - |
|            &#39;Span from preallocated array &#39;pool&#39;&#39; |   1000 |   1,004.87 ns |     16.886 ns |     15.795 ns |     4.078 ns |   1,000.94 ns |     985.84 ns |     993.87 ns |   1,015.74 ns |   1,030.93 ns |    995,155.5 |  1.65 |    0.26 |    3 |        - |        - |        - |         - |
|                                                  |        |               |               |               |              |               |               |               |               |               |              |       |         |      |          |          |          |           |
|                             **&#39;Allocate as needed&#39;** |  **10000** |   **3,618.63 ns** |     **66.337 ns** |    **110.834 ns** |    **18.472 ns** |   **3,594.44 ns** |   **3,435.67 ns** |   **3,551.89 ns** |   **3,701.35 ns** |   **3,909.66 ns** |    **276,347.4** |  **1.00** |    **0.00** |    **2** |   **6.3248** |        **-** |        **-** |  **40,024 B** |
|                                        ArrayPool |  10000 |   2,471.36 ns |     22.579 ns |     21.120 ns |     5.453 ns |   2,468.43 ns |   2,447.23 ns |   2,451.19 ns |   2,478.83 ns |   2,514.19 ns |    404,635.3 |  0.67 |    0.02 |    1 |        - |        - |        - |         - |
| &#39;Item from pointer to preallocated array &#39;pool&#39;&#39; |  10000 |  10,991.06 ns |    214.129 ns |    552.736 ns |    62.585 ns |  10,775.12 ns |  10,436.06 ns |  10,663.93 ns |  10,981.38 ns |  12,812.01 ns |     90,983.1 |  3.13 |    0.16 |    3 |        - |        - |        - |         - |
|            &#39;Span from preallocated array &#39;pool&#39;&#39; |  10000 |  10,824.17 ns |    199.923 ns |    187.008 ns |    48.285 ns |  10,855.97 ns |  10,534.45 ns |  10,704.88 ns |  10,940.28 ns |  11,215.16 ns |     92,385.8 |  2.92 |    0.09 |    3 |        - |        - |        - |         - |
|                                                  |        |               |               |               |              |               |               |               |               |               |              |       |         |      |          |          |          |           |
|                             **&#39;Allocate as needed&#39;** | **100000** |  **44,122.70 ns** |  **1,758.718 ns** |  **5,185.619 ns** |   **518.562 ns** |  **41,278.75 ns** |  **39,662.34 ns** |  **40,671.17 ns** |  **47,442.11 ns** |  **57,506.78 ns** |     **22,664.1** |  **1.00** |    **0.00** |    **2** | **124.9390** | **124.9390** | **124.9390** | **400,024 B** |
|                                        ArrayPool | 100000 |  36,445.91 ns |  3,452.459 ns | 10,179.654 ns | 1,017.965 ns |  33,107.53 ns |  25,222.46 ns |  28,612.70 ns |  45,130.26 ns |  69,482.34 ns |     27,437.9 |  0.84 |    0.25 |    1 |        - |        - |        - |      16 B |
| &#39;Item from pointer to preallocated array &#39;pool&#39;&#39; | 100000 | 162,389.81 ns | 12,894.135 ns | 37,408.224 ns | 3,798.230 ns | 147,649.68 ns | 116,530.59 ns | 132,376.15 ns | 192,439.43 ns | 275,619.95 ns |      6,158.0 |  3.65 |    0.59 |    3 |        - |        - |        - |         - |
|            &#39;Span from preallocated array &#39;pool&#39;&#39; | 100000 | 202,003.98 ns | 11,628.267 ns | 34,286.209 ns | 3,428.621 ns | 204,942.19 ns | 136,907.60 ns | 188,273.36 ns | 225,580.81 ns | 273,378.09 ns |      4,950.4 |  4.62 |    0.87 |    4 |        - |        - |        - |         - |

----------
**Array Copy Tests**


|                            Method |  Count |         Mean |      Error |     StdDev |     StdErr |       Median |          Min |           Q1 |           Q3 |          Max |          Op/s | Ratio | RatioSD | Rank | Allocated |
|---------------------------------- |------- |-------------:|-----------:|-----------:|-----------:|-------------:|-------------:|-------------:|-------------:|-------------:|--------------:|------:|--------:|-----:|----------:|
|         **&#39;Copy using Array.Copy()&#39;** |    **100** |     **4.759 ns** |  **0.2609 ns** |  **0.7315 ns** |  **0.0767 ns** |     **4.394 ns** |     **4.228 ns** |     **4.324 ns** |     **4.774 ns** |     **6.613 ns** | **210,129,894.8** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Copy using Buffer.MemoryCopy&lt;T&gt;&#39; |    100 |     5.064 ns |  0.0569 ns |  0.0504 ns |  0.0135 ns |     5.065 ns |     4.994 ns |     5.021 ns |     5.103 ns |     5.143 ns | 197,488,456.1 |  0.80 |    0.04 |    2 |         - |
|                                   |        |              |            |            |            |              |              |              |              |              |               |       |         |      |           |
|         **&#39;Copy using Array.Copy()&#39;** |   **1000** |    **24.799 ns** |  **1.4591 ns** |  **4.3023 ns** |  **0.4302 ns** |    **23.771 ns** |    **20.163 ns** |    **20.548 ns** |    **28.693 ns** |    **37.130 ns** |  **40,324,638.9** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Copy using Buffer.MemoryCopy&lt;T&gt;&#39; |   1000 |    28.366 ns |  0.2915 ns |  0.2727 ns |  0.0704 ns |    28.387 ns |    27.777 ns |    28.198 ns |    28.568 ns |    28.790 ns |  35,253,247.0 |  1.06 |    0.23 |    2 |         - |
|                                   |        |              |            |            |            |              |              |              |              |              |               |       |         |      |           |
|         **&#39;Copy using Array.Copy()&#39;** |  **10000** |   **146.887 ns** |  **0.7626 ns** |  **0.7133 ns** |  **0.1842 ns** |   **146.974 ns** |   **145.934 ns** |   **146.208 ns** |   **147.426 ns** |   **148.033 ns** |   **6,807,943.3** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Copy using Buffer.MemoryCopy&lt;T&gt;&#39; |  10000 |   145.307 ns |  1.2286 ns |  1.0891 ns |  0.2911 ns |   145.052 ns |   143.987 ns |   144.487 ns |   145.735 ns |   147.334 ns |   6,881,989.7 |  0.99 |    0.01 |    1 |         - |
|                                   |        |              |            |            |            |              |              |              |              |              |               |       |         |      |           |
|         **&#39;Copy using Array.Copy()&#39;** | **100000** | **2,466.015 ns** | **48.6846 ns** | **43.1577 ns** | **11.5344 ns** | **2,462.106 ns** | **2,385.767 ns** | **2,440.302 ns** | **2,503.870 ns** | **2,534.995 ns** |     **405,512.5** |  **1.00** |    **0.00** |    **2** |         **-** |
| &#39;Copy using Buffer.MemoryCopy&lt;T&gt;&#39; | 100000 | 1,953.516 ns | 39.0490 ns | 82.3676 ns | 11.2088 ns | 1,924.865 ns | 1,856.736 ns | 1,898.436 ns | 1,984.247 ns | 2,168.823 ns |     511,897.6 |  0.83 |    0.04 |    1 |         - |

----------
**Array Interface Iteration**


|                           Method |  Count |            Mean |          Error |         StdDev |        StdErr |          Median |             Min |              Q1 |              Q3 |             Max |         Op/s |  Ratio | RatioSD | Rank |    Gen 0 |   Allocated |
|--------------------------------- |------- |----------------:|---------------:|---------------:|--------------:|----------------:|----------------:|----------------:|----------------:|----------------:|-------------:|-------:|--------:|-----:|---------:|------------:|
|                    **&#39;T[] ForEach&#39;** |    **100** |        **33.14 ns** |       **0.353 ns** |       **0.330 ns** |      **0.085 ns** |        **33.09 ns** |        **32.52 ns** |        **32.93 ns** |        **33.36 ns** |        **33.74 ns** | **30,174,718.9** |   **0.99** |    **0.02** |    **1** |        **-** |           **-** |
|                        &#39;T[] For&#39; |    100 |        33.44 ns |       0.681 ns |       0.637 ns |      0.165 ns |        33.26 ns |        32.78 ns |        32.93 ns |        33.77 ns |        34.69 ns | 29,901,335.1 |   1.00 |    0.00 |    1 |        - |           - |
|            &#39;ICollection ForEach&#39; |    100 |     6,305.65 ns |     125.607 ns |     315.122 ns |     36.632 ns |     6,236.68 ns |     5,762.93 ns |     6,079.30 ns |     6,527.53 ns |     7,214.67 ns |    158,587.9 | 190.07 |   12.52 |    8 |   0.3815 |     2,432 B |
|         &#39;ICollection&lt;T&gt; ForEach&#39; |    100 |       403.83 ns |       7.136 ns |       7.931 ns |      1.820 ns |       401.21 ns |       394.14 ns |       399.84 ns |       406.20 ns |       420.56 ns |  2,476,293.8 |  12.13 |    0.39 |    4 |   0.0048 |        32 B |
|            &#39;IEnumerable ForEach&#39; |    100 |     4,715.60 ns |      40.706 ns |      38.077 ns |      9.831 ns |     4,706.11 ns |     4,663.34 ns |     4,687.45 ns |     4,741.61 ns |     4,783.22 ns |    212,062.1 | 141.04 |    2.37 |    6 |   0.3815 |     2,432 B |
|         &#39;IEnumerable&lt;T&gt; ForEach&#39; |    100 |       402.51 ns |       6.406 ns |       5.992 ns |      1.547 ns |       401.63 ns |       393.81 ns |       397.50 ns |       407.22 ns |       413.97 ns |  2,484,389.1 |  12.04 |    0.22 |    4 |   0.0048 |        32 B |
|                  &#39;IList ForEach&#39; |    100 |     5,366.09 ns |     265.262 ns |     782.130 ns |     78.213 ns |     5,400.17 ns |     4,317.29 ns |     4,594.29 ns |     5,869.63 ns |     7,502.37 ns |    186,355.3 | 154.27 |   23.81 |    7 |   0.3815 |     2,432 B |
|               &#39;IList&lt;T&gt; ForEach&#39; |    100 |       384.35 ns |       4.120 ns |       3.854 ns |      0.995 ns |       384.89 ns |       379.21 ns |       381.11 ns |       386.67 ns |       393.07 ns |  2,601,765.2 |  11.50 |    0.24 |    4 |   0.0048 |        32 B |
|                   &#39;IList&lt;T&gt; For&#39; |    100 |       224.63 ns |       2.039 ns |       1.907 ns |      0.492 ns |       224.10 ns |       222.21 ns |       223.18 ns |       225.81 ns |       229.10 ns |  4,451,844.7 |   6.72 |    0.11 |    2 |        - |           - |
|                      &#39;IList For&#39; |    100 |     4,232.43 ns |      78.986 ns |      84.514 ns |     19.920 ns |     4,218.67 ns |     4,094.04 ns |     4,179.01 ns |     4,277.19 ns |     4,409.10 ns |    236,271.1 | 126.68 |    2.59 |    5 |   0.3815 |     2,400 B |
| &#39;IReadOnlyCollection&lt;T&gt; ForEach&#39; |    100 |       390.49 ns |       6.611 ns |       6.184 ns |      1.597 ns |       393.31 ns |       380.75 ns |       385.73 ns |       394.70 ns |       400.34 ns |  2,560,874.6 |  11.68 |    0.23 |    4 |   0.0048 |        32 B |
|       &#39;IReadOnlyList&lt;T&gt; ForEach&#39; |    100 |       401.08 ns |       8.054 ns |      17.679 ns |      2.321 ns |       398.32 ns |       377.68 ns |       389.95 ns |       405.86 ns |       458.52 ns |  2,493,246.7 |  12.24 |    0.86 |    4 |   0.0048 |        32 B |
|           &#39;IReadOnlyList&lt;T&gt; For&#39; |    100 |       229.88 ns |       3.120 ns |       2.605 ns |      0.722 ns |       230.18 ns |       224.44 ns |       229.72 ns |       231.03 ns |       233.67 ns |  4,350,109.9 |   6.89 |    0.19 |    3 |        - |           - |
|                                  |        |                 |                |                |               |                 |                 |                 |                 |                 |              |        |         |      |          |             |
|                    **&#39;T[] ForEach&#39;** |   **1000** |       **280.47 ns** |       **8.572 ns** |      **24.731 ns** |      **2.524 ns** |       **275.91 ns** |       **248.41 ns** |       **258.68 ns** |       **299.31 ns** |       **367.93 ns** |  **3,565,396.1** |   **1.08** |    **0.08** |    **2** |        **-** |           **-** |
|                        &#39;T[] For&#39; |   1000 |       250.84 ns |       3.233 ns |       3.320 ns |      0.805 ns |       250.15 ns |       245.45 ns |       249.23 ns |       251.32 ns |       257.89 ns |  3,986,647.4 |   1.00 |    0.00 |    1 |        - |           - |
|            &#39;ICollection ForEach&#39; |   1000 |    50,343.64 ns |   1,967.893 ns |   5,802.377 ns |    580.238 ns |    46,996.89 ns |    44,401.92 ns |    45,482.51 ns |    55,667.83 ns |    63,158.38 ns |     19,863.5 | 206.95 |   26.56 |    7 |   3.7842 |    24,032 B |
|         &#39;ICollection&lt;T&gt; ForEach&#39; |   1000 |     3,700.79 ns |      65.625 ns |      61.385 ns |     15.850 ns |     3,674.13 ns |     3,627.54 ns |     3,657.52 ns |     3,749.90 ns |     3,835.51 ns |    270,212.9 |  14.76 |    0.20 |    4 |   0.0038 |        32 B |
|            &#39;IEnumerable ForEach&#39; |   1000 |    43,726.26 ns |     650.471 ns |     608.451 ns |    157.101 ns |    43,669.40 ns |    42,936.36 ns |    43,197.35 ns |    44,106.73 ns |    45,162.50 ns |     22,869.6 | 174.42 |    3.17 |    6 |   3.7842 |    24,032 B |
|         &#39;IEnumerable&lt;T&gt; ForEach&#39; |   1000 |     3,670.50 ns |      27.603 ns |      25.820 ns |      6.667 ns |     3,668.03 ns |     3,623.95 ns |     3,652.06 ns |     3,693.16 ns |     3,709.39 ns |    272,442.4 |  14.64 |    0.19 |    4 |   0.0038 |        32 B |
|                  &#39;IList ForEach&#39; |   1000 |    44,380.90 ns |     825.923 ns |     772.569 ns |    199.477 ns |    44,220.83 ns |    43,471.98 ns |    43,762.66 ns |    44,972.17 ns |    45,913.90 ns |     22,532.2 | 177.03 |    3.56 |    6 |   3.7842 |    24,032 B |
|               &#39;IList&lt;T&gt; ForEach&#39; |   1000 |     3,724.77 ns |      39.746 ns |      35.234 ns |      9.417 ns |     3,718.65 ns |     3,671.70 ns |     3,704.67 ns |     3,748.60 ns |     3,803.30 ns |    268,473.2 |  14.86 |    0.26 |    4 |   0.0038 |        32 B |
|                   &#39;IList&lt;T&gt; For&#39; |   1000 |     2,292.13 ns |      45.742 ns |     125.217 ns |     13.425 ns |     2,235.74 ns |     2,189.63 ns |     2,217.81 ns |     2,292.13 ns |     2,681.89 ns |    436,275.5 |   9.50 |    0.84 |    3 |        - |           - |
|                      &#39;IList For&#39; |   1000 |    41,985.74 ns |     284.316 ns |     265.949 ns |     68.668 ns |    41,970.47 ns |    41,533.08 ns |    41,786.44 ns |    42,149.01 ns |    42,436.58 ns |     23,817.6 | 167.47 |    2.09 |    5 |   3.7842 |    24,000 B |
| &#39;IReadOnlyCollection&lt;T&gt; ForEach&#39; |   1000 |     3,723.58 ns |      32.423 ns |      28.742 ns |      7.682 ns |     3,722.26 ns |     3,675.26 ns |     3,709.30 ns |     3,734.30 ns |     3,782.82 ns |    268,558.9 |  14.86 |    0.24 |    4 |   0.0038 |        32 B |
|       &#39;IReadOnlyList&lt;T&gt; ForEach&#39; |   1000 |     3,728.94 ns |      21.709 ns |      20.307 ns |      5.243 ns |     3,729.01 ns |     3,689.50 ns |     3,713.40 ns |     3,745.30 ns |     3,762.13 ns |    268,172.8 |  14.87 |    0.22 |    4 |   0.0038 |        32 B |
|           &#39;IReadOnlyList&lt;T&gt; For&#39; |   1000 |     2,210.42 ns |      15.137 ns |      11.818 ns |      3.412 ns |     2,215.61 ns |     2,183.52 ns |     2,201.32 ns |     2,218.04 ns |     2,222.18 ns |    452,402.8 |   8.81 |    0.15 |    3 |        - |           - |
|                                  |        |                 |                |                |               |                 |                 |                 |                 |                 |              |        |         |      |          |             |
|                    **&#39;T[] ForEach&#39;** |  **10000** |     **2,480.21 ns** |      **28.173 ns** |      **24.975 ns** |      **6.675 ns** |     **2,472.34 ns** |     **2,446.79 ns** |     **2,462.87 ns** |     **2,490.44 ns** |     **2,533.64 ns** |    **403,191.1** |   **1.00** |    **0.01** |    **1** |        **-** |           **-** |
|                        &#39;T[] For&#39; |  10000 |     2,479.78 ns |      22.468 ns |      21.016 ns |      5.426 ns |     2,481.48 ns |     2,448.21 ns |     2,464.95 ns |     2,493.74 ns |     2,523.66 ns |    403,262.3 |   1.00 |    0.00 |    1 |        - |           - |
|            &#39;ICollection ForEach&#39; |  10000 |   450,664.23 ns |   4,762.307 ns |   4,454.664 ns |  1,150.189 ns |   449,032.81 ns |   445,330.52 ns |   447,427.54 ns |   455,472.90 ns |   458,911.82 ns |      2,218.9 | 181.74 |    2.01 |    6 |  38.0859 |   240,032 B |
|         &#39;ICollection&lt;T&gt; ForEach&#39; |  10000 |    36,977.17 ns |     422.986 ns |     395.661 ns |    102.159 ns |    36,877.27 ns |    36,556.82 ns |    36,657.64 ns |    37,227.68 ns |    37,866.51 ns |     27,043.7 |  14.91 |    0.21 |    3 |        - |        32 B |
|            &#39;IEnumerable ForEach&#39; |  10000 |   440,496.38 ns |   8,157.467 ns |   7,630.499 ns |  1,970.186 ns |   438,060.89 ns |   432,176.12 ns |   434,677.51 ns |   444,265.14 ns |   457,686.23 ns |      2,270.2 | 177.65 |    3.52 |    5 |  38.0859 |   240,032 B |
|         &#39;IEnumerable&lt;T&gt; ForEach&#39; |  10000 |    36,923.64 ns |     301.570 ns |     267.334 ns |     71.448 ns |    36,907.21 ns |    36,401.15 ns |    36,799.58 ns |    37,136.48 ns |    37,355.09 ns |     27,082.9 |  14.90 |    0.20 |    3 |        - |        32 B |
|                  &#39;IList ForEach&#39; |  10000 |   476,414.52 ns |  12,121.599 ns |  34,386.957 ns |  3,565.761 ns |   461,221.48 ns |   441,994.63 ns |   455,110.25 ns |   482,209.57 ns |   569,192.48 ns |      2,099.0 | 219.27 |    9.70 |    7 |  38.0859 |   240,032 B |
|               &#39;IList&lt;T&gt; ForEach&#39; |  10000 |    36,810.24 ns |     251.114 ns |     222.606 ns |     59.494 ns |    36,760.12 ns |    36,536.97 ns |    36,651.23 ns |    36,871.08 ns |    37,279.57 ns |     27,166.4 |  14.85 |    0.19 |    3 |        - |        32 B |
|                   &#39;IList&lt;T&gt; For&#39; |  10000 |    22,074.95 ns |     126.265 ns |     111.930 ns |     29.915 ns |    22,059.32 ns |    21,925.39 ns |    22,007.81 ns |    22,134.80 ns |    22,359.59 ns |     45,300.2 |   8.91 |    0.09 |    2 |        - |           - |
|                      &#39;IList For&#39; |  10000 |   402,517.07 ns |   3,872.707 ns |   3,433.054 ns |    917.522 ns |   402,738.26 ns |   395,414.01 ns |   400,576.17 ns |   404,575.00 ns |   408,203.12 ns |      2,484.4 | 162.39 |    1.93 |    4 |  38.0859 |   240,000 B |
| &#39;IReadOnlyCollection&lt;T&gt; ForEach&#39; |  10000 |    36,816.23 ns |     218.154 ns |     193.388 ns |     51.685 ns |    36,818.44 ns |    36,470.48 ns |    36,679.02 ns |    36,895.48 ns |    37,180.02 ns |     27,161.9 |  14.85 |    0.15 |    3 |        - |        32 B |
|       &#39;IReadOnlyList&lt;T&gt; ForEach&#39; |  10000 |    37,731.43 ns |     665.521 ns |   1,645.003 ns |    193.865 ns |    37,105.96 ns |    36,388.18 ns |    36,871.87 ns |    37,515.64 ns |    43,512.96 ns |     26,503.1 |  16.10 |    1.00 |    3 |        - |        32 B |
|           &#39;IReadOnlyList&lt;T&gt; For&#39; |  10000 |    22,039.42 ns |     158.744 ns |     148.489 ns |     38.340 ns |    22,065.38 ns |    21,826.13 ns |    21,916.03 ns |    22,112.66 ns |    22,344.01 ns |     45,373.2 |   8.89 |    0.10 |    2 |        - |           - |
|                                  |        |                 |                |                |               |                 |                 |                 |                 |                 |              |        |         |      |          |             |
|                    **&#39;T[] ForEach&#39;** | **100000** |    **24,965.73 ns** |     **280.914 ns** |     **262.767 ns** |     **67.846 ns** |    **24,982.91 ns** |    **24,429.50 ns** |    **24,828.94 ns** |    **25,103.50 ns** |    **25,421.77 ns** |     **40,054.9** |   **1.00** |    **0.02** |    **1** |        **-** |           **-** |
|                        &#39;T[] For&#39; | 100000 |    24,988.13 ns |     367.489 ns |     343.749 ns |     88.756 ns |    24,942.79 ns |    24,486.06 ns |    24,733.16 ns |    25,140.39 ns |    25,682.75 ns |     40,019.0 |   1.00 |    0.00 |    1 |        - |           - |
|            &#39;ICollection ForEach&#39; | 100000 | 4,663,385.49 ns |  27,701.424 ns |  24,556.591 ns |  6,563.025 ns | 4,656,078.52 ns | 4,625,183.59 ns | 4,646,250.98 ns | 4,681,358.20 ns | 4,710,471.09 ns |        214.4 | 186.56 |    3.30 |    5 | 375.0000 | 2,400,032 B |
|         &#39;ICollection&lt;T&gt; ForEach&#39; | 100000 |   369,654.39 ns |   2,242.035 ns |   1,987.506 ns |    531.183 ns |   369,184.55 ns |   366,654.08 ns |   368,409.12 ns |   371,200.72 ns |   372,945.53 ns |      2,705.2 |  14.79 |    0.21 |    3 |        - |        32 B |
|            &#39;IEnumerable ForEach&#39; | 100000 | 4,990,289.50 ns | 220,968.240 ns | 651,529.842 ns | 65,152.984 ns | 4,757,632.81 ns | 4,301,846.88 ns | 4,415,105.27 ns | 5,462,233.40 ns | 6,792,057.81 ns |        200.4 | 237.87 |   25.20 |    5 | 375.0000 | 2,400,032 B |
|         &#39;IEnumerable&lt;T&gt; ForEach&#39; | 100000 |   371,595.68 ns |   4,343.646 ns |   3,850.529 ns |  1,029.097 ns |   371,729.42 ns |   366,337.60 ns |   368,775.24 ns |   373,126.15 ns |   379,641.02 ns |      2,691.1 |  14.87 |    0.30 |    3 |        - |        32 B |
|                  &#39;IList ForEach&#39; | 100000 | 4,694,311.72 ns |  81,029.927 ns | 137,594.965 ns | 22,620.473 ns | 4,650,526.56 ns | 4,513,776.56 ns | 4,603,207.81 ns | 4,754,300.00 ns | 5,078,842.19 ns |        213.0 | 189.68 |    6.76 |    5 | 375.0000 | 2,400,032 B |
|               &#39;IList&lt;T&gt; ForEach&#39; | 100000 |   402,224.50 ns |  13,545.307 ns |  39,938.644 ns |  3,993.864 ns |   382,351.61 ns |   356,554.74 ns |   371,107.48 ns |   435,103.78 ns |   526,843.99 ns |      2,486.2 |  17.23 |    1.70 |    3 |        - |        32 B |
|                   &#39;IList&lt;T&gt; For&#39; | 100000 |   222,684.55 ns |   3,440.825 ns |   8,309.978 ns |  1,000.404 ns |   220,208.67 ns |   214,452.81 ns |   218,039.67 ns |   224,709.33 ns |   257,734.42 ns |      4,490.7 |   9.25 |    0.60 |    2 |        - |           - |
|                      &#39;IList For&#39; | 100000 | 4,148,977.19 ns |  42,713.866 ns |  39,954.577 ns | 10,316.227 ns | 4,141,464.06 ns | 4,085,260.94 ns | 4,122,689.45 ns | 4,185,319.14 ns | 4,209,053.91 ns |        241.0 | 166.07 |    3.15 |    4 | 375.0000 | 2,400,000 B |
| &#39;IReadOnlyCollection&lt;T&gt; ForEach&#39; | 100000 |   374,118.02 ns |   4,429.091 ns |   3,926.274 ns |  1,049.341 ns |   373,143.12 ns |   368,553.76 ns |   371,995.09 ns |   375,823.57 ns |   381,178.22 ns |      2,673.0 |  14.97 |    0.28 |    3 |        - |        32 B |
|       &#39;IReadOnlyList&lt;T&gt; ForEach&#39; | 100000 |   369,692.28 ns |   5,282.174 ns |   4,940.949 ns |  1,275.748 ns |   367,041.46 ns |   362,866.11 ns |   366,144.36 ns |   374,412.79 ns |   377,804.25 ns |      2,705.0 |  14.80 |    0.34 |    3 |        - |        32 B |
|           &#39;IReadOnlyList&lt;T&gt; For&#39; | 100000 |   228,563.38 ns |   5,373.123 ns |  15,588.405 ns |  1,582.763 ns |   220,645.70 ns |   213,789.77 ns |   218,444.41 ns |   230,251.32 ns |   267,281.05 ns |      4,375.2 |   9.82 |    0.81 |    2 |        - |           - |

----------
**Array Sort Tests**


|                               Method |  Count |            Mean |         Error |          StdDev |        StdErr |          Median |             Min |              Q1 |              Q3 |             Max |         Op/s | Ratio | RatioSD | Rank |     Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
|------------------------------------- |------- |----------------:|--------------:|----------------:|--------------:|----------------:|----------------:|----------------:|----------------:|----------------:|-------------:|------:|--------:|-----:|----------:|----------:|----------:|------------:|
|               **&#39;Linq Array.OrderBy()&#39;** |    **100** |      **4,407.8 ns** |      **59.68 ns** |        **55.82 ns** |      **14.41 ns** |      **4,394.4 ns** |      **4,320.4 ns** |      **4,372.2 ns** |      **4,422.7 ns** |      **4,521.8 ns** |   **226,869.27** |  **1.00** |    **0.00** |    **3** |    **0.2975** |         **-** |         **-** |     **1,872 B** |
|                     Array.OrderByF() |    100 |        988.5 ns |      19.36 ns |        24.49 ns |       5.11 ns |        986.8 ns |        953.2 ns |        967.1 ns |        999.1 ns |      1,041.8 ns | 1,011,641.49 |  0.23 |    0.00 |    2 |    0.1335 |         - |         - |       848 B |
| &#39;PLinq Array.AsParallel().OrderBy()&#39; |    100 |     38,481.5 ns |   3,663.26 ns |    10,801.21 ns |   1,080.12 ns |     34,617.3 ns |     25,536.8 ns |     30,369.8 ns |     48,373.9 ns |     66,111.2 ns |    25,986.53 |  8.88 |    1.64 |    6 |   17.1204 |    2.3804 |         - |    69,222 B |
|                         Array.Sort() |    100 |        296.6 ns |      18.36 ns |        53.55 ns |       5.41 ns |        295.5 ns |        240.8 ns |        246.4 ns |        328.1 ns |        447.5 ns | 3,371,910.16 |  0.08 |    0.01 |    1 |         - |         - |         - |           - |
|                         List.OrderBy |    100 |      4,486.0 ns |      19.55 ns |        17.33 ns |       4.63 ns |      4,482.4 ns |      4,461.7 ns |      4,475.6 ns |      4,497.1 ns |      4,525.0 ns |   222,917.47 |  1.02 |    0.01 |    4 |    0.2975 |         - |         - |     1,904 B |
|                        List.OrderByF |    100 |      5,611.2 ns |      30.87 ns |        25.78 ns |       7.15 ns |      5,615.8 ns |      5,539.3 ns |      5,604.9 ns |      5,622.5 ns |      5,650.8 ns |   178,215.73 |  1.27 |    0.02 |    5 |    0.0839 |         - |         - |       552 B |
|                          List.Sort() |    100 |        280.8 ns |      10.36 ns |        30.56 ns |       3.06 ns |        277.2 ns |        243.3 ns |        250.9 ns |        305.5 ns |        354.6 ns | 3,561,055.38 |  0.07 |    0.00 |    1 |         - |         - |         - |           - |
|                                      |        |                 |               |                 |               |                 |                 |                 |                 |                 |              |       |         |      |           |           |           |             |
|               **&#39;Linq Array.OrderBy()&#39;** |   **1000** |     **84,885.4 ns** |     **472.07 ns** |       **418.47 ns** |     **111.84 ns** |     **84,887.0 ns** |     **84,210.0 ns** |     **84,571.8 ns** |     **85,026.2 ns** |     **85,708.6 ns** |    **11,780.59** |  **1.00** |    **0.00** |    **4** |    **2.5635** |         **-** |         **-** |    **16,272 B** |
|                     Array.OrderByF() |   1000 |     29,455.7 ns |   1,095.75 ns |     3,230.83 ns |     323.08 ns |     27,501.4 ns |     27,185.9 ns |     27,365.7 ns |     31,845.8 ns |     38,371.0 ns |    33,949.25 |  0.36 |    0.05 |    3 |    1.2817 |         - |         - |     8,048 B |
| &#39;PLinq Array.AsParallel().OrderBy()&#39; |   1000 |    129,419.7 ns |  12,437.99 ns |    36,673.71 ns |   3,667.37 ns |    122,285.2 ns |     75,837.1 ns |     97,444.6 ns |    165,298.0 ns |    196,484.6 ns |     7,726.80 |  1.09 |    0.14 |    6 |   22.4609 |    3.9063 |         - |   122,569 B |
|                         Array.Sort() |   1000 |      3,994.2 ns |     146.81 ns |       432.86 ns |      43.29 ns |      4,183.2 ns |      3,462.9 ns |      3,509.9 ns |      4,340.5 ns |      4,958.0 ns |   250,363.14 |  0.05 |    0.01 |    2 |         - |         - |         - |           - |
|                         List.OrderBy |   1000 |     85,837.5 ns |   1,188.47 ns |     1,111.69 ns |     287.04 ns |     85,432.4 ns |     84,382.7 ns |     85,094.9 ns |     86,443.2 ns |     88,051.0 ns |    11,649.92 |  1.01 |    0.01 |    4 |    2.5635 |         - |         - |    16,304 B |
|                        List.OrderByF |   1000 |     93,590.0 ns |   1,375.49 ns |     1,286.64 ns |     332.21 ns |     93,446.6 ns |     91,265.5 ns |     92,976.7 ns |     94,346.4 ns |     96,124.2 ns |    10,684.90 |  1.10 |    0.02 |    5 |    0.6104 |         - |         - |     4,152 B |
|                          List.Sort() |   1000 |      3,743.8 ns |      38.62 ns |        78.88 ns |      11.05 ns |      3,723.9 ns |      3,639.8 ns |      3,684.3 ns |      3,782.5 ns |      4,033.1 ns |   267,106.63 |  0.04 |    0.00 |    1 |         - |         - |         - |           - |
|                                      |        |                 |               |                 |               |                 |                 |                 |                 |                 |              |       |         |      |           |           |           |             |
|               **&#39;Linq Array.OrderBy()&#39;** |  **10000** |  **1,140,143.3 ns** |   **7,118.25 ns** |     **5,944.06 ns** |   **1,648.59 ns** |  **1,140,619.7 ns** |  **1,130,695.1 ns** |  **1,136,838.1 ns** |  **1,142,943.8 ns** |  **1,152,775.8 ns** |       **877.08** |  **1.00** |    **0.00** |    **3** |   **25.3906** |    **5.8594** |         **-** |   **160,272 B** |
|                     Array.OrderByF() |  10000 |    479,746.8 ns |   3,042.82 ns |     2,697.38 ns |     720.91 ns |    479,661.7 ns |    475,625.0 ns |    477,795.3 ns |    480,621.0 ns |    485,525.3 ns |     2,084.43 |  0.42 |    0.00 |    2 |   12.2070 |         - |         - |    80,048 B |
| &#39;PLinq Array.AsParallel().OrderBy()&#39; |  10000 |    559,012.6 ns |  47,484.16 ns |   140,008.11 ns |  14,000.81 ns |    493,089.6 ns |    416,658.5 ns |    445,621.9 ns |    710,935.0 ns |    864,600.6 ns |     1,788.87 |  0.38 |    0.01 |    2 |  137.6953 |    9.7656 |         - |   775,896 B |
|                         Array.Sort() |  10000 |     53,484.4 ns |   1,971.00 ns |     5,749.50 ns |     580.79 ns |     54,292.9 ns |     46,583.5 ns |     47,690.9 ns |     57,136.8 ns |     69,607.1 ns |    18,697.04 |  0.05 |    0.00 |    1 |         - |         - |         - |           - |
|                         List.OrderBy |  10000 |  1,184,776.6 ns |  22,151.53 ns |    19,636.76 ns |   5,248.14 ns |  1,179,572.6 ns |  1,156,454.9 ns |  1,176,131.9 ns |  1,191,886.7 ns |  1,226,707.8 ns |       844.04 |  1.04 |    0.02 |    4 |   25.3906 |    5.8594 |         - |   160,304 B |
|                        List.OrderByF |  10000 |  1,285,173.1 ns |  20,487.54 ns |    19,164.05 ns |   4,948.14 ns |  1,281,280.4 ns |  1,249,514.7 ns |  1,272,730.9 ns |  1,303,005.6 ns |  1,315,475.1 ns |       778.11 |  1.13 |    0.02 |    5 |    5.8594 |         - |         - |    40,152 B |
|                          List.Sort() |  10000 |     55,012.1 ns |     824.67 ns |       771.40 ns |     199.17 ns |     54,851.2 ns |     53,541.4 ns |     54,453.5 ns |     55,468.5 ns |     56,504.6 ns |    18,177.81 |  0.05 |    0.00 |    1 |         - |         - |         - |           - |
|                                      |        |                 |               |                 |               |                 |                 |                 |                 |                 |              |       |         |      |           |           |           |             |
|               **&#39;Linq Array.OrderBy()&#39;** | **100000** | **15,606,035.1 ns** | **289,376.42 ns** |   **676,408.05 ns** |  **83,898.09 ns** | **15,423,529.7 ns** | **14,979,290.6 ns** | **15,257,140.6 ns** | **15,674,523.4 ns** | **18,496,048.4 ns** |        **64.08** |  **1.00** |    **0.00** |    **4** |  **406.2500** |  **406.2500** |  **406.2500** | **1,602,034 B** |
|                     Array.OrderByF() | 100000 |  6,143,965.1 ns | 120,888.29 ns |   157,188.92 ns |  32,086.05 ns |  6,101,582.0 ns |  5,877,434.4 ns |  6,035,355.3 ns |  6,253,035.2 ns |  6,435,107.8 ns |       162.76 |  0.39 |    0.03 |    2 |  203.1250 |  203.1250 |  203.1250 |   800,043 B |
| &#39;PLinq Array.AsParallel().OrderBy()&#39; | 100000 |  7,458,489.7 ns | 533,163.46 ns | 1,572,044.50 ns | 157,204.45 ns |  8,065,553.9 ns |  4,980,988.3 ns |  5,748,154.5 ns |  8,569,432.0 ns | 11,147,761.7 ns |       134.08 |  0.49 |    0.11 |    3 | 1851.5625 | 1781.2500 | 1000.0000 | 8,966,646 B |
|                         Array.Sort() | 100000 |    525,143.1 ns |   5,377.12 ns |     5,029.76 ns |   1,298.68 ns |    523,843.7 ns |    519,180.1 ns |    521,061.0 ns |    528,684.8 ns |    533,768.5 ns |     1,904.24 |  0.03 |    0.00 |    1 |         - |         - |         - |           - |
|                         List.OrderBy | 100000 | 15,203,791.4 ns | 260,626.31 ns |   243,790.01 ns |  62,946.31 ns | 15,193,157.8 ns | 14,832,623.4 ns | 15,029,870.3 ns | 15,313,012.5 ns | 15,793,423.4 ns |        65.77 |  0.94 |    0.06 |    4 |  406.2500 |  406.2500 |  406.2500 | 1,602,066 B |
|                        List.OrderByF | 100000 | 16,092,447.1 ns | 103,614.54 ns |    91,851.59 ns |  24,548.37 ns | 16,100,175.0 ns | 15,913,290.6 ns | 16,045,238.3 ns | 16,150,824.2 ns | 16,243,343.8 ns |        62.14 |  0.99 |    0.07 |    5 |   93.7500 |   93.7500 |   93.7500 |   400,654 B |
|                          List.Sort() | 100000 |    532,555.9 ns |  10,626.63 ns |     9,940.15 ns |   2,566.54 ns |    528,698.2 ns |    520,594.5 ns |    525,416.6 ns |    537,536.8 ns |    555,187.6 ns |     1,877.74 |  0.03 |    0.00 |    1 |         - |         - |         - |           - |

----------
**Bitwise Vs Modulo**


|           Method |  Count |         Mean |      Error |       StdDev |     StdErr |       Median |          Min |           Q1 |           Q3 |          Max |         Op/s | Ratio | RatioSD | Rank | Allocated |
|----------------- |------- |-------------:|-----------:|-------------:|-----------:|-------------:|-------------:|-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|
| **&#39;Bitwise Modulo&#39;** |    **100** |     **31.33 ns** |   **0.416 ns** |     **0.683 ns** |   **0.115 ns** |     **31.10 ns** |     **30.64 ns** |     **30.84 ns** |     **31.72 ns** |     **34.02 ns** | **31,913,382.1** |  **1.00** |    **0.03** |    **1** |         **-** |
|  &#39;Normal Modulo&#39; |    100 |     31.70 ns |   0.616 ns |     0.546 ns |   0.146 ns |     31.61 ns |     30.98 ns |     31.34 ns |     32.22 ns |     32.55 ns | 31,547,759.7 |  1.00 |    0.00 |    1 |         - |
|                  |        |              |            |              |            |              |              |              |              |              |              |       |         |      |           |
| **&#39;Bitwise Modulo&#39;** |   **1000** |    **259.35 ns** |   **4.305 ns** |     **4.606 ns** |   **1.086 ns** |    **257.94 ns** |    **253.69 ns** |    **256.14 ns** |    **260.76 ns** |    **269.43 ns** |  **3,855,788.9** |  **1.02** |    **0.02** |    **1** |         **-** |
|  &#39;Normal Modulo&#39; |   1000 |    255.94 ns |   1.546 ns |     1.371 ns |   0.366 ns |    255.28 ns |    254.50 ns |    254.96 ns |    256.79 ns |    258.78 ns |  3,907,097.2 |  1.00 |    0.00 |    1 |         - |
|                  |        |              |            |              |            |              |              |              |              |              |              |       |         |      |           |
| **&#39;Bitwise Modulo&#39;** |  **10000** |  **2,529.52 ns** |  **49.336 ns** |    **48.455 ns** |  **12.114 ns** |  **2,507.99 ns** |  **2,471.80 ns** |  **2,493.83 ns** |  **2,558.83 ns** |  **2,624.61 ns** |    **395,332.0** |  **1.02** |    **0.02** |    **1** |         **-** |
|  &#39;Normal Modulo&#39; |  10000 |  2,495.89 ns |  21.960 ns |    19.467 ns |   5.203 ns |  2,495.37 ns |  2,463.33 ns |  2,485.80 ns |  2,505.33 ns |  2,526.47 ns |    400,658.4 |  1.00 |    0.00 |    1 |         - |
|                  |        |              |            |              |            |              |              |              |              |              |              |       |         |      |           |
| **&#39;Bitwise Modulo&#39;** | **100000** | **25,378.26 ns** | **401.528 ns** | **1,007.355 ns** | **117.103 ns** | **25,028.09 ns** | **24,575.85 ns** | **24,887.68 ns** | **25,287.39 ns** | **28,706.06 ns** |     **39,403.8** |  **1.08** |    **0.06** |    **1** |         **-** |
|  &#39;Normal Modulo&#39; | 100000 | 24,956.94 ns | 182.256 ns |   161.565 ns |  43.180 ns | 24,927.23 ns | 24,722.71 ns | 24,856.42 ns | 25,044.04 ns | 25,338.72 ns |     40,069.0 |  1.00 |    0.00 |    1 |         - |

----------
**Bitwise Vs Normal Division**


|             Method |  Count |         Mean |      Error |     StdDev |    StdErr |       Median |          Min |           Q1 |           Q3 |          Max |         Op/s | Ratio | RatioSD | Rank | Allocated |
|------------------- |------- |-------------:|-----------:|-----------:|----------:|-------------:|-------------:|-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|
| **&#39;Bitwise Division&#39;** |    **100** |     **31.85 ns** |   **0.430 ns** |   **0.359 ns** |  **0.100 ns** |     **31.72 ns** |     **31.29 ns** |     **31.61 ns** |     **31.94 ns** |     **32.50 ns** | **31,398,992.8** |  **1.02** |    **0.01** |    **2** |         **-** |
|  &#39;Normal Division&#39; |    100 |     31.10 ns |   0.451 ns |   0.422 ns |  0.109 ns |     30.96 ns |     30.40 ns |     30.88 ns |     31.47 ns |     31.99 ns | 32,158,874.3 |  1.00 |    0.00 |    1 |         - |
|                    |        |              |            |            |           |              |              |              |              |              |              |       |         |      |           |
| **&#39;Bitwise Division&#39;** |   **1000** |    **246.57 ns** |   **2.769 ns** |   **2.313 ns** |  **0.641 ns** |    **246.11 ns** |    **243.73 ns** |    **244.68 ns** |    **248.61 ns** |    **250.84 ns** |  **4,055,603.3** |  **0.88** |    **0.01** |    **1** |         **-** |
|  &#39;Normal Division&#39; |   1000 |    255.61 ns |   4.655 ns |  13.130 ns |  1.369 ns |    249.42 ns |    244.06 ns |    246.80 ns |    256.33 ns |    298.89 ns |  3,912,183.0 |  1.00 |    0.00 |    1 |         - |
|                    |        |              |            |            |           |              |              |              |              |              |              |       |         |      |           |
| **&#39;Bitwise Division&#39;** |  **10000** |  **2,421.29 ns** |  **25.140 ns** |  **22.286 ns** |  **5.956 ns** |  **2,414.35 ns** |  **2,393.16 ns** |  **2,405.75 ns** |  **2,437.27 ns** |  **2,460.38 ns** |    **413,002.9** |  **0.99** |    **0.02** |    **1** |         **-** |
|  &#39;Normal Division&#39; |  10000 |  2,443.02 ns |  34.577 ns |  30.651 ns |  8.192 ns |  2,438.19 ns |  2,387.69 ns |  2,428.31 ns |  2,457.02 ns |  2,501.11 ns |    409,329.6 |  1.00 |    0.00 |    1 |         - |
|                    |        |              |            |            |           |              |              |              |              |              |              |       |         |      |           |
| **&#39;Bitwise Division&#39;** | **100000** | **23,978.63 ns** | **223.192 ns** | **186.376 ns** | **51.691 ns** | **23,949.79 ns** | **23,666.10 ns** | **23,822.82 ns** | **24,113.10 ns** | **24,308.56 ns** |     **41,703.8** |  **0.99** |    **0.02** |    **1** |         **-** |
|  &#39;Normal Division&#39; | 100000 | 24,180.50 ns | 348.030 ns | 325.547 ns | 84.056 ns | 24,172.35 ns | 23,775.94 ns | 23,897.75 ns | 24,382.83 ns | 24,921.06 ns |     41,355.6 |  1.00 |    0.00 |    1 |         - |

----------
**Bitwise Vs Normal Multiplication**


|                   Method |  Count |         Mean |      Error |     StdDev |    StdErr |          Min |           Q1 |       Median |           Q3 |          Max |         Op/s | Ratio | RatioSD | Rank | Allocated |
|------------------------- |------- |-------------:|-----------:|-----------:|----------:|-------------:|-------------:|-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|
| **&#39;Bitwise Multiplication&#39;** |    **100** |     **31.60 ns** |   **0.656 ns** |   **0.730 ns** |  **0.167 ns** |     **30.77 ns** |     **30.94 ns** |     **31.50 ns** |     **32.19 ns** |     **32.88 ns** | **31,645,322.2** |  **1.01** |    **0.03** |    **1** |         **-** |
|  &#39;Normal Multiplication&#39; |    100 |     30.93 ns |   0.359 ns |   0.318 ns |  0.085 ns |     30.38 ns |     30.71 ns |     30.92 ns |     31.06 ns |     31.51 ns | 32,334,462.0 |  1.00 |    0.00 |    1 |         - |
|                          |        |              |            |            |           |              |              |              |              |              |              |       |         |      |           |
| **&#39;Bitwise Multiplication&#39;** |   **1000** |    **248.21 ns** |   **3.693 ns** |   **3.455 ns** |  **0.892 ns** |    **243.27 ns** |    **245.88 ns** |    **247.70 ns** |    **250.27 ns** |    **255.27 ns** |  **4,028,918.2** |  **0.99** |    **0.02** |    **1** |         **-** |
|  &#39;Normal Multiplication&#39; |   1000 |    251.44 ns |   3.182 ns |   2.484 ns |  0.717 ns |    246.38 ns |    250.08 ns |    251.59 ns |    253.06 ns |    255.28 ns |  3,977,113.8 |  1.00 |    0.00 |    1 |         - |
|                          |        |              |            |            |           |              |              |              |              |              |              |       |         |      |           |
| **&#39;Bitwise Multiplication&#39;** |  **10000** |  **2,455.89 ns** |  **30.350 ns** |  **28.389 ns** |  **7.330 ns** |  **2,420.31 ns** |  **2,431.38 ns** |  **2,454.53 ns** |  **2,479.89 ns** |  **2,505.26 ns** |    **407,184.2** |  **1.01** |    **0.02** |    **1** |         **-** |
|  &#39;Normal Multiplication&#39; |  10000 |  2,435.82 ns |  31.330 ns |  29.307 ns |  7.567 ns |  2,392.38 ns |  2,411.36 ns |  2,429.13 ns |  2,463.03 ns |  2,478.20 ns |    410,539.8 |  1.00 |    0.00 |    1 |         - |
|                          |        |              |            |            |           |              |              |              |              |              |              |       |         |      |           |
| **&#39;Bitwise Multiplication&#39;** | **100000** | **24,105.13 ns** | **393.615 ns** | **348.929 ns** | **93.255 ns** | **23,743.19 ns** | **23,815.67 ns** | **24,055.82 ns** | **24,291.79 ns** | **24,906.91 ns** |     **41,485.0** |  **1.00** |    **0.01** |    **1** |         **-** |
|  &#39;Normal Multiplication&#39; | 100000 | 24,137.89 ns | 285.225 ns | 266.799 ns | 68.887 ns | 23,738.70 ns | 23,928.76 ns | 24,047.06 ns | 24,379.82 ns | 24,506.15 ns |     41,428.6 |  1.00 |    0.00 |    1 |         - |

----------
**Checked Vs Unchecked Math**


|           Method |  Count |         Mean |      Error |     StdDev |     StdErr |          Min |           Q1 |       Median |           Q3 |          Max |         Op/s | Ratio | RatioSD | Rank | Allocated |
|----------------- |------- |-------------:|-----------:|-----------:|-----------:|-------------:|-------------:|-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|
|   **&#39;Checked math&#39;** |    **100** |     **31.14 ns** |   **0.547 ns** |   **0.485 ns** |   **0.130 ns** |     **30.49 ns** |     **30.84 ns** |     **31.18 ns** |     **31.35 ns** |     **32.00 ns** | **32,108,861.7** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Unchecked math&#39; |    100 |     31.26 ns |   0.498 ns |   0.466 ns |   0.120 ns |     30.68 ns |     30.85 ns |     31.12 ns |     31.56 ns |     32.09 ns | 31,992,365.5 |  1.00 |    0.02 |    1 |         - |
|                  |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|   **&#39;Checked math&#39;** |   **1000** |    **250.12 ns** |   **3.615 ns** |   **3.382 ns** |   **0.873 ns** |    **244.49 ns** |    **247.34 ns** |    **249.21 ns** |    **253.30 ns** |    **255.06 ns** |  **3,998,052.7** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Unchecked math&#39; |   1000 |    250.03 ns |   3.987 ns |   3.730 ns |   0.963 ns |    244.98 ns |    246.56 ns |    250.26 ns |    252.31 ns |    257.63 ns |  3,999,526.7 |  1.00 |    0.02 |    1 |         - |
|                  |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|   **&#39;Checked math&#39;** |  **10000** |  **2,421.88 ns** |  **27.134 ns** |  **25.381 ns** |   **6.553 ns** |  **2,382.54 ns** |  **2,402.51 ns** |  **2,421.47 ns** |  **2,440.53 ns** |  **2,466.80 ns** |    **412,902.3** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Unchecked math&#39; |  10000 |  2,410.42 ns |  16.765 ns |  13.999 ns |   3.883 ns |  2,383.23 ns |  2,403.67 ns |  2,413.46 ns |  2,415.58 ns |  2,437.15 ns |    414,865.6 |  1.00 |    0.01 |    1 |         - |
|                  |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|   **&#39;Checked math&#39;** | **100000** | **24,070.51 ns** | **220.235 ns** | **206.008 ns** |  **53.191 ns** | **23,795.42 ns** | **23,909.99 ns** | **24,019.20 ns** | **24,254.79 ns** | **24,433.31 ns** |     **41,544.6** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Unchecked math&#39; | 100000 | 24,539.26 ns | 462.473 ns | 532.584 ns | 119.089 ns | 23,770.66 ns | 24,150.07 ns | 24,455.89 ns | 25,079.37 ns | 25,539.57 ns |     40,751.0 |  1.02 |    0.02 |    1 |         - |

----------
**Class Vs Struct Access**


|        Method |  Count |         Mean |      Error |     StdDev |     StdErr |          Min |           Q1 |       Median |           Q3 |          Max |         Op/s | Ratio | RatioSD | Rank | Allocated |
|-------------- |------- |-------------:|-----------:|-----------:|-----------:|-------------:|-------------:|-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|
|  **&#39;Class test&#39;** |    **100** |     **31.44 ns** |   **0.623 ns** |   **0.583 ns** |   **0.151 ns** |     **30.64 ns** |     **31.01 ns** |     **31.38 ns** |     **31.88 ns** |     **32.45 ns** | **31,805,703.9** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Struct test&#39; |    100 |     31.22 ns |   0.525 ns |   0.491 ns |   0.127 ns |     30.68 ns |     30.89 ns |     31.05 ns |     31.46 ns |     32.12 ns | 32,027,557.1 |  0.99 |    0.02 |    1 |         - |
|               |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|  **&#39;Class test&#39;** |   **1000** |    **245.60 ns** |   **2.923 ns** |   **2.441 ns** |   **0.677 ns** |    **243.52 ns** |    **243.94 ns** |    **244.96 ns** |    **246.08 ns** |    **251.37 ns** |  **4,071,646.8** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Struct test&#39; |   1000 |    248.90 ns |   3.744 ns |   3.319 ns |   0.887 ns |    245.06 ns |    246.55 ns |    247.72 ns |    250.54 ns |    256.81 ns |  4,017,610.2 |  1.01 |    0.02 |    1 |         - |
|               |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|  **&#39;Class test&#39;** |  **10000** |  **2,426.29 ns** |  **24.594 ns** |  **23.006 ns** |   **5.940 ns** |  **2,388.97 ns** |  **2,406.61 ns** |  **2,426.37 ns** |  **2,434.97 ns** |  **2,465.43 ns** |    **412,152.3** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Struct test&#39; |  10000 |  2,465.69 ns |  33.790 ns |  31.608 ns |   8.161 ns |  2,400.71 ns |  2,440.91 ns |  2,466.26 ns |  2,487.32 ns |  2,511.12 ns |    405,565.8 |  1.02 |    0.02 |    1 |         - |
|               |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|  **&#39;Class test&#39;** | **100000** | **24,130.44 ns** | **276.078 ns** | **258.244 ns** |  **66.678 ns** | **23,785.59 ns** | **23,947.62 ns** | **24,067.41 ns** | **24,233.63 ns** | **24,654.35 ns** |     **41,441.4** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Struct test&#39; | 100000 | 24,325.95 ns | 420.947 ns | 393.754 ns | 101.667 ns | 23,802.42 ns | 24,019.54 ns | 24,143.17 ns | 24,637.95 ns | 25,040.93 ns |     41,108.4 |  1.01 |    0.02 |    1 |         - |

----------
**Class Vs Struct Creation**


|        Method |  Count |            Mean |          Error |         StdDev |        StdErr |          Median |             Min |              Q1 |              Q3 |             Max |         Op/s | Ratio | RatioSD | Rank |    Gen 0 |    Gen 1 |    Gen 2 |   Allocated |
|-------------- |------- |----------------:|---------------:|---------------:|--------------:|----------------:|----------------:|----------------:|----------------:|----------------:|-------------:|------:|--------:|-----:|---------:|---------:|---------:|------------:|
|  **&#39;Class test&#39;** |    **100** |       **447.01 ns** |       **8.900 ns** |       **8.741 ns** |      **2.185 ns** |       **445.31 ns** |       **436.74 ns** |       **440.87 ns** |       **452.37 ns** |       **465.93 ns** |  **2,237,069.8** |  **1.00** |    **0.00** |    **2** |   **0.5136** |   **0.0057** |        **-** |     **3,224 B** |
| &#39;Struct test&#39; |    100 |        60.75 ns |       1.097 ns |       0.973 ns |      0.260 ns |        60.55 ns |        59.52 ns |        60.20 ns |        60.93 ns |        62.69 ns | 16,459,798.9 |  0.14 |    0.00 |    1 |   0.0675 |        - |        - |       424 B |
|               |        |                 |                |                |               |                 |                 |                 |                 |                 |              |       |         |      |          |          |          |             |
|  **&#39;Class test&#39;** |   **1000** |     **5,142.52 ns** |     **273.153 ns** |     **801.109 ns** |     **80.515 ns** |     **4,716.65 ns** |     **4,292.84 ns** |     **4,465.71 ns** |     **5,754.54 ns** |     **7,660.50 ns** |    **194,457.1** |  **1.00** |    **0.00** |    **2** |   **5.0964** |   **0.5035** |        **-** |    **32,024 B** |
| &#39;Struct test&#39; |   1000 |       532.42 ns |       4.778 ns |       4.693 ns |      1.173 ns |       531.84 ns |       525.29 ns |       529.51 ns |       534.19 ns |       542.67 ns |  1,878,216.9 |  0.09 |    0.01 |    1 |   0.6409 |        - |        - |     4,024 B |
|               |        |                 |                |                |               |                 |                 |                 |                 |                 |              |       |         |      |          |          |          |             |
|  **&#39;Class test&#39;** |  **10000** |    **89,837.64 ns** |  **11,158.897 ns** |  **30,358.603 ns** |  **3,273.652 ns** |    **79,027.86 ns** |    **59,077.21 ns** |    **72,198.12 ns** |    **94,586.14 ns** |   **193,437.10 ns** |     **11,131.2** |  **1.00** |    **0.00** |    **2** |  **50.8423** |  **16.9067** |        **-** |   **320,024 B** |
| &#39;Struct test&#39; |  10000 |     5,462.45 ns |     209.765 ns |     618.497 ns |     61.850 ns |     5,253.87 ns |     4,799.19 ns |     4,918.28 ns |     5,921.05 ns |     7,375.48 ns |    183,068.2 |  0.07 |    0.02 |    1 |   6.3248 |        - |        - |    40,024 B |
|               |        |                 |                |                |               |                 |                 |                 |                 |                 |              |       |         |      |          |          |          |             |
|  **&#39;Class test&#39;** | **100000** | **2,279,456.50 ns** | **114,461.470 ns** | **333,889.479 ns** | **33,727.931 ns** | **2,198,443.16 ns** | **1,720,947.66 ns** | **2,064,430.96 ns** | **2,494,960.16 ns** | **3,151,168.16 ns** |        **438.7** |  **1.00** |    **0.00** |    **2** | **498.0469** | **248.0469** | **248.0469** | **3,200,024 B** |
| &#39;Struct test&#39; | 100000 |   153,246.75 ns |   1,748.649 ns |   1,550.132 ns |    414.290 ns |   153,064.27 ns |   150,707.57 ns |   152,094.36 ns |   154,586.94 ns |   155,568.26 ns |      6,525.4 |  0.08 |    0.00 |    1 | 124.7559 | 124.7559 | 124.7559 |   400,024 B |

----------
**Class Vs Struct Set**


|        Method |  Count |         Mean |      Error |     StdDev |     StdErr |       Median |          Min |           Q1 |           Q3 |          Max |         Op/s | Ratio | RatioSD | Rank | Allocated |
|-------------- |------- |-------------:|-----------:|-----------:|-----------:|-------------:|-------------:|-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|
|  **&#39;Class test&#39;** |    **100** |     **31.38 ns** |   **0.384 ns** |   **0.340 ns** |   **0.091 ns** |     **31.46 ns** |     **30.87 ns** |     **31.08 ns** |     **31.52 ns** |     **32.17 ns** | **31,870,417.8** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Struct test&#39; |    100 |     31.92 ns |   0.494 ns |   0.462 ns |   0.119 ns |     31.74 ns |     31.33 ns |     31.62 ns |     32.19 ns |     32.81 ns | 31,323,597.2 |  1.02 |    0.02 |    1 |         - |
|               |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|  **&#39;Class test&#39;** |   **1000** |    **247.25 ns** |   **2.183 ns** |   **1.935 ns** |   **0.517 ns** |    **247.46 ns** |    **243.94 ns** |    **245.81 ns** |    **248.32 ns** |    **250.74 ns** |  **4,044,548.8** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Struct test&#39; |   1000 |    249.41 ns |   2.587 ns |   2.293 ns |   0.613 ns |    248.82 ns |    246.32 ns |    247.69 ns |    251.02 ns |    254.51 ns |  4,009,529.4 |  1.01 |    0.01 |    1 |         - |
|               |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|  **&#39;Class test&#39;** |  **10000** |  **2,570.51 ns** |  **66.154 ns** | **189.809 ns** |  **19.474 ns** |  **2,472.14 ns** |  **2,391.45 ns** |  **2,432.19 ns** |  **2,705.71 ns** |  **3,084.04 ns** |    **389,027.6** |  **1.00** |    **0.00** |    **2** |         **-** |
| &#39;Struct test&#39; |  10000 |  2,416.64 ns |  24.405 ns |  21.634 ns |   5.782 ns |  2,415.41 ns |  2,381.77 ns |  2,403.41 ns |  2,432.22 ns |  2,451.10 ns |    413,797.0 |  0.87 |    0.03 |    1 |         - |
|               |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|  **&#39;Class test&#39;** | **100000** | **24,148.00 ns** | **254.980 ns** | **238.509 ns** |  **61.583 ns** | **24,070.82 ns** | **23,748.12 ns** | **23,985.82 ns** | **24,306.89 ns** | **24,554.00 ns** |     **41,411.3** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;Struct test&#39; | 100000 | 24,763.72 ns | 404.030 ns | 778.428 ns | 114.773 ns | 24,488.54 ns | 23,884.75 ns | 24,262.01 ns | 24,956.55 ns | 27,304.55 ns |     40,381.7 |  1.05 |    0.05 |    1 |         - |

----------
**Concurrent Dictionary Set Tests**


|               Method |     Mean |    Error |   StdDev | Ratio | RatioSD | Allocated |
|--------------------- |---------:|---------:|---------:|------:|--------:|----------:|
|          AddOrUpdate | 55.86 ns | 0.755 ns | 0.669 ns |  1.00 |    0.00 |         - |
| DoesContainKeyAssign | 52.09 ns | 0.853 ns | 0.798 ns |  0.93 |    0.02 |         - |
|    DoesNotContainKey | 51.62 ns | 0.361 ns | 0.320 ns |  0.92 |    0.01 |         - |

----------
**Concurrent Dictionary Tests**


|             Method |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD | Allocated |
|------------------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|
|        ContainsKey | 26.249 ns | 0.2027 ns | 0.1582 ns | 26.237 ns |  1.00 |    0.00 |         - |
|   ContainsKeyIndex | 25.996 ns | 0.2111 ns | 0.1871 ns | 25.946 ns |  0.99 |    0.01 |         - |
| DoesNotContainsKey |  9.576 ns | 0.1973 ns | 0.2700 ns |  9.626 ns |  0.36 |    0.01 |         - |
|        TryGetValue | 14.462 ns | 0.7659 ns | 2.2462 ns | 13.061 ns |  0.52 |    0.05 |         - |

----------
**Dictionary Assignment**


|      Method |     Mean |    Error |   StdDev |   StdErr |      Min |       Q1 |   Median |       Q3 |      Max |         Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|------------ |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|-------------:|------:|--------:|-----:|-------:|----------:|
| AddOrAssign | 46.97 ns | 0.952 ns | 0.844 ns | 0.226 ns | 45.38 ns | 46.49 ns | 46.92 ns | 47.39 ns | 48.59 ns | 21,287,993.6 |  1.00 |    0.14 |    1 | 0.0344 |     216 B |
|         Key | 59.32 ns | 3.390 ns | 9.782 ns | 0.998 ns | 38.64 ns | 53.18 ns | 59.25 ns | 65.21 ns | 82.65 ns | 16,857,934.8 |  1.00 |    0.00 |    2 | 0.0344 |     216 B |

----------
**Dictionary Key Tests**


|                                             Method |      Mean |    Error |    StdDev |   StdErr |    Median |       Min |        Q1 |        Q3 |       Max |         Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|--------------------------------------------------- |----------:|---------:|----------:|---------:|----------:|----------:|----------:|----------:|----------:|-------------:|------:|--------:|-----:|-------:|----------:|
|                             Dictionary&lt;int,string&gt; |  67.48 ns | 2.669 ns |  7.787 ns | 0.787 ns |  67.52 ns |  50.67 ns |  62.62 ns |  71.92 ns |  86.59 ns | 14,818,373.6 |  0.75 |    0.10 |    4 | 0.0343 |     216 B |
|                            Dictionary&lt;long,string&gt; |  49.16 ns | 0.950 ns |  1.478 ns | 0.261 ns |  48.77 ns |  47.25 ns |  47.91 ns |  50.26 ns |  52.18 ns | 20,342,174.7 |  0.54 |    0.08 |    2 | 0.0344 |     216 B |
|                           Dictionary&lt;short,string&gt; |  66.38 ns | 3.927 ns | 11.579 ns | 1.158 ns |  66.64 ns |  48.03 ns |  57.31 ns |  73.65 ns |  96.14 ns | 15,064,876.2 |  0.75 |    0.18 |    4 | 0.0344 |     216 B |
| &#39;Dictionary&lt;int,string&gt; Using string key HashCode&#39; |  84.56 ns | 6.319 ns | 18.333 ns | 1.861 ns |  87.69 ns |  62.43 ns |  66.02 ns |  95.94 ns | 130.65 ns | 11,825,596.2 |  0.93 |    0.16 |    5 | 0.0343 |     216 B |
|           &#39;Dictionary&lt;string,string&gt; key interned&#39; | 305.04 ns | 1.790 ns |  1.587 ns | 0.424 ns | 304.89 ns | 302.58 ns | 304.44 ns | 306.26 ns | 307.89 ns |  3,278,220.4 |  3.78 |    0.19 |    7 | 0.0343 |     216 B |
|                          Dictionary&lt;string,string&gt; |  90.45 ns | 3.712 ns | 10.946 ns | 1.095 ns |  89.83 ns |  74.67 ns |  79.97 ns |  99.36 ns | 118.62 ns | 11,055,511.0 |  1.00 |    0.00 |    6 | 0.0343 |     216 B |
|                            Dictionary&lt;uint,string&gt; |  47.92 ns | 0.952 ns |  0.891 ns | 0.230 ns |  47.84 ns |  46.80 ns |  47.13 ns |  48.69 ns |  49.30 ns | 20,869,045.9 |  0.59 |    0.04 |    2 | 0.0344 |     216 B |
|                           Dictionary&lt;ulong,string&gt; |  46.79 ns | 0.659 ns |  0.616 ns | 0.159 ns |  46.72 ns |  45.88 ns |  46.40 ns |  47.17 ns |  48.20 ns | 21,370,940.3 |  0.57 |    0.04 |    1 | 0.0344 |     216 B |
|                          Dictionary&lt;ushort,string&gt; |  54.40 ns | 2.549 ns |  7.396 ns | 0.751 ns |  50.70 ns |  47.29 ns |  49.08 ns |  59.16 ns |  76.72 ns | 18,382,376.3 |  0.61 |    0.11 |    3 | 0.0344 |     216 B |

----------
**Dictionary Vs Concurrent Dictionary**


|                              Method |      Mean |     Error |    StdDev |   StdErr |    Median |       Min |        Q1 |        Q3 |       Max |         Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|------------------------------------ |----------:|----------:|----------:|---------:|----------:|----------:|----------:|----------:|----------:|-------------:|------:|--------:|-----:|-------:|----------:|
| ConcurrentDictionary&lt;string,string&gt; | 308.49 ns | 26.365 ns | 76.907 ns | 7.769 ns | 284.45 ns | 228.63 ns | 243.19 ns | 345.80 ns | 501.62 ns |  3,241,570.8 |  4.01 |    0.22 |    2 | 0.1388 |     872 B |
|           Dictionary&lt;string,string&gt; |  82.86 ns |  1.497 ns |  1.169 ns | 0.337 ns |  82.93 ns |  80.43 ns |  82.42 ns |  83.75 ns |  84.49 ns | 12,068,811.7 |  1.00 |    0.00 |    1 | 0.0343 |     216 B |

----------
**Dynamic Tests**


|                 Method |  Count |          Mean |         Error |        StdDev |       StdErr |        Median |           Min |            Q1 |            Q3 |           Max |         Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|----------------------- |------- |--------------:|--------------:|--------------:|-------------:|--------------:|--------------:|--------------:|--------------:|--------------:|-------------:|------:|--------:|-----:|-------:|----------:|
|        **&#39;Dynamic value&#39;** |    **100** |     **877.08 ns** |     **50.269 ns** |    **148.218 ns** |    **14.822 ns** |     **902.51 ns** |     **661.40 ns** |     **736.56 ns** |     **988.65 ns** |   **1,218.49 ns** |  **1,140,142.5** |  **1.00** |    **0.00** |    **3** | **0.0038** |      **24 B** |
| &#39;Dynamic with casting&#39; |    100 |     569.04 ns |     11.256 ns |     12.044 ns |     2.839 ns |     568.40 ns |     555.86 ns |     559.96 ns |     573.38 ns |     601.25 ns |  1,757,331.6 |  0.55 |    0.04 |    2 | 0.0038 |      24 B |
|        &#39;Static typing&#39; |    100 |      32.50 ns |      0.272 ns |      0.213 ns |     0.061 ns |      32.55 ns |      32.04 ns |      32.45 ns |      32.67 ns |      32.72 ns | 30,764,559.7 |  0.03 |    0.00 |    1 |      - |         - |
|                        |        |               |               |               |              |               |               |               |               |               |              |       |         |      |        |           |
|        **&#39;Dynamic value&#39;** |   **1000** |   **7,588.35 ns** |    **424.096 ns** |  **1,250.458 ns** |   **125.046 ns** |   **6,782.50 ns** |   **6,480.01 ns** |   **6,582.24 ns** |   **8,846.62 ns** |  **11,377.59 ns** |    **131,781.0** |  **1.00** |    **0.00** |    **3** |      **-** |      **24 B** |
| &#39;Dynamic with casting&#39; |   1000 |   6,479.22 ns |    362.563 ns |  1,069.024 ns |   106.902 ns |   5,781.48 ns |   5,451.43 ns |   5,543.96 ns |   7,504.15 ns |   8,727.25 ns |    154,339.5 |  0.86 |    0.10 |    2 |      - |      24 B |
|        &#39;Static typing&#39; |   1000 |     263.11 ns |      4.308 ns |      3.819 ns |     1.021 ns |     262.70 ns |     255.91 ns |     261.23 ns |     264.98 ns |     269.85 ns |  3,800,675.8 |  0.03 |    0.00 |    1 |      - |         - |
|                        |        |               |               |               |              |               |               |               |               |               |              |       |         |      |        |           |
|        **&#39;Dynamic value&#39;** |  **10000** |  **86,950.63 ns** |  **5,224.265 ns** | **15,156.541 ns** | **1,538.914 ns** |  **87,497.24 ns** |  **64,086.21 ns** |  **76,272.73 ns** |  **94,619.43 ns** | **123,385.88 ns** |     **11,500.8** |  **1.00** |    **0.00** |    **3** |      **-** |      **24 B** |
| &#39;Dynamic with casting&#39; |  10000 |  53,913.06 ns |    422.004 ns |    374.096 ns |    99.981 ns |  53,817.08 ns |  53,501.37 ns |  53,693.48 ns |  54,125.48 ns |  54,834.87 ns |     18,548.4 |  0.71 |    0.12 |    2 |      - |      24 B |
|        &#39;Static typing&#39; |  10000 |   2,503.75 ns |     42.053 ns |     41.301 ns |    10.325 ns |   2,497.86 ns |   2,461.18 ns |   2,472.05 ns |   2,511.34 ns |   2,591.25 ns |    399,400.6 |  0.03 |    0.01 |    1 |      - |         - |
|                        |        |               |               |               |              |               |               |               |               |               |              |       |         |      |        |           |
|        **&#39;Dynamic value&#39;** | **100000** | **755,516.47 ns** | **26,160.715 ns** | **75,896.978 ns** | **7,706.171 ns** | **774,122.85 ns** | **633,500.78 ns** | **680,984.77 ns** | **801,412.11 ns** | **934,830.76 ns** |      **1,323.6** |  **1.00** |    **0.00** |    **3** |      **-** |      **24 B** |
| &#39;Dynamic with casting&#39; | 100000 | 536,680.48 ns |  4,012.300 ns |  3,556.799 ns |   950.595 ns | 535,851.37 ns | 532,473.24 ns | 533,994.97 ns | 538,449.71 ns | 544,076.86 ns |      1,863.3 |  0.79 |    0.08 |    2 |      - |      25 B |
|        &#39;Static typing&#39; | 100000 |  24,670.00 ns |    215.351 ns |    201.439 ns |    52.011 ns |  24,653.03 ns |  24,302.55 ns |  24,533.32 ns |  24,817.84 ns |  24,986.61 ns |     40,535.1 |  0.04 |    0.00 |    1 |      - |         - |

----------
**Dynamo Tests**


|                             Method | Count |             Mean |          Error |         StdDev |           Median |  Ratio | RatioSD | Rank |     Gen 0 |    Gen 1 |    Gen 2 |   Allocated |
|----------------------------------- |------ |-----------------:|---------------:|---------------:|-----------------:|-------:|--------:|-----:|----------:|---------:|---------:|------------:|
|                     **AnnonymousTest** |     **1** |         **14.10 ns** |       **0.360 ns** |       **1.021 ns** |         **13.85 ns** |   **1.02** |    **0.10** |    **1** |    **0.0089** |        **-** |        **-** |        **56 B** |
|                          ClassTest |     1 |         13.89 ns |       0.385 ns |       1.087 ns |         13.62 ns |   1.00 |    0.00 |    1 |    0.0089 |        - |        - |        56 B |
| ConcurrentDictionaryConversionTest |     1 |        164.44 ns |       3.373 ns |       5.818 ns |        163.15 ns |  12.14 |    1.01 |    3 |    0.1440 |   0.0007 |        - |       904 B |
|      DynamoAnonymousConversionTest |     1 |        355.11 ns |      10.561 ns |      30.639 ns |        356.11 ns |  25.64 |    2.77 |    6 |    0.0482 |        - |        - |       304 B |
|               DynamoConversionTest |     1 |        348.67 ns |      22.593 ns |      66.260 ns |        349.87 ns |  25.04 |    5.32 |    6 |    0.0482 |        - |        - |       304 B |
|        DynamoExpandoConversionTest |     1 |        258.71 ns |      15.081 ns |      44.230 ns |        261.49 ns |  19.01 |    3.75 |    5 |    0.0520 |        - |        - |       328 B |
|                         DynamoTest |     1 |        158.68 ns |       2.681 ns |       2.377 ns |        159.20 ns |  11.62 |    0.84 |    2 |    0.0458 |        - |        - |       288 B |
|                        ExpandoTest |     1 |        197.70 ns |       9.182 ns |      26.345 ns |        195.18 ns |  14.38 |    2.16 |    4 |    0.0370 |        - |        - |       232 B |
|                                    |       |                  |                |                |                  |        |         |      |           |          |          |             |
|                     **AnnonymousTest** |    **10** |         **72.29 ns** |       **0.544 ns** |       **0.483 ns** |         **72.27 ns** |   **1.11** |    **0.01** |    **2** |    **0.0548** |   **0.0001** |        **-** |       **344 B** |
|                          ClassTest |    10 |         65.19 ns |       0.582 ns |       0.486 ns |         65.33 ns |   1.00 |    0.00 |    1 |    0.0548 |   0.0001 |        - |       344 B |
| ConcurrentDictionaryConversionTest |    10 |      1,686.56 ns |      16.096 ns |      14.268 ns |      1,684.78 ns |  25.87 |    0.34 |    4 |    1.4038 |   0.0839 |        - |     8,824 B |
|      DynamoAnonymousConversionTest |    10 |      3,316.91 ns |     191.865 ns |     531.658 ns |      3,328.66 ns |  53.28 |    3.91 |    6 |    0.4501 |   0.0076 |        - |     2,824 B |
|               DynamoConversionTest |    10 |      2,632.49 ns |      39.118 ns |      34.678 ns |      2,623.16 ns |  40.32 |    0.66 |    5 |    0.4501 |   0.0076 |        - |     2,824 B |
|        DynamoExpandoConversionTest |    10 |      2,603.06 ns |     120.591 ns |     351.770 ns |      2,644.84 ns |  43.05 |    1.92 |    5 |    0.4883 |   0.0076 |        - |     3,064 B |
|                         DynamoTest |    10 |      1,541.76 ns |      25.794 ns |      37.808 ns |      1,526.64 ns |  23.61 |    0.68 |    3 |    0.4234 |   0.0057 |        - |     2,664 B |
|                        ExpandoTest |    10 |      1,775.89 ns |      76.786 ns |     225.199 ns |      1,790.17 ns |  31.18 |    3.49 |    4 |    0.3338 |   0.0038 |        - |     2,104 B |
|                                    |       |                  |                |                |                  |        |         |      |           |          |          |             |
|                     **AnnonymousTest** |   **100** |        **605.09 ns** |      **10.791 ns** |      **21.300 ns** |        **601.31 ns** |   **1.10** |    **0.04** |    **2** |    **0.5131** |   **0.0114** |        **-** |     **3,224 B** |
|                          ClassTest |   100 |        574.27 ns |       5.159 ns |       4.573 ns |        574.15 ns |   1.00 |    0.00 |    1 |    0.5131 |   0.0114 |        - |     3,224 B |
| ConcurrentDictionaryConversionTest |   100 |     17,551.00 ns |     347.005 ns |     371.291 ns |     17,542.41 ns |  30.72 |    0.68 |    4 |   14.0076 |   4.6387 |        - |    88,024 B |
|      DynamoAnonymousConversionTest |   100 |     33,530.56 ns |   1,072.940 ns |   3,078.466 ns |     33,133.80 ns |  51.86 |    7.51 |    7 |    4.4556 |   0.6104 |        - |    28,024 B |
|               DynamoConversionTest |   100 |     26,146.21 ns |     483.276 ns |     428.412 ns |     26,176.10 ns |  45.53 |    0.75 |    6 |    4.4556 |   0.6104 |        - |    28,024 B |
|        DynamoExpandoConversionTest |   100 |     20,406.49 ns |     384.280 ns |     359.456 ns |     20,208.87 ns |  35.58 |    0.84 |    5 |    4.8218 |   0.6714 |        - |    30,424 B |
|                         DynamoTest |   100 |     18,759.62 ns |   1,132.989 ns |   3,340.644 ns |     18,790.80 ns |  27.09 |    2.46 |    4 |    4.2114 |   0.5798 |        - |    26,424 B |
|                        ExpandoTest |   100 |     14,574.47 ns |     225.528 ns |     231.601 ns |     14,548.93 ns |  25.33 |    0.40 |    3 |    3.3112 |   0.4120 |        - |    20,824 B |
|                                    |       |                  |                |                |                  |        |         |      |           |          |          |             |
|                     **AnnonymousTest** |  **1000** |      **7,111.92 ns** |     **284.431 ns** |     **834.187 ns** |      **6,641.82 ns** |   **1.16** |    **0.12** |    **2** |    **5.0964** |   **0.8469** |        **-** |    **32,024 B** |
|                          ClassTest |  1000 |      6,169.14 ns |     176.351 ns |     491.595 ns |      5,945.26 ns |   1.00 |    0.00 |    1 |    5.0964 |   0.8469 |        - |    32,024 B |
| ConcurrentDictionaryConversionTest |  1000 |    322,879.36 ns |   2,914.804 ns |   2,583.898 ns |    321,833.62 ns |  51.56 |    4.66 |    6 |  140.1367 |  69.8242 |        - |   880,024 B |
|      DynamoAnonymousConversionTest |  1000 |    261,024.48 ns |   3,536.732 ns |   3,135.221 ns |    259,904.88 ns |  41.67 |    3.65 |    5 |   44.4336 |  17.5781 |        - |   280,025 B |
|               DynamoConversionTest |  1000 |    259,369.69 ns |   2,550.268 ns |   2,260.746 ns |    259,152.47 ns |  41.41 |    3.70 |    5 |   44.4336 |  17.5781 |        - |   280,025 B |
|        DynamoExpandoConversionTest |  1000 |    214,635.77 ns |   4,186.073 ns |   3,915.655 ns |    215,303.08 ns |  34.02 |    3.43 |    4 |   48.3398 |  22.4609 |        - |   304,024 B |
|                         DynamoTest |  1000 |    158,912.33 ns |   1,406.588 ns |   1,174.565 ns |    158,690.55 ns |  25.63 |    2.28 |    3 |   41.9922 |  20.2637 |        - |   264,025 B |
|                        ExpandoTest |  1000 |    179,256.24 ns |   9,554.429 ns |  27,104.325 ns |    167,395.51 ns |  28.69 |    3.14 |    3 |   32.9590 |  10.9863 |        - |   208,024 B |
|                                    |       |                  |                |                |                  |        |         |      |           |          |          |             |
|                     **AnnonymousTest** | **10000** |    **116,914.95 ns** |   **7,136.561 ns** |  **21,042.313 ns** |    **114,204.88 ns** |   **1.55** |    **0.08** |    **2** |   **50.7813** |  **25.3906** |        **-** |   **320,024 B** |
|                          ClassTest | 10000 |     81,488.43 ns |   1,222.357 ns |   1,143.393 ns |     81,191.04 ns |   1.00 |    0.00 |    1 |   50.7813 |  25.3906 |        - |   320,024 B |
| ConcurrentDictionaryConversionTest | 10000 | 13,037,652.58 ns | 257,695.664 ns | 296,762.710 ns | 13,121,786.72 ns | 159.60 |    4.81 |    8 | 1609.3750 | 703.1250 | 265.6250 | 8,800,026 B |
|      DynamoAnonymousConversionTest | 10000 |  3,661,782.37 ns |  59,113.502 ns |  52,402.580 ns |  3,643,004.30 ns |  44.98 |    0.85 |    6 |  445.3125 | 222.6563 |        - | 2,800,033 B |
|               DynamoConversionTest | 10000 |  3,746,548.16 ns |  67,990.820 ns |  69,821.566 ns |  3,759,405.47 ns |  46.05 |    1.23 |    7 |  445.3125 | 222.6563 |        - | 2,800,033 B |
|        DynamoExpandoConversionTest | 10000 |  3,467,463.52 ns | 126,720.894 ns | 369,650.796 ns |  3,264,697.07 ns |  49.48 |    3.55 |    5 |  484.3750 | 242.1875 |        - | 3,040,024 B |
|                         DynamoTest | 10000 |  2,504,872.01 ns |  30,818.472 ns |  27,319.773 ns |  2,498,462.89 ns |  30.77 |    0.51 |    4 |  417.9688 | 207.0313 |        - | 2,640,033 B |
|                        ExpandoTest | 10000 |  2,021,588.86 ns |  38,715.368 ns |  43,032.029 ns |  2,023,287.50 ns |  24.80 |    0.68 |    3 |  328.1250 | 164.0625 |        - | 2,080,024 B |

----------
**Enum Vs Byte Packing**


| Method |     Mean |     Error |    StdDev |    StdErr |   Median |      Min |       Q1 |       Q3 |      Max |          Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|--------------:|------:|--------:|-----:|-------:|----------:|
|   Byte | 2.354 ns | 0.0768 ns | 0.0681 ns | 0.0182 ns | 2.355 ns | 2.248 ns | 2.308 ns | 2.395 ns | 2.521 ns | 424,765,473.3 |  0.48 |    0.02 |    1 | 0.0051 |      32 B |
|   Enum | 4.954 ns | 0.1078 ns | 0.1059 ns | 0.0265 ns | 4.981 ns | 4.758 ns | 4.893 ns | 5.015 ns | 5.106 ns | 201,837,027.8 |  1.00 |    0.00 |    2 | 0.0089 |      56 B |
|   UInt | 5.005 ns | 0.2871 ns | 0.8421 ns | 0.0846 ns | 4.669 ns | 4.149 ns | 4.356 ns | 5.662 ns | 7.244 ns | 199,805,587.1 |  1.28 |    0.11 |    2 | 0.0089 |      56 B |

----------
**Field Vs Property Tests**


|          Method |  Count |         Mean |      Error |       StdDev |     StdErr |       Median |          Min |           Q1 |           Q3 |          Max |         Op/s | Ratio | RatioSD | Rank | Allocated |
|---------------- |------- |-------------:|-----------:|-------------:|-----------:|-------------:|-------------:|-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|
|    **&#39;Field test&#39;** |    **100** |     **34.76 ns** |   **1.230 ns** |     **3.608 ns** |   **0.363 ns** |     **33.54 ns** |     **30.99 ns** |     **31.46 ns** |     **36.80 ns** |     **44.56 ns** | **28,771,684.7** |  **1.16** |    **0.05** |    **1** |         **-** |
|    &#39;Local test&#39; |    100 |     32.55 ns |   0.304 ns |     0.270 ns |   0.072 ns |     32.46 ns |     32.20 ns |     32.35 ns |     32.71 ns |     33.15 ns | 30,725,451.9 |  1.00 |    0.02 |    1 |         - |
| &#39;Property test&#39; |    100 |     32.41 ns |   0.619 ns |     0.549 ns |   0.147 ns |     32.21 ns |     31.71 ns |     32.06 ns |     32.67 ns |     33.65 ns | 30,855,906.6 |  1.00 |    0.00 |    1 |         - |
|                 |        |              |            |              |            |              |              |              |              |              |              |       |         |      |           |
|    **&#39;Field test&#39;** |   **1000** |    **255.89 ns** |   **1.370 ns** |     **1.144 ns** |   **0.317 ns** |    **255.69 ns** |    **254.24 ns** |    **255.58 ns** |    **256.13 ns** |    **258.66 ns** |  **3,907,927.8** |  **0.99** |    **0.00** |    **1** |         **-** |
|    &#39;Local test&#39; |   1000 |    258.15 ns |   2.368 ns |     2.215 ns |   0.572 ns |    257.28 ns |    254.62 ns |    256.80 ns |    260.53 ns |    261.16 ns |  3,873,723.0 |  1.00 |    0.01 |    1 |         - |
| &#39;Property test&#39; |   1000 |    257.31 ns |   1.550 ns |     1.210 ns |   0.349 ns |    257.58 ns |    254.81 ns |    256.98 ns |    258.31 ns |    258.66 ns |  3,886,422.7 |  1.00 |    0.00 |    1 |         - |
|                 |        |              |            |              |            |              |              |              |              |              |              |       |         |      |           |
|    **&#39;Field test&#39;** |  **10000** |  **2,681.42 ns** |  **84.901 ns** |   **247.660 ns** |  **25.017 ns** |  **2,537.49 ns** |  **2,475.38 ns** |  **2,504.61 ns** |  **2,888.25 ns** |  **3,336.60 ns** |    **372,937.1** |  **1.20** |    **0.06** |    **1** |         **-** |
|    &#39;Local test&#39; |  10000 |  2,543.11 ns |  49.573 ns |    46.371 ns |  11.973 ns |  2,530.48 ns |  2,495.92 ns |  2,504.93 ns |  2,572.92 ns |  2,630.95 ns |    393,219.6 |  1.01 |    0.02 |    1 |         - |
| &#39;Property test&#39; |  10000 |  2,521.48 ns |  11.477 ns |    10.735 ns |   2.772 ns |  2,521.31 ns |  2,493.69 ns |  2,517.45 ns |  2,528.24 ns |  2,537.86 ns |    396,592.7 |  1.00 |    0.00 |    1 |         - |
|                 |        |              |            |              |            |              |              |              |              |              |              |       |         |      |           |
|    **&#39;Field test&#39;** | **100000** | **26,994.67 ns** | **776.838 ns** | **2,241.354 ns** | **228.757 ns** | **26,027.60 ns** | **24,888.33 ns** | **25,473.48 ns** | **28,765.40 ns** | **33,842.31 ns** |     **37,044.4** |  **0.88** |    **0.09** |    **1** |         **-** |
|    &#39;Local test&#39; | 100000 | 33,136.43 ns | 886.114 ns | 2,528.132 ns | 260.757 ns | 32,770.84 ns | 28,487.84 ns | 31,291.90 ns | 34,762.79 ns | 40,488.69 ns |     30,178.3 |  1.08 |    0.08 |    3 |         - |
| &#39;Property test&#39; | 100000 | 31,208.59 ns | 620.956 ns | 1,534.850 ns | 180.884 ns | 30,618.19 ns | 28,835.71 ns | 30,118.02 ns | 31,921.12 ns | 34,848.88 ns |     32,042.5 |  1.00 |    0.00 |    2 |         - |

----------
**Hash Table Vs Dictionary**


|                      Method |      Mean |    Error |    StdDev |   StdErr |    Median |      Min |       Q1 |        Q3 |       Max |         Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|---------------------------- |----------:|---------:|----------:|---------:|----------:|---------:|---------:|----------:|----------:|-------------:|------:|--------:|-----:|-------:|----------:|
|      Dictionary&lt;int,string&gt; |  34.76 ns | 0.695 ns |  0.744 ns | 0.175 ns |  34.70 ns | 33.47 ns | 34.27 ns |  35.26 ns |  36.22 ns | 28,768,185.0 |  0.63 |    0.03 |    1 | 0.0344 |     216 B |
|   Dictionary&lt;string,string&gt; |  54.32 ns | 1.104 ns |  1.844 ns | 0.307 ns |  54.08 ns | 51.70 ns | 52.90 ns |  55.37 ns |  58.72 ns | 18,408,432.4 |  1.00 |    0.00 |    2 | 0.0343 |     216 B |
|    &#39;Hashtable (int,string)&#39; |  55.84 ns | 2.008 ns |  5.889 ns | 0.592 ns |  56.38 ns | 48.57 ns | 50.06 ns |  59.81 ns |  72.13 ns | 17,908,300.9 |  1.12 |    0.07 |    2 | 0.0344 |     216 B |
| &#39;Hashtable (string,string)&#39; |  56.30 ns | 3.156 ns |  9.307 ns | 0.931 ns |  57.15 ns | 45.87 ns | 46.97 ns |  62.40 ns |  85.50 ns | 17,761,621.5 |  1.20 |    0.13 |    2 | 0.0268 |     168 B |
|            StringDictionary | 116.01 ns | 6.278 ns | 18.510 ns | 1.851 ns | 107.23 ns | 95.11 ns | 99.25 ns | 131.71 ns | 168.59 ns |  8,620,151.0 |  2.52 |    0.18 |    3 | 0.0408 |     256 B |

----------
**IEnumerable Conversion**


|           Method |  Count |           Mean |        Error |       StdDev |      StdErr |         Median |            Min |             Q1 |             Q3 |            Max |        Op/s | Ratio | RatioSD | Rank |    Gen 0 |    Gen 1 |    Gen 2 |   Allocated |
|----------------- |------- |---------------:|-------------:|-------------:|------------:|---------------:|---------------:|---------------:|---------------:|---------------:|------------:|------:|--------:|-----:|---------:|---------:|---------:|------------:|
|            **Array** |    **100** |     **1,690.3 ns** |     **83.16 ns** |    **234.56 ns** |    **24.45 ns** |     **1,602.6 ns** |     **1,456.6 ns** |     **1,511.8 ns** |     **1,780.7 ns** |     **2,415.9 ns** |   **591,623.7** |  **2.12** |    **0.21** |    **7** |   **0.2785** |        **-** |        **-** |     **1,768 B** |
|     &#39;Array Copy&#39; |    100 |       604.4 ns |     29.21 ns |     86.13 ns |     8.61 ns |       551.8 ns |       514.8 ns |       529.5 ns |       695.7 ns |       793.9 ns | 1,654,570.7 |  0.75 |    0.04 |    2 |   0.2251 |        - |        - |     1,416 B |
| &#39;Array for loop&#39; |    100 |       788.5 ns |     15.72 ns |     31.02 ns |     4.48 ns |       786.5 ns |       743.1 ns |       761.0 ns |       809.4 ns |       864.1 ns | 1,268,168.2 |  0.84 |    0.04 |    4 |   0.2518 |        - |        - |     1,584 B |
|      IEnumerable |    100 |       936.3 ns |     14.60 ns |     14.34 ns |     3.59 ns |       934.1 ns |       910.1 ns |       926.8 ns |       945.1 ns |       965.6 ns | 1,068,087.2 |  1.00 |    0.00 |    5 |   0.1316 |        - |        - |       832 B |
|             List |    100 |     1,430.4 ns |     69.78 ns |    205.74 ns |    20.57 ns |     1,511.2 ns |     1,176.9 ns |     1,203.3 ns |     1,585.5 ns |     1,957.6 ns |   699,099.1 |  1.75 |    0.11 |    6 |   0.3204 |        - |        - |     2,016 B |
|    &#39;List CopyTo&#39; |    100 |       558.1 ns |     31.96 ns |     94.23 ns |     9.42 ns |       501.3 ns |       466.1 ns |       476.2 ns |       645.6 ns |       795.9 ns | 1,791,751.0 |  0.59 |    0.10 |    1 |   0.2651 |        - |        - |     1,664 B |
|  &#39;List for loop&#39; |    100 |       720.7 ns |     46.67 ns |    136.88 ns |    13.76 ns |       721.4 ns |       556.6 ns |       593.8 ns |       823.4 ns |     1,052.1 ns | 1,387,463.7 |  0.83 |    0.13 |    3 |   0.2918 |        - |        - |     1,832 B |
|                  |        |                |              |              |             |                |                |                |                |                |             |       |         |      |          |          |          |             |
|            **Array** |   **1000** |     **8,280.9 ns** |    **116.80 ns** |    **103.54 ns** |    **27.67 ns** |     **8,266.7 ns** |     **8,141.1 ns** |     **8,197.6 ns** |     **8,325.5 ns** |     **8,486.2 ns** |   **120,759.6** |  **1.39** |    **0.15** |    **5** |   **1.8921** |   **0.0153** |        **-** |    **11,888 B** |
|     &#39;Array Copy&#39; |   1000 |     3,503.1 ns |     68.78 ns |     64.34 ns |    16.61 ns |     3,484.8 ns |     3,439.2 ns |     3,449.2 ns |     3,551.5 ns |     3,661.4 ns |   285,458.6 |  0.58 |    0.08 |    1 |   1.8272 |   0.0153 |        - |    11,480 B |
| &#39;Array for loop&#39; |   1000 |     4,033.1 ns |     62.83 ns |     55.70 ns |    14.89 ns |     4,019.9 ns |     3,969.3 ns |     4,001.0 ns |     4,061.7 ns |     4,149.7 ns |   247,950.6 |  0.68 |    0.08 |    2 |   1.8616 |   0.0153 |        - |    11,704 B |
|      IEnumerable |   1000 |     6,441.4 ns |    363.20 ns |  1,070.90 ns |   107.09 ns |     5,833.2 ns |     5,340.0 ns |     5,613.8 ns |     7,242.5 ns |     9,074.2 ns |   155,246.6 |  1.00 |    0.00 |    4 |   0.7095 |        - |        - |     4,488 B |
|             List |   1000 |     8,945.6 ns |    172.11 ns |    184.15 ns |    43.41 ns |     8,890.2 ns |     8,708.7 ns |     8,800.3 ns |     9,087.6 ns |     9,305.9 ns |   111,786.5 |  1.41 |    0.24 |    6 |   2.0447 |   0.0153 |        - |    12,912 B |
|    &#39;List CopyTo&#39; |   1000 |     4,313.8 ns |     80.13 ns |    106.97 ns |    21.39 ns |     4,302.0 ns |     4,142.9 ns |     4,246.5 ns |     4,395.4 ns |     4,532.2 ns |   231,813.7 |  0.63 |    0.13 |    3 |   1.9913 |   0.0267 |        - |    12,504 B |
|  &#39;List for loop&#39; |   1000 |     4,365.5 ns |     72.78 ns |    117.53 ns |    20.16 ns |     4,327.6 ns |     4,235.4 ns |     4,289.7 ns |     4,400.3 ns |     4,684.0 ns |   229,067.0 |  0.62 |    0.12 |    3 |   2.0218 |   0.0305 |        - |    12,728 B |
|                  |        |                |              |              |             |                |                |                |                |                |             |       |         |      |          |          |          |             |
|            **Array** |  **10000** |    **79,485.7 ns** |    **845.31 ns** |    **790.70 ns** |   **204.16 ns** |    **79,203.0 ns** |    **78,616.5 ns** |    **78,884.4 ns** |    **80,008.7 ns** |    **81,318.0 ns** |    **12,580.9** |  **1.47** |    **0.03** |    **6** |  **20.5078** |   **1.9531** |        **-** |   **129,416 B** |
|     &#39;Array Copy&#39; |  10000 |    31,167.0 ns |    447.38 ns |    396.59 ns |   105.99 ns |    31,234.5 ns |    30,351.8 ns |    30,872.2 ns |    31,402.0 ns |    31,881.7 ns |    32,085.3 |  0.57 |    0.01 |    2 |  16.4185 |   1.3428 |        - |   103,472 B |
| &#39;Array for loop&#39; |  10000 |    41,787.6 ns |  1,652.43 ns |  4,846.28 ns |   487.07 ns |    39,203.4 ns |    37,640.4 ns |    38,157.9 ns |    46,147.7 ns |    54,862.7 ns |    23,930.5 |  0.80 |    0.10 |    3 |  20.5078 |   2.2583 |        - |   129,232 B |
|      IEnumerable |  10000 |    54,241.4 ns |  1,000.57 ns |    935.94 ns |   241.66 ns |    53,773.3 ns |    53,066.1 ns |    53,543.1 ns |    54,851.8 ns |    56,347.2 ns |    18,436.1 |  1.00 |    0.00 |    5 |  10.4370 |   1.2817 |        - |    66,024 B |
|             List |  10000 |    88,270.6 ns |  3,274.05 ns |  9,446.37 ns |   964.12 ns |    83,533.6 ns |    80,660.2 ns |    81,805.0 ns |    91,047.5 ns |   113,851.8 ns |    11,328.8 |  1.55 |    0.13 |    7 |  20.8740 |   2.5635 |        - |   131,864 B |
|    &#39;List CopyTo&#39; |  10000 |    30,190.5 ns |    359.11 ns |    299.87 ns |    83.17 ns |    30,193.9 ns |    29,787.1 ns |    29,955.9 ns |    30,427.4 ns |    30,732.9 ns |    33,123.0 |  0.56 |    0.01 |    1 |  16.7847 |   2.0752 |        - |   105,920 B |
|  &#39;List for loop&#39; |  10000 |    43,034.9 ns |  1,705.39 ns |  5,028.37 ns |   502.84 ns |    40,686.0 ns |    38,076.5 ns |    39,319.3 ns |    47,165.1 ns |    57,846.7 ns |    23,236.9 |  0.89 |    0.10 |    4 |  20.9351 |   3.4790 |        - |   131,680 B |
|                  |        |                |              |              |             |                |                |                |                |                |             |       |         |      |          |          |          |             |
|            **Array** | **100000** |   **965,729.2 ns** | **18,623.40 ns** | **20,699.86 ns** | **4,748.87 ns** |   **960,648.8 ns** |   **934,459.6 ns** |   **952,481.4 ns** |   **977,953.7 ns** | **1,006,883.8 ns** |     **1,035.5** |  **1.23** |    **0.08** |    **6** | **298.8281** | **298.8281** | **298.8281** | **1,225,568 B** |
|     &#39;Array Copy&#39; | 100000 |   452,540.5 ns | 14,378.21 ns | 41,941.91 ns | 4,236.77 ns |   431,112.0 ns |   416,530.0 ns |   426,025.8 ns |   467,278.0 ns |   556,253.4 ns |     2,209.7 |  0.66 |    0.05 |    1 | 299.8047 | 299.8047 | 299.8047 | 1,100,800 B |
| &#39;Array for loop&#39; | 100000 |   505,668.1 ns |  7,568.31 ns |  7,079.40 ns | 1,827.89 ns |   504,960.3 ns |   495,949.2 ns |   500,123.4 ns |   508,026.2 ns |   518,372.9 ns |     1,977.6 |  0.64 |    0.04 |    3 | 299.8047 | 299.8047 | 299.8047 | 1,225,384 B |
|      IEnumerable | 100000 |   690,524.8 ns | 26,817.87 ns | 79,073.10 ns | 7,907.31 ns |   656,108.7 ns |   605,796.3 ns |   623,662.7 ns |   750,376.7 ns |   927,361.2 ns |     1,448.2 |  1.00 |    0.00 |    5 | 124.0234 | 124.0234 | 124.0234 |   524,848 B |
|             List | 100000 | 1,049,636.6 ns | 20,207.57 ns | 26,976.53 ns | 5,395.31 ns | 1,047,216.0 ns | 1,011,385.5 ns | 1,026,488.3 ns | 1,066,040.2 ns | 1,109,960.9 ns |       952.7 |  1.34 |    0.07 |    7 | 398.4375 | 398.4375 | 398.4375 | 1,573,824 B |
|    &#39;List CopyTo&#39; | 100000 |   453,988.7 ns | 20,837.31 ns | 61,439.28 ns | 6,143.93 ns |   420,574.7 ns |   401,410.0 ns |   408,675.7 ns |   501,326.0 ns |   627,540.3 ns |     2,202.7 |  0.66 |    0.07 |    2 | 389.1602 | 389.1602 | 389.1602 | 1,449,072 B |
|  &#39;List for loop&#39; | 100000 |   605,808.5 ns | 27,083.82 ns | 79,004.78 ns | 7,980.69 ns |   600,767.8 ns |   479,177.8 ns |   546,176.1 ns |   646,794.2 ns |   800,673.5 ns |     1,650.7 |  0.88 |    0.11 |    4 | 387.6953 | 387.6953 | 387.6953 | 1,573,635 B |

----------
**IEnumerable Tests**


|               Method |  Count |          Mean |         Error |        StdDev |       StdErr |        Median |           Min |            Q1 |            Q3 |           Max |         Op/s | Ratio | RatioSD | Rank |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|--------------------- |------- |--------------:|--------------:|--------------:|-------------:|--------------:|--------------:|--------------:|--------------:|--------------:|-------------:|------:|--------:|-----:|--------:|--------:|--------:|----------:|
|     **&#39;ArrayList test&#39;** |    **100** |     **505.33 ns** |      **7.388 ns** |      **6.911 ns** |     **1.784 ns** |     **503.84 ns** |     **496.32 ns** |     **500.53 ns** |     **508.86 ns** |     **521.18 ns** |  **1,978,905.9** |  **2.56** |    **0.32** |    **5** |  **0.0076** |       **-** |       **-** |      **48 B** |
|         &#39;Array test&#39; |    100 |      32.00 ns |      0.439 ns |      0.411 ns |     0.106 ns |      31.86 ns |      31.43 ns |      31.69 ns |      32.30 ns |      32.71 ns | 31,247,541.6 |  0.16 |    0.02 |    1 |       - |       - |       - |         - |
| &#39;ConcurrentBag test&#39; |    100 |     463.95 ns |      3.536 ns |      3.307 ns |     0.854 ns |     463.65 ns |     458.30 ns |     462.30 ns |     467.10 ns |     468.97 ns |  2,155,402.2 |  2.35 |    0.28 |    4 |  0.0725 |       - |       - |     456 B |
|       &#39;HashSet test&#39; |    100 |     203.08 ns |      3.552 ns |      3.323 ns |     0.858 ns |     202.83 ns |     198.94 ns |     200.24 ns |     205.39 ns |     208.98 ns |  4,924,120.5 |  1.03 |    0.13 |    3 |       - |       - |       - |         - |
|          &#39;List test&#39; |    100 |     194.00 ns |      6.916 ns |     19.619 ns |     2.034 ns |     183.43 ns |     172.75 ns |     179.80 ns |     206.26 ns |     252.77 ns |  5,154,602.2 |  1.00 |    0.00 |    2 |       - |       - |       - |         - |
|                      |        |               |               |               |              |               |               |               |               |               |              |       |         |      |         |         |         |           |
|     **&#39;ArrayList test&#39;** |   **1000** |   **6,077.77 ns** |    **178.494 ns** |    **500.515 ns** |    **52.468 ns** |   **5,972.13 ns** |   **5,460.86 ns** |   **5,700.25 ns** |   **6,269.21 ns** |   **7,562.61 ns** |    **164,534.0** |  **3.42** |    **0.27** |    **5** |  **0.0076** |       **-** |       **-** |      **48 B** |
|         &#39;Array test&#39; |   1000 |     251.66 ns |      5.053 ns |      7.717 ns |     1.386 ns |     248.58 ns |     244.03 ns |     246.38 ns |     253.78 ns |     273.66 ns |  3,973,635.1 |  0.15 |    0.00 |    1 |       - |       - |       - |         - |
| &#39;ConcurrentBag test&#39; |   1000 |   4,242.04 ns |     81.811 ns |    103.465 ns |    21.574 ns |   4,235.63 ns |   4,086.83 ns |   4,148.29 ns |   4,305.47 ns |   4,439.28 ns |    235,735.6 |  2.42 |    0.09 |    4 |  0.6409 |  0.0076 |       - |   4,056 B |
|       &#39;HashSet test&#39; |   1000 |   1,971.16 ns |     23.371 ns |     20.717 ns |     5.537 ns |   1,968.27 ns |   1,925.76 ns |   1,964.25 ns |   1,984.90 ns |   2,004.93 ns |    507,315.7 |  1.13 |    0.03 |    3 |       - |       - |       - |         - |
|          &#39;List test&#39; |   1000 |   1,748.23 ns |     34.606 ns |     37.028 ns |     8.728 ns |   1,740.35 ns |   1,699.07 ns |   1,717.02 ns |   1,778.54 ns |   1,819.20 ns |    572,007.9 |  1.00 |    0.00 |    2 |       - |       - |       - |         - |
|                      |        |               |               |               |              |               |               |               |               |               |              |       |         |      |         |         |         |           |
|     **&#39;ArrayList test&#39;** |  **10000** |  **47,658.76 ns** |  **1,598.432 ns** |  **4,687.926 ns** |   **471.154 ns** |  **45,443.73 ns** |  **43,173.21 ns** |  **44,329.61 ns** |  **49,901.31 ns** |  **59,560.46 ns** |     **20,982.5** |  **2.14** |    **0.25** |    **5** |       **-** |       **-** |       **-** |      **48 B** |
|         &#39;Array test&#39; |  10000 |   2,548.48 ns |     50.491 ns |    130.334 ns |    14.757 ns |   2,494.11 ns |   2,395.44 ns |   2,465.39 ns |   2,574.03 ns |   2,963.41 ns |    392,390.6 |  0.11 |    0.01 |    1 |       - |       - |       - |         - |
| &#39;ConcurrentBag test&#39; |  10000 |  41,284.45 ns |    466.935 ns |    413.926 ns |   110.626 ns |  41,273.57 ns |  40,603.12 ns |  40,947.65 ns |  41,676.27 ns |  41,892.67 ns |     24,222.2 |  1.57 |    0.25 |    4 |  6.3477 |  0.6714 |       - |  40,056 B |
|       &#39;HashSet test&#39; |  10000 |  19,780.47 ns |    310.650 ns |    242.535 ns |    70.014 ns |  19,736.80 ns |  19,506.46 ns |  19,672.78 ns |  19,815.43 ns |  20,227.13 ns |     50,554.9 |  0.72 |    0.10 |    2 |       - |       - |       - |         - |
|          &#39;List test&#39; |  10000 |  22,475.92 ns |  1,202.317 ns |  3,430.277 ns |   353.806 ns |  21,454.56 ns |  17,219.84 ns |  20,262.82 ns |  23,574.10 ns |  31,651.68 ns |     44,492.1 |  1.00 |    0.00 |    3 |       - |       - |       - |         - |
|                      |        |               |               |               |              |               |               |               |               |               |              |       |         |      |         |         |         |           |
|     **&#39;ArrayList test&#39;** | **100000** | **451,734.69 ns** |  **8,059.056 ns** |  **8,276.057 ns** | **2,007.239 ns** | **449,775.59 ns** | **444,991.02 ns** | **445,757.81 ns** | **454,202.64 ns** | **477,244.43 ns** |      **2,213.7** |  **2.10** |    **0.04** |    **4** |       **-** |       **-** |       **-** |      **49 B** |
|         &#39;Array test&#39; | 100000 |  24,119.59 ns |    212.578 ns |    177.512 ns |    49.233 ns |  24,122.42 ns |  23,823.25 ns |  24,026.14 ns |  24,214.33 ns |  24,518.18 ns |     41,460.1 |  0.11 |    0.00 |    1 |       - |       - |       - |         - |
| &#39;ConcurrentBag test&#39; | 100000 | 441,126.82 ns |  6,062.129 ns |  5,373.920 ns | 1,436.240 ns | 438,505.59 ns | 435,264.65 ns | 437,313.29 ns | 444,501.09 ns | 453,501.17 ns |      2,266.9 |  2.05 |    0.04 |    3 | 64.4531 | 64.4531 | 64.4531 | 400,359 B |
|       &#39;HashSet test&#39; | 100000 | 227,935.21 ns | 11,282.329 ns | 33,089.119 ns | 3,325.582 ns | 234,599.95 ns | 191,273.83 ns | 194,765.65 ns | 254,267.57 ns | 311,844.90 ns |      4,387.2 |  0.96 |    0.13 |    2 |       - |       - |       - |         - |
|          &#39;List test&#39; | 100000 | 215,376.32 ns |  4,109.914 ns |  3,643.332 ns |   973.721 ns | 214,766.25 ns | 210,172.40 ns | 212,422.59 ns | 217,689.55 ns | 221,878.21 ns |      4,643.0 |  1.00 |    0.00 |    2 |       - |       - |       - |         - |

----------
**Increment Tests**


| Method |  Count |         Mean |        Error |       StdDev |     StdErr |       Median |          Min |           Q1 |           Q3 |          Max |         Op/s | Ratio | RatioSD | Rank | Allocated |
|------- |------- |-------------:|-------------:|-------------:|-----------:|-------------:|-------------:|-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|
|   **x+=1** |    **100** |     **35.38 ns** |     **0.555 ns** |     **0.463 ns** |   **0.128 ns** |     **35.24 ns** |     **34.75 ns** |     **35.09 ns** |     **35.76 ns** |     **36.15 ns** | **28,266,741.2** |  **1.13** |    **0.01** |    **2** |         **-** |
|    x++ |    100 |     31.23 ns |     0.170 ns |     0.142 ns |   0.039 ns |     31.25 ns |     30.92 ns |     31.12 ns |     31.32 ns |     31.44 ns | 32,021,328.4 |  1.00 |    0.00 |    1 |         - |
|    ++x |    100 |     31.56 ns |     0.257 ns |     0.228 ns |   0.061 ns |     31.52 ns |     31.26 ns |     31.38 ns |     31.70 ns |     31.95 ns | 31,685,870.9 |  1.01 |    0.01 |    1 |         - |
|        |        |              |              |              |            |              |              |              |              |              |              |       |         |      |           |
|   **x+=1** |   **1000** |    **251.95 ns** |     **2.253 ns** |     **2.108 ns** |   **0.544 ns** |    **251.71 ns** |    **249.08 ns** |    **250.03 ns** |    **253.52 ns** |    **255.41 ns** |  **3,969,042.9** |  **1.00** |    **0.02** |    **1** |         **-** |
|    x++ |   1000 |    253.80 ns |     5.100 ns |     6.264 ns |   1.335 ns |    254.58 ns |    243.57 ns |    247.54 ns |    258.97 ns |    262.84 ns |  3,940,042.5 |  1.00 |    0.00 |    1 |         - |
|    ++x |   1000 |    259.64 ns |     4.179 ns |     3.909 ns |   1.009 ns |    259.50 ns |    253.98 ns |    256.44 ns |    261.33 ns |    267.74 ns |  3,851,533.5 |  1.03 |    0.03 |    1 |         - |
|        |        |              |              |              |            |              |              |              |              |              |              |       |         |      |           |
|   **x+=1** |  **10000** |  **2,490.82 ns** |    **19.642 ns** |    **17.412 ns** |   **4.654 ns** |  **2,484.00 ns** |  **2,464.71 ns** |  **2,480.85 ns** |  **2,501.91 ns** |  **2,526.15 ns** |    **401,473.4** |  **0.95** |    **0.09** |    **1** |         **-** |
|    x++ |  10000 |  2,716.50 ns |    85.144 ns |   245.661 ns |  25.073 ns |  2,622.87 ns |  2,467.59 ns |  2,506.28 ns |  2,835.74 ns |  3,315.29 ns |    368,120.3 |  1.00 |    0.00 |    2 |         - |
|    ++x |  10000 |  2,502.83 ns |    36.159 ns |    33.823 ns |   8.733 ns |  2,497.52 ns |  2,446.72 ns |  2,482.19 ns |  2,520.83 ns |  2,564.23 ns |    399,547.2 |  0.95 |    0.10 |    1 |         - |
|        |        |              |              |              |            |              |              |              |              |              |              |       |         |      |           |
|   **x+=1** | **100000** | **26,654.59 ns** | **1,025.398 ns** | **2,824.244 ns** | **301.065 ns** | **25,003.73 ns** | **23,908.64 ns** | **24,709.62 ns** | **27,859.12 ns** | **36,346.03 ns** |     **37,517.0** |  **1.11** |    **0.05** |    **1** |         **-** |
|    x++ | 100000 | 25,080.77 ns |   319.793 ns |   299.135 ns |  77.236 ns | 25,149.76 ns | 24,549.74 ns | 24,834.49 ns | 25,307.57 ns | 25,565.75 ns |     39,871.2 |  1.00 |    0.00 |    1 |         - |
|    ++x | 100000 | 24,717.75 ns |   465.955 ns |   478.501 ns | 116.054 ns | 24,655.80 ns | 24,112.66 ns | 24,322.14 ns | 24,948.74 ns | 25,573.83 ns |     40,456.8 |  0.99 |    0.03 |    1 |         - |

----------
**Is Tests**


|           Method |       Mean |     Error |    StdDev |     Median | Ratio | RatioSD | Allocated |
|----------------- |-----------:|----------:|----------:|-----------:|------:|--------:|----------:|
| IsAssignableTest | 12.3103 ns | 0.2844 ns | 0.2521 ns | 12.2654 ns |     ? |       ? |         - |
|     IsMethodTest |  0.0017 ns | 0.0028 ns | 0.0068 ns |  0.0000 ns |     ? |       ? |         - |
|           IsTest |  0.0525 ns | 0.0266 ns | 0.0306 ns |  0.0550 ns |     ? |       ? |         - |
|       IsTypeTest | 14.3258 ns | 0.1220 ns | 0.1082 ns | 14.3113 ns |     ? |       ? |         - |

----------
**List Add Vs Add Range**


|             Method |  Count |            Mean |         Error |        StdDev |       StdErr |          Median |             Min |              Q1 |              Q3 |            Max |         Op/s | Ratio | RatioSD | Rank |   Gen 0 |   Gen 1 |   Gen 2 |   Allocated |
|------------------- |------- |----------------:|--------------:|--------------:|-------------:|----------------:|----------------:|----------------:|----------------:|---------------:|-------------:|------:|--------:|-----:|--------:|--------:|--------:|------------:|
| **&#39;List&lt;T&gt; AddRange&#39;** |    **100** |        **89.80 ns** |      **2.608 ns** |      **7.441 ns** |     **0.767 ns** |        **90.38 ns** |        **76.88 ns** |        **83.00 ns** |        **95.35 ns** |       **111.6 ns** | **11,135,729.0** |  **0.20** |    **0.01** |    **1** |  **0.1364** |  **0.0004** |       **-** |       **856 B** |
|      &#39;List&lt;T&gt; Add&#39; |    100 |       495.52 ns |      4.973 ns |      4.652 ns |     1.201 ns |       494.55 ns |       487.48 ns |       492.89 ns |       498.69 ns |       504.7 ns |  2,018,085.9 |  1.00 |    0.00 |    2 |  0.3490 |  0.0010 |       - |     2,192 B |
|                    |        |                 |               |               |              |                 |                 |                 |                 |                |              |       |         |      |         |         |         |             |
| **&#39;List&lt;T&gt; AddRange&#39;** |   **1000** |       **528.82 ns** |     **10.099 ns** |     **13.131 ns** |     **2.680 ns** |       **525.25 ns** |       **512.66 ns** |       **519.35 ns** |       **535.30 ns** |       **565.2 ns** |  **1,890,986.3** |  **0.14** |    **0.00** |    **1** |  **1.2836** |  **0.0372** |       **-** |     **8,056 B** |
|      &#39;List&lt;T&gt; Add&#39; |   1000 |     3,932.36 ns |     58.353 ns |     48.727 ns |    13.515 ns |     3,913.81 ns |     3,890.99 ns |     3,907.65 ns |     3,934.55 ns |     4,063.1 ns |    254,300.1 |  1.00 |    0.00 |    2 |  2.6398 |  0.0763 |       - |    16,600 B |
|                    |        |                 |               |               |              |                 |                 |                 |                 |                |              |       |         |      |         |         |         |             |
| **&#39;List&lt;T&gt; AddRange&#39;** |  **10000** |     **6,605.80 ns** |    **363.975 ns** |  **1,073.190 ns** |   **107.319 ns** |     **6,915.60 ns** |     **5,109.83 ns** |     **5,319.75 ns** |     **7,323.74 ns** |     **8,983.5 ns** |    **151,382.1** |  **0.09** |    **0.01** |    **1** | **12.6572** |  **2.1057** |       **-** |    **80,056 B** |
|      &#39;List&lt;T&gt; Add&#39; |  10000 |    84,983.39 ns |    998.916 ns |    885.513 ns |   236.663 ns |    84,771.99 ns |    83,735.46 ns |    84,268.60 ns |    85,417.66 ns |    86,469.5 ns |     11,767.0 |  1.00 |    0.00 |    2 | 41.6260 | 41.6260 | 41.6260 |   262,456 B |
|                    |        |                 |               |               |              |                 |                 |                 |                 |                |              |       |         |      |         |         |         |             |
| **&#39;List&lt;T&gt; AddRange&#39;** | **100000** |   **358,020.30 ns** |  **9,974.876 ns** | **28,938.923 ns** | **2,938.302 ns** |   **358,724.02 ns** |   **286,790.72 ns** |   **345,003.12 ns** |   **374,715.14 ns** |   **419,262.0 ns** |      **2,793.1** |  **0.28** |    **0.03** |    **1** | **12.2070** | **12.2070** | **12.2070** |   **800,093 B** |
|      &#39;List&lt;T&gt; Add&#39; | 100000 | 1,238,757.02 ns | 22,879.417 ns | 45,161.738 ns | 6,518.535 ns | 1,243,350.29 ns | 1,162,668.16 ns | 1,204,489.31 ns | 1,264,289.94 ns | 1,337,551.0 ns |        807.3 |  1.00 |    0.00 |    2 | 44.9219 | 25.3906 | 25.3906 | 2,097,591 B |

----------
**List Interface Iteration**


|                           Method |  Count |          Mean |         Error |        StdDev |       StdErr |        Median |           Min |            Q1 |            Q3 |           Max |         Op/s | Ratio | RatioSD | Rank |    Gen 0 |   Allocated |
|--------------------------------- |------- |--------------:|--------------:|--------------:|-------------:|--------------:|--------------:|--------------:|--------------:|--------------:|-------------:|------:|--------:|-----:|---------:|------------:|
|            **&#39;ICollection ForEach&#39;** |    **100** |   **1,145.69 ns** |     **22.717 ns** |     **56.573 ns** |     **6.621 ns** |   **1,124.04 ns** |   **1,068.74 ns** |   **1,102.65 ns** |   **1,186.64 ns** |   **1,301.34 ns** |    **872,836.7** | **21.90** |    **1.34** |   **10** |   **0.3872** |     **2,440 B** |
|         &#39;ICollection&lt;T&gt; ForEach&#39; |    100 |     605.98 ns |     11.331 ns |     22.629 ns |     3.233 ns |     597.53 ns |     584.72 ns |     590.53 ns |     611.28 ns |     676.50 ns |  1,650,225.9 | 11.69 |    0.51 |    6 |   0.0057 |        40 B |
|            &#39;IEnumerable ForEach&#39; |    100 |     794.31 ns |      8.220 ns |      6.864 ns |     1.904 ns |     794.45 ns |     782.95 ns |     789.46 ns |     798.49 ns |     806.24 ns |  1,258,948.1 | 14.77 |    0.18 |    9 |   0.3881 |     2,440 B |
|         &#39;IEnumerable&lt;T&gt; ForEach&#39; |    100 |     690.26 ns |     29.570 ns |     87.188 ns |     8.719 ns |     696.16 ns |     587.19 ns |     596.37 ns |     749.72 ns |     897.35 ns |  1,448,728.2 | 14.09 |    0.60 |    7 |   0.0057 |        40 B |
|                  &#39;IList ForEach&#39; |    100 |     808.65 ns |     16.149 ns |     17.279 ns |     4.073 ns |     805.94 ns |     784.02 ns |     792.92 ns |     824.63 ns |     839.12 ns |  1,236,625.3 | 15.04 |    0.35 |    9 |   0.3881 |     2,440 B |
|               &#39;IList&lt;T&gt; ForEach&#39; |    100 |     755.91 ns |     14.787 ns |     20.729 ns |     3.989 ns |     749.01 ns |     733.65 ns |     737.24 ns |     767.95 ns |     803.23 ns |  1,322,917.5 | 14.04 |    0.38 |    8 |   0.0057 |        40 B |
|                   &#39;IList&lt;T&gt; For&#39; |    100 |     209.74 ns |      4.172 ns |     10.543 ns |     1.217 ns |     204.93 ns |     202.32 ns |     203.73 ns |     210.81 ns |     247.34 ns |  4,767,762.6 |  4.18 |    0.26 |    2 |        - |           - |
|                      &#39;IList For&#39; |    100 |     431.31 ns |      5.453 ns |      5.101 ns |     1.317 ns |     430.96 ns |     424.32 ns |     427.43 ns |     434.28 ns |     441.35 ns |  2,318,538.9 |  8.02 |    0.12 |    5 |   0.3824 |     2,400 B |
| &#39;IReadOnlyCollection&lt;T&gt; ForEach&#39; |    100 |     598.64 ns |      2.314 ns |      2.164 ns |     0.559 ns |     598.57 ns |     594.20 ns |     597.83 ns |     600.05 ns |     602.84 ns |  1,670,443.5 | 11.13 |    0.07 |    6 |   0.0057 |        40 B |
|       &#39;IReadOnlyList&lt;T&gt; ForEach&#39; |    100 |     832.41 ns |     42.320 ns |    122.778 ns |    12.466 ns |     803.45 ns |     598.43 ns |     749.90 ns |     901.60 ns |   1,151.27 ns |  1,201,335.0 | 15.02 |    0.82 |    9 |   0.0057 |        40 B |
|           &#39;IReadOnlyList&lt;T&gt; For&#39; |    100 |     260.01 ns |      5.221 ns |      9.281 ns |     1.467 ns |     256.78 ns |     249.05 ns |     252.98 ns |     262.35 ns |     283.51 ns |  3,846,051.6 |  4.87 |    0.19 |    4 |        - |           - |
|                &#39;List&lt;T&gt; ForEach&#39; |    100 |     219.04 ns |     15.499 ns |     44.219 ns |     4.561 ns |     187.59 ns |     183.15 ns |     185.32 ns |     245.46 ns |     363.92 ns |  4,565,425.3 |  5.09 |    0.75 |    3 |        - |           - |
|                    &#39;List&lt;T&gt; For&#39; |    100 |      53.78 ns |      0.266 ns |      0.249 ns |     0.064 ns |      53.75 ns |      53.36 ns |      53.65 ns |      53.98 ns |      54.34 ns | 18,593,067.5 |  1.00 |    0.00 |    1 |        - |           - |
|                                  |        |               |               |               |              |               |               |               |               |               |              |       |         |      |          |             |
|            **&#39;ICollection ForEach&#39;** |   **1000** |   **7,833.56 ns** |     **87.299 ns** |     **72.899 ns** |    **20.218 ns** |   **7,802.37 ns** |   **7,729.98 ns** |   **7,799.26 ns** |   **7,881.54 ns** |   **7,976.37 ns** |    **127,655.9** | **14.22** |    **2.06** |    **7** |   **3.8300** |    **24,040 B** |
|         &#39;ICollection&lt;T&gt; ForEach&#39; |   1000 |   6,632.26 ns |    289.027 ns |    852.204 ns |    85.220 ns |   6,445.30 ns |   5,701.11 ns |   5,777.23 ns |   7,211.66 ns |   8,847.49 ns |    150,778.1 | 11.89 |    1.21 |    6 |        - |        40 B |
|            &#39;IEnumerable ForEach&#39; |   1000 |   7,811.83 ns |    145.978 ns |    136.548 ns |    35.256 ns |   7,767.46 ns |   7,670.36 ns |   7,710.62 ns |   7,878.72 ns |   8,119.21 ns |    128,011.0 | 13.90 |    2.12 |    7 |   3.8300 |    24,040 B |
|         &#39;IEnumerable&lt;T&gt; ForEach&#39; |   1000 |   6,507.00 ns |    278.760 ns |    817.554 ns |    82.167 ns |   6,439.04 ns |   5,655.19 ns |   5,719.33 ns |   7,138.72 ns |   8,990.66 ns |    153,680.6 | 11.65 |    1.16 |    6 |        - |        40 B |
|                  &#39;IList ForEach&#39; |   1000 |   8,410.15 ns |     86.724 ns |     81.122 ns |    20.945 ns |   8,371.06 ns |   8,326.08 ns |   8,348.06 ns |   8,466.59 ns |   8,608.67 ns |    118,903.9 | 14.94 |    2.16 |    8 |   3.8300 |    24,040 B |
|               &#39;IList&lt;T&gt; ForEach&#39; |   1000 |   5,855.04 ns |     78.744 ns |     69.804 ns |    18.656 ns |   5,849.05 ns |   5,750.56 ns |   5,804.09 ns |   5,895.12 ns |   6,015.43 ns |    170,793.1 | 10.52 |    1.44 |    6 |        - |        40 B |
|                   &#39;IList&lt;T&gt; For&#39; |   1000 |   2,407.91 ns |    107.889 ns |    318.112 ns |    31.811 ns |   2,481.92 ns |   1,971.09 ns |   2,004.90 ns |   2,615.58 ns |   3,178.07 ns |    415,298.7 |  4.33 |    0.59 |    4 |        - |           - |
|                      &#39;IList For&#39; |   1000 |   4,231.63 ns |     80.244 ns |     89.191 ns |    20.462 ns |   4,212.46 ns |   4,106.69 ns |   4,196.45 ns |   4,252.37 ns |   4,490.17 ns |    236,315.6 |  7.34 |    1.05 |    5 |   3.8223 |    24,000 B |
| &#39;IReadOnlyCollection&lt;T&gt; ForEach&#39; |   1000 |   5,794.18 ns |     43.754 ns |     38.787 ns |    10.366 ns |   5,791.37 ns |   5,743.20 ns |   5,771.86 ns |   5,809.46 ns |   5,876.71 ns |    172,586.8 | 10.41 |    1.44 |    6 |        - |        40 B |
|       &#39;IReadOnlyList&lt;T&gt; ForEach&#39; |   1000 |   5,765.61 ns |     56.653 ns |    121.953 ns |    16.297 ns |   5,733.29 ns |   5,667.76 ns |   5,715.84 ns |   5,766.46 ns |   6,488.56 ns |    173,442.3 |  9.64 |    1.24 |    6 |        - |        40 B |
|           &#39;IReadOnlyList&lt;T&gt; For&#39; |   1000 |   1,994.98 ns |     23.308 ns |     21.803 ns |     5.629 ns |   1,987.75 ns |   1,969.15 ns |   1,983.25 ns |   2,007.45 ns |   2,042.94 ns |    501,258.4 |  3.55 |    0.53 |    3 |        - |           - |
|                &#39;List&lt;T&gt; ForEach&#39; |   1000 |   1,791.56 ns |     35.848 ns |     78.688 ns |    10.332 ns |   1,763.42 ns |   1,744.43 ns |   1,753.90 ns |   1,783.33 ns |   2,151.35 ns |    558,171.9 |  3.02 |    0.45 |    2 |        - |           - |
|                    &#39;List&lt;T&gt; For&#39; |   1000 |     561.32 ns |     24.888 ns |     72.994 ns |     7.336 ns |     512.61 ns |     496.04 ns |     500.40 ns |     626.12 ns |     757.39 ns |  1,781,501.7 |  1.00 |    0.00 |    1 |        - |           - |
|                                  |        |               |               |               |              |               |               |               |               |               |              |       |         |      |          |             |
|            **&#39;ICollection ForEach&#39;** |  **10000** |  **83,860.05 ns** |  **1,158.404 ns** |  **1,026.895 ns** |   **274.449 ns** |  **83,638.05 ns** |  **82,078.16 ns** |  **83,253.53 ns** |  **84,288.31 ns** |  **85,563.26 ns** |     **11,924.6** | **17.70** |    **0.26** |    **7** |  **38.2080** |   **240,040 B** |
|         &#39;ICollection&lt;T&gt; ForEach&#39; |  10000 |  59,337.85 ns |    647.961 ns |    574.401 ns |   153.515 ns |  59,172.17 ns |  58,679.90 ns |  59,004.06 ns |  59,505.23 ns |  60,728.77 ns |     16,852.7 | 12.53 |    0.15 |    6 |        - |        40 B |
|            &#39;IEnumerable ForEach&#39; |  10000 |  84,926.02 ns |  1,524.826 ns |  3,936.062 ns |   445.671 ns |  83,405.40 ns |  82,276.50 ns |  82,767.32 ns |  85,075.52 ns | 100,093.64 ns |     11,775.0 | 19.40 |    1.16 |    7 |  38.2080 |   240,040 B |
|         &#39;IEnumerable&lt;T&gt; ForEach&#39; |  10000 |  60,159.46 ns |  1,078.701 ns |  2,765.123 ns |   315.115 ns |  59,145.42 ns |  58,273.65 ns |  58,795.34 ns |  59,761.83 ns |  73,023.44 ns |     16,622.5 | 13.29 |    0.98 |    6 |        - |        40 B |
|                  &#39;IList ForEach&#39; |  10000 |  93,087.21 ns |  5,316.914 ns | 15,593.589 ns | 1,567.215 ns |  84,863.60 ns |  81,912.61 ns |  82,530.70 ns | 104,601.41 ns | 142,472.83 ns |     10,742.6 | 23.27 |    4.52 |    7 |  38.2080 |   240,040 B |
|               &#39;IList&lt;T&gt; ForEach&#39; |  10000 |  59,865.00 ns |    763.079 ns |  1,674.977 ns |   219.935 ns |  59,279.46 ns |  58,880.63 ns |  59,140.74 ns |  59,764.12 ns |  68,897.74 ns |     16,704.2 | 12.84 |    0.64 |    6 |        - |        40 B |
|                   &#39;IList&lt;T&gt; For&#39; |  10000 |  19,909.12 ns |    213.819 ns |    189.545 ns |    50.658 ns |  19,853.24 ns |  19,677.67 ns |  19,761.04 ns |  20,014.10 ns |  20,362.37 ns |     50,228.2 |  4.20 |    0.05 |    3 |        - |           - |
|                      &#39;IList For&#39; |  10000 |  46,890.52 ns |  2,476.182 ns |  7,262.212 ns |   729.880 ns |  42,529.90 ns |  40,791.22 ns |  41,377.99 ns |  53,116.28 ns |  68,820.34 ns |     21,326.3 | 10.27 |    1.77 |    4 |  38.2080 |   240,000 B |
| &#39;IReadOnlyCollection&lt;T&gt; ForEach&#39; |  10000 |  58,657.82 ns |  1,208.823 ns |  3,226.593 ns |   354.165 ns |  57,426.51 ns |  56,668.70 ns |  57,000.74 ns |  58,102.31 ns |  70,691.02 ns |     17,048.0 | 12.98 |    1.22 |    5 |        - |        40 B |
|       &#39;IReadOnlyList&lt;T&gt; ForEach&#39; |  10000 |  62,792.76 ns |  1,986.397 ns |  5,794.413 ns |   585.324 ns |  59,578.55 ns |  58,528.87 ns |  59,138.24 ns |  66,205.39 ns |  79,563.31 ns |     15,925.4 | 14.41 |    1.40 |    6 |        - |        40 B |
|           &#39;IReadOnlyList&lt;T&gt; For&#39; |  10000 |  19,915.07 ns |    132.689 ns |    110.801 ns |    30.731 ns |  19,944.76 ns |  19,713.57 ns |  19,820.20 ns |  20,006.87 ns |  20,068.50 ns |     50,213.2 |  4.20 |    0.05 |    3 |        - |           - |
|                &#39;List&lt;T&gt; ForEach&#39; |  10000 |  18,300.88 ns |    683.059 ns |  1,904.096 ns |   200.709 ns |  17,592.67 ns |  16,492.50 ns |  17,079.66 ns |  18,600.59 ns |  23,220.13 ns |     54,642.2 |  4.75 |    0.15 |    2 |        - |           - |
|                    &#39;List&lt;T&gt; For&#39; |  10000 |   4,739.57 ns |     44.365 ns |     37.047 ns |    10.275 ns |   4,731.23 ns |   4,686.92 ns |   4,712.64 ns |   4,761.27 ns |   4,803.41 ns |    210,989.7 |  1.00 |    0.00 |    1 |        - |           - |
|                                  |        |               |               |               |              |               |               |               |               |               |              |       |         |      |          |             |
|            **&#39;ICollection ForEach&#39;** | **100000** | **818,055.40 ns** | **16,033.539 ns** | **41,387.683 ns** | **4,686.232 ns** | **805,305.96 ns** | **779,512.70 ns** | **789,137.43 ns** | **828,297.80 ns** | **953,353.61 ns** |      **1,222.4** | **17.66** |    **1.41** |    **7** | **381.8359** | **2,400,040 B** |
|         &#39;ICollection&lt;T&gt; ForEach&#39; | 100000 | 569,801.86 ns | 11,127.606 ns | 10,928.797 ns | 2,732.199 ns | 567,895.80 ns | 550,775.10 ns | 563,315.01 ns | 576,502.81 ns | 589,706.15 ns |      1,755.0 | 11.93 |    0.29 |    6 |        - |        40 B |
|            &#39;IEnumerable ForEach&#39; | 100000 | 801,465.23 ns | 15,401.423 ns | 16,479.350 ns | 3,884.220 ns | 796,562.40 ns | 781,208.89 ns | 788,145.87 ns | 814,550.15 ns | 828,928.91 ns |      1,247.7 | 16.82 |    0.44 |    7 | 381.8359 | 2,400,040 B |
|         &#39;IEnumerable&lt;T&gt; ForEach&#39; | 100000 | 576,296.50 ns |  9,589.157 ns |  8,969.704 ns | 2,315.968 ns | 576,658.50 ns | 563,332.91 ns | 568,478.71 ns | 582,489.01 ns | 592,870.61 ns |      1,735.2 | 12.05 |    0.25 |    6 |        - |        41 B |
|                  &#39;IList ForEach&#39; | 100000 | 805,049.84 ns |  8,164.166 ns | 18,921.696 ns | 2,365.212 ns | 802,055.13 ns | 772,227.83 ns | 791,431.76 ns | 816,016.53 ns | 858,640.04 ns |      1,242.2 | 16.94 |    0.76 |    7 | 381.8359 | 2,400,040 B |
|               &#39;IList&lt;T&gt; ForEach&#39; | 100000 | 566,158.48 ns | 10,540.276 ns | 10,824.088 ns | 2,625.227 ns | 569,338.28 ns | 541,913.67 ns | 562,697.07 ns | 572,939.65 ns | 582,836.82 ns |      1,766.3 | 11.82 |    0.28 |    6 |        - |        40 B |
|                   &#39;IList&lt;T&gt; For&#39; | 100000 | 194,341.80 ns |  2,433.237 ns |  2,157.001 ns |   576.483 ns | 194,211.68 ns | 190,740.19 ns | 193,320.78 ns | 195,936.46 ns | 197,804.64 ns |      5,145.6 |  4.06 |    0.07 |    3 |        - |           - |
|                      &#39;IList For&#39; | 100000 | 407,787.71 ns |  8,103.629 ns |  8,670.792 ns | 2,043.725 ns | 407,743.55 ns | 394,557.47 ns | 400,512.59 ns | 412,572.47 ns | 428,294.78 ns |      2,452.3 |  8.49 |    0.22 |    4 | 382.3242 | 2,400,000 B |
| &#39;IReadOnlyCollection&lt;T&gt; ForEach&#39; | 100000 | 554,437.40 ns |  5,598.486 ns |  4,962.912 ns | 1,326.394 ns | 553,444.24 ns | 545,965.97 ns | 551,923.32 ns | 557,208.62 ns | 561,826.03 ns |      1,803.6 | 11.59 |    0.19 |    5 |        - |        40 B |
|       &#39;IReadOnlyList&lt;T&gt; ForEach&#39; | 100000 | 607,136.93 ns | 19,509.362 ns | 56,288.970 ns | 5,744.969 ns | 578,306.30 ns | 543,218.36 ns | 568,994.85 ns | 642,707.10 ns | 765,641.99 ns |      1,647.1 | 14.33 |    0.89 |    6 |        - |        40 B |
|           &#39;IReadOnlyList&lt;T&gt; For&#39; | 100000 | 191,316.58 ns |  2,383.960 ns |  2,113.318 ns |   564.808 ns | 190,344.35 ns | 188,609.59 ns | 189,956.84 ns | 193,304.41 ns | 195,119.41 ns |      5,226.9 |  4.00 |    0.06 |    3 |        - |           - |
|                &#39;List&lt;T&gt; ForEach&#39; | 100000 | 170,360.08 ns |  2,899.457 ns |  2,712.154 ns |   700.275 ns | 170,020.56 ns | 165,619.02 ns | 169,128.09 ns | 172,343.93 ns | 175,410.42 ns |      5,869.9 |  3.56 |    0.06 |    2 |        - |           - |
|                    &#39;List&lt;T&gt; For&#39; | 100000 |  47,863.59 ns |    631.754 ns |    560.033 ns |   149.675 ns |  47,961.43 ns |  46,963.37 ns |  47,450.96 ns |  48,175.22 ns |  48,719.93 ns |     20,892.7 |  1.00 |    0.00 |    1 |        - |           - |

----------
**List Vs Dictionary**


|                          Method |  Count |        Mean |      Error |     StdDev |    StdErr |      Median |         Min |          Q1 |          Q3 |         Max |         Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|-------------------------------- |------- |------------:|-----------:|-----------:|----------:|------------:|------------:|------------:|------------:|------------:|-------------:|------:|--------:|-----:|-------:|----------:|
|            **&#39;ArrayList for loop&#39;** |    **100** |   **570.06 ns** |   **9.989 ns** |   **8.855 ns** |  **2.367 ns** |   **568.93 ns** |   **559.66 ns** |   **564.00 ns** |   **574.14 ns** |   **585.50 ns** |  **1,754,195.4** | **0.487** |    **0.01** |    **4** |      **-** |         **-** |
|                Collection.First |    100 | 1,267.70 ns |  17.267 ns |  16.151 ns |  4.170 ns | 1,267.33 ns | 1,241.93 ns | 1,256.13 ns | 1,275.59 ns | 1,302.17 ns |    788,833.1 | 1.082 |    0.02 |    9 | 0.0057 |      40 B |
|       Collection.FirstOrDefault |    100 | 1,179.86 ns |  13.373 ns |  12.509 ns |  3.230 ns | 1,179.18 ns | 1,156.06 ns | 1,172.70 ns | 1,190.62 ns | 1,201.16 ns |    847,557.5 | 1.007 |    0.02 |    7 | 0.0057 |      40 B |
|           &#39;Collection for loop&#39; |    100 |   661.16 ns |   8.257 ns |   7.320 ns |  1.956 ns |   659.61 ns |   651.53 ns |   655.32 ns |   665.83 ns |   674.70 ns |  1,512,495.9 | 0.564 |    0.01 |    5 |      - |         - |
|                  &#39;Dictionary[]&#39; |    100 |    11.51 ns |   0.257 ns |   0.241 ns |  0.062 ns |    11.48 ns |    11.18 ns |    11.33 ns |    11.64 ns |    12.07 ns | 86,848,246.4 | 0.010 |    0.00 |    1 |      - |         - |
|          Dictionary.TryGetValue |    100 |    11.35 ns |   0.146 ns |   0.122 ns |  0.034 ns |    11.39 ns |    11.12 ns |    11.32 ns |    11.42 ns |    11.53 ns | 88,080,890.9 | 0.010 |    0.00 |    1 |      - |         - |
|                   HashSet.First |    100 | 1,192.80 ns |  32.768 ns |  96.103 ns |  9.659 ns | 1,148.69 ns | 1,092.63 ns | 1,117.07 ns | 1,261.18 ns | 1,487.35 ns |    838,362.3 | 1.110 |    0.07 |    7 | 0.0057 |      40 B |
|          HashSet.FirstOrDefault |    100 | 1,211.75 ns |   9.871 ns |   8.243 ns |  2.286 ns | 1,210.24 ns | 1,196.46 ns | 1,206.16 ns | 1,217.42 ns | 1,225.00 ns |    825,252.7 | 1.035 |    0.02 |    8 | 0.0057 |      40 B |
|         &#39;ImmutableDictionary[]&#39; |    100 |    28.16 ns |   0.484 ns |   0.453 ns |  0.117 ns |    28.34 ns |    27.39 ns |    27.83 ns |    28.44 ns |    28.85 ns | 35,506,337.8 | 0.024 |    0.00 |    2 |      - |         - |
| ImmutableDictionary.TryGetValue |    100 |    28.40 ns |   0.499 ns |   0.466 ns |  0.120 ns |    28.35 ns |    27.67 ns |    28.09 ns |    28.91 ns |    28.99 ns | 35,213,227.1 | 0.024 |    0.00 |    2 |      - |         - |
|             ImmutableList.First |    100 | 5,245.82 ns |  63.485 ns |  59.384 ns | 15.333 ns | 5,249.25 ns | 5,143.23 ns | 5,202.59 ns | 5,293.48 ns | 5,341.79 ns |    190,628.1 | 4.485 |    0.08 |   10 | 0.0076 |      72 B |
|    ImmutableList.FirstOrDefault |    100 | 5,302.15 ns |  54.411 ns |  50.896 ns | 13.141 ns | 5,303.57 ns | 5,228.02 ns | 5,266.56 ns | 5,343.82 ns | 5,385.89 ns |    188,602.8 | 4.529 |    0.08 |   10 | 0.0076 |      72 B |
|        &#39;ImmutableList for loop&#39; |    100 | 1,014.22 ns |  20.198 ns |  19.837 ns |  4.959 ns | 1,015.68 ns |   989.99 ns |   994.28 ns | 1,024.80 ns | 1,050.78 ns |    985,981.7 | 0.867 |    0.01 |    6 |      - |         - |
|                      List.First |    100 | 1,171.46 ns |  16.630 ns |  14.742 ns |  3.940 ns | 1,165.09 ns | 1,153.00 ns | 1,160.96 ns | 1,180.99 ns | 1,204.60 ns |    853,638.5 | 1.000 |    0.00 |    7 | 0.0057 |      40 B |
|             List.FirstOrDefault |    100 | 1,181.19 ns |   9.409 ns |   8.341 ns |  2.229 ns | 1,183.25 ns | 1,169.21 ns | 1,173.08 ns | 1,186.71 ns | 1,194.67 ns |    846,604.5 | 1.008 |    0.01 |    7 | 0.0057 |      40 B |
|                 &#39;List for loop&#39; |    100 |   337.40 ns |   3.874 ns |   3.434 ns |  0.918 ns |   337.11 ns |   331.98 ns |   334.92 ns |   339.02 ns |   345.07 ns |  2,963,860.6 | 0.288 |    0.00 |    3 |      - |         - |
|                                 |        |             |            |            |           |             |             |             |             |             |              |       |         |      |        |           |
|            **&#39;ArrayList for loop&#39;** |   **1000** |   **565.30 ns** |   **3.453 ns** |   **2.883 ns** |  **0.800 ns** |   **565.28 ns** |   **562.26 ns** |   **563.04 ns** |   **566.20 ns** |   **572.40 ns** |  **1,768,984.6** | **0.447** |    **0.01** |    **4** |      **-** |         **-** |
|                Collection.First |   1000 | 1,189.83 ns |  23.315 ns |  21.809 ns |  5.631 ns | 1,184.58 ns | 1,156.08 ns | 1,174.89 ns | 1,203.65 ns | 1,229.65 ns |    840,456.8 | 0.940 |    0.02 |    6 | 0.0057 |      40 B |
|       Collection.FirstOrDefault |   1000 | 1,164.32 ns |  12.330 ns |  11.533 ns |  2.978 ns | 1,163.20 ns | 1,147.23 ns | 1,155.02 ns | 1,169.44 ns | 1,187.93 ns |    858,869.9 | 0.921 |    0.01 |    6 | 0.0057 |      40 B |
|           &#39;Collection for loop&#39; |   1000 |   612.10 ns |   4.186 ns |   3.495 ns |  0.969 ns |   612.08 ns |   605.58 ns |   609.42 ns |   613.42 ns |   618.39 ns |  1,633,709.8 | 0.484 |    0.01 |    5 |      - |         - |
|                  &#39;Dictionary[]&#39; |   1000 |    11.39 ns |   0.227 ns |   0.201 ns |  0.054 ns |    11.31 ns |    11.22 ns |    11.26 ns |    11.47 ns |    11.83 ns | 87,773,555.9 | 0.009 |    0.00 |    1 |      - |         - |
|          Dictionary.TryGetValue |   1000 |    11.28 ns |   0.244 ns |   0.308 ns |  0.064 ns |    11.17 ns |    10.92 ns |    11.04 ns |    11.48 ns |    12.00 ns | 88,647,418.6 | 0.009 |    0.00 |    1 |      - |         - |
|                   HashSet.First |   1000 | 1,184.26 ns |  17.119 ns |  15.175 ns |  4.056 ns | 1,187.00 ns | 1,150.96 ns | 1,179.42 ns | 1,191.41 ns | 1,205.40 ns |    844,406.0 | 0.936 |    0.01 |    6 | 0.0057 |      40 B |
|          HashSet.FirstOrDefault |   1000 | 1,334.12 ns |  29.649 ns |  82.158 ns |  8.709 ns | 1,300.29 ns | 1,267.71 ns | 1,282.79 ns | 1,335.78 ns | 1,582.84 ns |    749,556.7 | 1.186 |    0.05 |    8 | 0.0057 |      40 B |
|         &#39;ImmutableDictionary[]&#39; |   1000 |    28.97 ns |   0.479 ns |   0.448 ns |  0.116 ns |    28.89 ns |    28.38 ns |    28.63 ns |    29.17 ns |    29.92 ns | 34,515,419.3 | 0.023 |    0.00 |    2 |      - |         - |
| ImmutableDictionary.TryGetValue |   1000 |    29.04 ns |   0.442 ns |   0.414 ns |  0.107 ns |    29.09 ns |    28.50 ns |    28.65 ns |    29.30 ns |    29.78 ns | 34,439,559.4 | 0.023 |    0.00 |    2 |      - |         - |
|             ImmutableList.First |   1000 | 5,551.94 ns |  72.502 ns |  64.271 ns | 17.177 ns | 5,538.41 ns | 5,468.98 ns | 5,501.48 ns | 5,579.36 ns | 5,694.67 ns |    180,117.2 | 4.388 |    0.06 |   10 | 0.0076 |      72 B |
|    ImmutableList.FirstOrDefault |   1000 | 5,315.28 ns |  56.739 ns |  53.073 ns | 13.703 ns | 5,301.77 ns | 5,247.16 ns | 5,266.21 ns | 5,347.05 ns | 5,414.91 ns |    188,137.0 | 4.205 |    0.05 |    9 | 0.0076 |      72 B |
|        &#39;ImmutableList for loop&#39; |   1000 | 1,331.95 ns |  17.320 ns |  16.201 ns |  4.183 ns | 1,330.03 ns | 1,304.22 ns | 1,319.23 ns | 1,344.72 ns | 1,356.83 ns |    750,778.8 | 1.052 |    0.02 |    8 |      - |         - |
|                      List.First |   1000 | 1,265.22 ns |  12.978 ns |  11.505 ns |  3.075 ns | 1,262.34 ns | 1,251.82 ns | 1,257.65 ns | 1,269.68 ns | 1,286.25 ns |    790,374.0 | 1.000 |    0.00 |    7 | 0.0057 |      40 B |
|             List.FirstOrDefault |   1000 | 1,176.25 ns |  10.274 ns |   9.108 ns |  2.434 ns | 1,174.64 ns | 1,163.31 ns | 1,169.67 ns | 1,180.58 ns | 1,193.81 ns |    850,162.6 | 0.930 |    0.01 |    6 | 0.0057 |      40 B |
|                 &#39;List for loop&#39; |   1000 |   343.38 ns |   5.552 ns |   4.921 ns |  1.315 ns |   342.03 ns |   337.59 ns |   339.80 ns |   347.39 ns |   353.06 ns |  2,912,203.6 | 0.271 |    0.00 |    3 |      - |         - |
|                                 |        |             |            |            |           |             |             |             |             |             |              |       |         |      |        |           |
|            **&#39;ArrayList for loop&#39;** |  **10000** |   **574.03 ns** |   **8.104 ns** |   **7.184 ns** |  **1.920 ns** |   **576.97 ns** |   **561.15 ns** |   **570.56 ns** |   **577.43 ns** |   **584.30 ns** |  **1,742,057.6** | **0.448** |    **0.01** |    **4** |      **-** |         **-** |
|                Collection.First |  10000 | 1,163.74 ns |  12.741 ns |  11.918 ns |  3.077 ns | 1,164.90 ns | 1,137.96 ns | 1,154.87 ns | 1,171.29 ns | 1,187.92 ns |    859,301.2 | 0.907 |    0.02 |    7 | 0.0057 |      40 B |
|       Collection.FirstOrDefault |  10000 | 1,212.12 ns |  14.385 ns |  12.752 ns |  3.408 ns | 1,211.59 ns | 1,183.59 ns | 1,208.01 ns | 1,220.86 ns | 1,231.88 ns |    825,000.7 | 0.945 |    0.02 |    8 | 0.0057 |      40 B |
|           &#39;Collection for loop&#39; |  10000 |   666.14 ns |  12.380 ns |  11.580 ns |  2.990 ns |   664.70 ns |   646.08 ns |   658.02 ns |   673.05 ns |   689.10 ns |  1,501,189.5 | 0.520 |    0.02 |    5 |      - |         - |
|                  &#39;Dictionary[]&#39; |  10000 |    11.55 ns |   0.247 ns |   0.284 ns |  0.064 ns |    11.46 ns |    11.07 ns |    11.42 ns |    11.60 ns |    12.24 ns | 86,572,904.2 | 0.009 |    0.00 |    1 |      - |         - |
|          Dictionary.TryGetValue |  10000 |    11.53 ns |   0.089 ns |   0.078 ns |  0.021 ns |    11.54 ns |    11.44 ns |    11.46 ns |    11.56 ns |    11.70 ns | 86,715,147.8 | 0.009 |    0.00 |    1 |      - |         - |
|                   HashSet.First |  10000 | 1,131.74 ns |  14.129 ns |  13.216 ns |  3.412 ns | 1,128.32 ns | 1,114.75 ns | 1,123.43 ns | 1,141.55 ns | 1,162.50 ns |    883,594.0 | 0.881 |    0.02 |    6 | 0.0057 |      40 B |
|          HashSet.FirstOrDefault |  10000 | 1,221.00 ns |   8.073 ns |   7.552 ns |  1.950 ns | 1,219.25 ns | 1,209.44 ns | 1,215.44 ns | 1,225.81 ns | 1,234.15 ns |    818,998.3 | 0.952 |    0.02 |    8 | 0.0057 |      40 B |
|         &#39;ImmutableDictionary[]&#39; |  10000 |    27.54 ns |   0.283 ns |   0.264 ns |  0.068 ns |    27.49 ns |    27.05 ns |    27.40 ns |    27.69 ns |    27.94 ns | 36,306,611.6 | 0.021 |    0.00 |    2 |      - |         - |
| ImmutableDictionary.TryGetValue |  10000 |    27.96 ns |   0.244 ns |   0.216 ns |  0.058 ns |    27.99 ns |    27.54 ns |    27.85 ns |    28.08 ns |    28.38 ns | 35,766,839.7 | 0.022 |    0.00 |    2 |      - |         - |
|             ImmutableList.First |  10000 | 5,660.38 ns | 111.631 ns | 104.420 ns | 26.961 ns | 5,647.33 ns | 5,530.70 ns | 5,584.53 ns | 5,685.67 ns | 5,878.35 ns |    176,666.7 | 4.414 |    0.12 |   11 | 0.0076 |      72 B |
|    ImmutableList.FirstOrDefault |  10000 | 5,537.41 ns | 105.387 ns |  98.579 ns | 25.453 ns | 5,517.45 ns | 5,336.96 ns | 5,483.52 ns | 5,600.82 ns | 5,744.68 ns |    180,589.8 | 4.321 |    0.13 |   11 | 0.0076 |      72 B |
|        &#39;ImmutableList for loop&#39; |  10000 | 2,104.24 ns |  41.351 ns |  38.679 ns |  9.987 ns | 2,091.64 ns | 2,056.73 ns | 2,081.22 ns | 2,129.92 ns | 2,196.82 ns |    475,230.6 | 1.641 |    0.05 |   10 |      - |         - |
|                      List.First |  10000 | 1,282.93 ns |  24.187 ns |  21.441 ns |  5.730 ns | 1,277.39 ns | 1,251.92 ns | 1,269.73 ns | 1,296.50 ns | 1,326.05 ns |    779,464.6 | 1.000 |    0.00 |    9 | 0.0057 |      40 B |
|             List.FirstOrDefault |  10000 | 1,198.64 ns |  17.623 ns |  15.622 ns |  4.175 ns | 1,197.81 ns | 1,179.69 ns | 1,185.57 ns | 1,214.29 ns | 1,226.08 ns |    834,280.1 | 0.935 |    0.02 |    8 | 0.0057 |      40 B |
|                 &#39;List for loop&#39; |  10000 |   335.76 ns |   5.390 ns |   5.042 ns |  1.302 ns |   334.83 ns |   329.17 ns |   331.73 ns |   339.06 ns |   343.94 ns |  2,978,305.6 | 0.262 |    0.00 |    3 |      - |         - |
|                                 |        |             |            |            |           |             |             |             |             |             |              |       |         |      |        |           |
|            **&#39;ArrayList for loop&#39;** | **100000** |   **619.27 ns** |  **11.149 ns** |   **9.883 ns** |  **2.641 ns** |   **615.39 ns** |   **601.44 ns** |   **613.34 ns** |   **629.20 ns** |   **634.65 ns** |  **1,614,791.7** |  **0.53** |    **0.01** |    **5** |      **-** |         **-** |
|                Collection.First | 100000 | 1,283.01 ns |  23.696 ns |  34.733 ns |  6.450 ns | 1,290.29 ns | 1,229.35 ns | 1,254.36 ns | 1,299.79 ns | 1,369.97 ns |    779,415.9 |  1.10 |    0.04 |    9 | 0.0057 |      40 B |
|       Collection.FirstOrDefault | 100000 | 1,207.60 ns |  22.018 ns |  20.596 ns |  5.318 ns | 1,205.33 ns | 1,175.75 ns | 1,194.30 ns | 1,223.87 ns | 1,241.58 ns |    828,091.6 |  1.04 |    0.02 |    8 | 0.0057 |      40 B |
|           &#39;Collection for loop&#39; | 100000 |   645.75 ns |   9.680 ns |   8.581 ns |  2.293 ns |   646.58 ns |   634.18 ns |   638.33 ns |   649.11 ns |   663.53 ns |  1,548,590.3 |  0.55 |    0.01 |    6 |      - |         - |
|                  &#39;Dictionary[]&#39; | 100000 |    13.23 ns |   0.158 ns |   0.148 ns |  0.038 ns |    13.24 ns |    12.97 ns |    13.14 ns |    13.32 ns |    13.53 ns | 75,577,230.7 |  0.01 |    0.00 |    1 |      - |         - |
|          Dictionary.TryGetValue | 100000 |    13.05 ns |   0.270 ns |   0.240 ns |  0.064 ns |    13.02 ns |    12.62 ns |    12.96 ns |    13.19 ns |    13.46 ns | 76,618,974.8 |  0.01 |    0.00 |    1 |      - |         - |
|                   HashSet.First | 100000 | 1,174.38 ns |  22.209 ns |  20.774 ns |  5.364 ns | 1,173.40 ns | 1,153.97 ns | 1,155.84 ns | 1,181.29 ns | 1,218.22 ns |    851,510.3 |  1.01 |    0.03 |    7 | 0.0057 |      40 B |
|          HashSet.FirstOrDefault | 100000 | 1,249.51 ns |  24.826 ns |  33.141 ns |  6.628 ns | 1,243.56 ns | 1,206.17 ns | 1,223.09 ns | 1,270.16 ns | 1,325.04 ns |    800,312.7 |  1.09 |    0.04 |    8 | 0.0057 |      40 B |
|         &#39;ImmutableDictionary[]&#39; | 100000 |    32.49 ns |   0.643 ns |   0.740 ns |  0.166 ns |    32.38 ns |    31.71 ns |    31.81 ns |    32.81 ns |    34.40 ns | 30,775,696.0 |  0.03 |    0.00 |    3 |      - |         - |
| ImmutableDictionary.TryGetValue | 100000 |    26.79 ns |   0.560 ns |   0.524 ns |  0.135 ns |    26.75 ns |    26.07 ns |    26.42 ns |    27.12 ns |    27.95 ns | 37,332,450.1 |  0.02 |    0.00 |    2 |      - |         - |
|             ImmutableList.First | 100000 | 5,626.10 ns |  86.734 ns |  81.131 ns | 20.948 ns | 5,649.20 ns | 5,484.52 ns | 5,555.68 ns | 5,671.09 ns | 5,749.95 ns |    177,743.1 |  4.83 |    0.10 |   11 | 0.0076 |      72 B |
|    ImmutableList.FirstOrDefault | 100000 | 5,549.44 ns |  89.894 ns |  84.087 ns | 21.711 ns | 5,533.67 ns | 5,443.70 ns | 5,473.20 ns | 5,598.35 ns | 5,732.07 ns |    180,198.3 |  4.77 |    0.12 |   11 | 0.0076 |      72 B |
|        &#39;ImmutableList for loop&#39; | 100000 | 2,485.55 ns |  39.192 ns |  36.660 ns |  9.466 ns | 2,471.42 ns | 2,439.56 ns | 2,457.75 ns | 2,504.26 ns | 2,557.02 ns |    402,325.0 |  2.14 |    0.04 |   10 |      - |         - |
|                      List.First | 100000 | 1,164.40 ns |  22.596 ns |  21.136 ns |  5.457 ns | 1,162.09 ns | 1,132.03 ns | 1,148.51 ns | 1,177.84 ns | 1,202.33 ns |    858,810.6 |  1.00 |    0.00 |    7 | 0.0057 |      40 B |
|             List.FirstOrDefault | 100000 | 1,224.52 ns |  18.444 ns |  17.252 ns |  4.455 ns | 1,220.41 ns | 1,195.75 ns | 1,212.47 ns | 1,236.09 ns | 1,259.05 ns |    816,647.5 |  1.05 |    0.03 |    8 | 0.0057 |      40 B |
|                 &#39;List for loop&#39; | 100000 |   347.20 ns |   6.279 ns |   5.874 ns |  1.517 ns |   344.81 ns |   340.70 ns |   342.81 ns |   351.96 ns |   359.13 ns |  2,880,213.8 |  0.30 |    0.01 |    4 |      - |         - |

----------
**List Vs Yield**


|             Method |     Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|
|               List | 40.11 ns | 0.773 ns | 0.794 ns |  1.00 |    0.00 | 0.0255 |     160 B |
| ListNotConstructor | 31.98 ns | 0.449 ns | 0.420 ns |  0.80 |    0.02 | 0.0178 |     112 B |
|              Yield | 68.67 ns | 1.142 ns | 1.069 ns |  1.71 |    0.05 | 0.0254 |     160 B |

----------
**Loop Tests**


|                   Method |  Count |         Mean |      Error |     StdDev |     StdErr |          Min |           Q1 |       Median |           Q3 |          Max |         Op/s | Ratio | RatioSD | Rank | Allocated |
|------------------------- |------- |-------------:|-----------:|-----------:|-----------:|-------------:|-------------:|-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|
|           **&#39;Foreach loop&#39;** |    **100** |     **32.03 ns** |   **0.217 ns** |   **0.181 ns** |   **0.050 ns** |     **31.71 ns** |     **31.95 ns** |     **32.04 ns** |     **32.23 ns** |     **32.26 ns** | **31,217,549.2** |  **1.00** |    **0.00** |    **1** |         **-** |
|               &#39;For loop&#39; |    100 |     31.74 ns |   0.392 ns |   0.306 ns |   0.088 ns |     31.32 ns |     31.46 ns |     31.77 ns |     32.00 ns |     32.16 ns | 31,504,079.3 |  0.99 |    0.01 |    1 |         - |
| &#39;For loop, local length&#39; |    100 |     31.36 ns |   0.344 ns |   0.305 ns |   0.082 ns |     30.95 ns |     31.13 ns |     31.31 ns |     31.56 ns |     31.94 ns | 31,889,885.2 |  0.98 |    0.01 |    1 |         - |
|                          |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|           **&#39;Foreach loop&#39;** |   **1000** |    **249.78 ns** |   **4.965 ns** |   **6.456 ns** |   **1.318 ns** |    **242.36 ns** |    **245.17 ns** |    **247.41 ns** |    **254.06 ns** |    **265.16 ns** |  **4,003,527.0** |  **1.00** |    **0.00** |    **1** |         **-** |
|               &#39;For loop&#39; |   1000 |    245.05 ns |   2.314 ns |   1.932 ns |   0.536 ns |    241.67 ns |    243.70 ns |    244.78 ns |    246.18 ns |    248.75 ns |  4,080,827.1 |  0.97 |    0.03 |    1 |         - |
| &#39;For loop, local length&#39; |   1000 |    246.73 ns |   3.544 ns |   3.141 ns |   0.840 ns |    242.27 ns |    243.51 ns |    247.36 ns |    249.09 ns |    250.88 ns |  4,052,951.9 |  0.98 |    0.03 |    1 |         - |
|                          |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|           **&#39;Foreach loop&#39;** |  **10000** |  **2,448.37 ns** |  **48.014 ns** |  **55.293 ns** |  **12.364 ns** |  **2,367.05 ns** |  **2,395.42 ns** |  **2,449.62 ns** |  **2,493.93 ns** |  **2,545.85 ns** |    **408,435.8** |  **1.00** |    **0.00** |    **1** |         **-** |
|               &#39;For loop&#39; |  10000 |  2,431.79 ns |  42.099 ns |  39.379 ns |  10.168 ns |  2,372.12 ns |  2,403.62 ns |  2,434.19 ns |  2,455.92 ns |  2,513.31 ns |    411,218.9 |  0.99 |    0.03 |    1 |         - |
| &#39;For loop, local length&#39; |  10000 |  2,453.94 ns |  48.129 ns |  53.495 ns |  12.273 ns |  2,399.52 ns |  2,408.93 ns |  2,425.05 ns |  2,510.83 ns |  2,546.94 ns |    407,507.2 |  1.00 |    0.03 |    1 |         - |
|                          |        |              |            |            |            |              |              |              |              |              |              |       |         |      |           |
|           **&#39;Foreach loop&#39;** | **100000** | **24,869.27 ns** | **452.832 ns** | **401.423 ns** | **107.285 ns** | **24,300.04 ns** | **24,544.40 ns** | **24,831.29 ns** | **25,132.79 ns** | **25,710.15 ns** |     **40,210.3** |  **1.00** |    **0.00** |    **1** |         **-** |
|               &#39;For loop&#39; | 100000 | 24,411.12 ns | 392.783 ns | 367.410 ns |  94.865 ns | 23,893.65 ns | 24,138.24 ns | 24,390.41 ns | 24,623.74 ns | 25,159.70 ns |     40,964.9 |  0.98 |    0.02 |    1 |         - |
| &#39;For loop, local length&#39; | 100000 | 24,366.73 ns | 257.123 ns | 240.513 ns |  62.100 ns | 24,027.55 ns | 24,240.62 ns | 24,330.50 ns | 24,523.35 ns | 24,775.51 ns |     41,039.6 |  0.98 |    0.02 |    1 |         - |

----------
**Memory Stream Vs Recyclable Memory Stream**


|                 Method |  Count |         Mean |        Error |       StdDev |     StdErr |       Median |          Min |           Q1 |           Q3 |          Max |         Op/s | Ratio | RatioSD | Rank |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|----------------------- |------- |-------------:|-------------:|-------------:|-----------:|-------------:|-------------:|-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|--------:|--------:|--------:|----------:|
|           **MemoryStream** |    **100** |     **56.35 ns** |     **1.134 ns** |     **1.165 ns** |   **0.282 ns** |     **56.38 ns** |     **54.42 ns** |     **55.45 ns** |     **57.05 ns** |     **58.74 ns** | **17,744,841.0** |  **1.00** |    **0.00** |    **1** |  **0.0752** |  **0.0001** |       **-** |     **472 B** |
| RecyclableMemoryStream |    100 |    496.58 ns |     9.463 ns |     9.294 ns |   2.324 ns |    493.88 ns |    484.57 ns |    489.64 ns |    502.68 ns |    514.44 ns |  2,013,781.1 |  8.81 |    0.25 |    2 |  0.0801 |       - |       - |     504 B |
|                        |        |              |              |              |            |              |              |              |              |              |              |       |         |      |         |         |         |           |
|           **MemoryStream** |   **1000** |    **141.82 ns** |     **2.258 ns** |     **1.763 ns** |   **0.509 ns** |    **142.23 ns** |    **139.16 ns** |    **140.20 ns** |    **142.67 ns** |    **145.05 ns** |  **7,051,193.6** |  **1.00** |    **0.00** |    **1** |  **0.3364** |  **0.0012** |       **-** |   **2,112 B** |
| RecyclableMemoryStream |   1000 |    576.57 ns |    18.261 ns |    52.980 ns |   5.379 ns |    548.85 ns |    525.02 ns |    536.39 ns |    616.02 ns |    725.28 ns |  1,734,395.2 |  4.78 |    0.21 |    2 |  0.2232 |       - |       - |   1,400 B |
|                        |        |              |              |              |            |              |              |              |              |              |              |       |         |      |         |         |         |           |
|           **MemoryStream** |  **10000** |  **1,046.46 ns** |    **19.302 ns** |    **18.055 ns** |   **4.662 ns** |  **1,042.23 ns** |  **1,025.40 ns** |  **1,033.78 ns** |  **1,054.37 ns** |  **1,083.82 ns** |    **955,600.8** |  **1.00** |    **0.00** |    **1** |  **3.2043** |  **0.1183** |       **-** |  **20,112 B** |
| RecyclableMemoryStream |  10000 |  1,176.64 ns |    23.074 ns |    27.468 ns |   5.994 ns |  1,165.95 ns |  1,134.12 ns |  1,158.75 ns |  1,196.51 ns |  1,231.11 ns |    849,874.5 |  1.13 |    0.03 |    2 |  1.6556 |       - |       - |  10,400 B |
|                        |        |              |              |              |            |              |              |              |              |              |              |       |         |      |         |         |         |           |
|           **MemoryStream** | **100000** | **77,519.66 ns** | **2,499.443 ns** | **7,369.665 ns** | **736.967 ns** | **73,913.46 ns** | **69,551.97 ns** | **72,587.04 ns** | **82,677.00 ns** | **96,711.00 ns** |     **12,900.0** |  **1.00** |    **0.00** |    **2** | **62.3779** | **62.3779** | **62.3779** | **200,112 B** |
| RecyclableMemoryStream | 100000 | 40,736.41 ns |   768.374 ns |   718.738 ns | 185.577 ns | 40,810.80 ns | 39,565.89 ns | 40,269.64 ns | 41,197.51 ns | 42,124.31 ns |     24,548.1 |  0.55 |    0.04 |    1 | 31.1890 | 31.1890 | 31.1890 | 100,400 B |

----------
**Method Call Tests**


|                  Method |  Count |            Mean |         Error |        StdDev |        StdErr |          Median |             Min |              Q1 |              Q3 |             Max |         Op/s |  Ratio | RatioSD | Rank |     Gen 0 |   Allocated |
|------------------------ |------- |----------------:|--------------:|--------------:|--------------:|----------------:|----------------:|----------------:|----------------:|----------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **&#39;Direct call to method&#39;** |    **100** |        **30.87 ns** |      **0.243 ns** |      **0.189 ns** |      **0.055 ns** |        **30.83 ns** |        **30.62 ns** |        **30.74 ns** |        **30.97 ns** |        **31.23 ns** | **32,393,694.1** |   **1.00** |    **0.00** |    **1** |         **-** |           **-** |
|            Func(Method) |    100 |       712.07 ns |     31.120 ns |     90.778 ns |      9.170 ns |       662.62 ns |       632.58 ns |       648.36 ns |       781.93 ns |       978.81 ns |  1,404,362.3 |  21.90 |    1.82 |    3 |    1.0195 |     6,400 B |
|              Func(_=&gt;_) |    100 |       175.83 ns |      1.681 ns |      1.572 ns |      0.406 ns |       176.06 ns |       173.03 ns |       174.72 ns |       176.66 ns |       178.07 ns |  5,687,372.8 |   5.71 |    0.05 |    2 |         - |           - |
|          &#39;Local method&#39; |    100 |        30.95 ns |      0.307 ns |      0.272 ns |      0.073 ns |        30.88 ns |        30.63 ns |        30.77 ns |        31.10 ns |        31.42 ns | 32,310,172.8 |   1.00 |    0.01 |    1 |         - |           - |
|    &#39;MethodInfo, cached&#39; |    100 |     5,972.51 ns |     60.136 ns |     56.252 ns |     14.524 ns |     5,975.44 ns |     5,867.06 ns |     5,938.16 ns |     6,000.20 ns |     6,064.88 ns |    167,433.7 | 193.81 |    2.25 |    4 |    0.3815 |     2,400 B |
|                         |        |                 |               |               |               |                 |                 |                 |                 |                 |              |        |         |      |           |             |
| **&#39;Direct call to method&#39;** |   **1000** |       **251.59 ns** |      **4.910 ns** |      **4.593 ns** |      **1.186 ns** |       **249.84 ns** |       **246.62 ns** |       **248.00 ns** |       **254.29 ns** |       **259.70 ns** |  **3,974,670.9** |   **1.00** |    **0.00** |    **1** |         **-** |           **-** |
|            Func(Method) |   1000 |     6,308.92 ns |     86.229 ns |     80.659 ns |     20.826 ns |     6,290.41 ns |     6,215.49 ns |     6,241.87 ns |     6,385.39 ns |     6,435.42 ns |    158,505.8 |  25.09 |    0.68 |    3 |   10.2005 |    64,000 B |
|              Func(_=&gt;_) |   1000 |     1,698.41 ns |     14.669 ns |     11.452 ns |      3.306 ns |     1,697.22 ns |     1,682.96 ns |     1,689.25 ns |     1,706.15 ns |     1,716.21 ns |    588,787.5 |   6.79 |    0.09 |    2 |         - |           - |
|          &#39;Local method&#39; |   1000 |       250.63 ns |      3.322 ns |      2.945 ns |      0.787 ns |       250.59 ns |       246.08 ns |       248.15 ns |       252.33 ns |       256.66 ns |  3,989,897.4 |   1.00 |    0.02 |    1 |         - |           - |
|    &#39;MethodInfo, cached&#39; |   1000 |    66,242.90 ns |  1,248.425 ns |  1,106.696 ns |    295.777 ns |    66,136.64 ns |    64,964.71 ns |    65,400.54 ns |    66,851.57 ns |    68,793.02 ns |     15,096.0 | 263.65 |    7.07 |    4 |    3.7842 |    24,000 B |
|                         |        |                 |               |               |               |                 |                 |                 |                 |                 |              |        |         |      |           |             |
| **&#39;Direct call to method&#39;** |  **10000** |     **2,561.94 ns** |     **50.464 ns** |     **69.076 ns** |     **13.547 ns** |     **2,543.54 ns** |     **2,441.82 ns** |     **2,519.05 ns** |     **2,606.59 ns** |     **2,727.19 ns** |    **390,329.6** |   **1.00** |    **0.00** |    **1** |         **-** |           **-** |
|            Func(Method) |  10000 |    65,629.10 ns |  1,305.972 ns |  1,787.629 ns |    350.583 ns |    65,587.47 ns |    62,870.74 ns |    64,173.22 ns |    66,757.56 ns |    70,240.54 ns |     15,237.1 |  25.63 |    0.91 |    3 |  101.9287 |   640,000 B |
|              Func(_=&gt;_) |  10000 |    17,589.30 ns |    297.545 ns |    740.990 ns |     86.726 ns |    17,377.16 ns |    17,122.59 ns |    17,314.27 ns |    17,474.96 ns |    20,617.09 ns |     56,852.7 |   7.02 |    0.45 |    2 |         - |           - |
|          &#39;Local method&#39; |  10000 |     2,528.18 ns |     49.108 ns |     48.231 ns |     12.058 ns |     2,520.59 ns |     2,460.60 ns |     2,494.31 ns |     2,555.80 ns |     2,634.11 ns |    395,541.6 |   0.98 |    0.03 |    1 |         - |           - |
|    &#39;MethodInfo, cached&#39; |  10000 |   673,946.13 ns |  7,061.675 ns |  6,605.496 ns |  1,705.532 ns |   675,496.97 ns |   664,033.98 ns |   668,257.52 ns |   678,890.53 ns |   686,748.63 ns |      1,483.8 | 260.87 |    8.45 |    4 |   38.0859 |   240,000 B |
|                         |        |                 |               |               |               |                 |                 |                 |                 |                 |              |        |         |      |           |             |
| **&#39;Direct call to method&#39;** | **100000** |    **24,756.99 ns** |     **51.204 ns** |     **39.977 ns** |     **11.540 ns** |    **24,771.03 ns** |    **24,683.09 ns** |    **24,742.00 ns** |    **24,781.67 ns** |    **24,803.59 ns** |     **40,392.6** |   **1.00** |    **0.00** |    **1** |         **-** |           **-** |
|            Func(Method) | 100000 |   687,589.38 ns | 18,852.580 ns | 52,240.410 ns |  5,537.472 ns |   665,103.61 ns |   646,082.32 ns |   656,028.61 ns |   687,837.79 ns |   850,823.93 ns |      1,454.4 |  29.18 |    3.20 |    3 | 1019.5313 | 6,400,000 B |
|              Func(_=&gt;_) | 100000 |   173,885.30 ns |  1,448.752 ns |  1,284.281 ns |    343.239 ns |   173,979.49 ns |   172,209.84 ns |   172,992.88 ns |   174,502.50 ns |   176,804.30 ns |      5,750.9 |   7.03 |    0.05 |    2 |         - |           - |
|          &#39;Local method&#39; | 100000 |    25,254.51 ns |    419.184 ns |    837.156 ns |    119.594 ns |    24,955.76 ns |    24,518.42 ns |    24,797.88 ns |    25,337.59 ns |    27,900.32 ns |     39,596.9 |   1.04 |    0.06 |    1 |         - |           - |
|    &#39;MethodInfo, cached&#39; | 100000 | 6,849,768.19 ns | 69,527.077 ns | 61,633.943 ns | 16,472.364 ns | 6,834,344.53 ns | 6,764,375.00 ns | 6,813,370.51 ns | 6,882,742.97 ns | 6,988,984.38 ns |        146.0 | 276.84 |    2.80 |    4 |  375.0000 | 2,400,000 B |

----------
**Null Equality Tests**


|          Method |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD | Allocated |
|---------------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|
|          Equals | 0.4851 ns | 0.0079 ns | 0.0066 ns | 0.4872 ns | 10.49 |    1.29 |         - |
|      EqualsFunc | 0.6063 ns | 0.0806 ns | 0.2260 ns | 0.4952 ns | 21.08 |    5.59 |         - |
|    EqualsObject | 0.0381 ns | 0.0083 ns | 0.0078 ns | 0.0360 ns |  0.85 |    0.16 |         - |
|              Is | 0.0468 ns | 0.0073 ns | 0.0057 ns | 0.0471 ns |  1.00 |    0.00 |         - |
| ReferenceEquals | 0.0366 ns | 0.0072 ns | 0.0064 ns | 0.0352 ns |  0.76 |    0.10 |         - |

----------
**Object Creation Tests**


|                                                   Method |           Mean |       Error |        StdDev |      StdErr |         Median |            Min |             Q1 |             Q3 |            Max |             Op/s | Ratio | RatioSD | Rank |  Gen 0 |  Gen 1 | Allocated |
|--------------------------------------------------------- |---------------:|------------:|--------------:|------------:|---------------:|---------------:|---------------:|---------------:|---------------:|-----------------:|------:|--------:|-----:|-------:|-------:|----------:|
|      &#39;Create object using Activator.CreateInstance&lt;T&gt;()&#39; |     26.4678 ns |   0.2905 ns |     0.2425 ns |   0.0673 ns |     26.3805 ns |     26.1593 ns |     26.3390 ns |     26.5540 ns |     26.8473 ns |     37,781,800.1 |     ? |       ? |    3 | 0.0038 |      - |      24 B |
|         &#39;Create object using Activator.CreateInstance()&#39; |     29.9794 ns |   0.6351 ns |     1.4719 ns |   0.1840 ns |     29.3092 ns |     28.7621 ns |     29.1018 ns |     30.0155 ns |     35.4322 ns |     33,356,241.4 |     ? |       ? |    4 | 0.0038 |      - |      24 B |
|   &#39;Create object using ConstructorInfo, cached in field&#39; |     72.6548 ns |   1.4224 ns |     1.5220 ns |   0.3587 ns |     72.7255 ns |     69.9149 ns |     71.8405 ns |     73.4710 ns |     75.4013 ns |     13,763,724.5 |     ? |       ? |    7 | 0.0076 |      - |      48 B |
| &#39;Create object using compiled lambda expression, cached&#39; |      3.7021 ns |   0.2123 ns |     0.6261 ns |   0.0626 ns |      3.3590 ns |      3.1457 ns |      3.2249 ns |      4.1423 ns |      5.3694 ns |    270,119,319.3 |     ? |       ? |    2 | 0.0038 |      - |      24 B |
|             &#39;Create object using ConstructorInfo cached&#39; |     70.8486 ns |   0.5032 ns |     0.4707 ns |   0.1215 ns |     70.6595 ns |     70.2778 ns |     70.4919 ns |     71.1468 ns |     71.7138 ns |     14,114,603.0 |     ? |       ? |    6 | 0.0076 |      - |      48 B |
|                    &#39;Create object using ConstructorInfo&#39; |    109.1200 ns |   1.7495 ns |     4.2586 ns |   0.5090 ns |    107.8103 ns |    105.7136 ns |    106.8997 ns |    109.8230 ns |    127.5463 ns |      9,164,226.9 |     ? |       ? |    8 | 0.0126 |      - |      80 B |
|                  &#39;Create object using FormatterServices&#39; |     31.9839 ns |   0.4694 ns |     0.3664 ns |   0.1058 ns |     31.8756 ns |     31.6382 ns |     31.7563 ns |     32.0733 ns |     32.8622 ns |     31,265,754.8 |     ? |       ? |    5 | 0.0038 |      - |      24 B |
|         &#39;Create object using compiled lambda expression&#39; | 49,978.7618 ns | 824.1060 ns | 1,926.3211 ns | 238.9307 ns | 49,528.0273 ns | 48,069.6411 ns | 48,743.7622 ns | 50,396.4111 ns | 58,308.7708 ns |         20,008.5 |     ? |       ? |    9 | 0.6104 | 0.3052 |   3,927 B |
|                                &#39;Create object using new&#39; |      0.0569 ns |   0.0381 ns |     0.1074 ns |   0.0112 ns |      0.0000 ns |      0.0000 ns |      0.0000 ns |      0.0270 ns |      0.3421 ns | 17,575,990,437.4 |     ? |       ? |    1 |      - |      - |         - |

----------
**Partitioned Parallel**


|                         Method |  Count |       Mean |      Error |     StdDev |    StdErr |     Median |       Min |        Q1 |         Q3 |        Max |      Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|------------------------------- |------- |-----------:|-----------:|-----------:|----------:|-----------:|----------:|----------:|-----------:|-----------:|----------:|------:|--------:|-----:|-------:|----------:|
|                   **Parallel.For** |    **100** |   **4.177 μs** |  **0.2961 μs** |  **0.8731 μs** | **0.0873 μs** |   **3.973 μs** |  **3.044 μs** |  **3.398 μs** |   **5.093 μs** |   **6.277 μs** | **239,383.9** |  **1.00** |    **0.00** |    **1** | **0.4158** |      **3 KB** |
|               Parallel.ForEach |    100 |   6.351 μs |  0.1022 μs |  0.1177 μs | 0.0263 μs |   6.336 μs |  6.205 μs |  6.245 μs |   6.438 μs |   6.617 μs | 157,466.2 |  1.40 |    0.22 |    2 | 0.4349 |      3 KB |
| &#39;Parallel.ForEach partitioned&#39; |    100 |  11.851 μs |  0.2861 μs |  0.7686 μs | 0.0839 μs |  11.556 μs | 11.158 μs | 11.341 μs |  12.038 μs |  14.598 μs |  84,380.6 |  2.94 |    0.67 |    3 | 0.9155 |      6 KB |
|                                |        |            |            |            |           |            |           |           |            |            |           |       |         |      |        |           |
|                   **Parallel.For** |   **1000** |  **18.360 μs** |  **0.3101 μs** |  **0.2901 μs** | **0.0749 μs** |  **18.251 μs** | **17.918 μs** | **18.164 μs** |  **18.641 μs** |  **18.833 μs** |  **54,467.3** |  **1.00** |    **0.00** |    **2** | **0.6256** |      **4 KB** |
|               Parallel.ForEach |   1000 |  20.711 μs |  0.4029 μs |  0.5238 μs | 0.1069 μs |  20.838 μs | 19.755 μs | 20.251 μs |  21.058 μs |  21.830 μs |  48,282.8 |  1.12 |    0.03 |    3 | 0.6409 |      4 KB |
| &#39;Parallel.ForEach partitioned&#39; |   1000 |  10.362 μs |  0.4758 μs |  1.2944 μs | 0.1396 μs |   9.709 μs |  9.475 μs |  9.565 μs |  10.708 μs |  15.156 μs |  96,504.3 |  0.70 |    0.07 |    1 | 0.8240 |      5 KB |
|                                |        |            |            |            |           |            |           |           |            |            |           |       |         |      |        |           |
|                   **Parallel.For** |  **10000** |  **32.842 μs** |  **2.8922 μs** |  **8.5277 μs** | **0.8528 μs** |  **37.600 μs** | **19.847 μs** | **22.988 μs** |  **39.051 μs** |  **46.253 μs** |  **30,449.2** |  **1.00** |    **0.00** |    **2** | **0.7019** |      **4 KB** |
|               Parallel.ForEach |  10000 |  35.297 μs |  3.1657 μs |  9.3340 μs | 0.9334 μs |  42.064 μs | 22.118 μs | 25.020 μs |  43.587 μs |  46.770 μs |  28,330.7 |  1.08 |    0.07 |    3 | 0.7324 |      4 KB |
| &#39;Parallel.ForEach partitioned&#39; |  10000 |  17.447 μs |  1.5157 μs |  4.4692 μs | 0.4469 μs |  20.887 μs | 11.407 μs | 12.228 μs |  21.468 μs |  22.693 μs |  57,317.2 |  0.53 |    0.03 |    1 | 0.9308 |      6 KB |
|                                |        |            |            |            |           |            |           |           |            |            |           |       |         |      |        |           |
|                   **Parallel.For** | **100000** | **124.321 μs** | **10.1238 μs** | **29.8502 μs** | **2.9850 μs** | **134.103 μs** | **68.740 μs** | **93.015 μs** | **145.604 μs** | **169.495 μs** |   **8,043.7** |  **1.00** |    **0.00** |    **2** | **0.6104** |      **4 KB** |
|               Parallel.ForEach | 100000 | 136.863 μs | 14.6948 μs | 43.3281 μs | 4.3328 μs | 123.707 μs | 85.467 μs | 93.132 μs | 176.282 μs | 221.686 μs |   7,306.6 |  1.10 |    0.19 |    3 | 0.6104 |      4 KB |
| &#39;Parallel.ForEach partitioned&#39; | 100000 |  30.530 μs |  2.3220 μs |  6.8466 μs | 0.6847 μs |  34.715 μs | 18.383 μs | 23.196 μs |  35.399 μs |  40.059 μs |  32,755.0 |  0.25 |    0.06 |    1 | 1.0071 |      6 KB |

----------
**Pattern Matching Tests**


|             Method |     Mean |     Error |    StdDev |    StdErr |   Median |      Min |       Q1 |       Q3 |      Max |          Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|--------------:|------:|--------:|-----:|-------:|----------:|
|                 As | 2.151 ns | 0.0698 ns | 0.0747 ns | 0.0176 ns | 2.148 ns | 2.053 ns | 2.093 ns | 2.208 ns | 2.303 ns | 464,990,670.2 |  0.80 |    0.08 |    1 | 0.0038 |      24 B |
|                 Is | 2.395 ns | 0.1561 ns | 0.4577 ns | 0.0460 ns | 2.168 ns | 1.967 ns | 2.088 ns | 2.727 ns | 3.636 ns | 417,462,520.2 |  1.10 |    0.11 |    3 | 0.0038 |      24 B |
| &#39;Pattern matching&#39; | 2.178 ns | 0.1330 ns | 0.3902 ns | 0.0392 ns | 1.996 ns | 1.813 ns | 1.884 ns | 2.500 ns | 3.397 ns | 459,195,081.5 |  1.00 |    0.00 |    2 | 0.0038 |      24 B |

----------
**Read File**


|                           Method |       Mean |     Error |     StdDev |    StdErr |     Median |        Min |         Q1 |         Q3 |        Max |    Op/s |  Ratio | RatioSD | Rank |      Gen 0 |     Gen 1 |     Gen 2 |  Allocated |
|--------------------------------- |-----------:|----------:|-----------:|----------:|-----------:|-----------:|-----------:|-----------:|-----------:|--------:|-------:|--------:|-----:|-----------:|----------:|----------:|-----------:|
|                        File.Read |   3.653 ms | 0.0170 ms |  0.0142 ms | 0.0039 ms |   3.648 ms |   3.634 ms |   3.643 ms |   3.662 ms |   3.683 ms | 273.762 |   1.00 |    0.00 |    1 |   500.0000 |  500.0000 |  500.0000 |  11,719 KB |
|                   File.ReadAsync |   3.780 ms | 0.0575 ms |  0.0538 ms | 0.0139 ms |   3.764 ms |   3.701 ms |   3.743 ms |   3.818 ms |   3.910 ms | 264.527 |   1.04 |    0.02 |    2 |   500.0000 |  500.0000 |  500.0000 |  11,721 KB |
|        &#39;File.Read in loop, 1024&#39; |  18.151 ms | 0.3584 ms |  0.4660 ms | 0.0951 ms |  18.133 ms |  17.505 ms |  17.825 ms |  18.480 ms |  19.406 ms |  55.092 |   4.93 |    0.12 |    7 |          - |         - |         - |       5 KB |
| &#39;File.Read in loop, 1024, async&#39; |  77.365 ms | 1.5472 ms |  1.5196 ms | 0.3799 ms |  77.597 ms |  74.967 ms |  75.950 ms |  78.564 ms |  80.122 ms |  12.926 |  21.10 |    0.44 |   10 |   166.6667 |         - |         - |   1,469 KB |
|        &#39;File.Read in loop, 2048&#39; |  17.705 ms | 0.2227 ms |  0.1975 ms | 0.0528 ms |  17.676 ms |  17.315 ms |  17.615 ms |  17.827 ms |  18.117 ms |  56.481 |   4.84 |    0.05 |    6 |          - |         - |         - |       6 KB |
| &#39;File.Read in loop, 2048, async&#39; |  52.240 ms | 0.4904 ms |  0.4348 ms | 0.1162 ms |  52.213 ms |  51.479 ms |  52.032 ms |  52.483 ms |  52.966 ms |  19.142 |  14.30 |    0.16 |    8 |   100.0000 |         - |         - |     820 KB |
|        &#39;File.Read in loop, 4096&#39; |  17.158 ms | 0.2380 ms |  0.1987 ms | 0.0551 ms |  17.099 ms |  16.889 ms |  17.029 ms |  17.277 ms |  17.641 ms |  58.282 |   4.70 |    0.06 |    5 |          - |         - |         - |       4 KB |
| &#39;File.Read in loop, 4096, async&#39; |  64.581 ms | 1.1828 ms |  1.1616 ms | 0.2904 ms |  64.855 ms |  61.820 ms |  64.176 ms |  65.431 ms |  66.174 ms |  15.484 |  17.64 |    0.33 |    9 |          - |         - |         - |     495 KB |
|                    File.OpenRead |   3.781 ms | 0.0708 ms |  0.0920 ms | 0.0188 ms |   3.805 ms |   3.657 ms |   3.690 ms |   3.847 ms |   3.945 ms | 264.474 |   1.03 |    0.03 |    2 |   500.0000 |  500.0000 |  500.0000 |  11,719 KB |
|                File.ReadAllBytes |   3.693 ms | 0.0232 ms |  0.0206 ms | 0.0055 ms |   3.690 ms |   3.658 ms |   3.680 ms |   3.704 ms |   3.734 ms | 270.812 |   1.01 |    0.01 |    1 |   500.0000 |  500.0000 |  500.0000 |  11,719 KB |
|           File.ReadAllBytesAsync |   4.719 ms | 0.0989 ms |  0.2899 ms | 0.0291 ms |   4.716 ms |   4.155 ms |   4.503 ms |   4.909 ms |   5.450 ms | 211.919 |   1.30 |    0.07 |    4 |   414.0625 |  414.0625 |  414.0625 |  11,721 KB |
|       &#39;File.ReadAllBytes static&#39; |   4.470 ms | 0.0891 ms |  0.1738 ms | 0.0253 ms |   4.458 ms |   4.176 ms |   4.348 ms |   4.587 ms |   4.863 ms | 223.717 |   1.24 |    0.05 |    3 |   500.0000 |  500.0000 |  500.0000 |  11,719 KB |
|                File.ReadAllLines | 146.018 ms | 2.9034 ms |  7.3902 ms | 0.8477 ms | 142.874 ms | 136.701 ms | 141.156 ms | 151.213 ms | 166.085 ms |   6.848 |  39.43 |    1.44 |   12 |  9000.0000 | 4000.0000 | 1750.0000 |  72,942 KB |
|           File.ReadAllLinesAsync | 387.959 ms | 7.7523 ms | 17.8123 ms | 2.2441 ms | 382.485 ms | 341.220 ms | 376.208 ms | 399.452 ms | 440.527 ms |   2.578 | 108.66 |    5.77 |   13 | 21000.0000 | 8000.0000 | 3000.0000 | 143,698 KB |
|                 File.ReadAllText |  52.695 ms | 2.0376 ms |  5.9758 ms | 0.6006 ms |  50.680 ms |  44.849 ms |  48.321 ms |  55.836 ms |  68.592 ms |  18.977 |  14.63 |    1.05 |    8 |  4545.4545 | 2181.8182 | 1000.0000 |  46,994 KB |
|            File.ReadAllTextAsync | 131.793 ms | 5.8084 ms | 17.0351 ms | 1.7121 ms | 125.540 ms | 110.466 ms | 118.233 ms | 144.238 ms | 179.126 ms |   7.588 |  36.95 |    2.70 |   11 |  4333.3333 | 2000.0000 | 1000.0000 |  47,419 KB |

----------
**Reflection Caching Tests**


|                                     Method |       Mean |     Error |    StdDev |    StdErr |        Min |         Q1 |     Median |         Q3 |        Max |         Op/s | Ratio | Rank |  Gen 0 | Allocated |
|------------------------------------------- |-----------:|----------:|----------:|----------:|-----------:|-----------:|-----------:|-----------:|-----------:|-------------:|------:|-----:|-------:|----------:|
|                &#39;Get properties with cache&#39; |  0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns |  0.0000 ns |  0.0000 ns |  0.0000 ns |  0.0000 ns |  0.0000 ns |     Infinity | 0.000 |    1 |      - |         - |
| &#39;Get properties with ConcurrentDictionary&#39; | 13.3829 ns | 0.2907 ns | 0.2856 ns | 0.0714 ns | 12.9851 ns | 13.1144 ns | 13.4544 ns | 13.5712 ns | 13.8953 ns | 74,722,416.0 | 0.357 |    2 |      - |         - |
|           &#39;Get properties with dictionary&#39; | 14.5359 ns | 0.1625 ns | 0.1440 ns | 0.0385 ns | 14.2815 ns | 14.4192 ns | 14.5331 ns | 14.6461 ns | 14.7799 ns | 68,795,279.0 | 0.387 |    3 |      - |         - |
|             &#39;Get properties without cache&#39; | 37.5307 ns | 0.5671 ns | 0.5027 ns | 0.1344 ns | 36.8879 ns | 37.0765 ns | 37.4976 ns | 37.9777 ns | 38.3430 ns | 26,644,839.4 | 1.000 |    4 | 0.0063 |      40 B |

----------
**Regex Or Contains**


|              Method |       Mean |     Error |    StdDev |     Median | Ratio | RatioSD | Allocated |
|-------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|----------:|
|            Contains |   151.8 ns |   1.60 ns |   1.42 ns |   151.5 ns |  0.04 |    0.00 |         - |
|  ContainsIgnoreCase | 3,505.7 ns |  59.32 ns |  55.48 ns | 3,510.4 ns |  1.00 |    0.00 |         - |
|           RegexCall | 3,069.0 ns |  65.36 ns | 181.12 ns | 3,009.4 ns |  0.93 |    0.09 |         - |
| RegexCallIgnoreCase | 8,941.6 ns | 107.94 ns |  95.68 ns | 8,948.9 ns |  2.55 |    0.05 |         - |

----------
**Span Vs Array**


|                Method |  Count |          Mean |        Error |       StdDev |     StdErr |        Median |           Min |            Q1 |            Q3 |           Max |         Op/s | Ratio | RatioSD | Rank |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|---------------------- |------- |--------------:|-------------:|-------------:|-----------:|--------------:|--------------:|--------------:|--------------:|--------------:|-------------:|------:|--------:|-----:|---------:|---------:|---------:|----------:|
|                 **Array** |    **100** |      **73.16 ns** |     **3.172 ns** |     **9.351 ns** |   **0.935 ns** |      **67.31 ns** |      **64.32 ns** |      **65.96 ns** |      **80.90 ns** |      **97.95 ns** | **13,668,734.9** |  **1.00** |    **0.00** |    **2** |   **0.0675** |        **-** |        **-** |     **424 B** |
| &#39;Array With Pointers&#39; |    100 |      50.70 ns |     1.029 ns |     1.101 ns |   0.259 ns |      51.02 ns |      48.45 ns |      50.14 ns |      51.39 ns |      52.62 ns | 19,725,429.5 |  0.58 |    0.04 |    1 |   0.0675 |        - |        - |     424 B |
|                  Span |    100 |      67.91 ns |     1.355 ns |     1.762 ns |   0.360 ns |      67.43 ns |      65.73 ns |      66.42 ns |      69.18 ns |      71.27 ns | 14,725,139.0 |  0.80 |    0.06 |    2 |   0.0675 |        - |        - |     424 B |
|     &#39;Span With Slice&#39; |    100 |      74.46 ns |     2.606 ns |     7.562 ns |   0.768 ns |      70.55 ns |      67.81 ns |      68.92 ns |      78.94 ns |      97.70 ns | 13,429,341.6 |  1.02 |    0.10 |    3 |   0.0675 |        - |        - |     424 B |
|                       |        |               |              |              |            |               |               |               |               |               |              |       |         |      |          |          |          |           |
|                 **Array** |   **1000** |     **614.53 ns** |     **5.780 ns** |     **5.124 ns** |   **1.370 ns** |     **615.30 ns** |     **603.89 ns** |     **611.33 ns** |     **618.40 ns** |     **620.97 ns** |  **1,627,268.1** |  **1.00** |    **0.00** |    **2** |   **0.6409** |        **-** |        **-** |   **4,024 B** |
| &#39;Array With Pointers&#39; |   1000 |     417.96 ns |     8.317 ns |     8.899 ns |   2.097 ns |     414.37 ns |     407.85 ns |     411.60 ns |     423.61 ns |     439.18 ns |  2,392,581.3 |  0.68 |    0.02 |    1 |   0.6413 |        - |        - |   4,024 B |
|                  Span |   1000 |     610.16 ns |     8.020 ns |     7.502 ns |   1.937 ns |     610.94 ns |     594.91 ns |     606.90 ns |     614.70 ns |     622.37 ns |  1,638,909.3 |  0.99 |    0.02 |    2 |   0.6409 |        - |        - |   4,024 B |
|     &#39;Span With Slice&#39; |   1000 |     599.40 ns |    11.889 ns |    11.121 ns |   2.871 ns |     596.80 ns |     582.80 ns |     593.06 ns |     607.05 ns |     625.11 ns |  1,668,345.7 |  0.97 |    0.02 |    2 |   0.6409 |        - |        - |   4,024 B |
|                       |        |               |              |              |            |               |               |               |               |               |              |       |         |      |          |          |          |           |
|                 **Array** |  **10000** |   **6,551.54 ns** |   **125.110 ns** |   **104.473 ns** |  **28.976 ns** |   **6,516.17 ns** |   **6,418.85 ns** |   **6,472.31 ns** |   **6,588.51 ns** |   **6,774.45 ns** |    **152,635.9** |  **1.00** |    **0.00** |    **2** |   **6.3248** |        **-** |        **-** |  **40,024 B** |
| &#39;Array With Pointers&#39; |  10000 |   4,172.81 ns |    35.655 ns |    29.773 ns |   8.258 ns |   4,166.95 ns |   4,123.72 ns |   4,157.48 ns |   4,196.96 ns |   4,218.52 ns |    239,646.5 |  0.64 |    0.01 |    1 |   6.3248 |        - |        - |  40,024 B |
|                  Span |  10000 |   6,798.20 ns |    73.786 ns |   168.049 ns |  21.342 ns |   6,755.40 ns |   6,607.03 ns |   6,685.07 ns |   6,881.05 ns |   7,425.85 ns |    147,097.8 |  1.06 |    0.04 |    3 |   6.3248 |        - |        - |  40,024 B |
|     &#39;Span With Slice&#39; |  10000 |   6,527.40 ns |   113.685 ns |   106.341 ns |  27.457 ns |   6,509.15 ns |   6,394.84 ns |   6,440.68 ns |   6,570.18 ns |   6,737.36 ns |    153,200.3 |  1.00 |    0.02 |    2 |   6.3248 |        - |        - |  40,024 B |
|                       |        |               |              |              |            |               |               |               |               |               |              |       |         |      |          |          |          |           |
|                 **Array** | **100000** | **154,037.63 ns** | **2,382.690 ns** | **2,228.770 ns** | **575.466 ns** | **154,262.13 ns** | **150,484.38 ns** | **152,915.36 ns** | **155,181.75 ns** | **157,903.76 ns** |      **6,491.9** |  **1.00** |    **0.00** |    **3** | **124.7559** | **124.7559** | **124.7559** | **400,024 B** |
| &#39;Array With Pointers&#39; | 100000 | 135,354.12 ns | 2,624.042 ns | 2,577.160 ns | 644.290 ns | 135,428.96 ns | 131,184.72 ns | 133,563.78 ns | 137,276.57 ns | 140,092.19 ns |      7,388.0 |  0.88 |    0.02 |    1 | 124.7559 | 124.7559 | 124.7559 | 400,024 B |
|                  Span | 100000 | 156,877.26 ns | 3,018.657 ns | 6,996.197 ns | 874.525 ns | 155,535.31 ns | 149,645.68 ns | 153,576.82 ns | 157,187.54 ns | 183,059.67 ns |      6,374.4 |  1.05 |    0.09 |    3 | 124.7559 | 124.7559 | 124.7559 | 400,024 B |
|     &#39;Span With Slice&#39; | 100000 | 147,434.08 ns | 2,228.140 ns | 1,975.188 ns | 527.891 ns | 147,251.45 ns | 144,633.40 ns | 145,783.25 ns | 148,958.80 ns | 151,034.30 ns |      6,782.7 |  0.96 |    0.02 |    2 | 124.7559 | 124.7559 | 124.7559 | 400,024 B |

----------
**Stack Alloc Vs New**


|              Method |  Count |         Mean |       Error |      StdDev |    StdErr |       Median |          Min |           Q1 |           Q3 |          Max |        Op/s | Ratio | RatioSD | Rank |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|-------------------- |------- |-------------:|------------:|------------:|----------:|-------------:|-------------:|-------------:|-------------:|-------------:|------------:|------:|--------:|-----:|---------:|---------:|---------:|----------:|
|                 **New** |    **100** |     **143.4 ns** |     **1.28 ns** |     **1.13 ns** |   **0.30 ns** |     **143.3 ns** |     **141.1 ns** |     **143.0 ns** |     **144.1 ns** |     **145.6 ns** | **6,973,890.5** |  **1.02** |    **0.05** |    **2** |   **0.0675** |        **-** |        **-** |     **424 B** |
| &#39;Stackalloc struct&#39; |    100 |     140.5 ns |     1.47 ns |     1.38 ns |   0.36 ns |     140.4 ns |     139.1 ns |     139.2 ns |     141.2 ns |     143.4 ns | 7,117,229.7 |  1.00 |    0.05 |    1 |        - |        - |        - |         - |
|          Stackalloc |    100 |     142.7 ns |     2.88 ns |     8.41 ns |   0.85 ns |     138.6 ns |     135.0 ns |     136.6 ns |     150.3 ns |     169.6 ns | 7,009,266.6 |  1.00 |    0.00 |    1 |        - |        - |        - |         - |
|                     |        |              |             |             |           |              |              |              |              |              |             |       |         |      |          |          |          |           |
|                 **New** |   **1000** |   **1,478.5 ns** |    **14.88 ns** |    **12.43 ns** |   **3.45 ns** |   **1,481.0 ns** |   **1,453.5 ns** |   **1,470.4 ns** |   **1,485.4 ns** |   **1,495.2 ns** |   **676,342.3** |  **1.03** |    **0.01** |    **2** |   **0.6409** |        **-** |        **-** |   **4,024 B** |
| &#39;Stackalloc struct&#39; |   1000 |   1,455.4 ns |    13.01 ns |    12.17 ns |   3.14 ns |   1,455.4 ns |   1,424.1 ns |   1,451.4 ns |   1,462.5 ns |   1,473.2 ns |   687,080.7 |  1.01 |    0.01 |    1 |        - |        - |        - |         - |
|          Stackalloc |   1000 |   1,435.3 ns |    16.53 ns |    15.46 ns |   3.99 ns |   1,432.2 ns |   1,410.4 ns |   1,424.6 ns |   1,444.0 ns |   1,466.5 ns |   696,730.5 |  1.00 |    0.00 |    1 |        - |        - |        - |         - |
|                     |        |              |             |             |           |              |              |              |              |              |             |       |         |      |          |          |          |           |
|                 **New** |  **10000** |  **12,860.8 ns** |   **129.29 ns** |   **120.94 ns** |  **31.23 ns** |  **12,864.8 ns** |  **12,637.5 ns** |  **12,790.0 ns** |  **12,939.1 ns** |  **13,059.4 ns** |    **77,755.6** |  **0.90** |    **0.01** |    **1** |   **6.3171** |        **-** |        **-** |  **40,024 B** |
| &#39;Stackalloc struct&#39; |  10000 |  14,257.3 ns |   126.57 ns |   118.39 ns |  30.57 ns |  14,269.3 ns |  14,105.8 ns |  14,149.6 ns |  14,337.4 ns |  14,471.6 ns |    70,139.5 |  1.00 |    0.01 |    2 |        - |        - |        - |         - |
|          Stackalloc |  10000 |  14,239.2 ns |   109.09 ns |   102.05 ns |  26.35 ns |  14,244.6 ns |  14,089.3 ns |  14,170.2 ns |  14,303.5 ns |  14,460.4 ns |    70,228.6 |  1.00 |    0.00 |    2 |        - |        - |        - |         - |
|                     |        |              |             |             |           |              |              |              |              |              |             |       |         |      |          |          |          |           |
|                 **New** | **100000** | **234,425.9 ns** | **2,515.29 ns** | **2,229.74 ns** | **595.92 ns** | **233,713.2 ns** | **231,161.0 ns** | **233,112.9 ns** | **236,207.2 ns** | **237,829.7 ns** |     **4,265.7** |  **1.78** |    **0.02** |    **2** | **124.7559** | **124.7559** | **124.7559** | **400,024 B** |
| &#39;Stackalloc struct&#39; | 100000 | 131,723.4 ns |   741.38 ns |   619.08 ns | 171.70 ns | 131,718.9 ns | 131,048.5 ns | 131,283.6 ns | 132,069.6 ns | 133,325.4 ns |     7,591.7 |  1.00 |    0.01 |    1 |        - |        - |        - |         - |
|          Stackalloc | 100000 | 131,612.3 ns | 1,164.57 ns | 1,089.34 ns | 281.27 ns | 131,801.0 ns | 129,995.8 ns | 130,720.8 ns | 132,530.8 ns | 133,822.0 ns |     7,598.1 |  1.00 |    0.00 |    1 |        - |        - |        - |         - |

----------
**Static Constructor Tests**


|             Method |  Count |          Mean |        Error |     StdDev |     StdErr |           Min |            Q1 |        Median |            Q3 |           Max |         Op/s | Ratio | RatioSD | Rank | Allocated |
|------------------- |------- |--------------:|-------------:|-----------:|-----------:|--------------:|--------------:|--------------:|--------------:|--------------:|-------------:|------:|--------:|-----:|----------:|
|   **&#39;No constructor&#39;** |    **100** |      **32.77 ns** |     **0.510 ns** |   **0.477 ns** |   **0.123 ns** |      **31.92 ns** |      **32.44 ns** |      **32.68 ns** |      **32.99 ns** |      **33.65 ns** | **30,515,230.8** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;With constructor&#39; |    100 |     126.27 ns |     1.161 ns |   1.030 ns |   0.275 ns |     124.81 ns |     125.72 ns |     126.16 ns |     126.70 ns |     128.58 ns |  7,919,742.0 |  3.85 |    0.07 |    2 |         - |
|                    |        |               |              |            |            |               |               |               |               |               |              |       |         |      |           |
|   **&#39;No constructor&#39;** |   **1000** |     **250.09 ns** |     **3.900 ns** |   **3.648 ns** |   **0.942 ns** |     **246.62 ns** |     **247.28 ns** |     **248.62 ns** |     **252.67 ns** |     **257.80 ns** |  **3,998,585.5** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;With constructor&#39; |   1000 |   1,240.69 ns |    21.862 ns |  34.675 ns |   6.036 ns |   1,200.62 ns |   1,217.21 ns |   1,226.64 ns |   1,258.69 ns |   1,329.77 ns |    806,005.5 |  5.06 |    0.14 |    2 |         - |
|                    |        |               |              |            |            |               |               |               |               |               |              |       |         |      |           |
|   **&#39;No constructor&#39;** |  **10000** |   **2,447.32 ns** |    **25.048 ns** |  **22.204 ns** |   **5.934 ns** |   **2,416.30 ns** |   **2,429.35 ns** |   **2,445.99 ns** |   **2,464.96 ns** |   **2,482.47 ns** |    **408,609.7** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;With constructor&#39; |  10000 |  12,091.98 ns |    83.886 ns |  74.363 ns |  19.874 ns |  11,920.42 ns |  12,052.54 ns |  12,116.31 ns |  12,140.61 ns |  12,172.37 ns |     82,699.5 |  4.94 |    0.06 |    2 |         - |
|                    |        |               |              |            |            |               |               |               |               |               |              |       |         |      |           |
|   **&#39;No constructor&#39;** | **100000** |  **24,348.32 ns** |   **196.234 ns** | **173.956 ns** |  **46.492 ns** |  **23,947.97 ns** |  **24,288.56 ns** |  **24,367.23 ns** |  **24,452.35 ns** |  **24,600.28 ns** |     **41,070.6** |  **1.00** |    **0.00** |    **1** |         **-** |
| &#39;With constructor&#39; | 100000 | 122,337.54 ns | 1,058.974 ns | 990.565 ns | 255.763 ns | 120,918.95 ns | 121,576.46 ns | 122,283.19 ns | 122,918.75 ns | 124,375.96 ns |      8,174.1 |  5.02 |    0.05 |    2 |         - |

----------
**String Concat Tests**


|             Method |  Count |               Mean |             Error |              StdDev |            StdErr |             Median |                Min |                 Q1 |                 Q3 |                Max |           Op/s | Ratio | Rank |        Gen 0 |        Gen 1 |        Gen 2 |     Allocated |
|------------------- |------- |-------------------:|------------------:|--------------------:|------------------:|-------------------:|-------------------:|-------------------:|-------------------:|-------------------:|---------------:|------:|-----:|-------------:|-------------:|-------------:|--------------:|
| **&#39;char list concat&#39;** |    **100** |           **801.7 ns** |           **9.05 ns** |             **8.47 ns** |           **2.19 ns** |           **803.3 ns** |           **790.0 ns** |           **794.5 ns** |           **808.4 ns** |           **813.7 ns** | **1,247,335.8591** |  **0.26** |    **2** |       **0.6227** |       **0.0038** |            **-** |          **4 KB** |
|      StringBuilder |    100 |           737.6 ns |           9.23 ns |             7.71 ns |           2.14 ns |           738.5 ns |           722.4 ns |           734.5 ns |           740.0 ns |           753.8 ns | 1,355,751.4270 |  0.24 |    1 |       0.3633 |       0.0029 |            - |          2 KB |
|    &#39;string concat&#39; |    100 |         3,036.7 ns |          40.54 ns |            35.94 ns |           9.61 ns |         3,032.3 ns |         2,968.3 ns |         3,018.5 ns |         3,062.3 ns |         3,106.6 ns |   329,301.3121 |  1.00 |    3 |       6.8169 |       0.0343 |            - |         42 KB |
|                    |        |                    |                   |                     |                   |                    |                    |                    |                    |                    |                |       |      |              |              |              |               |
| **&#39;char list concat&#39;** |   **1000** |         **6,834.4 ns** |          **98.34 ns** |           **215.85 ns** |          **28.34 ns** |         **6,780.7 ns** |         **6,519.5 ns** |         **6,713.8 ns** |         **6,923.4 ns** |         **7,567.4 ns** |   **146,318.0404** |  **0.03** |    **2** |       **5.2109** |       **0.3204** |            **-** |         **32 KB** |
|      StringBuilder |   1000 |         6,105.8 ns |         102.14 ns |           242.75 ns |          29.66 ns |         6,042.4 ns |         5,901.9 ns |         5,966.3 ns |         6,133.3 ns |         7,122.0 ns |   163,777.6380 |  0.03 |    1 |       2.6855 |       0.1526 |            - |         16 KB |
|    &#39;string concat&#39; |   1000 |       217,358.0 ns |       4,048.20 ns |         3,786.69 ns |         977.72 ns |       216,830.1 ns |       211,904.0 ns |       215,052.6 ns |       219,287.0 ns |       224,946.8 ns |     4,600.7046 |  1.00 |    3 |     641.6016 |      29.7852 |            - |      3,934 KB |
|                    |        |                    |                   |                     |                   |                    |                    |                    |                    |                    |                |       |      |              |              |              |               |
| **&#39;char list concat&#39;** |  **10000** |       **102,902.0 ns** |         **800.27 ns** |           **709.42 ns** |         **189.60 ns** |       **102,676.5 ns** |       **101,661.3 ns** |       **102,517.8 ns** |       **103,573.3 ns** |       **104,004.3 ns** |     **9,717.9881** | **0.004** |    **2** |      **83.2520** |      **41.6260** |      **41.6260** |        **413 KB** |
|      StringBuilder |  10000 |        67,368.3 ns |       2,861.10 ns |         8,254.94 ns |         842.52 ns |        66,872.7 ns |        56,435.6 ns |        60,443.4 ns |        72,460.6 ns |        92,378.6 ns |    14,843.7782 | 0.003 |    1 |      25.6348 |       8.4839 |            - |        158 KB |
|    &#39;string concat&#39; |  10000 |    23,154,501.4 ns |     454,588.02 ns |       968,764.13 ns |     130,628.13 ns |    22,853,118.8 ns |    21,947,287.5 ns |    22,558,767.2 ns |    23,691,192.2 ns |    26,056,200.0 ns |        43.1881 | 1.000 |    3 |   63375.0000 |    9937.5000 |            - |    390,898 KB |
|                    |        |                    |                   |                     |                   |                    |                    |                    |                    |                    |                |       |      |              |              |              |               |
| **&#39;char list concat&#39;** | **100000** |       **964,097.5 ns** |      **18,124.53 ns** |        **17,800.72 ns** |       **4,450.18 ns** |       **968,823.9 ns** |       **936,612.6 ns** |       **947,185.3 ns** |       **978,820.7 ns** |       **985,947.4 ns** |     **1,037.2395** | **0.000** |    **2** |    **1019.5313** |    **1000.0000** |     **998.0469** |      **3,611 KB** |
|      StringBuilder | 100000 |       779,260.0 ns |      15,560.94 ns |        12,994.09 ns |       3,603.91 ns |       777,503.3 ns |       759,001.7 ns |       769,611.1 ns |       789,385.2 ns |       799,055.7 ns |     1,283.2688 | 0.000 |    1 |     249.0234 |     249.0234 |     249.0234 |      1,567 KB |
|    &#39;string concat&#39; | 100000 | 6,190,556,745.0 ns | 370,598,118.37 ns | 1,092,716,913.58 ns | 109,271,691.36 ns | 6,491,308,900.0 ns | 3,300,983,200.0 ns | 5,275,894,375.0 ns | 7,045,883,125.0 ns | 8,453,799,700.0 ns |         0.1615 | 1.000 |    3 | 8700000.0000 | 8640000.0000 | 8629000.0000 | 39,064,865 KB |

----------
**String Formatting**


|                 Method |       Mean |    Error |   StdDev |  StdErr |     Median |      Min |         Q1 |         Q3 |        Max |        Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|----------------------- |-----------:|---------:|---------:|--------:|-----------:|---------:|-----------:|-----------:|-----------:|------------:|------:|--------:|-----:|-------:|----------:|
|          StringBuilder | 1,021.2 ns | 20.31 ns | 18.00 ns | 4.81 ns | 1,020.3 ns | 987.2 ns | 1,009.7 ns | 1,031.3 ns | 1,056.3 ns |   979,230.9 |  2.11 |    0.05 |    5 | 0.1068 |     680 B |
|        &#39;string concat&#39; |   362.1 ns |  7.18 ns | 13.66 ns | 2.04 ns |   356.5 ns | 349.6 ns |   353.1 ns |   366.3 ns |   409.4 ns | 2,761,791.5 |  0.76 |    0.04 |    1 | 0.0443 |     280 B |
|          string.format |   482.8 ns |  5.68 ns |  5.32 ns | 1.37 ns |   481.7 ns | 475.9 ns |   479.1 ns |   486.0 ns |   495.6 ns | 2,071,049.8 |  1.00 |    0.00 |    2 | 0.0324 |     208 B |
| &#39;string interpolation&#39; |   638.6 ns | 26.69 ns | 77.43 ns | 7.86 ns |   633.1 ns | 476.4 ns |   604.7 ns |   686.7 ns |   808.0 ns | 1,565,905.3 |  1.09 |    0.16 |    4 | 0.0324 |     208 B |
|         string.replace |   505.1 ns |  9.98 ns | 22.74 ns | 2.89 ns |   496.3 ns | 484.6 ns |   489.4 ns |   510.7 ns |   575.9 ns | 1,979,675.2 |  1.12 |    0.04 |    3 | 0.0687 |     432 B |

----------
**String Format Vs String Builder**


|            Method |      Mean |    Error |    StdDev |    Median | Ratio | RatioSD |  Gen 0 | Allocated |
|------------------ |----------:|---------:|----------:|----------:|------:|--------:|-------:|----------:|
|     StringBuilder | 149.24 ns | 7.370 ns | 21.381 ns | 151.22 ns |  0.61 |    0.02 | 0.0739 |     464 B |
| StringBuilderPool |  70.58 ns | 2.142 ns |  5.972 ns |  67.94 ns |  0.43 |    0.02 | 0.0191 |     120 B |
|      StringFormat | 197.08 ns | 2.688 ns |  2.098 ns | 197.47 ns |  1.00 |    0.00 | 0.0279 |     176 B |

----------
**String Replace**


|             Method | Count |           Mean |        Error |       StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|------------------- |------ |---------------:|-------------:|-------------:|------:|--------:|---------:|---------:|---------:|----------:|
| **StringBuilderPools** |     **1** |       **615.8 μs** |     **28.28 μs** |     **83.40 μs** |  **1.00** |    **0.00** | **153.3203** | **153.3203** | **153.3203** |    **512 KB** |
|       StringConcat |     1 |       178.4 μs |      3.48 μs |      3.73 μs |  0.26 |    0.02 |  83.2520 |  83.2520 |  83.2520 |    256 KB |
|                    |       |                |              |              |       |         |          |          |          |           |
| **StringBuilderPools** |    **10** |     **4,002.0 μs** |     **38.13 μs** |     **33.80 μs** |  **1.00** |    **0.00** | **148.4375** | **148.4375** | **148.4375** |    **512 KB** |
|       StringConcat |    10 |       222.3 μs |      4.43 μs |      7.28 μs |  0.06 |    0.00 |  83.2520 |  83.2520 |  83.2520 |    256 KB |
|                    |       |                |              |              |       |         |          |          |          |           |
| **StringBuilderPools** |   **100** |    **39,463.0 μs** |    **788.76 μs** |  **1,025.61 μs** | **1.000** |    **0.00** | **100.0000** | **100.0000** | **100.0000** |    **512 KB** |
|       StringConcat |   100 |       180.7 μs |      3.54 μs |      4.07 μs | 0.005 |    0.00 |  83.2520 |  83.2520 |  83.2520 |    256 KB |
|                    |       |                |              |              |       |         |          |          |          |           |
| **StringBuilderPools** |  **1000** |   **388,446.6 μs** |  **2,353.10 μs** |  **2,085.96 μs** | **1.000** |    **0.00** |        **-** |        **-** |        **-** |    **512 KB** |
|       StringConcat |  1000 |       222.2 μs |      4.43 μs |      9.63 μs | 0.001 |    0.00 |  83.2520 |  83.2520 |  83.2520 |    256 KB |
|                    |       |                |              |              |       |         |          |          |          |           |
| **StringBuilderPools** | **10000** | **3,941,443.4 μs** | **52,848.96 μs** | **41,260.99 μs** | **1.000** |    **0.00** |        **-** |        **-** |        **-** |    **512 KB** |
|       StringConcat | 10000 |       216.6 μs |      3.00 μs |      2.66 μs | 0.000 |    0.00 |  83.2520 |  83.2520 |  83.2520 |    256 KB |

----------
**String Substring**


|           Method |     Mean |    Error |   StdDev |   StdErr |   Median |       Min |       Q1 |       Q3 |      Max |         Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|----------------- |---------:|---------:|---------:|---------:|---------:|----------:|---------:|---------:|---------:|-------------:|------:|--------:|-----:|-------:|----------:|
|       Span.Slice | 17.29 ns | 1.255 ns | 3.701 ns | 0.370 ns | 15.14 ns | 14.248 ns | 14.53 ns | 20.09 ns | 28.21 ns | 57,842,293.8 |  1.49 |    0.31 |    2 | 0.0140 |      88 B |
| String.Substring | 11.84 ns | 0.539 ns | 1.512 ns | 0.158 ns | 11.41 ns |  8.608 ns | 10.83 ns | 12.36 ns | 16.03 ns | 84,425,176.5 |  1.00 |    0.00 |    1 | 0.0064 |      40 B |

----------
**String Trim Tests**


|                       Method |     Mean |    Error |   StdDev |   StdErr |   Median |      Min |       Q1 |       Q3 |      Max |         Op/s | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|----------------------------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|-------------:|------:|--------:|-----:|-------:|----------:|
|      &#39;String substring trim&#39; | 13.82 ns | 0.287 ns | 0.254 ns | 0.068 ns | 13.89 ns | 13.47 ns | 13.59 ns | 14.07 ns | 14.12 ns | 72,350,765.9 |  0.91 |    0.03 |    1 | 0.0089 |      56 B |
|                String.Trim() | 20.23 ns | 1.792 ns | 5.285 ns | 0.528 ns | 17.92 ns | 14.71 ns | 15.83 ns | 24.81 ns | 38.03 ns | 49,436,478.6 |  1.00 |    0.00 |    2 | 0.0089 |      56 B |
| String.TrimStart().TrimEnd() | 37.56 ns | 2.013 ns | 5.809 ns | 0.593 ns | 36.23 ns | 30.16 ns | 32.85 ns | 40.85 ns | 53.64 ns | 26,622,339.7 |  2.01 |    0.60 |    3 | 0.0178 |     112 B |

----------
**Vector Vs Byte Math**


|               Method |  Count |          Mean |         Error |         StdDev |        StdErr |        Median |           Min |            Q1 |            Q3 |           Max |         Op/s | Ratio | RatioSD | Rank |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|--------------------- |------- |--------------:|--------------:|---------------:|--------------:|--------------:|--------------:|--------------:|--------------:|--------------:|-------------:|------:|--------:|-----:|---------:|---------:|---------:|----------:|
|        **&#39;byte[] test&#39;** |    **100** |     **287.49 ns** |      **5.615 ns** |       **6.008 ns** |      **1.416 ns** |     **286.18 ns** |     **280.40 ns** |     **283.19 ns** |     **289.70 ns** |     **299.51 ns** |  **3,478,324.9** |  **1.00** |    **0.00** |    **6** |   **0.0672** |        **-** |        **-** |     **424 B** |
|        &#39;Struct test&#39; |    100 |     188.09 ns |      7.605 ns |      22.303 ns |      2.242 ns |     176.09 ns |     168.80 ns |     171.83 ns |     204.45 ns |     249.11 ns |  5,316,740.6 |  0.77 |    0.04 |    5 |   0.0675 |        - |        - |     424 B |
|   &#39;Struct UInt test&#39; |    100 |      90.79 ns |      1.803 ns |       1.686 ns |      0.435 ns |      90.15 ns |      88.99 ns |      89.64 ns |      91.64 ns |      95.13 ns | 11,013,981.8 |  0.32 |    0.01 |    4 |   0.0675 |        - |        - |     424 B |
|  &#39;Vector&lt;byte&gt; test&#39; |    100 |      67.84 ns |      3.805 ns |      10.856 ns |      1.120 ns |      65.22 ns |      49.83 ns |      62.03 ns |      73.06 ns |      95.96 ns | 14,741,280.9 |  0.29 |    0.03 |    3 |   0.1351 |        - |        - |     848 B |
| &#39;Vector&lt;float&gt; test&#39; |    100 |      44.27 ns |      0.707 ns |       0.662 ns |      0.171 ns |      44.31 ns |      43.27 ns |      43.68 ns |      44.79 ns |      45.38 ns | 22,586,837.8 |  0.15 |    0.00 |    1 |   0.1351 |   0.0001 |        - |     848 B |
|  &#39;Vector&lt;uint&gt; test&#39; |    100 |      55.13 ns |      3.042 ns |       8.873 ns |      0.896 ns |      57.37 ns |      43.32 ns |      45.10 ns |      60.45 ns |      76.53 ns | 18,138,891.5 |  0.18 |    0.02 |    2 |   0.1351 |   0.0001 |        - |     848 B |
|                      |        |               |               |                |               |               |               |               |               |               |              |       |         |      |          |          |          |           |
|        **&#39;byte[] test&#39;** |   **1000** |   **2,867.65 ns** |    **135.720 ns** |     **398.044 ns** |     **40.005 ns** |   **2,911.03 ns** |   **2,163.50 ns** |   **2,770.42 ns** |   **3,115.18 ns** |   **3,641.63 ns** |    **348,717.3** |  **1.00** |    **0.00** |    **4** |   **0.6409** |        **-** |        **-** |   **4,024 B** |
|        &#39;Struct test&#39; |   1000 |   1,643.34 ns |     20.206 ns |      18.901 ns |      4.880 ns |   1,641.17 ns |   1,616.40 ns |   1,631.08 ns |   1,655.60 ns |   1,674.50 ns |    608,518.0 |  0.73 |    0.02 |    3 |   0.6409 |        - |        - |   4,024 B |
|   &#39;Struct UInt test&#39; |   1000 |     662.96 ns |     12.761 ns |      12.533 ns |      3.133 ns |     661.30 ns |     643.36 ns |     654.12 ns |     669.22 ns |     693.29 ns |  1,508,393.7 |  0.30 |    0.01 |    2 |   0.6409 |        - |        - |   4,024 B |
|  &#39;Vector&lt;byte&gt; test&#39; |   1000 |     488.45 ns |     27.879 ns |      81.765 ns |      8.218 ns |     494.48 ns |     387.45 ns |     405.13 ns |     544.30 ns |     746.28 ns |  2,047,283.0 |  0.18 |    0.05 |    1 |   1.2827 |   0.0095 |        - |   8,048 B |
| &#39;Vector&lt;float&gt; test&#39; |   1000 |     472.68 ns |     27.305 ns |      80.508 ns |      8.051 ns |     490.16 ns |     369.46 ns |     389.75 ns |     523.63 ns |     681.33 ns |  2,115,612.6 |  0.17 |    0.04 |    1 |   1.2827 |   0.0095 |        - |   8,048 B |
|  &#39;Vector&lt;uint&gt; test&#39; |   1000 |     481.83 ns |     22.576 ns |      66.565 ns |      6.656 ns |     487.25 ns |     363.99 ns |     439.16 ns |     527.29 ns |     649.40 ns |  2,075,432.0 |  0.17 |    0.04 |    1 |   1.2827 |   0.0095 |        - |   8,048 B |
|                      |        |               |               |                |               |               |               |               |               |               |              |       |         |      |          |          |          |           |
|        **&#39;byte[] test&#39;** |  **10000** |  **27,413.07 ns** |  **1,045.791 ns** |   **3,034.029 ns** |    **308.059 ns** |  **27,637.41 ns** |  **20,938.08 ns** |  **26,574.79 ns** |  **28,966.79 ns** |  **33,562.31 ns** |     **36,479.0** |  **1.00** |    **0.00** |    **5** |   **6.3171** |        **-** |        **-** |  **40,024 B** |
|        &#39;Struct test&#39; |  10000 |  15,160.47 ns |    165.819 ns |     155.107 ns |     40.048 ns |  15,118.60 ns |  14,975.96 ns |  15,044.60 ns |  15,242.26 ns |  15,527.11 ns |     65,961.0 |  0.68 |    0.07 |    4 |   6.3171 |        - |        - |  40,024 B |
|   &#39;Struct UInt test&#39; |  10000 |   5,894.09 ns |    115.242 ns |     145.745 ns |     30.390 ns |   5,833.12 ns |   5,678.86 ns |   5,794.75 ns |   5,954.57 ns |   6,272.69 ns |    169,661.6 |  0.24 |    0.04 |    2 |   6.3248 |        - |        - |  40,024 B |
|  &#39;Vector&lt;byte&gt; test&#39; |  10000 |   3,927.80 ns |    176.057 ns |     490.777 ns |     51.732 ns |   3,727.47 ns |   3,353.76 ns |   3,574.11 ns |   4,216.87 ns |   5,537.29 ns |    254,595.7 |  0.14 |    0.02 |    1 |  12.6572 |        - |        - |  80,048 B |
| &#39;Vector&lt;float&gt; test&#39; |  10000 |  13,235.00 ns |  1,173.470 ns |   3,460.002 ns |    346.000 ns |  13,978.66 ns |   7,334.00 ns |   9,855.84 ns |  16,233.18 ns |  20,473.46 ns |     75,557.2 |  0.50 |    0.15 |    4 |  12.6343 |        - |        - |  80,048 B |
|  &#39;Vector&lt;uint&gt; test&#39; |  10000 |   8,298.92 ns |    613.609 ns |   1,809.241 ns |    180.924 ns |   7,828.34 ns |   5,720.37 ns |   6,830.11 ns |   9,611.42 ns |  12,649.95 ns |    120,497.6 |  0.31 |    0.10 |    3 |  12.6495 |        - |        - |  80,048 B |
|                      |        |               |               |                |               |               |               |               |               |               |              |       |         |      |          |          |          |           |
|        **&#39;byte[] test&#39;** | **100000** | **621,913.71 ns** | **34,472.370 ns** | **100,557.522 ns** | **10,157.844 ns** | **618,224.07 ns** | **438,658.59 ns** | **542,686.69 ns** | **695,798.56 ns** | **864,300.10 ns** |      **1,607.9** |  **1.00** |    **0.00** |    **5** | **124.0234** | **124.0234** | **124.0234** | **400,024 B** |
|        &#39;Struct test&#39; | 100000 | 509,432.10 ns | 22,018.440 ns |  63,175.033 ns |  6,481.622 ns | 491,645.31 ns | 427,414.84 ns | 461,427.15 ns | 537,925.39 ns | 667,493.07 ns |      1,963.0 |  0.84 |    0.20 |    4 | 124.0234 | 124.0234 | 124.0234 | 400,024 B |
|   &#39;Struct UInt test&#39; | 100000 | 403,169.37 ns | 15,520.182 ns |  45,761.605 ns |  4,576.161 ns | 398,249.83 ns | 310,292.63 ns | 373,467.46 ns | 435,169.09 ns | 526,603.71 ns |      2,480.3 |  0.66 |    0.14 |    3 | 124.5117 | 124.5117 | 124.5117 | 400,024 B |
|  &#39;Vector&lt;byte&gt; test&#39; | 100000 | 431,116.78 ns | 38,800.963 ns | 114,405.515 ns | 11,440.552 ns | 424,939.99 ns | 293,917.04 ns | 316,041.16 ns | 526,480.85 ns | 717,953.08 ns |      2,319.6 |  0.71 |    0.21 |    3 | 249.5117 | 249.5117 | 249.5117 | 800,048 B |
| &#39;Vector&lt;float&gt; test&#39; | 100000 | 348,110.99 ns | 20,443.516 ns |  58,656.280 ns |  6,018.007 ns | 324,133.98 ns | 284,980.52 ns | 307,034.45 ns | 362,237.35 ns | 498,055.86 ns |      2,872.6 |  0.58 |    0.16 |    2 | 249.5117 | 249.5117 | 249.5117 | 800,048 B |
|  &#39;Vector&lt;uint&gt; test&#39; | 100000 | 286,964.06 ns |  5,595.130 ns |   6,871.322 ns |  1,464.971 ns | 284,693.70 ns | 279,010.89 ns | 281,912.99 ns | 292,465.49 ns | 304,550.49 ns |      3,484.8 |  0.52 |    0.10 |    1 | 249.5117 | 249.5117 | 249.5117 | 800,048 B |

----------

