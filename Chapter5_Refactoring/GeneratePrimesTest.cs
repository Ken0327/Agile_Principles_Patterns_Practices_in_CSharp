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