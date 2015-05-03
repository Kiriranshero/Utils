using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    public class WorkerPool<T>
        where T : new()
    {
        Action<T> Create = null;        
        Action<T> Call = null;

        ConcurrentStack<Worker> Stack = new ConcurrentStack<Worker>();

        public class Worker : IDisposable
        {
            public T Work { get; internal set; }
            ConcurrentStack<Worker> Parent { get; set; }

            internal Worker(ConcurrentStack<Worker> parent, T work)                 
            {
                Parent = parent;
                Work = work;
            }

            public void Dispose()
            {                
                Parent.Push(this);
            }
        }

        public WorkerPool(Action<T> create = null, Action<T> call = null)
        {
            this.Create = create;
            this.Call = call;
        }
      
        public Worker GetWorker()
        {
            Worker tResult = null;
            if (Stack.TryPop(out tResult) == false)
            {
                tResult = new Worker(Stack, new T());
                if (Create != null) Create(tResult.Work);
            }
            if (Call != null) Call(tResult.Work);
            return tResult;
        }

        public int Count
        {
            get { return Stack.Count; }
        }     
    }
}
