using System;
using System.Configuration;

namespace FdbDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var repository = new Repository(new FdbDatabaseProvider(ConfigurationManager.AppSettings["clusterFileName"], "DB"));

            int i = 0;
            while (true)
            {
                i++;
                repository.CreateFeature(string.Format("f{0}", i), GetBytes("some meta goes here"));
                var feature = repository.GetFeature(string.Format("f{0}", i));

                Console.ReadKey();
                Console.WriteLine();
            }
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
