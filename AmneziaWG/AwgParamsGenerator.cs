using System;
using System.Security.Cryptography;

public static class AwgParamsGenerator
{
    public static void Main()
    {
        int jc = RandomNumberGenerator.GetInt32(4, 13);
        int jmin = RandomNumberGenerator.GetInt32(64, 161);
        int jmax = RandomNumberGenerator.GetInt32(Math.Max(jmin + 1, 161), 401);

        int s1 = RandomNumberGenerator.GetInt32(16, 151);
        int s2;
        do
        {
            s2 = RandomNumberGenerator.GetInt32(16, 151);
        }
        while (s2 == s1 + 56); // старое практическое ограничение из гайдов

        int s3 = RandomNumberGenerator.GetInt32(8, 65);
        int s4 = RandomNumberGenerator.GetInt32(0, 33);

        uint h1 = RandomUIntInBlock(100_000_000u, 100_000_063u);
        uint h2 = RandomUIntInBlock(700_000_000u, 700_000_063u);
        uint h3 = RandomUIntInBlock(1_500_000_000u, 1_500_000_063u);
        uint h4 = RandomUIntInBlock(2_000_000_000u, 2_000_000_063u);

        Console.WriteLine($"Jc = {jc}");
        Console.WriteLine($"Jmin = {jmin}");
        Console.WriteLine($"Jmax = {jmax}");
        Console.WriteLine();
        Console.WriteLine($"S1 = {s1}");
        Console.WriteLine($"S2 = {s2}");
        Console.WriteLine($"S3 = {s3}");
        Console.WriteLine($"S4 = {s4}");
        Console.WriteLine();
        Console.WriteLine($"H1 = {h1}-{h1 + 63}");
        Console.WriteLine($"H2 = {h2}-{h2 + 63}");
        Console.WriteLine($"H3 = {h3}-{h3 + 63}");
        Console.WriteLine($"H4 = {h4}-{h4 + 63}");
        Console.WriteLine();
        Console.WriteLine("I1 = <b 0xc700000001><rc 8><t><r 24>");
        Console.WriteLine("I2 = <b 0xc800000001><rd 6><r 12>");
        Console.WriteLine("I3 = <b 0xc900000001><t><r 8>");
    }

    private static uint RandomUIntInBlock(uint min, uint maxInclusive)
    {
        byte[] bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        uint value = BitConverter.ToUInt32(bytes, 0);
        return min + (value % (maxInclusive - min + 1));
    }
}