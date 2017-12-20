using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprises.Framework
{
    /// <summary>
    /// 计算奖金接口
    /// </summary>
    public interface IBonus
    {
        double Calculate(double basicWage);
    }
}
