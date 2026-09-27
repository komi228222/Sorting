using System;
using System.Diagnostics;

namespace Sorting
{
  public class SortResult
  {
    public string AlgorithmName { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public long ElapsedMicroseconds { get; set; }
    public long ElapsedTicks { get; set; }
    public long TotalIterations { get; set; }
    public long ComparisonCount { get; set; }
    public long SwapCount { get; set; }
    public double[] SortedValues { get; set; }
  }

  public static class SortingAlgorithms
  {
    public static event Action<double[], string> OnStepCompleted;

    private const int BogoIterationLimit = 200000;

    private static void PublishStep(double[] values, string algorithmName)
    {
      if (OnStepCompleted != null)
      {
        OnStepCompleted((double[])values.Clone(), algorithmName);
      }
    }

    public static SortResult BubbleSort(double[] sourceValues, bool ascending)
    {
      double[] workingValues = (double[])sourceValues.Clone();
      Stopwatch stopwatch = Stopwatch.StartNew();
      long comparisonCount = 0;
      long swapCount = 0;

      for (int outerIndex = 0; outerIndex < workingValues.Length - 1; outerIndex++)
      {
        for (int innerIndex = 0; innerIndex < workingValues.Length - 1 - outerIndex; innerIndex++)
        {
          comparisonCount++;
          bool shouldSwap = ascending
            ? workingValues[innerIndex] > workingValues[innerIndex + 1]
            : workingValues[innerIndex] < workingValues[innerIndex + 1];

          if (shouldSwap)
          {
            double temporaryValue = workingValues[innerIndex];
            workingValues[innerIndex] = workingValues[innerIndex + 1];
            workingValues[innerIndex + 1] = temporaryValue;
            swapCount++;
            PublishStep(workingValues, "Bubble");
          }
        }
      }

      stopwatch.Stop();
      SortResult result = new SortResult();
      result.AlgorithmName = "Пузырьковая";
      result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      result.ElapsedMicroseconds = stopwatch.ElapsedTicks * 1000000L / Stopwatch.Frequency;
      result.ElapsedTicks = stopwatch.ElapsedTicks;
      result.ComparisonCount = comparisonCount;
      result.SwapCount = swapCount;
      result.TotalIterations = comparisonCount + swapCount;
      result.SortedValues = workingValues;
      return result;
    }

    public static SortResult InsertionSort(double[] sourceValues, bool ascending)
    {
      double[] workingValues = (double[])sourceValues.Clone();
      Stopwatch stopwatch = Stopwatch.StartNew();
      long comparisonCount = 0;
      long swapCount = 0;

      for (int currentIndex = 1; currentIndex < workingValues.Length; currentIndex++)
      {
        double currentValue = workingValues[currentIndex];
        int scanIndex = currentIndex - 1;

        while (scanIndex >= 0)
        {
          comparisonCount++;
          bool shouldShift = ascending
            ? workingValues[scanIndex] > currentValue
            : workingValues[scanIndex] < currentValue;

          if (!shouldShift) break;

          workingValues[scanIndex + 1] = workingValues[scanIndex];
          swapCount++;
          scanIndex--;
          PublishStep(workingValues, "Insertion");
        }

        workingValues[scanIndex + 1] = currentValue;
      }

      stopwatch.Stop();
      SortResult result = new SortResult();
      result.AlgorithmName = "Вставками";
      result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      result.ElapsedMicroseconds = stopwatch.ElapsedTicks * 1000000L / Stopwatch.Frequency;
      result.ElapsedTicks = stopwatch.ElapsedTicks;
      result.ComparisonCount = comparisonCount;
      result.SwapCount = swapCount;
      result.TotalIterations = comparisonCount + swapCount;
      result.SortedValues = workingValues;
      return result;
    }

    public static SortResult ShakerSort(double[] sourceValues, bool ascending)
    {
      double[] workingValues = (double[])sourceValues.Clone();
      Stopwatch stopwatch = Stopwatch.StartNew();
      long comparisonCount = 0;
      long swapCount = 0;

      int leftBound = 0;
      int rightBound = workingValues.Length - 1;

      while (leftBound < rightBound)
      {
        for (int forwardIndex = leftBound; forwardIndex < rightBound; forwardIndex++)
        {
          comparisonCount++;
          bool shouldSwap = ascending
            ? workingValues[forwardIndex] > workingValues[forwardIndex + 1]
            : workingValues[forwardIndex] < workingValues[forwardIndex + 1];

          if (shouldSwap)
          {
            double temporaryValue = workingValues[forwardIndex];
            workingValues[forwardIndex] = workingValues[forwardIndex + 1];
            workingValues[forwardIndex + 1] = temporaryValue;
            swapCount++;
            PublishStep(workingValues, "Shaker");
          }
        }
        rightBound--;

        for (int backwardIndex = rightBound; backwardIndex > leftBound; backwardIndex--)
        {
          comparisonCount++;
          bool shouldSwap = ascending
            ? workingValues[backwardIndex - 1] > workingValues[backwardIndex]
            : workingValues[backwardIndex - 1] < workingValues[backwardIndex];

          if (shouldSwap)
          {
            double temporaryValue = workingValues[backwardIndex - 1];
            workingValues[backwardIndex - 1] = workingValues[backwardIndex];
            workingValues[backwardIndex] = temporaryValue;
            swapCount++;
            PublishStep(workingValues, "Shaker");
          }
        }
        leftBound++;
      }

      stopwatch.Stop();
      SortResult result = new SortResult();
      result.AlgorithmName = "Шейкерная";
      result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      result.ElapsedMicroseconds = stopwatch.ElapsedTicks * 1000000L / Stopwatch.Frequency;
      result.ElapsedTicks = stopwatch.ElapsedTicks;
      result.ComparisonCount = comparisonCount;
      result.SwapCount = swapCount;
      result.TotalIterations = comparisonCount + swapCount;
      result.SortedValues = workingValues;
      return result;
    }

