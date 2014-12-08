using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FoundationDB.Client;
using FoundationDB.Layers.Directories;

namespace FdbDemo
{
    public class Repository
    {
        private const string FeatureSubspace = "f";
        private readonly FdbDatabaseProvider _dbProvider;
        private readonly ConcurrentDictionary<string, FdbDirectorySubspace> _directoriesCache =
            new ConcurrentDictionary<string, FdbDirectorySubspace>();

        private readonly Stopwatch stopwatch;

        public Repository(FdbDatabaseProvider dbProvider)
        {
            _dbProvider = dbProvider;
            stopwatch = new Stopwatch();
        }

        public void CreateFeature(string name, byte[] feature)
        {
            var featureSpace = GetOrCreateDir(FeatureSubspace);
            var fkey = featureSpace.Pack(name);

            stopwatch.Restart();
            _dbProvider.Db.WriteAsync(trans => trans.Set(fkey, Slice.FromStream(new MemoryStream(feature))), new CancellationToken()).Wait();
            stopwatch.Stop();
            Console.WriteLine(string.Format("Created feature in {0} ms", stopwatch.ElapsedMilliseconds));
        }

        public Slice GetFeature(string name)
        {
            var featureSpace = GetOrCreateDir(FeatureSubspace);
            var fkey = featureSpace.Pack(name);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var featureSlice = _dbProvider.Db.ReadAsync(trans => trans.GetAsync(fkey), new CancellationToken()).Result;
            sw.Stop();
            Console.WriteLine(string.Format("Read feature in {0} ms", sw.ElapsedMilliseconds));
            return featureSlice;
        }

        private FdbDirectorySubspace GetOrCreateDir(string name)
        {
            FdbDirectorySubspace directory;

            if (_directoriesCache.TryGetValue(name, out directory))
                return directory;

            directory = _dbProvider.Db.Directory.CreateOrOpenAsync(name, new CancellationToken()).Result;
            _directoriesCache.TryAdd(name, directory);
            return directory;
        }
    }
}
