using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextFramework.UI
{
    public class UIFlagEnumComparer : IEqualityComparer<UIType>
    {
        public bool Equals(UIType x, UIType y)
        {
            return x == y;
        }

        public int GetHashCode(UIType x)
        {
            return (int)x;
        }
    }
}
