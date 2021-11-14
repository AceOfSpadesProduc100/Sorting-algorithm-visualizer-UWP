﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AlgoUWP
{
    // We are initializing a COM interface for use within the namespace
    // This interface allows access to memory at the byte level which we need to populate audio data that is generated
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    class Subarray
    {
        public int start;
        public int end;
        public Subarray(int start, int end)
        {
            this.start = start;
            this.end = end;
        }
    }
    class PDQPair
    {
        public int pivotPosition;
        public bool alreadyPartitioned;

        public PDQPair(int pivotPos, bool presorted)
        {
            pivotPosition = pivotPos;
            alreadyPartitioned = presorted;
        }

        public int GetPivotPosition()
        {
            return pivotPosition;
        }

        public bool GetPresortBool()
        {
            return alreadyPartitioned;
        }
    }
    public sealed partial class MainPage : Page
    {
        //bool for the pause button && extra functionallity
        private bool isPaused;

        //current array of numbers (the one being shown)
        public int[] arr;

        //currently highlighted indexes
        public int[] selectedArr;

        //all sorting steps (arrays of numbers)
        private List<int[]> sortHistory;

        //all highlighted indexes during the sorting steps
        private List<int[]> selectedHistory;

        //timer that we'll use when drawing the array
        private DispatcherTimer timer;

        //for custom arrays
        private bool isPremade;

        //how many comparisons were needed for the sort in total
        public long comparisons;

        //how many array accesses were needed for the sort
        public long arrAccesses;

        public long swaps;

        public long writes;

        public long reversals;

        //Audio variables
        private AudioGraph graph;
        private AudioDeviceOutputNode deviceOutputNode;
        private AudioFrameInputNode frameInputNode;
        public double theta = 0;

        public MainPage()
        {
            InitializeComponent();
            if (timer != null) { timer.Stop(); }
            isPaused = false;
            selectedHistory = new List<int[]>();
            sortHistory = new List<int[]>();
            arr = new int[(int)ArraySize.Value];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = i + 1;
                selectedArr = new int[] { i };
            }
            Array.Sort(arr);
            AddHistorySnap();
            DrawHistory();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await CreateAudioGraph();
        }

        private void ShuffleBtn_Click(object sender, RoutedEventArgs e)
        {
            //disable the shuffle button if the shuffle is set to "none" (option 4)
            //if (shufflecomboBox.SelectedIndex == 4)
            //{
            ShuffleArray();
            //}

            //frameInputNode.Start();
        }

        public void RandomShuffle(int[] array, int start, int end)
        {
            Random random = new();
            for (int i = start; i < end; i++)
            {
                int randomIndex = random.Next(end - i) + i;
                Swap(array, i, randomIndex);
            }
        }
        public void Reversearraycopy(int[] src, int srcPos, int[] dest, int destPos, int length)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                Write(dest, destPos + i, src[srcPos + i]);
            }
        }

        private void SwapBlocksBackwards(int[] array, int a, int b, int len)
        {
            for (int i = 0; i < len; i++)
            {
                Swap(array, a + len - i - 1, b + len - i - 1);
            }
        }

        private void BlockSwap(int[] array, int a, int b, int len)
        {
            for (int i = 0; i < len; i++)
            {
                Swap(array, a + i, b + i);
            }
        }

        private void ShiftForwards(int[] array, int start, int length)
        {
            int temp = array[start];
            for (int i = 0; i < length; i++)
            {
                Write(array, start + i, array[start + i + 1]);
            }
            Write(array, start + length, temp);
        }

        private void ShiftBackwards(int[] array, int start, int length)
        {
            int temp = array[start + length];
            for (int i = length; i > 0; i--)
            {
                Write(array, start + i, array[start + i - 1]);
            }
            Write(array, start, temp);
        }

        public void HolyGriesMills(int[] array, int pos, int lenA, int lenB)
        {
            while (lenA > 1 && lenB > 1)
            {
                while (lenA <= lenB)
                {
                    BlockSwap(array, pos, pos + lenA, lenA);
                    pos += lenA;
                    lenB -= lenA;
                }

                if (lenA <= 1 || lenB <= 1) break;

                while (lenA > lenB)
                {
                    SwapBlocksBackwards(array, pos + lenA - lenB, pos + lenA, lenB);
                    lenA -= lenB;
                }
            }

            if (lenA == 1)
            {
                ShiftForwards(array, pos, lenB);
            }
            else if (lenB == 1)
            {
                ShiftBackwards(array, pos, lenA);
            }
        }

        private void ShuffleArray()
        {
            //create a random starting array, if its not already premade
            if (!isPremade)
            {
                Random random = new();
                if (timer != null) { timer.Stop(); }
                isPaused = false;
                selectedHistory = new List<int[]>();
                sortHistory = new List<int[]>();
                switch (shufflecomboBox.SelectedIndex)
                {
                    case 0: //reset
                        int n = arr.Length - 1;
                        double c = 2 * Math.PI / n;
                        arr = new int[(int)ArraySize.Value];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            arr[i] = i + 1;
                            selectedArr = new int[] { i };
                        }
                        Array.Sort(arr);
                        AddHistorySnap();
                        switch (distribcomboBox.SelectedIndex)
                        {
                            //case 0: linear, default
                            case 1: //few unique
                                int l = 0, r, t = Math.Min(arr.Length, 8);
                                for (int i = 0; i < t; i++)
                                {
                                    if (random.NextDouble() < 0.5)
                                    {
                                        l++;
                                    }
                                    selectedArr = new int[] { i };
                                }

                                r = arr.Length - (t - l);
                                for (int i = 0; i < l; i++)
                                {
                                    arr[i] = (int)(arr.Length * 0.25);
                                    selectedArr = new int[] { i };
                                }

                                for (int i = 0; i < r; i++)
                                {
                                    arr[i] = arr.Length / 2;
                                    selectedArr = new int[] { i };
                                }

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = (int)(arr.Length * 0.75);
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 2: //no unique
                                int val = arr.Length / 2;

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = val;
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 3: //noise
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = random.Next(arr.Length);
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 4: //quadratic curve
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = (int)(Math.Pow(i, 2) / arr.Length);
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 5: //square root curve
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = (int)(Math.Sqrt(i) * Math.Sqrt(arr.Length));
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 6: //cubic curve
                                double midl = (arr.Length - 1) / 2d;

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = (int)((Math.Pow(i - midl, 3) / Math.Pow(midl, 3 - 1)) + midl);
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 7: //quintic curve
                                double midd = (arr.Length - 1) / 2d;

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = (int)((Math.Pow(i - midd, 5) / Math.Pow(midd, 5 - 1)) + midd);
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 8: //cube root curve
                                double h = arr.Length / 2d;

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    double vall = i / h - 1,
                                           root = vall < 0 ? -Math.Pow(-vall, 1d / 3) : Math.Pow(vall, 1d / 3);

                                    arr[i] = (int)(h * (root + 1));
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 9: //fifth root curve
                                double hh = arr.Length / 2d;

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    double vall = i / hh - 1,
                                           root = vall < 0 ? -Math.Pow(-vall, 1d / 5) : Math.Pow(vall, 1d / 5);

                                    arr[i] = (int)(hh * (root + 1));
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 10: //sine

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = (int)(n * (Math.Sin(c * i) + 1) / 2);
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 11: //cosine

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = (int)(n * (Math.Cos(c * i) + 1) / 2);
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 12: //tangent

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = (int)(n * (Math.Tan(c * i) + 1) / 2);
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 13: //perlin noise
                                int[] perlinNoise = new int[arr.Length];

                                float step = 1f / arr.Length;
                                float randomStart = random.Next(arr.Length);
                                int octave = (int)(Math.Log(arr.Length) / Math.Log(2));

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    int value = (int)(PerlinNoise.ReturnFracBrownNoise(randomStart, octave) * arr.Length);
                                    perlinNoise[i] = value;
                                    randomStart += step;
                                    selectedArr = new int[] { i };
                                }

                                int minimum = int.MaxValue;
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (perlinNoise[i] < minimum)
                                    {
                                        minimum = perlinNoise[i];
                                    }
                                    selectedArr = new int[] { i };
                                }
                                minimum = Math.Abs(minimum);
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    perlinNoise[i] += minimum;
                                    selectedArr = new int[] { i };
                                }

                                double maximum = double.MinValue;
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (perlinNoise[i] > maximum)
                                    {
                                        maximum = perlinNoise[i];
                                    }
                                    selectedArr = new int[] { i };
                                }
                                double scale = arr.Length / maximum;
                                if (scale is < 1.0 or > 1.8)
                                {
                                    for (int i = 0; i < arr.Length; i++)
                                    {
                                        perlinNoise[i] = (int)(perlinNoise[i] * scale);
                                        selectedArr = new int[] { i };
                                    }
                                }

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = Math.Min(perlinNoise[i], arr.Length - 1);
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 14: //perlin noise curve
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    int value = 0 - (int)(PerlinNoise.ReturnNoise((float)i / arr.Length) * arr.Length);
                                    arr[i] = Math.Min(value, arr.Length - 1);
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 15: //bell curve
                                double stepp = 8d / arr.Length;
                                double position = -4;
                                int constant = 1264;
                                double factor = arr.Length / 512d;

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    double square = Math.Pow(position, 2);
                                    double negativeSquare = 0 - square;
                                    double halfNegSquare = negativeSquare / 2d;
                                    double numerator = constant * factor * Math.Pow(Math.E, halfNegSquare);

                                    double doublePi = 2 * Math.PI;
                                    double denominator = Math.Sqrt(doublePi);

                                    arr[i] = (int)(numerator / denominator);
                                    position += stepp;
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 16: //ruler
                                int steppp = Math.Max(1, arr.Length / 256);
                                int floorLog2 = (int)(Math.Log(arr.Length / steppp) / Math.Log(2));
                                int lowest;
                                for (lowest = steppp; 2 * lowest <= arr.Length / 4; lowest *= 2) ;
                                bool[] digits = new bool[floorLog2 + 2];

                                int iii, jj;
                                for (iii = 0; iii + steppp <= arr.Length; iii += steppp)
                                {
                                    for (jj = 0; digits[jj]; jj++) ;
                                    digits[jj] = true;

                                    for (int kk = 0; kk < steppp; kk++)
                                    {
                                        int value = arr.Length / 2 - Math.Min((1 << jj) * steppp, lowest);
                                        arr[iii + kk] = value;
                                    }

                                    for (int kk = 0; kk < jj; kk++) digits[kk] = false;
                                    selectedArr = new int[] { iii };
                                }

                                for (jj = 0; digits[jj]; jj++) ;
                                digits[jj] = true;
                                while (iii < arr.Length)
                                {
                                    int value = Math.Max(arr.Length / 2 - ((1 << jj) * steppp), arr.Length / 4);
                                    arr[iii++] = value;
                                    selectedArr = new int[] { iii };
                                }
                                AddHistorySnap();
                                break;
                            case 17: //blancmange curve
                                int floorLog22 = (int)(Math.Log(arr.Length) / Math.Log(2));

                                for (int i = 0; i < arr.Length; i++)
                                {
                                    int value = (int)(arr.Length * curveSum(floorLog22, (double)i / arr.Length));
                                    arr[i] = value;
                                    selectedArr = new int[] { i };
                                }
                                double curveSum(int n, double x)
                                {
                                    double sum = 0;
                                    while (n >= 0)
                                    {
                                        sum += curve(n--, x);
                                    }

                                    return sum;
                                }

                                double curve(int n, double x)
                                {
                                    return triangleWave((1 << n) * x) / (1 << n);
                                }

                                double triangleWave(double x)
                                {
                                    return Math.Abs(x - (int)(x + 0.5));
                                }
                                AddHistorySnap();
                                break;
                            case 18: //cantor function
                                cantor(arr, 0, arr.Length, 0, arr.Length - 1);
                                void cantor(int[] array, int a, int b, int min, int max)
                                {
                                    if (b - a < 1 || max == min)
                                    {
                                        return;
                                    }

                                    int mid = (min + max) / 2;
                                    if (b - a == 1)
                                    {
                                        array[a] = mid;
                                        return;
                                    }

                                    int t1 = (a + a + b) / 3, t2 = (a + b + b + 2) / 3;

                                    for (int i = t1; i < t2; i++)
                                    {
                                        array[i] = mid;
                                        selectedArr = new int[] { i };
                                    }

                                    cantor(array, a, t1, min, mid);
                                    cantor(array, t2, b, mid + 1, max);
                                }
                                AddHistorySnap();
                                break;
                            case 19: //sum of divisors
                                int[] nn = new int[arr.Length];

                                nn[0] = 0;
                                nn[1] = 1;
                                double maxq = 1;

                                for (int i = 2; i < arr.Length; i++)
                                {
                                    nn[i] = sumDivisors(i);
                                    if (nn[i] > maxq) maxq = nn[i];
                                    selectedArr = new int[] { i };
                                }

                                double scalee = Math.Min((arr.Length - 1) / maxq, 1);
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = (int)(nn[i] * scalee);
                                    selectedArr = new int[] { i };
                                }
                                int sumDivisors(int n)
                                {
                                    int sum = n + 1;
                                    for (int i = 2; i <= (int)Math.Sqrt(n); i++)
                                    {
                                        if (n % i == 0)
                                        {
                                            if (i == n / i)
                                            {
                                                sum += i;
                                            }
                                            else
                                            {
                                                sum += i + n / i;
                                            }
                                        }
                                        selectedArr = new int[] { i };
                                    }
                                    return sum;
                                }
                                AddHistorySnap();
                                break;
                            case 20: //oeis fly straight
                                int[] fsd = new int[arr.Length];

                                double maxx;
                                maxx = fsd[0] = fsd[1] = 1;
                                for (int i = 2; i < arr.Length; i++)
                                {
                                    int g = gcd(fsd[i - 1], i);
                                    fsd[i] = fsd[i - 1] / g + (g == 1 ? i + 1 : 0);
                                    if (fsd[i] > maxx)
                                    {
                                        maxx = fsd[i];
                                    }
                                    selectedArr = new int[] { i };
                                }

                                double scalew = Math.Min((arr.Length - 1) / maxx, 1);
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = (int)(fsd[i] * scalew);
                                    selectedArr = new int[] { i };
                                }
                                int gcd(int a, int b)
                                {
                                    return b == 0 ? a : gcd(b, a % b);
                                }
                                AddHistorySnap();
                                break;
                            case 21: //decreasing random
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    int rr = random.Next(arr.Length - i) + i;
                                    arr[i] = rr;
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            case 22: //modulo function
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = 2 * (arr.Length % (i + 1));
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                            default: //case 0
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    arr[i] = i + 1;
                                    selectedArr = new int[] { i };
                                }
                                AddHistorySnap();
                                break;
                        }

                        break;
                    case 1: //random
                        for (int i = 0; i < arr.Length; i++)
                        {
                            int oldArrItem = arr[i];
                            int switchIndex = random.Next(i, arr.Length);
                            arr[i] = arr[switchIndex];
                            arr[switchIndex] = oldArrItem;
                            selectedArr = new int[] { i };
                        }
                        AddHistorySnap();
                        break;
                    case 2: //reversed
                        Array.Reverse(arr);
                        AddHistorySnap();
                        break;
                    case 3: //slight shuffle
                        for (int i = 0; i < Math.Max(arr.Length / 20, 1); i++)
                        {
                            Swap(arr, random.Next(arr.Length), random.Next(arr.Length));
                        }
                        AddHistorySnap();
                        break;
                    case 4: //sorted
                        Array.Sort(arr);
                        AddHistorySnap();
                        break;
                    case 5: //reverse sorted
                        Array.Sort(arr);
                        Array.Reverse(arr);
                        AddHistorySnap();
                        break;
                    case 6: //scrambled tail
                        int[] aux = new int[arr.Length];
                        int ii = 0, jk = 0, k = 0;
                        while (ii < arr.Length)
                        {
                            if (random.NextDouble() < 1 / 7d)
                                Write(aux, k++, arr[ii++]);
                            else
                                Write(arr, jk++, arr[ii++]);
                        }
                        Array.Copy(aux, 0, arr, jk, k);
                        RandomShuffle(arr, jk, arr.Length);
                        AddHistorySnap();
                        break;
                    case 7: //scrambled head
                        int[] auxx = new int[arr.Length];
                        int il = arr.Length - 1, jl = arr.Length - 1, kl = 0;
                        while (il >= 0)
                        {
                            if (random.NextDouble() < 1 / 7d)
                                Write(auxx, kl++, arr[il--]);
                            else
                                Write(arr, jl--, arr[il--]);
                        }
                        Reversearraycopy(auxx, 0, arr, 0, kl);
                        RandomShuffle(arr, 0, jl);
                        AddHistorySnap();
                        break;
                    case 8: //shifted element
                        int start = random.Next(arr.Length);
                        int dest = random.Next(arr.Length);
                        if (dest < start)
                        {
                            HolyGriesMills(arr, dest, start, start + 1);
                        }
                        else
                        {
                            HolyGriesMills(arr, start, start + 1, dest);
                        }
                        AddHistorySnap();
                        break;
                    case 9: //noisy
                        int ik, size = Math.Max(4, (int)(Math.Sqrt(arr.Length) / 2));
                        for (ik = 0; ik + size <= arr.Length; ik += random.Next(size - 1) + 1)
                            RandomShuffle(arr, ik, ik + size);
                        RandomShuffle(arr, ik, arr.Length);
                        AddHistorySnap();
                        break;
                    case 10: //scrambled odds
                        for (int i = 1; i < arr.Length; i += 2)
                        {
                            int randomIndex = (((random.Next(arr.Length - i) / 2)) * 2) + i;
                            Swap(arr, i, randomIndex);
                        }
                        AddHistorySnap();
                        break;
                    case 11: //final merge pass
                        int count = 2;

                        int kp = 0;
                        int[] temp = new int[arr.Length];

                        for (int jp = 0; jp < count; jp++)
                            for (int i = jp; i < arr.Length; i += count)
                                Write(temp, kp++, arr[i]);

                        for (int i = 0; i < arr.Length; i++)
                            Write(arr, i, temp[i]);
                        AddHistorySnap();
                        break;
                    case 12: //Shuffled final merge pass
                        RandomShuffle(arr, 0, arr.Length);
                        Array.Sort(arr, 0, arr.Length / 2);
                        Array.Sort(arr, arr.Length / 2, arr.Length);
                        AddHistorySnap();
                        break;
                    case 13: //shuffled second half
                        RandomShuffle(arr, 0, arr.Length);
                        Array.Sort(arr, 0, arr.Length / 2);
                        AddHistorySnap();
                        break;
                    case 14: //partitioned
                        Array.Sort(arr, 0, arr.Length);
                        RandomShuffle(arr, 0, arr.Length / 2);
                        RandomShuffle(arr, arr.Length / 2, arr.Length);
                        AddHistorySnap();
                        break;
                    case 15: //sawtooth
                        int countt = 4;

                        int kr = 0;
                        int[] tempp = new int[arr.Length];

                        for (int j = 0; j < countt; j++)
                            for (int i = j; i < arr.Length; i += countt)
                                Write(tempp, kr++, arr[i]);

                        for (int i = 0; i < arr.Length; i++)
                            Write(arr, i, tempp[i]);
                        AddHistorySnap();
                        break;
                    case 16: //pipe organ
                        int[] tempr = new int[arr.Length];

                        for (int i = 0, j = 0; i < arr.Length; i += 2)
                        {
                            tempr[j++] = arr[i];
                        }
                        for (int i = 1, j = arr.Length; i < arr.Length; i += 2)
                        {
                            tempr[--j] = arr[i];
                        }
                        for (int i = 0; i < arr.Length; i++)
                        {
                            Write(arr, i, tempr[i]);
                        }
                        AddHistorySnap();
                        break;
                    case 17: //inverted pipe organ
                        int[] tempe = new int[arr.Length];

                        Array.Reverse(arr, 0, arr.Length - 1);
                        for (int i = 0, j = 0; i < arr.Length; i += 2)
                        {
                            tempe[j++] = arr[i];
                        }
                        for (int i = 1, j = arr.Length; i < arr.Length; i += 2)
                        {
                            tempe[--j] = arr[i];
                        }
                        for (int i = 0; i < arr.Length; i++)
                        {
                            Write(arr, i, tempe[i]);
                        }
                        AddHistorySnap();
                        break;
                    case 18: //interlaced
                        int[] referenceArray = new int[arr.Length];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            referenceArray[i] = arr[i];
                        }

                        int leftIndex = 1;
                        int rightIndex = arr.Length - 1;

                        for (int i = 1; i < arr.Length; i++)
                        {
                            if (i % 2 == 0)
                            {
                                Write(arr, i, referenceArray[leftIndex++]);
                            }
                            else
                            {
                                Write(arr, i, referenceArray[rightIndex--]);
                            }
                        }
                        AddHistorySnap();
                        break;
                    case 19: //double layered
                        for (int i = 0; i < arr.Length / 2; i += 2)
                        {
                            Swap(arr, i, arr.Length - i - 1);
                        }
                        AddHistorySnap();
                        break;
                    case 20: //final radix pass
                        int currentL = arr.Length;

                        currentL -= currentL % 2;
                        int mid = currentL / 2;
                        int[] tempw = new int[mid];

                        for (int i = 0; i < mid; i++)
                            Write(tempw, i, arr[i]);

                        for (int i = mid, j = 0; i < currentL; i++, j += 2)
                        {
                            Write(arr, j, arr[i]);
                            Write(arr, j + 1, tempw[i - mid]);
                        }
                        AddHistorySnap();
                        break;
                    case 21: //recursive final radix pass
                        weaveRec(arr, 0, arr.Length, 1);

                        void weaveRec(int[] array, int pos, int length, int gap)
                        {
                            if (length < 2) return;

                            int mod2 = length % 2;
                            length -= mod2;
                            int mid = length / 2;
                            int[] temp = new int[mid];

                            for (int i = pos, j = 0; i < pos + gap * mid; i += gap, j++)
                                Write(temp, j, array[i]);

                            for (int i = pos + gap * mid, j = pos, k = 0; i < pos + gap * length; i += gap, j += 2 * gap, k++)
                            {
                                Write(array, j, array[i]);
                                Write(array, j + gap, temp[k]);
                            }

                            weaveRec(array, pos, mid + mod2, 2 * gap);
                            weaveRec(array, pos + gap, mid, 2 * gap);
                        }
                        AddHistorySnap();
                        break;
                    case 22: //half rotation
                        int a = 0, m = (arr.Length + 1) / 2;

                        if (arr.Length % 2 == 0)
                            while (m < arr.Length) Swap(arr, a++, m++);

                        else
                        {
                            int tempf = arr[a];
                            while (m < arr.Length)
                            {
                                Write(arr, a++, arr[m]);
                                Write(arr, m++, arr[a]);
                            }
                            Write(arr, a, tempf);
                        }
                        AddHistorySnap();
                        break;
                    case 23: //half reverse
                        Array.Reverse(arr, 0, arr.Length - 1);
                        Array.Reverse(arr, arr.Length / 4, (3 * arr.Length + 3) / 4 - 1);
                        AddHistorySnap();
                        break;
                    case 24: //binary search tree traversal
                        int len = arr.Length;
                        int[] tempt = new int[len];
                        Array.Copy(arr, tempt, arr.Length);

                        // credit to sam walko/anon

                        Queue<Subarray> q = new();
                        q.Enqueue(new Subarray(0, arr.Length));
                        int iw = 0;

                        while (q.Count != 0)
                        {
                            Subarray sub = q.Peek();
                            if (sub.start != sub.end)
                            {
                                int midl = (sub.start + sub.end) / 2;
                                Write(arr, iw, tempt[midl]);
                                iw++;
                                q.Enqueue(new Subarray(sub.start, midl));
                                q.Enqueue(new Subarray(midl + 1, sub.end));
                            }
                        }
                        AddHistorySnap();
                        break;
                    case 25: //inverted binary search tree
                        int[] tempq = new int[arr.Length];

                        // credit to sam walko/anon
                        Queue<Subarray> qq = new();
                        qq.Enqueue(new Subarray(0, arr.Length));
                        int ie = 0;

                        while (qq.Count != 0)
                        {
                            Subarray sub = qq.Peek();
                            if (sub.start != sub.end)
                            {
                                int midw = (sub.start + sub.end) / 2;
                                Write(tempq, ie, midw);
                                ie++;
                                qq.Enqueue(new Subarray(sub.start, midw));
                                qq.Enqueue(new Subarray(midw + 1, sub.end));
                            }
                        }
                        int lenn = arr.Length;
                        int[] temp2 = new int[lenn];
                        Array.Copy(arr, temp2, arr.Length);
                        for (ie = 0; ie < arr.Length; ie++)
                            Write(arr, tempq[ie], temp2[ie]);
                        AddHistorySnap();
                        break;
                    case 26: //logarithmic slopes
                        int[] tempg = new int[arr.Length];
                        for (int i = 0; i < arr.Length; i++)
                            Write(tempg, i, arr[i]);

                        Write(arr, 0, 0);
                        for (int i = 1; i < arr.Length; i++)
                        {
                            int log = (int)(Math.Log(i) / Math.Log(2));
                            int power = (int)Math.Pow(2, log);
                            int value = tempg[2 * (i - power) + 1];
                            Write(arr, i, value);
                        }
                        AddHistorySnap();
                        break;
                    case 27: //max heapified
                        for (int i = arr.Length / 2 - 1; i >= 0; i--)
                        {
                            Heapify(i, arr.Length);
                        }
                        selectedArr = new int[] { arr.Length - 1 };

                        void Heapify(int i, int topI)
                        {
                            int maxI = i;
                            int leftChildI = i * 2 + 1;
                            int rightChildI = i * 2 + 2;

                            comparisons++;
                            if (leftChildI < topI)
                            {
                                arrAccesses += 2;
                                comparisons++;
                                if (arr[leftChildI] > arr[maxI])
                                {
                                    maxI = leftChildI;
                                }
                            }

                            comparisons++;
                            if (rightChildI < topI)
                            {
                                arrAccesses += 2;
                                comparisons++;
                                if (arr[rightChildI] > arr[maxI])
                                {
                                    maxI = rightChildI;
                                }
                            }

                            comparisons++;
                            if (maxI != i)
                            {
                                int oldI = arr[i];
                                arr[i] = arr[maxI];
                                arr[maxI] = oldI;

                                arrAccesses += 4;

                                selectedArr = new int[] { i };
                                Heapify(maxI, topI);
                            }
                        }
                        AddHistorySnap();
                        break;
                    case 28: //min heapified
                        break;
                    case 29: //smooth heapified
                        break;
                    case 30: //poplar heapified
                        break;
                    case 31: //triangular heapified
                        break;
                    case 32: //first circle run
                        RandomShuffle(arr, 0, arr.Length);

                        int nw = 1;
                        for (; nw < arr.Length; nw *= 2) ;

                        circleSortRoutine(arr, 0, nw - 1, arr.Length);
                        void circleSortRoutine(int[] array, int lo, int hi, int end)
                        {
                            if (lo == hi) return;

                            int high = hi;
                            int low = lo;
                            int mid = (hi - lo) / 2;

                            while (lo < hi)
                            {
                                if (hi < end && lo > hi)
                                    Swap(array, lo, hi);

                                lo++;
                                hi--;
                            }

                            circleSortRoutine(array, low, low + mid, end);
                            if (low + mid + 1 < end) circleSortRoutine(array, low + mid + 1, high, end);
                        }
                        AddHistorySnap();
                        break;
                    case 33: //final pairwise pass
                        RandomShuffle(arr, 0, arr.Length);

                        //create pairs
                        for (int i = 1; i < arr.Length; i += 2)
                            if (CompareValues(i - 1, i) > 0)
                                Swap(arr, i - 1, i);

                        int[] temps = new int[arr.Length];

                        //sort the smaller and larger of the pairs separately with pigeonhole sort
                        for (int mq = 0; mq < 2; mq++)
                        {
                            for (int kd = mq; kd < arr.Length; kd += 2)
                                Write(temps, arr[kd], temps[arr[kd]] + 1);

                            int i = 0, j = mq;
                            while (true)
                            {
                                while (i < arr.Length && temps[i] == 0) i++;
                                if (i >= arr.Length) break;

                                Write(arr, j, i);

                                j += 2;
                                Write(temps, i, temps[i] - 1);
                            }
                        }
                        AddHistorySnap();
                        break;
                    case 34: //recursive reversal
                        reversalRec(arr, 0, arr.Length);
                        void reversalRec(int[] array, int a, int b)
                        {
                            if (b - a < 2) return;

                            Array.Reverse(array, a, b - 1);

                            int m = (a + b) / 2;
                            reversalRec(array, a, m);
                            reversalRec(array, m, b);
                        }
                        AddHistorySnap();
                        break;
                    case 35: //gray code fractal
                        reversalRec2(arr, 0, arr.Length, false);
                        void reversalRec2(int[] array, int a, int b, bool bw)
                        {
                            if (b - a < 3) return;

                            int m = (a + b) / 2;

                            if (bw) Array.Reverse(array, a, m - 1);
                            else Array.Reverse(array, m, b - 1);

                            reversalRec2(array, a, m, false);
                            reversalRec2(array, m, b, true);
                        }
                        AddHistorySnap();
                        break;
                    case 36: //sierpinski triangle
                        int[] triangle = new int[arr.Length];
                        triangleRec(triangle, 0, arr.Length);

                        int[] temph = new int[arr.Length];
                        Array.Copy(arr, temph, arr.Length);
                        for (int i = 0; i < arr.Length; i++)
                            Write(arr, i, temph[triangle[i]]);

                        void triangleRec(int[] array, int a, int b)
                        {
                            if (b - a < 2) return;
                            if (b - a == 2)
                            {
                                array[a + 1]++;
                                return;
                            }

                            int h = (b - a) / 3, t1 = (a + a + b) / 3, t2 = (a + b + b + 2) / 3;
                            for (int i = a; i < t1; i++) array[i] += h;
                            for (int i = t1; i < t2; i++) array[i] += 2 * h;

                            triangleRec(array, a, t1);
                            triangleRec(array, t1, t2);
                            triangleRec(array, t2, b);
                        }
                        AddHistorySnap();
                        break;
                    case 37: //triangular input
                        int[] triangleq = new int[arr.Length];

                        int jq = 0, kq = 2;
                        int max = 0;

                        for (int i = 1; i < arr.Length; i++, jq++)
                        {
                            if (i == kq)
                            {
                                jq = 0;
                                kq *= 2;
                            }
                            triangleq[i] = triangleq[jq] + 1;
                            if (triangleq[i] > max) max = triangleq[i];
                        }
                        int[] cnt = new int[max + 1];

                        for (int i = 0; i < arr.Length; i++)
                            cnt[triangleq[i]]++;

                        for (int i = 1; i < cnt.Length; i++)
                            cnt[i] += cnt[i - 1];

                        for (int i = arr.Length - 1; i >= 0; i--)
                            triangleq[i] = --cnt[triangleq[i]];

                        int[] tempc = new int[arr.Length];
                        Array.Copy(arr, tempc, arr.Length);
                        for (int i = 0; i < arr.Length; i++)
                            Write(arr, i, tempc[triangleq[i]]);
                        AddHistorySnap();
                        break;
                    case 38: //quicksort adversary
                        for (int j = arr.Length - arr.Length % 2 - 2, i = j - 1; i >= 0; i -= 2, j--)
                            Swap(arr, i, j);
                        AddHistorySnap();
                        break;
                    case 39: //pdq adversary
                        int[] copy = new int[arr.Length];
                        int[] tempm;
                        bool hasCandidate;
                        int gas, frozen, candidate;

                        hasCandidate = false;
                        frozen = 1;
                        temp = new int[arr.Length];
                        gas = arr.Length;
                        for (int i = 0; i < arr.Length; i++)
                        {
                            Write(copy, i, arr[i]);
                            Write(arr, i, i);
                            Write(temp, i, gas);
                        }

                        //pdqLoop(arr, 0, arr.Length, false, pdqLog(arr.Length));

                        for (int i = 0; i < arr.Length; i++)
                        {
                            Write(arr, i, copy[temp[i] - 1]);
                        }

                        int compare(int ap, int bp)
                        {
                            int a, b;
                            if (!hasCandidate)
                            {
                                candidate = 0;
                                hasCandidate = true;
                            }

                            a = ap;
                            b = bp;

                            if (temp[a] == gas && temp[b] == gas)
                                if (a == candidate)
                                    temp[a] = frozen++;
                                else
                                    temp[b] = frozen++;

                            if (temp[a] == gas)
                            {
                                candidate = a;
                                return 1;
                            }

                            if (temp[b] == gas)
                            {
                                candidate = b;
                                return -1;
                            }

                            if (temp[a] < temp[b])
                                return -1;
                            if (temp[a] > temp[b])
                                return 1;
                            return 0;
                        }

                        void pdqLoop(int[] array, int begin, int end, bool Branchless, int badAllowed)
                        {
                            bool leftmost = true;

                            while (true)
                            {
                                int size = end - begin;

                                if (size < 24)
                                {
                                    if (leftmost) pdqInsertSort(array, begin, end);
                                    else pdqUnguardInsertSort(array, begin, end);
                                    return;
                                }

                                int halfSize = size / 2;
                                if (size > 128)
                                {
                                    pdqSortThree(array, begin, begin + halfSize, end - 1);
                                    pdqSortThree(array, begin + 1, begin + (halfSize - 1), end - 2);
                                    pdqSortThree(array, begin + 2, begin + (halfSize + 1), end - 3);
                                    pdqSortThree(array, begin + (halfSize - 1), begin + halfSize, begin + (halfSize + 1));
                                    Swap(array, begin, begin + halfSize);
                                }
                                else pdqSortThree(array, begin + halfSize, begin, end - 1);

                                if (!leftmost && !(compare(array[begin - 1], array[begin]) < 0))
                                {
                                    begin = pdqPartLeft(array, begin, end) + 1;
                                    continue;
                                }

                                PDQPair partResult = pdqPartRight(array, begin, end);

                                int pivotPos = partResult.GetPivotPosition();
                                bool alreadyParted = partResult.GetPresortBool();

                                int leftSize = pivotPos - begin;
                                int rightSize = end - (pivotPos + 1);
                                bool highUnbalance = leftSize < size / 8 || rightSize < size / 8;

                                if (highUnbalance)
                                {
                                    if (--badAllowed == 0)
                                    {
                                        int length = end - begin;
                                        for (int i = length / 2; i >= 1; i--)
                                        {
                                            //siftDown(array, i, length, begin);
                                        }
                                        return;
                                    }

                                    if (leftSize >= 24)
                                    {
                                        Swap(array, begin, begin + leftSize / 4);
                                        Swap(array, pivotPos - 1, pivotPos - leftSize / 4);

                                        if (leftSize > 128)
                                        {
                                            Swap(array, begin + 1, begin + (leftSize / 4 + 1));
                                            Swap(array, begin + 2, begin + (leftSize / 4 + 2));
                                            Swap(array, pivotPos - 2, pivotPos - (leftSize / 4 + 1));
                                            Swap(array, pivotPos - 3, pivotPos - (leftSize / 4 + 2));
                                        }
                                    }

                                    if (rightSize >= 24)
                                    {
                                        Swap(array, pivotPos + 1, pivotPos + (1 + rightSize / 4));
                                        Swap(array, end - 1, end - rightSize / 4);

                                        if (rightSize > 128)
                                        {
                                            Swap(array, pivotPos + 2, pivotPos + (2 + rightSize / 4));
                                            Swap(array, pivotPos + 3, pivotPos + (3 + rightSize / 4));
                                            Swap(array, end - 2, end - (1 + rightSize / 4));
                                            Swap(array, end - 3, end - (2 + rightSize / 4));
                                        }
                                    }
                                }
                                else
                                {
                                    if (alreadyParted && pdqPartialInsertSort(array, begin, pivotPos)
                                                      && pdqPartialInsertSort(array, pivotPos + 1, end))
                                        return;
                                }

                                pdqLoop(array, begin, pivotPos, Branchless, badAllowed);
                                begin = pivotPos + 1;
                                leftmost = false;
                            }
                        }

                        void siftDown(int[] array, int root, int dist, int start, bool isMax)
                        {
                            int compareVal = 0;

                            if (isMax) compareVal = -1;
                            else compareVal = 1;

                            while (root <= dist / 2)
                            {
                                int leaf = 2 * root;
                                if (leaf < dist && compare(array[start + leaf - 1], array[start + leaf]) == compareVal)
                                {
                                    leaf++;
                                }
                                if (compare(array[start + root - 1], array[start + leaf - 1]) == compareVal)
                                {
                                    Swap(array, start + root - 1, start + leaf - 1);
                                    root = leaf;
                                }
                                else break;
                            }
                        }

                        PDQPair pdqPartRight(int[] array, int begin, int end)
                        {
                            int pivot = array[begin];
                            int first = begin;
                            int last = end;

                            while (compare(array[++first], pivot) < 0)
                            {

                            }

                            if (first - 1 == begin)
                                while (first < last && !(compare(array[--last], pivot) < 0))
                                {

                                }
                            else
                                while (!(compare(array[--last], pivot) < 0))
                                {

                                }

                            bool alreadyParted = first >= last;

                            while (first < last)
                            {
                                Swap(array, first, last);
                                while (compare(array[++first], pivot) < 0)
                                {

                                }
                                while (!(compare(array[--last], pivot) < 0))
                                {

                                }
                            }


                            int pivotPos = first - 1;
                            Write(array, begin, array[pivotPos]);
                            Write(array, pivotPos, pivot);

                            return new PDQPair(pivotPos, alreadyParted);
                        }

                        bool pdqPartialInsertSort(int[] array, int begin, int end)
                        {
                            if (begin == end) return true;

                            int limit = 0;
                            for (int cur = begin + 1; cur != end; ++cur)
                            {
                                if (limit > 8) return false;

                                int sift = cur;
                                int siftMinusOne = cur - 1;

                                if (compare(array[sift], array[siftMinusOne]) < 0)
                                {
                                    int tmp = array[sift];

                                    do
                                    {
                                        Write(array, sift--, array[siftMinusOne]);
                                    } while (sift != begin && compare(tmp, array[--siftMinusOne]) < 0);

                                    Write(array, sift, tmp);
                                    limit += cur - sift;
                                }
                            }
                            return true;
                        }

                        int pdqPartLeft(int[] array, int begin, int end)
                        {
                            int pivot = array[begin];
                            int first = begin;
                            int last = end;

                            while (compare(pivot, array[--last]) < 0)
                            {

                            }

                            if (last + 1 == end)
                                while (first < last && !(compare(pivot, array[++first]) < 0))
                                {

                                }
                            else
                                while (!(compare(pivot, array[++first]) < 0))
                                {

                                }

                            while (first < last)
                            {
                                Swap(array, first, last);
                                while (compare(pivot, array[--last]) < 0)
                                {

                                }
                                while (!(compare(pivot, array[++first]) < 0))
                                {

                                }
                            }


                            int pivotPos = last;
                            Write(array, begin, array[pivotPos]);
                            Write(array, pivotPos, pivot);

                            return pivotPos;
                        }

                        void pdqSortThree(int[] array, int a, int b, int c)
                        {
                            pdqSortTwo(array, a, b);
                            pdqSortTwo(array, b, c);
                            pdqSortTwo(array, a, b);
                        }

                        void pdqSortTwo(int[] array, int a, int b)
                        {
                            if (compare(array[b], array[a]) < 0)
                            {
                                Swap(array, a, b);
                            }
                        }

                        void pdqInsertSort(int[] array, int begin, int end)
                        {
                            if (begin == end) return;

                            for (int cur = begin + 1; cur != end; ++cur)
                            {
                                int sift = cur;
                                int siftMinusOne = cur - 1;

                                if (compare(array[sift], array[siftMinusOne]) < 0)
                                {
                                    int tmp = array[sift];
                                    do
                                    {
                                        Write(array, sift--, array[siftMinusOne]);
                                    } while (sift != begin && compare(tmp, array[--siftMinusOne]) < 0);

                                    Write(array, sift, tmp);
                                }
                            }
                        }

                        void pdqUnguardInsertSort(int[] array, int begin, int end)
                        {
                            if (begin == end) return;

                            for (int cur = begin + 1; cur != end; ++cur)
                            {
                                int sift = cur;
                                int siftMinusOne = cur - 1;

                                if (compare(array[sift], array[siftMinusOne]) < 0)
                                {
                                    int tmp = array[sift];

                                    do
                                    {
                                        Write(array, sift--, array[siftMinusOne]);
                                    } while (compare(tmp, array[--siftMinusOne]) < 0);

                                    Write(array, sift, tmp);
                                }
                            }
                        }
                        AddHistorySnap();
                        break;
                    case 40: //grailsort adversary
                        if (arr.Length <= 16) Array.Reverse(arr, 0, arr.Length - 1);
                        else
                        {
                            int blockLen = 1;
                            while (blockLen * blockLen < arr.Length) blockLen *= 2;

                            int numKeys = (arr.Length - 1) / blockLen + 1;
                            int keys = blockLen + numKeys;

                            RandomShuffle(arr, 0, arr.Length);
                            Array.Sort(arr, 0, keys);
                            Array.Reverse(arr, 0, keys - 1);
                            Array.Sort(arr, keys, arr.Length);

                            push(arr, keys, arr.Length, blockLen);
                        }
                        void rotate(int[] array, int a, int m, int b)
                        {
                            Array.Reverse(array, a, m - 1);
                            Array.Reverse(array, m, b - 1);
                            Array.Reverse(array, a, b - 1);
                        }

                        void push(int[] array, int a, int b, int bLen)
                        {
                            int len = b - a,
                                b1 = b - len % bLen, len1 = b1 - a;
                            if (len1 <= 2 * bLen) return;

                            int m = bLen;
                            while (2 * m < len) m *= 2;
                            m += a;

                            if (b1 - m < bLen) push(array, a, m, bLen);
                            else
                            {
                                m = a + b1 - m;
                                rotate(array, m - (bLen - 2), b1 - (bLen - 1), b1);
                                MultiSwap(array, a, m);
                                rotate(array, a, m, b1);
                                m = a + b1 - m;

                                push(array, a, m, bLen);
                                push(array, m, b, bLen);
                            }
                        }
                        AddHistorySnap();
                        break;
                    case 41: //shuffle merge adversary
                        int nq = arr.Length;

                        int[] tmp = new int[nq];
                        int d = 2, end = 1 << (int)(Math.Log(nq - 1) / Math.Log(2) + 1);

                        while (d <= end)
                        {
                            int i = 0, dec = 0;
                            double sleep = 1d / d;

                            while (i < nq)
                            {
                                int j = i;
                                dec += nq;
                                while (dec >= d)
                                {
                                    dec -= d;
                                    j++;
                                }
                                int kf = j;
                                dec += nq;
                                while (dec >= d)
                                {
                                    dec -= d;
                                    kf++;
                                }
                                shuffleMergeBad(arr, tmp, i, j, kf);
                                i = kf;
                            }
                            d *= 2;
                        }
                        void shuffleMergeBad(int[] array, int[] tmp, int a, int m, int b)
                        {
                            if ((b - a) % 2 == 1)
                            {
                                if (m - a > b - m) a++;
                                else b--;
                            }
                            shuffleBad(array, tmp, a, b);
                        }

                        //length is always even
                        void shuffleBad(int[] array, int[] tmp, int a, int b)
                        {
                            if (b - a < 2) return;

                            int m = (a + b) / 2;
                            int s = (b - a - 1) / 4 + 1;

                            a = m - s;
                            b = m + s;
                            int j = a;

                            for (int i = a + 1; i < b; i += 2)
                                Write(tmp, j++, array[i]);
                            for (int i = a; i < b; i += 2)
                                Write(tmp, j++, array[i]);

                            Array.Copy(tmp, a, array, a, b - a);
                        }
                        AddHistorySnap();
                        break;
                    case 42: //bit reversal
                        int leng = 1 << (int)(Math.Log(arr.Length) / Math.Log(2));
                        bool pow2 = leng == arr.Length;

                        int[] tempb = new int[arr.Length];
                        Array.Copy(arr, tempb, arr.Length);
                        for (int i = 0; i < leng; i++) arr[i] = i;

                        int mb = 0;
                        int d1 = leng >> 1, d2 = d1 + (d1 >> 1);

                        for (int i = 1; i < leng - 1; i++)
                        {
                            int j = d1;

                            for (
                                int kb = i, nb = d2;
                                (kb & 1) == 0;
                                j -= nb, kb >>= 1, nb >>= 1
                            ) ;
                            mb += j;
                            if (mb > i) Swap(arr, i, mb);
                        }

                        if (!pow2)
                        {
                            for (int i = leng; i < arr.Length; i++)
                                Write(arr, i, arr[i - leng]);

                            int[] cntq = new int[leng];

                            for (int i = 0; i < arr.Length; i++)
                                cntq[arr[i]]++;

                            for (int i = 1; i < cntq.Length; i++)
                                cntq[i] += cntq[i - 1];

                            for (int i = arr.Length - 1; i >= 0; i--)
                                Write(arr, i, --cntq[arr[i]]);
                        }
                        int[] bits = new int[arr.Length];
                        Array.Copy(arr, bits, arr.Length);

                        for (int i = 0; i < arr.Length; i++)
                            Write(arr, i, tempb[bits[i]]);
                        AddHistorySnap();
                        break;
                    case 43: //block random
                        int cl = arr.Length;
                        int blockSize = pow2lte((int)Math.Sqrt(cl));
                        cl -= cl % blockSize;
                        for (int i = 0; i < arr.Length; i += blockSize)
                        {
                            int randomIndex = random.Next((arr.Length - i) / blockSize) * blockSize + i;
                            blockSwap(arr, i, randomIndex, blockSize);
                        }
                        void blockSwap(int[] array, int a, int b, int len)
                        {
                            for (int i = 0; i < len; i++)
                            {
                                Swap(array, a + i, b + i);
                            }
                        }

                        int pow2lte(int value)
                        {
                            int val;
                            for (val = 1; val <= value; val <<= 1) ;
                            return val >> 1;
                        }
                        AddHistorySnap();
                        break;
                    case 44: //block reverse
                        int cl2 = arr.Length;
                        int blockSizez = pow2lte2((int)Math.Sqrt(cl2));
                        cl2 -= cl2 % blockSizez;

                        int i1 = 0, j1 = cl2 - blockSizez;
                        while (i1 < j1)
                        {
                            blockSwap2(arr, i1, j1, blockSizez);
                            i1 += blockSizez;
                            j1 -= blockSizez;
                        }
                        void blockSwap2(int[] array, int a, int b, int len)
                        {
                            for (int i = 0; i < len; i++)
                            {
                                Swap(array, a + i, b + i);
                            }
                        }

                        int pow2lte2(int value)
                        {
                            int val;
                            for (val = 1; val <= value; val <<= 1) ;
                            return val >> 1;
                        }
                        AddHistorySnap();
                        break;
                    default: //none
                        AddHistorySnap();
                        break;

                }
                DrawHistory();
            }
            isPremade = false;

        }

        // Returns floor(log2(n)), assumes n > 0.
        public int PdqLog(int n)
        {
            int log = 0;
            while ((n >>= 1) != 0) ++log;
            return log;
        }

        public void MultiSwap(int[] array, int pos, int to)
        {
            if (to - pos > 0)
            {
                for (int i = pos; i < to; i++)
                {
                    Swap(array, i, i + 1);
                }
            }
            else
            {
                for (int i = pos; i > to; i--)
                {
                    Swap(array, i, i - 1);
                }
            }
        }

        private void Visualize_Click(object sender, RoutedEventArgs e)
        {
            //initialize everything
            if (timer != null) { timer.Stop(); }
            isPaused = false;
            comparisons = 0;
            arrAccesses = 0;
            swaps = 0;
            writes = 0;
            reversals = 0;
            
            selectedHistory = new List<int[]>();
            sortHistory = new List<int[]>();
            ResetPauseButtonText();


            switch (comboBox.SelectedIndex)
            {
                case 0:
                    MergeSort(0, arr.Length);
                    DrawHistory();
                    break;
                case 1:
                    InsertionSort();
                    DrawHistory();
                    break;
                case 2:
                    QuickSort(0, arr.Length);
                    DrawHistory();
                    break;
                case 3:
                    LRQuickSort(arr, 0, arr.Length - 1);
                    DrawHistory();
                    break;
                case 4:
                    BubbleSort();
                    DrawHistory();
                    break;
                case 5:
                    SelectionSort();
                    DrawHistory();
                    break;
                case 6:
                    HeapSort();
                    DrawHistory();
                    break;
                case 7:
                    OddEvenSort(arr, arr.Length); //first custom sort, also the first to not be hardcoded to the program's system.
                    DrawHistory();
                    break;
                case 8:
                    RadixSort(arr);
                    DrawHistory();
                    break;
                case 9:
                    ShellSort(arr);
                    DrawHistory();
                    break;
                case 10:
                    MoveToBackSort();
                    DrawHistory();
                    break;
                case 11:
                    InsertionWhat(arr);
                    DrawHistory();
                    break;
                default:
                    MessageDialog dialog = new("You need to select an algorithm.", "Alert");
                    //dialog.ShowAsync();
                    break;
            }

        }

        private bool isLeftButtonDown;
        private void Canv_MouseLeftButtonDown(object sender, PointerRoutedEventArgs e)
        {
            isLeftButtonDown = true;
        }

        private void Canv_MouseMove(object sender, PointerRoutedEventArgs e)
        {
            if (isLeftButtonDown && isPaused && e.GetCurrentPoint(canv).Position.X > 0 && e.GetCurrentPoint(canv).Position.X < canv.ActualWidth && e.GetCurrentPoint(canv).Position.Y > 0 && e.GetCurrentPoint(canv).Position.Y < canv.ActualHeight)
            {
                isPremade = true;
                PointerPoint a = e.GetCurrentPoint(canv);
                arr[(int)Math.Ceiling(a.Position.X / (canv.ActualWidth / arr.Length)) - 1] = arr.Length - (int)Math.Ceiling(a.Position.Y / (canv.ActualHeight / arr.Length));
                DrawNumbers(arr, null);
            }
            else
            {
                isLeftButtonDown = false;
            }
        }

        private void Window_MouseLeftButtonUp(object sender, PointerRoutedEventArgs e)
        {
            isLeftButtonDown = false;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPremade)
            {
                MessageDialog dialog = new("To visualize a premade array click \"Visualize\"", "Alert");
                //dialog.ShowAsync();
                return;
            }
            if (timer == null)
            {
                MessageDialog dialog = new("There is no running preview.", "Alert");
                //dialog.ShowAsync();
                return;
            }
            if (isPaused)
            {
                PlayPreview();
            }
            else
            {
                PausePreview();
            }
        }

        private void PausePreview()
        {
            if (timer != null)
            {
                timer.Stop();
            }
            pauseButton.Content = "Play";
            isPaused = true;
        }

        private void PlayPreview()
        {
            if (timer != null)
            {
                timer.Start();
            }
            pauseButton.Content = "Pause";
            isPaused = false;
        }

        private void ResetPauseButtonText()
        {
            pauseButton.Content = "Pause";
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (arr != null)
            {
                DrawNumbers(arr, selectedArr);
            }
        }

        public void AddHistorySnap()
        {
            int[] historySnap = new int[arr.Length];
            arr.CopyTo(historySnap, 0);
            sortHistory.Add(historySnap);
            selectedHistory.Add(selectedArr);
        }

        private void DrawNumbers(int[] arr, int[] selectedHistory)
        {
            canv.Children.Clear();

            int howMany = arr.Length;
            double size = canv.ActualWidth / howMany;

            for (int i = 0; i < howMany; i++)
            {
                Rectangle rect = new();
                Canvas.SetLeft(rect, size * i);
                Canvas.SetTop(rect, 0);
                rect.Width = size;
                rect.Height = (canv.ActualHeight - 5) / howMany * arr[i];
                rect.Fill = selectedHistory != null && selectedHistory.Contains(i) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                canv.Children.Add(rect);
            }
        }

        private void DrawHistory()
        {
            int counter = 0;
            timer = new DispatcherTimer();
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                arr = sortHistory[counter];
                selectedArr = selectedHistory[counter];
                DrawNumbers(sortHistory[counter], selectedHistory[counter]);
                counter++;
                if (counter < sortHistory.Count)
                {
                    timer.Interval = TimeSpan.FromMilliseconds(speedSlider.Value);
                    timer.Start();
                }
                else
                {
                    isPaused = true;
                    timer = null;
                    ResetPauseButtonText();
                }
            };
            timer.Start();
        }

        public void Swap(int[] arr, int i, int j)
        {
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
            swaps++;
            SwapsText.Text = "Swaps: " + swaps;
        }

        public void Write(int[] array, int at, int equals)
        {
            array[at] = equals;
            writes++;
            WritesText.Text = "Writes: " + writes;
        }

        public int CompareValues(int left, int right)
        {
            comparisons++;
            CompsText.Text = "Comparisons: " + comparisons;
            if (left > right) return 1;
            else if (left < right) return -1;
            else return 0;
        }

        public int[] MergeSort(int startI, int endI)
        {
            int length = endI - startI;
            if (length == 1)
            {
                arrAccesses++;
                return new int[] { arr[startI] };
            }
            int[] A = MergeSort(startI, startI + length / 2);
            int[] B = MergeSort(startI + length / 2, endI);
            int[] AB = new int[A.Length + B.Length];
            int iA = 0;
            int iB = 0;

            for (int i = 0; i < AB.Length; i++)
            {

                arrAccesses += 4;
                if (iB < B.Length && (iA == A.Length || B[iB] < A[iA]))
                {
                    AB[i] = B[iB];
                    arr[startI + i] = B[iB];
                    iB++;
                }
                else
                {
                    AB[i] = A[iA];
                    arr[startI + i] = A[iA];
                    iA++;
                }
                selectedArr = new int[] { startI + i };
                AddHistorySnap();
            }

            return AB;
        }

        private void InsertionSort()
        {
            for (int i = 1; i < arr.Length; i++)
            {
                int curr = i;
                while (curr - 1 >= 0 && arr[curr - 1] > arr[curr])
                {
                    arrAccesses += 6;

                    comparisons += 2;
                    int oldIValue = arr[curr];
                    arr[curr] = arr[curr - 1];
                    arr[curr - 1] = oldIValue;
                    curr--;
                }

                comparisons++;
                if (curr - 1 >= 0)
                {
                    comparisons++;
                }

                selectedArr = new int[] { curr };
                AddHistorySnap();
            }
        }

        private void QuickSort(int startI, int endI)
        {
            comparisons++;
            if (endI - startI < 1)
            {
                return;
            }

            int pI = endI - 1;

            int i = startI;
            int j = startI;

            while (j < endI - 1)
            {
                comparisons++;
                arrAccesses += 2;
                if (arr[j] <= arr[pI])
                {
                    comparisons++;
                    arrAccesses += 4;
                    int oldJ = arr[j];
                    arr[j] = arr[i];
                    arr[i] = oldJ;
                    i++;
                    selectedArr = new int[] { j, i };
                    AddHistorySnap();
                }
                j++;
            }
            comparisons++;

            int oldI = arr[i];
            arr[i] = arr[pI];
            arr[pI] = oldI;
            pI = i;
            arrAccesses += 4;

            selectedArr = new int[] { pI, i };
            AddHistorySnap();

            QuickSort(startI, pI);
            QuickSort(pI + 1, endI);
        }

        // Thanks to Timo Bingmann for providing a good reference for Quick Sort w/ LR pointers.
        private void LRQuickSort(int[] a, int p, int r)
        {
            int pivot = p + (r - p + 1) / 2;
            int x = a[pivot];

            int i = p;
            int j = r;

            while (i <= j)
            {
                while (a[i] < x)
                {
                    i++;
                    selectedArr = new int[] { i };
                    AddHistorySnap();
                }
                while (a[j] > x)
                {
                    j--;
                    selectedArr = new int[] { j };
                    AddHistorySnap();
                }

                if (i <= j)
                {
                    // Follow the pivot and highlight it.
                    if (i == pivot)
                    {
                        selectedArr = new int[] { j };
                        AddHistorySnap();
                    }
                    if (j == pivot)
                    {
                        selectedArr = new int[] { i };
                        AddHistorySnap();
                    }

                    Swap(a, i, j);

                    i++;
                    j--;

                    i++;
                    j--;
                }
            }

            if (p < j)
            {
                LRQuickSort(a, p, j);
            }
            if (i < r)
            {
                LRQuickSort(a, i, r);
            }
        }

        private void BubbleSort()
        {
            for (int i = arr.Length; i >= 0; i--)
            {
                for (int j = 0; j < i - 1; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        int oldJ = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = oldJ;
                        arrAccesses += 6;

                    }
                    comparisons++;
                }
                selectedArr = new int[] { i };
                AddHistorySnap();

            }
        }

        private void SelectionSort()
        {
            for (int i = 0; i < arr.Length; i++)
            {
                int minI = i;
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[minI] > arr[j])
                    {
                        minI = j;
                    }
                    arrAccesses += 2;
                    comparisons++;
                }
                int oldI = arr[i];
                arr[i] = arr[minI];
                arr[minI] = oldI;
                arrAccesses += 4;
                selectedArr = new int[] { i };
                AddHistorySnap();

            }
        }

        private void HeapSort()
        {

            for (int i = arr.Length / 2 - 1; i >= 0; i--)
            {
                Heapify(i, arr.Length);
            }

            for (int i = arr.Length - 1; i >= 0; i--)
            {
                int oldI = arr[i];
                arr[i] = arr[0];
                arr[0] = oldI;
                arrAccesses += 4;
                Heapify(0, i);
            }

            selectedArr = new int[] { arr.Length - 1 };

            AddHistorySnap();

            void Heapify(int i, int topI)
            {
                int maxI = i;
                int leftChildI = i * 2 + 1;
                int rightChildI = i * 2 + 2;

                comparisons++;
                if (leftChildI < topI)
                {
                    arrAccesses += 2;
                    comparisons++;
                    if (arr[leftChildI] > arr[maxI])
                    {
                        maxI = leftChildI;
                    }
                }

                comparisons++;
                if (rightChildI < topI)
                {
                    arrAccesses += 2;
                    comparisons++;
                    if (arr[rightChildI] > arr[maxI])
                    {
                        maxI = rightChildI;
                    }
                }

                comparisons++;
                if (maxI != i)
                {
                    int oldI = arr[i];
                    arr[i] = arr[maxI];
                    arr[maxI] = oldI;

                    arrAccesses += 4;

                    selectedArr = new int[] { i };
                    AddHistorySnap();
                    Heapify(maxI, topI);
                }
            }
        }

        public void OddEvenSort(int[] array, int length)
        {
            bool isSorted = false;

            while (!isSorted)
            {
                isSorted = true;

                //Swap i and i+1 if they are out of order, for i == odd numbers
                for (int i = 1; i <= length - 2; i += 2)
                {
                    if (array[i] > array[i + 1])
                    {
                        int temp = array[i];
                        array[i] = array[i + 1];
                        array[i + 1] = temp;
                        isSorted = false;
                    }
                    selectedArr = new int[] { i };
                    AddHistorySnap();
                }

                //Swap i and i+1 if they are out of order, for i == even numbers
                for (int i = 0; i <= length - 2; i += 2)
                {
                    if (array[i] > array[i + 1])
                    {
                        int temp = array[i];
                        array[i] = array[i + 1];
                        array[i + 1] = temp;
                        isSorted = false;
                    }
                    selectedArr = new int[] { i };
                    AddHistorySnap();
                }
            }
            return;
        }

        public int[] RadixSort(int[] array)
        {
            bool isFinished = false;
            int digitPosition = 0;

            List<Queue<int>> buckets = new();
            InitializeBuckets(buckets);

            while (!isFinished)
            {
                isFinished = true;

                foreach (int value in array)
                {
                    int bucketNumber = GetBucketNumber(value, digitPosition);
                    if (bucketNumber > 0)
                    {
                        isFinished = false;
                    }

                    buckets[bucketNumber].Enqueue(value);
                    selectedArr = new int[] { value };
                    AddHistorySnap();
                }

                int i = 0;
                foreach (Queue<int> bucket in buckets)
                {
                    while (bucket.Count > 0)
                    {
                        array[i] = bucket.Dequeue();
                        i++;
                        selectedArr = new int[] { i };
                        AddHistorySnap();
                    }

                }

                digitPosition++;
            }

            return array;
        }

        private int GetBucketNumber(int value, int digitPosition)
        {
            int bucketNumber = value / (int)Math.Pow(10, digitPosition) % 10;
            return bucketNumber;
        }

        private void InitializeBuckets(List<Queue<int>> buckets)
        {
            for (int i = 0; i < 10; i++)
            {
                Queue<int> q = new();
                buckets.Add(q);
                selectedArr = new int[] { i };
                AddHistorySnap();
            }
        }

        private int ShellSort(int[] array)
        {
            int length = array.Length;

            for (int h = length / 2; h > 0; h /= 2)
            {
                for (int i = h; i < length; i += 1)
                {

                    int temp = array[i];

                    int j;
                    for (j = i; j >= h && array[j - h] > temp; j -= h)
                    {
                        array[j] = array[j - h];
                        selectedArr = new int[] { j, h };
                        AddHistorySnap();
                    }

                    array[j] = temp;
                    selectedArr = new int[] { i, j };
                    AddHistorySnap();
                }
                selectedArr = new int[] { h };
                AddHistorySnap();
            }
            return 0;
        }

        private int Cur_List_Ptr = 0;
        public void MoveToBackSort()
        {
            if (Cur_List_Ptr >= arr.Length - 1) Cur_List_Ptr = 0;
            if (arr[Cur_List_Ptr] > arr[Cur_List_Ptr + 1])
            {
                Turn_Around(Cur_List_Ptr);
            }
            Cur_List_Ptr++;
        }

        private void Turn_Around(int Cur_Ptr)
        {
            int Temp_Value = arr[Cur_List_Ptr];
            int Ending = arr.Length - 1;
            for (int i = Cur_List_Ptr; i < Ending; i++)
            {
                arr[i] = arr[i + 1];
                selectedArr = new int[] { i };
                AddHistorySnap();
            }
            arr[Ending] = Temp_Value;
            selectedArr = new int[] { Ending };
        }

        public void InsertionWhat(int[] MyArray)
        {
            bool sorted = false;
            // while is not sorted
            while (!sorted)
            {
                sorted = true;
                for (int i = 1; i < MyArray.Length; i++)
                {
                    int j = i;

                    // Insert MyArray[i] into list 0..i-1 
                    while (j > 0 && MyArray[j] < MyArray[j - 1])
                    {
                        // Swap MyArray[j] and MyArray[j-1] 
                        SwapValues(i, j - 1);
                        selectedArr = new int[] { i, j };
                        AddHistorySnap();

                        // Decrement j by 1 
                        j--;
                        sorted = false;
                    }
                }
            }
        }


        private void SwapValues(int i, int v)
        {
            //swap values
            int temp = arr[i];
            arr[i] = arr[v];
            arr[v] = temp;

            //also swap the brushes
            AddHistorySnap();
        }







































        private unsafe AudioFrame GenerateAudioData(uint samples)
        {
            // Buffer size is (number of samples) * (size of each sample)
            // We choose to generate single channel (mono) audio. For multi-channel, multiply by number of channels
            uint bufferSize = samples * sizeof(float);
            AudioFrame frame = new(bufferSize);

            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                float* dataInFloat;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* dataInBytes, out uint capacityInBytes);

                // Cast to float since the data we are generating is float
                dataInFloat = (float*)dataInBytes;

                float freq = 1000; // choosing to generate frequency of 1kHz
                float amplitude = 0.3f;
                int sampleRate = (int)graph.EncodingProperties.SampleRate;
                double sampleIncrement = freq * (Math.PI * 2) / sampleRate;

                // Generate a 1kHz sine wave and populate the values in the memory buffer
                for (int i = 0; i < samples; i++)
                {
                    for (int j = 0; j < arr.Length; j++)
                    {
                        dataInFloat[i] = amplitude * arr[j];
                    }
                    //double sinValue = amplitude * Math.Sin(theta);

                    theta += sampleIncrement;
                }
            }

            return frame;
        }

        private async Task CreateAudioGraph()
        {
            // Create an AudioGraph with default settings
            AudioGraphSettings settings = new(AudioRenderCategory.Media);
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }

            graph = result.Graph;

            // Create a device output node
            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await graph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output node
            }

            deviceOutputNode = deviceOutputNodeResult.DeviceOutputNode;

            // Create the FrameInputNode at the same format as the graph, except explicitly set mono.
            AudioEncodingProperties nodeEncodingProperties = graph.EncodingProperties;
            nodeEncodingProperties.ChannelCount = 1;
            frameInputNode = graph.CreateFrameInputNode(nodeEncodingProperties);
            frameInputNode.AddOutgoingConnection(deviceOutputNode);

            // Initialize the Frame Input Node in the stopped state
            frameInputNode.Stop();

            // Hook up an event handler so we can start generating samples when needed
            // This event is triggered when the node is required to provide data
            frameInputNode.QuantumStarted += Node_QuantumStarted;

            // Start the graph since we will only start/stop the frame input node
            graph.Start();
        }

        private void Node_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
        {
            // GenerateAudioData can provide PCM audio data by directly synthesizing it or reading from a file.
            // Need to know how many samples are required. In this case, the node is running at the same rate as the rest of the graph
            // For minimum latency, only provide the required amount of samples. Extra samples will introduce additional latency.
            uint numSamplesNeeded = (uint)args.RequiredSamples;

            if (numSamplesNeeded != 0)
            {
                AudioFrame audioData = GenerateAudioData(numSamplesNeeded);
                frameInputNode.AddFrame(audioData);
            }
        }
    }
}
