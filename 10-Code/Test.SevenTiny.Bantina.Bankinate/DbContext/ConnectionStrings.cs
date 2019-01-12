﻿using SevenTiny.Bantina.Configuration;
using SevenTiny.Bantina.Bankinate.Helpers;
using System.Collections.Generic;
using Test.SevenTiny.Bantina.Bankinate.Model;

namespace Test.SevenTiny.Bantina.Bankinate.DbContext
{
    public class ConnectionStrings : ConfigBase<ConnectionStrings>
    {
        public string Key { get; set; }
        public string Value { get; set; }

        private static Dictionary<string, string> dictionary;

        private static void Initial()
        {
            dictionary = new Dictionary<string, string>();
            foreach (var item in Configs)
            {
                dictionary.AddOrUpdate(item.Key, item.Value);
            }
        }

        public static string Get(string key)
        {
            if (dictionary != null && dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            Initial();
            return dictionary.SafeGet(key);
        }
    }
}
