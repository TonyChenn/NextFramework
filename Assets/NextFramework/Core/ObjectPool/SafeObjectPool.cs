using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextFramework
{
    public class SafeObjectPool<T> : Pool<T>, ISingleton 
        where T : IPoolable, new()
    {
        protected SafeObjectPool()
        {
            factory = new DefaultObjectFactory<T>();
        }
        public void InitSingleton() { }
        public static SafeObjectPool<T> Singlton
        {
            get { return NormalSingltonProperty<SafeObjectPool<T>>.Singlton; }
        }
        
        public void Dispose()
        {
            NormalSingltonProperty<SafeObjectPool<T>>.Dispose();
        }


        public override bool Recycle(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
