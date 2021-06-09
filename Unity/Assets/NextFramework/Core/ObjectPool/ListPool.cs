using System.Collections.Generic;

namespace NextFramework
{
    public static class ListPool<T>
    {
        static Stack<List<T>> dataStack = new Stack<List<T>>(8);


        public static List<T> Alloc()
        {
            return dataStack.Count > 0 ? dataStack.Pop() : new List<T>(8);
        }

        public static bool Recycle(List<T> list)
        {
            list.Clear();
            dataStack.Push(list);

            return true;
        }
    }

    /// <summary>
    /// List 静态拓展
    /// </summary>
    public static class ListExtention
    {
        public static void Recycle<T>(this List<T> list)
        {
            ListPool<T>.Recycle(list);
        }
    }
}
