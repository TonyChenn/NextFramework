using System.Collections.Generic;
using System.Text;

namespace NextFramework
{
    public static class StringBuilderPool
    {
        static Stack<StringBuilder> dataStack = new Stack<StringBuilder>();


        public static StringBuilder Alloc()
        {
            return dataStack.Count > 0 ? dataStack.Pop() : new StringBuilder();
        }

        public static bool Recycle(StringBuilder builder)
        {
            builder.Remove(0, builder.Length);
            dataStack.Push(builder);
            return true;
        }
    }

    /// <summary>
    /// StringBuilder 静态拓展
    /// </summary>
    public static class StringBuilderExtention
    {
        public static void Recycle(this StringBuilder builder)
        {
            StringBuilderPool.Recycle(builder);
        }
    }
}