    public static SortResult QuickSort(double[] sourceValues, bool ascending)
    {
      double[] workingValues = (double[])sourceValues.Clone();
      Stopwatch stopwatch = Stopwatch.StartNew();
      long comparisonCount = 0;
      long swapCount = 0;

      QuickSortRecursive(
        workingValues, 0, workingValues.Length - 1, ascending,
        ref comparisonCount, ref swapCount);

      stopwatch.Stop();
      SortResult result = new SortResult();
      result.AlgorithmName = "Быстрая";
      result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      result.ElapsedMicroseconds = stopwatch.ElapsedTicks * 1000000L / Stopwatch.Frequency;
      result.ElapsedTicks = stopwatch.ElapsedTicks;
      result.ComparisonCount = comparisonCount;
      result.SwapCount = swapCount;
      result.TotalIterations = comparisonCount + swapCount;
      result.SortedValues = workingValues;
      return result;
    }

    private static void QuickSortRecursive(
      double[] values, int lowBound, int highBound, bool ascending,
      ref long comparisonCount, ref long swapCount)
    {
      if (lowBound >= highBound) return;

      int pivotIndex = Partition(
        values, lowBound, highBound, ascending,
        ref comparisonCount, ref swapCount);

      QuickSortRecursive(
        values, lowBound, pivotIndex - 1, ascending,
        ref comparisonCount, ref swapCount);
      QuickSortRecursive(
        values, pivotIndex + 1, highBound, ascending,
        ref comparisonCount, ref swapCount);
    }

    private static int Partition(
      double[] values, int lowBound, int highBound, bool ascending,
      ref long comparisonCount, ref long swapCount)
    {
      double pivotValue = values[highBound];
      int smallerIndex = lowBound - 1;

      for (int scanIndex = lowBound; scanIndex < highBound; scanIndex++)
      {
        comparisonCount++;
        bool belongsToLeft = ascending
          ? values[scanIndex] <= pivotValue
          : values[scanIndex] >= pivotValue;

        if (belongsToLeft)
        {
          smallerIndex++;
          double temporaryValue = values[smallerIndex];
          values[smallerIndex] = values[scanIndex];
          values[scanIndex] = temporaryValue;
          swapCount++;
        }
      }

      double lastTemporaryValue = values[smallerIndex + 1];
      values[smallerIndex + 1] = values[highBound];
      values[highBound] = lastTemporaryValue;
      swapCount++;

      return smallerIndex + 1;
    }

    public static SortResult BogoSort(
      double[] sourceValues, bool ascending, int iterationLimit)
    {
      double[] workingValues = (double[])sourceValues.Clone();
      Stopwatch stopwatch = Stopwatch.StartNew();
      Random randomGenerator = new Random();
      long comparisonCount = 0;
      long swapCount = 0;
      long shuffleIteration = 0;

      while (!IsSorted(workingValues, ascending) && shuffleIteration < iterationLimit)
      {
        for (int position = workingValues.Length - 1; position > 0; position--)
        {
          int randomPosition = randomGenerator.Next(position + 1);
          double temporaryValue = workingValues[position];
          workingValues[position] = workingValues[randomPosition];
          workingValues[randomPosition] = temporaryValue;
          swapCount++;
        }

        shuffleIteration++;
        comparisonCount++;
        PublishStep(workingValues, "Bogo");
      }

      stopwatch.Stop();
      SortResult result = new SortResult();
      result.AlgorithmName = "BOGO";
      result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      result.ElapsedMicroseconds = stopwatch.ElapsedTicks * 1000000L / Stopwatch.Frequency;
      result.ElapsedTicks = stopwatch.ElapsedTicks;
      result.ComparisonCount = comparisonCount;
      result.SwapCount = swapCount;
      result.TotalIterations = shuffleIteration;
      result.SortedValues = workingValues;
      return result;
    }

    private static bool IsSorted(double[] values, bool ascending)
    {
      for (int index = 0; index < values.Length - 1; index++)
      {
        if (ascending && values[index] > values[index + 1]) return false;
        if (!ascending && values[index] < values[index + 1]) return false;
      }
      return true;
    }
  }
}