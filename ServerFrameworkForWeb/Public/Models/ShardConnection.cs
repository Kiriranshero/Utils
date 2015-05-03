using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    public interface IShardConnection<T>
    {
        string Get(T inKey);
        void Add(short index, string inConnection);
    }

    public class ShardConnection<T> : IShardConnection<T>
    {
        private Dictionary<int, string> ConnectionList = new Dictionary<int, string>();
        private Func<T, int> _Sharding = null;

        public ShardConnection(Func<T, int> inSharding)
        {
            _Sharding = inSharding;
        }

        public void Add(short index, string connectionString)
        {
            ConnectionList.Add(index, connectionString);
        }

        public string Get(T inKey)
        {
            return ConnectionList[_Sharding(inKey)];
        }
    }
}
