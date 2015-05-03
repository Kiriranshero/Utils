using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    public interface IShardAdapter
    {
    }

    public abstract class ShardAdapter<T, J> : IShardAdapter
        where T : Component, new()
    {
        private IShardConnection<J> _Connections = null;
        private ConcurrentStack<Worker> _Stack = new ConcurrentStack<Worker>();

        public class Worker : IDisposable
        {
            public T Work { get; internal set; }
            internal ConcurrentStack<Worker> Parent { get; set; }

            internal Worker() { }

            public void Dispose()
            {
                Parent.Push(this);
            }
        }

        public ShardAdapter(IShardConnection<J> inConnections)
        {
            _Connections = inConnections;
        }

        public abstract void Change(dynamic inAdapter, string inConnection);

        public Worker GetWorker(J inKey)
        {
            Worker tResult = null;
            if (_Stack.TryPop(out tResult) == false) tResult = new Worker { Parent = _Stack, Work = new T() };
            Change(tResult.Work, _Connections.Get(inKey));
            return tResult;
        }
    }
}
