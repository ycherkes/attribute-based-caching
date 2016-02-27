using System.IO;
using BplusDotNet;

namespace CacheAspect
{

    #region

    #endregion

    public class BTreeCache : ICache
    {
        #region Public Indexers

        public object this[string key]
        {
            get
            {
                return _treeCache.ContainsKey(key) ? _treeCache[key] : null;
            }
            set
            {
                _treeCache[key] = value;
                SaveCache();
            }
        }

        #endregion

        #region Static Fields

        private static string _datafile;

        private static SerializedTree _treeCache;

        private static string _treefile;

        #endregion

        #region Constructors and Destructors

        public BTreeCache()
        {
            _datafile = CacheService.DiskPath + "datafile";
            _treefile = CacheService.DiskPath + "treefile";
            LoadCache();
        }

        ~BTreeCache()
        {
            CloseCache();
        }

        #endregion

        #region Public Methods and Operators

        public bool Contains(string key)
        {
            return _treeCache.ContainsKey(key);
        }

        public void Delete(string key)
        {
            _treeCache.RemoveKey(key);
            SaveCache();
        }

        public void CloseCache()
        {
            _treeCache.Shutdown();
        }

        public void LoadCache()
        {
            if (_treeCache != null) return;

            if (File.Exists(_treefile) && File.Exists(_datafile))
            {
                _treeCache = new SerializedTree(hBplusTreeBytes.ReOpen(_treefile, _datafile));
            }
            else
            {
                _treeCache = new SerializedTree(hBplusTreeBytes.Initialize(_treefile, _datafile, 500));
            }
            _treeCache.SetFootPrintLimit(10);
        }

        public void SaveCache()
        {
            _treeCache?.Commit();
        }

        public void Clear()
        {
            var key = _treeCache.FirstKey();
            while (!string.IsNullOrWhiteSpace(key))
            {
                Delete(key);
                key = _treeCache.FirstKey();
            }
        }

        #endregion
    }
}