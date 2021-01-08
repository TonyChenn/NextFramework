using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextFramework
{
    public interface IPoolable
    {
        bool HasRecycled { get; set; }

        /// <summary>
        /// 初始化,删除Texture等资源
        /// </summary>
        void OnRecycle();
    }
}
