using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.IO;

class RSA
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Įveskite p: ");
        BigInteger p = BigInteger.Parse(Console.ReadLine());
        Console.WriteLine("Įveskite q: ");
        BigInteger q = BigInteger.Parse(Console.ReadLine());
        Console.WriteLine("Įveskite tekstą: ");
        string text = Console.ReadLine();

        BigInteger n = p * q;
        BigInteger phi = (p - 1) * (q - 1);
        BigInteger e = 17; 
        BigInteger d = CalculateD(e, phi);

        Console.WriteLine($"Viešasis raktas (e, n): ({e}, {n})");
        Console.WriteLine($"Privatus raktas (d, n): ({d}, {n})");

        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        BigInteger[] encryptedText = new BigInteger[textBytes.Length];
        for (int i = 0; i < textBytes.Length; i++)
        {
            encryptedText[i] = Encrypt(textBytes[i], e, n);
        }
        Console.WriteLine("Užšifruotas tekstas: " + string.Join(" ", encryptedText));

        SaveToFile(encryptedText, "encryptedText.txt");
        Console.WriteLine("Užšifruotas tekstas išsaugotas.");

        SaveKeyToFile(e, d, n, "keys.txt");
        Console.WriteLine("Raktai išsaugoti.");

        BigInteger[] encryptedTextFromFile = LoadFromFile("encryptedText.txt");
        string decryptedText = DecryptText(encryptedTextFromFile, e, n);
        Console.WriteLine($"Dešifruotas tekstas: {decryptedText}");

        Console.ReadKey();
    }

    private static BigInteger Encrypt(BigInteger message, BigInteger e, BigInteger n)
    {
        return BigInteger.ModPow(message, e, n);
    }

    private static string DecryptText(BigInteger[] encryptedText, BigInteger e, BigInteger n)
    {
        
        var (p, q) = Factorize(n);
        BigInteger phi = (p - 1) * (q - 1);
        BigInteger d = CalculateD(e, phi);

        StringBuilder builder = new StringBuilder();
        foreach (var item in encryptedText)
        {
            var decryptedByte = Decrypt(item, d, n);
            builder.Append((char)decryptedByte);
        }
        return builder.ToString();
    }

    private static BigInteger Decrypt(BigInteger encryptedMessage, BigInteger d, BigInteger n)
    {
        return BigInteger.ModPow(encryptedMessage, d, n);
    }

    private static BigInteger CalculateD(BigInteger e, BigInteger phi)
    {
        BigInteger x, y;
        ExtendedEuclid(e, phi, out x, out y);
        BigInteger d = x;
        if (d < 0) d += phi; 
        return d;
    }


    private static BigInteger ExtendedEuclid(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
    {
        if (a == 0)
        {
            x = 0; y = 1;
            return b;
        }

        BigInteger x1, y1;
        BigInteger gcd = ExtendedEuclid(b % a, a, out x1, out y1);
        x = y1 - (b / a) * x1;
        y = x1;
        return gcd;
    }

    private static void SaveToFile(BigInteger[] encryptedText, string filename)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            foreach (var item in encryptedText)
            {
                writer.WriteLine(item);
            }
        }
    }

    private static void SaveKeyToFile(BigInteger e, BigInteger d, BigInteger n, string filename)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            writer.WriteLine($"Viešasis raktas: (e={e}, n={n})");
            writer.WriteLine($"Privatus raktas: (d={d}, n={n})");
        }
    }

    private static BigInteger[] LoadFromFile(string filename)
    {
        List<BigInteger> encryptedList = new List<BigInteger>();
        using (StreamReader reader = new StreamReader(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                encryptedList.Add(BigInteger.Parse(line));
            }
        }
        return encryptedList.ToArray();
    }

    private static (BigInteger, BigInteger) Factorize(BigInteger n)
    {
        BigInteger p, q;

        for (p = 2; p < n; p++)
        {
            if (n % p == 0) 
            {
                q = n / p;
                
                return (p, q);
                
            }
        }

        return (0, 0);
    }
}
