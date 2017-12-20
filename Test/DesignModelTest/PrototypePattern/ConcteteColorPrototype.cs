using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace Enterprises.Framework
{
    [Serializable]
    public class ConcteteColorPrototype : ColorPrototype
    {
        private int _red, _green, _blue;
        public ConcteteColorPrototype(int red, int green, int blue)
        {
            this._red = red;
            this._green = green;
            this._blue = blue;
        }

        public override ColorPrototype Clone(bool Deep)
        {
            if (Deep)
                return CreateDeepCopy();
            else
                return (ColorPrototype)this.MemberwiseClone();
        }

        //实现深拷贝
        public ColorPrototype CreateDeepCopy()
        {
            ColorPrototype colorPrototype;
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, this);
            memoryStream.Position = 0;
            colorPrototype = (ColorPrototype)formatter.Deserialize(memoryStream);
            return colorPrototype;
        }

        public ConcteteColorPrototype Create(int red, int green, int blue)
        {
            return new ConcteteColorPrototype(red, green, blue);
        }

        public void Display(string _colorname)
        {
            Console.WriteLine("{0}'s RGB Values are: {1},{2},{3}", _colorname, _red, _green, _blue);
        }
    }

    public class ColorManager
    {
        Hashtable colors = new Hashtable();
        public ColorPrototype this[string name]
        {
            get
            {
                return (ColorPrototype)colors[name];
            }
            set
            {
                colors.Add(name, value);
            }
        }

        //public static void Main()
        //{
        //    ColorManager colormanager = new ColorManager();
        //    //初始化颜色
        //    colormanager["red"] = new ConcteteColorPrototype(255, 0, 0);

        //    colormanager["green"] = new ConcteteColorPrototype(0, 255, 0);

        //    colormanager["blue"] = new ConcteteColorPrototype(0, 0, 255);

        //    colormanager["angry"] = new ConcteteColorPrototype(255, 54, 0);

        //    colormanager["peace"] = new ConcteteColorPrototype(128, 211, 128);

        //    colormanager["flame"] = new ConcteteColorPrototype(211, 34, 20);

        //    //使用颜色
        //    string colorName = "red";
        //    ConcteteColorPrototype c1 = (ConcteteColorPrototype)colormanager[colorName].Clone(true);
        //    c1.Display(colorName);

        //    colorName = "peace";
        //    ConcteteColorPrototype c2 = (ConcteteColorPrototype)colormanager[colorName].Clone(true);
        //    c2.Display(colorName);



        //    colorName = "flame";
        //    ConcteteColorPrototype c3 = (ConcteteColorPrototype)colormanager[colorName].Clone(true);
        //    c3.Display(colorName);

        //    Console.ReadLine();

        //}

    }

}
