using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TVProgViewer.TVProgApp.Controllers
{
    internal static class ControllerExtensions
    {
        internal static T Mapper<T>(this object obj, T dest) 
        {
            var result = default(T);
            result = dest;
            try
            {
                Type typeSource = obj.GetType();
                Type typeDest = typeof(T);

                foreach (PropertyInfo propInfoDest in typeDest.GetProperties())
                {
                    foreach (PropertyInfo propInfoSource in typeSource.GetProperties())
                    {
                        if (propInfoDest.Name == propInfoSource.Name)
                        {
                            propInfoDest.SetValue(result, propInfoSource.GetValue(obj));
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        internal static List<T> Mapper<T>(this List<object> obj, List<T> dest)
        {
            var result = new List<T>();
            for (int i =0; i<obj.Count; i++)
              dest.Add(default(T));


            foreach (T o in obj)
            {
                foreach (T d in dest)
                {
                    result.Add(o.Mapper<T>(d));
                    break;
                }
            }
            return result;
        }
    }
}
