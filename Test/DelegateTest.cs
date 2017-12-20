using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
//    题目：

//假设我们是中国国家航天局人员，当玉兔号离开嫦娥三号之后，我们需要能够控制玉兔号在月球上开展探测工作。我们先假定虹湾区是一个很大的平原，我们在虹湾区建立一个坐标轴，如下图：

//         玉兔号离开嫦娥三号后，根据自身安装的定位系统可以知道自己的初始位置，我们记为 X0 , Y0 ； 同时玉兔号也可以知道当前它的朝向，如东、西、南、北（暂时只考虑这四个方向）。

//中国国家航天局会向玉兔号发送指令，我们先暂定为3种：

//    F : 当玉兔号接收到这条指令之后，会向前移动一个坐标单位的距离
//    L : 当玉兔号接受到这条指令之后，会原地向左旋转90度
//    R : 当玉兔号接收到这条指令之后，会原地向右旋转90度

//要求：

//一）设计一个玉兔号的主程序，能够接收中国国家航天局发送过来的指令序列（如FFLFRFLL），执行该指令序列之后，玉兔号能够走到正确的位置，并知道当前正确的位置。（如：玉兔号初始位置为 (0,0)，方向朝东，执行指令 FFLFRFLL之后，位置为 (3,1) 方向朝西）

        

//二）主程序中，不允许出现switch case语句，也不允许出现else关键字，也不允许使用三元表达式，if关键字出现的次数要求在5次以下（0-4次）

 

//三）主程序可以用任何语言编写，如Java、C#、Ruby、Python、PHP等

 

//四）在所选语言允许的情况下，请编写相应的单元测试       

    //public delegate void OperatorDelegate();
    //static void Main(string[] args)
    //{

    //    string instruct = "";

    //    Detector YuTu3 = new Detector(0, 0, (int)Diretion.East);
    //    var Dictory = new System.Collections.Generic.Dictionary<string, OperatorDelegate>();
    //    Dictory["F"] = new OperatorDelegate(YuTu3.DealFont);
    //    Dictory["L"] = new OperatorDelegate(YuTu3.DealLeft);
    //    Dictory["R"] = new OperatorDelegate(YuTu3.DealRight);

    //    while ("exit" != (instruct = Console.ReadLine()))
    //    {
    //        if (Dictory.ContainsKey(instruct))
    //        {
    //            Dictory[instruct]();
    //        }
    //    };

    //    YuTu3.Print();

    //}


    //探测器
    public class Detector
    {
        private delegate void DelegateFont();

        private int _x;
        private int _y;
        private int _direction;
        private const int MaxDirection = 4; //常量表示当前有4个方向
        private readonly Dictionary<string, DelegateFont> _dictionary = new Dictionary<string, DelegateFont>();

        //构造函数
        public Detector(int x, int y, int direction)
        {
            this._x = x;
            this._y = y;
            this._direction = direction;
            _dictionary["0"] = new DelegateFont(NorthAdd);
            _dictionary["1"] = new DelegateFont(EastAdd);
            _dictionary["2"] = new DelegateFont(SouthAdd);
            _dictionary["3"] = new DelegateFont(WestAdd);
        }

        /// <summary>
        /// 逆时针
        /// </summary>
        public void DealLeft()
        {
            _direction = (_direction - 1 + MaxDirection)%MaxDirection;

        }

        /// <summary>
        /// 顺时针
        /// </summary>
        public void DealRight()
        {
            _direction = (_direction + 1)%MaxDirection;
        }

        public void DealFont()
        {
            //没有使用委托实现
            //if (direction == (int)Diretion.North) { ++y; return; }
            //if (direction == (int)Diretion.South) { --y; return; }
            //if (direction == (int)Diretion.West) { --x; return; }
            //++x;
            //使用委托+字典实现
            if (_dictionary.ContainsKey(_direction.ToString()))
            {
                //调用委托
                _dictionary[_direction.ToString()]();
            }

        }

        public void Print()
        {
            Console.WriteLine("x:" + _x + ",y:" + _y);
            Console.WriteLine("Direction:" + (Diretion) _direction);
            Console.ReadKey();
        }

        private void NorthAdd()
        {
            ++_y;
        }

        private void SouthAdd()
        {
            --_y;
        }

        private void WestAdd()
        {
            --_x;
        }

        private void EastAdd()
        {
            ++_x;
        }



    }



    public enum Diretion
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3

    }
}
