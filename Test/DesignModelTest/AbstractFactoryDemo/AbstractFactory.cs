using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;

namespace Enterprises.Framework
{
    public abstract class AbstractFactory
    {
        public static AbstractFactory GetInstance()
        {
            string factoryName = ConfigurationSettings.AppSettings["factoryName"];
            string AssemblyName = ConfigurationSettings.AppSettings["AssemblyName"];
            AbstractFactory instance;

            if (factoryName != "")
                instance = (AbstractFactory)Assembly.Load(AssemblyName).CreateInstance(factoryName);
            else
                instance = null;

            return instance;
        }


        public abstract ITax CreateTax();

        public abstract IBonus CreateBonus();
    }
}
