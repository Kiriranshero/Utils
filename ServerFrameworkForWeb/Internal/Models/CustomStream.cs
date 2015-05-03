using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework
{
    sealed class CustomStream : MemoryStream, IDisposable
    {
        public override void Close() { }

        void IDisposable.Dispose()
        {
            base.Close();
        }
    }
}
