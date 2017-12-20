using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 从一个对象中拷贝数据到另一个对象
    /// </summary>
    public class EntityUtil
    {
        /// <summary>
        /// 从一个对象中拷贝数据到另一个对象（属性名一致的才会拷贝,用于泛型实体）
        /// </summary>
        public static void CloneDataList<TSource, TDestination>(List<TSource> sourcelist,List<TDestination> destinationlist)
        {
            if (sourcelist != null)
            {
                for (int i = 0; i < sourcelist.Count; i++)
                {
                    destinationlist.Add(Activator.CreateInstance<TDestination>());
                    for (int x = 0; x < destinationlist.Count; x++)
                    {
                        PropertyInfo[] destinationProperties =destinationlist[destinationlist.Count - 1].GetType().GetProperties();
                        //PropertyInfo[] sourceProperties = sourcelist[i].GetType().GetProperties();

                        foreach (PropertyInfo property in destinationProperties)
                        {
                            PropertyInfo sourceProperty = sourcelist[i].GetType().GetProperty(property.Name);
                            if (sourceProperty != null)
                            {
                                object value = sourceProperty.GetValue(sourcelist[i], null);
                                try
                                {
                                    property.SetValue(destinationlist[destinationlist.Count - 1], value, null);
                                }
                                catch
                                {
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 从一个对象中拷贝数据到另一个对象（属性名一致的才会拷贝）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CloneData<TSource, TDestination>(TSource source, TDestination destination)
        {
            if (source != null)
            {
                PropertyInfo[] destinationProperties = destination.GetType().GetProperties();
                // PropertyInfo[] sourceProperties = source.GetType().GetProperties();
                foreach (PropertyInfo property in destinationProperties)
                {
                    PropertyInfo sourceProperty = source.GetType().GetProperty(property.Name);
                    if (sourceProperty != null)
                    {
                        object value = sourceProperty.GetValue(source, null);
                        try
                        {
                            property.SetValue(destination, value, null);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 从一个对象中拷贝数据到另一个对象（属性名一致的才会拷贝,用于泛型实体）
        /// </summary>
        public static void CloneData<TSource, TDestination>(List<TSource> sourcelist, List<TDestination> destinationlist)
        {
            if (sourcelist != null)
            {
                for (int i = 0; i < sourcelist.Count; i++)
                {
                    destinationlist.Add(Activator.CreateInstance<TDestination>());
                    for (int x = 0; x < destinationlist.Count; x++)
                    {
                        PropertyInfo[] destinationProperties =destinationlist[destinationlist.Count - 1].GetType().GetProperties();
                        //PropertyInfo[] sourceProperties = sourcelist[i].GetType().GetProperties();

                        foreach (PropertyInfo property in destinationProperties)
                        {
                            PropertyInfo sourceProperty = sourcelist[i].GetType().GetProperty(property.Name);
                            if (sourceProperty != null)
                            {
                                object value = sourceProperty.GetValue(sourcelist[i], null);
                                try
                                {
                                    property.SetValue(destinationlist[destinationlist.Count - 1], value, null);
                                }
                                catch
                                {
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }



        /// <summary>
        /// 将实体里的属性与值，拷贝出来(如果值超过128个字符,则截取)
        /// </summary>
        /// <param name="destination"></param>
        public static string CloneDataToString(object destination)
        {
            var sb = new StringBuilder();
            PropertyInfo[] destinationProperties = destination.GetType().GetProperties();
            foreach (PropertyInfo item in destinationProperties)
            {
                string name = item.Name.Replace("\r\n", "");
                object value = item.GetValue(destination, null);

                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    if (value != null)
                    {
                        if (value.ToString().Length > 128)
                        {
                            value = value.ToString().Substring(0, 128);
                        }
                    }
                    sb.Append(string.Format("{0}:{1},", name, value));
                }
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
    }
}
