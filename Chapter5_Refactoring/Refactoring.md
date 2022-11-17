重構 Refactoring
======

## 1. 重構定義
- 在不改變程式碼外在行為的前提下，對程式碼做出修改，以改進程式碼內部結構的過程。
- 每一個軟體模組都有三個職責
    1. 它運行時所達成的功能，也是該模組存在的原因
    2. 它要應對變化。幾乎所有的模組在它們的生命週期期間都會發生改變，開發者有責任保證這種改變應該盡可能地簡單。一個難以改變的模組是有問題的，即便能夠工作，也需要對他進行修正。
    3. 要讓閱讀者能夠理解。對該模組不熟悉的開發人員也應該能夠容易地閱讀並理解它。一個無法被理解的模組也是有問題的，同樣需要對它進行修正。


## 2. 產生質數的程式 : 一個簡單的重構範例

Listing 5-1 GeneratePrimes.cs, version 1

```csharp
/// <remark> 
/// 這個類別產生使用者指定之最大值範圍內的質數。 
/// 使用的演算法是Eratosthenes 篩選法。
///
/// Eratosthenes, 生於西元前276年利比亞的Cyrene,西元前194 年逝世於亞歷山大。 
/// 他是第一個計算地球周長的人,也因研究考慮留年的曆法和掌管亞歷山大圖書館而聞名。 
///
/// 這個演算法非常簡單。先給定一個整數陣列,其內容由2開始遞增,先劃掉2的倍數。
/// 然後找下一個尚未被劃掉的整數,去劃掉它的所有倍數。如此反覆執行到傳入之最大值的平方根為止。
/// Written by Robert C. Martin on 9 Dec 1999 in Java /// Translated to C# by Micah Martin on 12 Jan 2005. 
///</remark>

using System;
/// <summary> 
/// author: Robert C. Martin 
/// </summary> 
public class GeneratePrimes
{
    /// <summary> 
    /// 產生一個包含質數的陣列
    /// </summary>
    ///
    /// <param name="maxValue">產生的最大值</param>
    public static int[] GeneratePrimeNumbers(int maxValue)
    {
        if (maxValue >= 2) // 僅在此時有意義
        {
            // 宣告
            int s = maxValue + 1; //陣列大小
            bool [] f = new bool[s];
            int i;

            // 將陣列元素初始為true
            for (i = 0; i < s; i++)
                f[i] = true;
            
            // 去掉已知的非質數
            f[0] = f[1] = false;

            // sieve (篩選 ; 過濾)
            int j;
            for (i = 2; i < Math.Sqrt(s) + 1; i++)
            {
                if (f[i]) // 如果i未被劃掉，就劃掉其倍數
                {
                    for (j = 2 * i; j < s; j +=i)
                    f[j] = false; // 倍數不是質數
                }
            }

            // 有多少個質數?
            int count = 0;
            for (i = 0; i < s; i ++)
            {
                if (f[i])
                    count++; // 累加
            }

            int [] primes = new int[count];

            // 把質數轉移到結果列中
            for (i = 0, j = 0; i < s; i++)
            {
                if (f[i]) // 若為質數
                    primes[j++] = i;
            }

            return primes; // 回傳primes結果陣列
        }
        else // maxValue < 2
        {
            return new int[0]; // 若輸入不合理的值，則回傳空陣列
        }
    }
}
````

Listing 5-2 GeneratePrimesTest.cs

```csharp
using NUnit.Framework;

[TextFixture]
public class GeneratePrimesTest
{
    [Test]
    public void TestPrimes()
    {
        int [] nullArray = GeneratePrimes.GeneratePrimeNumbers(0);
        Assert.AreEqual(nullArray.Length, 0);

        int [] minArray = GeneratePrimes.GeneratePrimeNumbers(2);
        Assert.AreEqual(minArray.Length, 1);
        Assert.AreEqual(minArray[0], 2);

        int [] threeArray = GeneratePrimes.GeneratePrimeNumbers(3);
        Assert.AreEqual(threeArray.Length, 2);
        Assert.AreEqual(threeArray[0], 2);
        Assert.AreEqual(threeArray[1], 3);
        
        int [] centArray = GeneratePrimes.GeneratePrimeNumbers(100);
        Assert.AreEqual(threeArray.Length, 25);
        Assert.AreEqual(threeArray[24], 97);
    }
}
````



## 3. 重構
- 主函式要被分為3個獨立的函式
    1. 第一個函式會對所有的變數進行初始化動作，並做好過濾所需的準備工作
    2. 第二個函式執行過濾工作
    3. 第三個函式則是把過濾結果存放到一個整數陣列中
- 對這三個函式的提取迫使我把該函式的一些區域變數提升為類別的靜態欄位(static field)，這更清楚地顯示出哪些是區域變數，哪些變數的影響更廣泛

Listing 5-3 PrimeGenerator.cs, version 2

