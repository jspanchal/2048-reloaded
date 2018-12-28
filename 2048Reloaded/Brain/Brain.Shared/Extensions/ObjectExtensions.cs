using System;
using Newtonsoft.Json;

namespace Brain.Extensions
{

    public static class ObjectExtensions
    {
		public static string ToJson(this object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented);
        }

        public static T FromJson<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

   		public static T Clone<T>(this T source)
        {
            return source.ToJson().FromJson<T>();
        }

        /// <summary>
        /// Prevent Null Object exception
        /// old code:  x.y.z   if x or y return null, then crash
        /// new code:  x.ReadValue(xObj => xObj.y).ReadValue(yObj => yObj.z);        safe to use, and will return default(z Type);
        /// </summary>
        public static TResult ReadValue<T, TResult>(this T obj, Func<T, TResult> func) where T : class
        {
            return ReadValue(obj, func, default(TResult));
        }

        public static TResult ReadValue<T, TResult>(this T obj, Func<T, TResult> func, TResult defaultValue) where T : class
        {
            return (obj != null) ? func(obj) : defaultValue;
        }


    }
}
