//using Microsoft.Ajax.Utilities;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Newtonsoft.Json;

//namespace QuanLyLaptop.Repository
//{
//    public static class SessionExtension
//    {
//        public static void SetJson(this ISession session, string key, object value)
//        {
//            session.SetString(key, JsonConvert.SerializeObject(value));

//        }
//        public static T GetJson<T>(this ISession session, string key)
//        {
//            var sessionData = session.GetString(key);
//            return sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData);
//        }
//    }
//}