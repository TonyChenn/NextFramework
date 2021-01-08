using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextFramework
{
    public interface IPool<T>
    {
        T Alloc();
        bool Recycle(T obj);
    }
}
