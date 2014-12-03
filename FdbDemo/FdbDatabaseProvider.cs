using FoundationDB.Client;

namespace FdbDemo
{
    public class FdbDatabaseProvider
    {
        private const string LocalCluster = "fdb.local.cluster";
        private readonly string _clusterFilePath;
        private readonly string _databaseName;
        private IFdbDatabase _db;

        public FdbDatabaseProvider(string clusterFilePath, string databaseName)
        {
            _clusterFilePath = clusterFilePath;
            _databaseName = databaseName;
        }

        public IFdbDatabase Db
        {
            get
            {
                if (_db == null)
                {
                    _db = (_clusterFilePath == LocalCluster)
                        ? Fdb.OpenAsync().Result
                        : Fdb.OpenAsync(_clusterFilePath, _databaseName).Result;
                }

                return _db;
            }
        }
    }
}