```csharp
/// <remark> 
/// 這個類別產生使用者指定之最大值範圍內的質數。 
/// 使用的演算法是Eratosthenes 篩選法。
/// 給定一個整數陣列,其內容由2開始遞增:
/// 找第一個尚未被劃掉的整數,去劃掉它的所有倍數。
/// 如此反覆執行，直到陣列中再也找不到最多的倍數。
///</remark>

using System;

public class PrimeGenerator
{
    private static int s;
    private static bool[] f;
    private static bool[] primes;
    
    public static int[] GeneratePrimeNumbers(int maxValue)
    {       
        if (maxValue < 2)
        {
            return new int[0];
        }
        else
        {
            InitializeSieve(maxValue);
            Sieve();
            LoadParimes();
            return primes;
        }
    }

    private static void LoadPrimes()
    {
        int i;
        int j;

        // 有多少個質數?
        int count = 0;
        for (i = 0; i < s; i ++)
        {
            if (f[i])
                count++; // 累加
        }

        primes = new int[count];

        // 把質數轉移到結果列中
        for (i = 0, j = 0; i < s; i++)
        {
            if (f[i]) // 若為質數
                primes[j++] = i;
        }
    }

    private static void Sieve()
    {
        int i;
        int j;
        for (i = 2; i < Math.Sqrt(s) + 1; i++)
        {
            if (f[i]) // 如果i未被劃掉，就劃掉其倍數
            {
                for (j = 2 * i; j < s; j +=i)
                f[j] = false; // 倍數不是質數
            }
        }
    }

    private static void InitializeSieve(int maxValue)
    {
        // 宣告
        s = maxValue + 1; //陣列大小
        f = new bool[s];
        int i;

        // 將陣列元素初始為true
        for (i = 0; i < s; i++)
            f[i] = true;
        
        // 去掉已知的非質數
        f[0] = f[1] = false;
    }
}
````

Listing 5-7 PrimeGenerator.cs (最終版)

```csharp
/// <remark> 
/// 這個類別產生使用者指定之最大值範圍內的質數。 
/// 使用的演算法是Eratosthenes 篩選法。
/// 給定一個整數陣列,其內容由2開始遞增:
/// 找第一個尚未被劃掉的整數,去劃掉它的所有倍數。
/// 如此反覆執行，直到陣列中再也找不到最多的倍數。
///</remark>

using System;

public class PrimeGenerator
{
    private static bool[] crossOut;
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
        crossOut = new bool[maxValue + 1];
        for (int i = 2; i < crossOut.Length; i++)
            crossOut[i] = false;
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
        for (int i = 2; i < crossedOut.Length; i ++)
        {
            if (NotCrossed(i))
                count++; // 累加count
        }
        return count;
    }

    private static int CrossOutMultiples()
    {
        int limit = DetermineIterationLimit();
        for (int i = 2; i <= limit; i ++)
        {
            if (NotCrossed(i))
                CrossOutputMultiplesOf(i);
        }
    }

    private static int DetermineIterationLimit()
    {
        // 陣列中的每個倍數都有一個小於或等於陣列大小平方根的質因數
        // 因此我們不用劃掉那些比這個平方根還大的倍數
        double iterationLimit = Math.Sqrt(crossOut.Length);
        return (int) iterationLimit;
    }

    private static void CrossOutMultiplesOf(int i)
    {
        for (int multiple = 2 * i; multiple < crossOut.Length; multiple += i)
            crossOut[Multiple] = true;
    }

    private static bool NotCrossed(int i)
    {
        return crossedOut[i] == false;
    }
}
````

Listing 5-8 GeneratePrimesTest.cs

```csharp
using NUnit.Framework;

[TextFixture]
public class GeneratePrimesTest
{
    [Test]
    public void TestPrimes()
    {
        int [] nullArray = GeneratePrimes.GeneratePrimeNumbers(0);
        Assert.AreEqual(nullArray.Length, 0);

        int [] minArray = GeneratePrimes.GeneratePrimeNumbers(2);
        Assert.AreEqual(minArray.Length, 1);
        Assert.AreEqual(minArray[0], 2);

        int [] threeArray = GeneratePrimes.GeneratePrimeNumbers(3);
        Assert.AreEqual(threeArray.Length, 2);
        Assert.AreEqual(threeArray[0], 2);
        Assert.AreEqual(threeArray[1], 3);
        
        int [] centArray = GeneratePrimes.GeneratePrimeNumbers(100);
        Assert.AreEqual(threeArray.Length, 25);
        Assert.AreEqual(threeArray[24], 97);
    }

    [Test]
    public void TestExhaustive()
    {
        for (int i = 2; i < 500; i++)
            VerifyPrimeList(GeneratePrimes.GeneratePrimeNumbers(i));
    }

    private void VerifyPrimeList(int[] list)
    {
        for (int i = 0; i < list.Length; i++)
            VerifyPrime(list[i]);
    }

    private void VerifyPrime(int n)
    {
        for (int factor = 2; factor < n; factor++)
            Assert.IsTrue(n%factor != 0);
    }
}
````


## 4. 總結
- 大多數的情況下，提取方法所增加的可讀性，值得花費一些微小的額外開銷。我的建議是，假設這種損失是可以忽略的，那就等到時候再去證明這個假設是錯誤的。
- 經常對你所撰寫的和維護的每一個模組進行這種重構。
- 重構的目的是為了每天、每小時、每分鐘都清理你的程式碼。我們不想讓髒亂累積，不想鑿去並清洗隨時間累積下來乾硬的程式。
- 我們想透過微小的努力，就能對我們的系統進行擴展和修改，最重要的就是保持程式碼的整潔。(the cleanliness of the code)