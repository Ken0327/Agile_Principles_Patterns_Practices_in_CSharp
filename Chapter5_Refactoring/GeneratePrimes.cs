/// <remark> 
/// 這個類別產生使用者指定之最大值範圍內的質數。 
/// 使用的演算法是Eratosthenes 篩選法。
/// 給定一個整數陣列,其內容由2開始遞增:
/// 找第一個尚未被劃掉的整數,去劃掉它的所有倍數。
/// 如此反覆執行，直到陣列中再也找不到最多的倍數。
///</remark>

using System;

public class GeneratePrimes
{
    private static bool[] crossedOut;
    private static int[] result;

    public static int[] GeneratePrimeNumbers(int maxValue)
    {
        if (maxValue < 2)
        {
            return new int[0];
        }
        else
        {
            UncrossIntegersUpTo(maxValue);
            CrossOutMultiples();
            PutUncrossedIntegersIntoResult();
            return result;
        }
    }

    private static void UncrossIntegersUpTo(int maxValue)
    {
        crossedOut = new bool[maxValue + 1];
        for (int i = 2; i < crossedOut.Length; i++)
            crossedOut[i] = false;
    }

    private static void PutUncrossedIntegersIntoResult()
    {
        result = new int[NumberOfUncrossedIntegers()];
        for (int j = 0, i = 2; i < crossedOut.Length; i++)
        {
            if (NotCrossed(i))
                result[j++] = i;
        }
    }

    private static int NumberOfUncrossedIntegers()
    {
        int count = 0;
        for (int i = 2; i < crossedOut.Length; i++)
        {
            if (NotCrossed(i))
                count++; // 累加count
        }
        return count;
    }

    private static void CrossOutMultiples()
    {
        int limit = DetermineIterationLimit();
        for (int i = 2; i <= limit; i++)
        {
            if (NotCrossed(i))
                CrossOutputMultiplesOf(i);
        }
    }

    private static int DetermineIterationLimit()
    {
        // 陣列中的每個倍數都有一個小於或等於陣列大小平方根的質因數
        // 因此我們不用劃掉那些比這個平方根還大的倍數
        double iterationLimit = Math.Sqrt(crossedOut.Length);
        return (int)iterationLimit;
    }

    private static void CrossOutputMultiplesOf(int i)
    {
        for (int multiple = 2 * i; multiple < crossedOut.Length; multiple += i)
            crossedOut[multiple] = true;
    }

    private static bool NotCrossed(int i)
    {
        return crossedOut[i] == false;
    }
}
