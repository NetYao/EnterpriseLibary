using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprises.Framework
{
    /// <summary>
    /// 个人所得税接口
    /// </summary>
    public interface ITax
    {
        double Calculate(double basicWage,double bouns);
    }
}
