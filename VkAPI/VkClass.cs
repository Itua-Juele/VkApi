using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace VkAPI
{
    public static partial class VK
    {
        /// <summary>
        /// Возвращает список из значений, если ответ сервера имел вид
        /// {"response":[{..},{..}, .. , {..}]}
        /// </summary>
        /// <param name="dataJson">ответ сервера</param>
        /// <returns></returns>
        internal static Dictionary<string, string>[] A_ListValues(string dataJson)
        {
            Dictionary<string, string>[] data;
            if (dataJson.Substring(2, 5) == "error")
            {
                data = new Dictionary<string, string>[1] { VkJson.ResponseError(dataJson) };
            }
            else
            {
                string[] response = VkJson.ListDictionary(dataJson.Substring(12, dataJson.Length - 13));
                data = new Dictionary<string, string>[response.Length];
                for (int i = 0; i < response.Length; i++)
                {
                    VkJson.FillDictionary(ref data[i], response[i], "");
                }
            }
            return data;
        }
        
        /// <summary>
        /// Возвращает список из значений, если ответ сервера имел вид
        /// {.. "count":число, "name_key":[{..},{..}, .. , {..}]}
        /// или {.."name_key":[число,{..},{..}, .. , {..}]}
        /// </summary>
        /// <param name="key">Имя ключа</param>
        /// <param name="dataJson">ответ сервера</param>
        /// <returns></returns>
        internal static Dictionary<string, string>[] A_ListInKey(string key,string dataJson)
        {
            int pos_key = VkJson.SearchKey(key, dataJson);
            Dictionary<string, string>[] data;
            if (pos_key != -1)
            {
                int[] set = VkJson.GetSettingsList(pos_key - 1, dataJson);
                string count = "";
                pos_key = VkJson.SearchKey("count", dataJson);
                if (pos_key != -1)
                {
                    count = VkJson.GetValueDictionary("count", dataJson);
                    dataJson = dataJson.Substring(set[0], set[1]);
                }
                else
                {
                    dataJson = dataJson.Substring(set[0], set[1]);
                    foreach (char x in dataJson)
                    {
                        if (x == ',')
                        {
                            dataJson = "[" + dataJson.Substring(count.Length + 1);
                            count = count.Substring(1);
                            break;
                        }
                        count += x;
                    }
                }
                char s = dataJson[1];
                if (s == '{')
                {
                    string[] users = VkJson.ListDictionary(dataJson);
                    data = new Dictionary<string, string>[1 + users.Length];
                    for (int i = 0; i < users.Length; i++)
                    {
                        VkJson.FillDictionary(ref data[i + 1], users[i]);
                    }
                }
                else
                {
                    data = new Dictionary<string, string>[2];
                    data[1] = new Dictionary<string, string>();
                    data[1].Add("items", dataJson);
                }

                data[0] = new Dictionary<string, string>();
                data[0].Add("count", count);
                return data;
            }
            else
            {
                data = new Dictionary<string, string>[1] { VkJson.ResponseError(dataJson) };
                return data;
            }
        }
    }
}