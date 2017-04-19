using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tracker.Common
{
    static class Common
    {
        public static dynamic Combine(dynamic item1, dynamic item2)
        {


            // var dictionary1 = (IDictionary<string, object>)item1;
            //   var dictionary2 = (IDictionary<string, object>)item2;
            var result = new ExpandoObject();
            var d = result as IDictionary<string, object>; //work with the Expando as a Dictionary


            Dictionary<string, string> parameters = new Dictionary<string, string>();

            foreach (PropertyInfo property in item1.GetType().GetProperties())
            {
                var val = property.GetValue(item1, null);
                if (val != null)
                {
                    d[property.Name] = val;
                }
            }

            foreach (PropertyInfo property in item2.GetType().GetProperties())
            {
                var val = property.GetValue(item2, null);
                if (val != null)
                {
                    d[property.Name] = val;
                }
            }
            //foreach (var pair in dictionary1.Concat(dictionary2))
            //{
            //    d[pair.Key] = pair.Value;
            //}

            return result;
        }
    }
}
