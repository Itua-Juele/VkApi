using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace VkAPI
{
    /// <summary>
    /// Клас для работы с VK API
    /// </summary>
    public static partial class VK
    {
        /// <summary>
        /// Класс для работы с Users методами
        /// </summary>
        public static class Users
        {
            // Метод user.get |------------------------------------------------
            private static Dictionary<string, string>[] VkGet(string url)
            {
                string dataJson = VkJson.getResponse(url);
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
            /// Возвращает общую информацию о пользователях
            /// </summary>
            /// <fields name="ids">id пользователей</fields>
            /// <fields name="name_case">падеж для склонения имени и фамилии пользователя</fields>
            /// <returns></returns>
            public static Dictionary<string, string>[] Get(string[] ids, string name_case = "nom")
            {
                string url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&name_case={1}",
                    String.Join(",", ids), name_case);
                return VkGet(url);
            }
            /// <summary>
            /// Возвращает расширеную информацию о пользователях
            /// </summary>
            /// <fields name="ids">id пользователей</fields>
            /// <fields name="fields">параметры, которые вы хотите получить</fields>
            /// <fields name="name_case">падеж для склонения имени и фамилии пользователя</fields>
            /// <returns></returns>
            public static Dictionary<string, string>[] Get(string[] ids, string[] fields, string name_case = "nom")
            {
                string url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields={1}&name_case={2}",
                    String.Join(",", ids), String.Join(",", fields), name_case);
                return VkGet(url);
            }
            /// <summary>
            /// Заносит в класс VkUser общую информацию о пользователе
            /// </summary>
            /// <fields name="user">экземпляр класса VkUser</fields>
            /// <fields name="name_case">падеж для склонения имени и фамилии пользователя</fields>
            public static void Get(ref VkUser user, string name_case = "nom")
            {
                string url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&name_case={1}",
                    user.id, name_case);
                Dictionary<string, string> data = VkGet(url)[0];
                if (data.ContainsKey("error_code"))
                {
                    user.Error = new Dictionary<string, string>(data);
                }
                else
                {
                    user.Info["first_name"] = data["first_name"];
                    user.Info["last_name"] = data["last_name"];
                    user.LastName = user.Info["last_name"];
                    user.FirstName = user.Info["first_name"];
                }
                data.Clear();
            }
            /// <summary>
            /// Заносит в класс VkUser информацию, указанную параметрами
            /// </summary>
            /// <fields name="user">экземпляр класса VkUser</fields>
            /// <fields name="fields">параметры запроса</fields>
            /// <fields name="name_case">падеж для склонения имени и фамилии пользователя</fields>
            public static void Get(ref VkUser user, string[] fields, string name_case = "nom")
            {
                string url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields={1}&name_case={2}",
                    user.id, String.Join(",", fields), name_case);
                Dictionary<string, string> data = VkGet(url)[0];
                if (data.ContainsKey("error_code"))
                {
                    user.Error = new Dictionary<string, string>(data);
                }
                else
                {
                    foreach (string key in data.Keys)
                    {
                        if (user.Info.ContainsKey(key))
                        {
                            user.Info[key] = data[key];
                        }
                        else
                        {
                            user.Info.Add(key, data[key]);
                        }

                        if (key == "first_name")
                        {
                            user.FirstName = data[key];
                        }
                        else if (key == "last_name")
                        {
                            user.LastName = data[key];
                        }
                    }
                }
                data.Clear();
            }

            // Метод users.getFollowers |--------------------------------------
            private static Dictionary<string, string>[] VkGetFollowers(string url)
            {
                string dataJson = VkJson.getResponse(url);
                Dictionary<string, string>[] data;
                if (dataJson.Substring(2, 5) == "error")
                {
                    data = new Dictionary<string, string>[1] { VkJson.ResponseError(dataJson) };
                }
                else
                {
                    string count = "0";
                    char s;
                    for (int i = 21; i < dataJson.Length; i++)
                    {
                        s = dataJson[i];
                        if (s == ',') // ищем количество фоловеров
                        {
                            count = dataJson.Substring(21, i - 21);
                        }
                        if (s == '[')
                        {
                            dataJson = dataJson.Substring(i, dataJson.Length - i - 2);
                            break;
                        }
                    }
                    s = dataJson[1];
                    if (s == '{')
                    {
                        string[] items = VkJson.ListDictionary(dataJson);
                        data = new Dictionary<string, string>[items.Length + 1];
                        data[0] = new Dictionary<string, string>();
                        data[0].Add("count", count);
                        for (int i = 0; i < items.Length; i++)
                        {
                            //data[i + 1] = new Dictionary<string, string>();
                            VkJson.FillDictionary(ref data[i + 1], items[i], "");
                        }
                    }
                    else
                    {
                        data = new Dictionary<string, string>[1] { new Dictionary<string, string>() };
                        data[0].Add("count", count);
                        data[0].Add("items", dataJson.Substring(1, dataJson.Length - 2));
                    }
                }
                return data;
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="id">идентификатор пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
            /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] GetFollowers(string id, int offset, int count, string name_case = "nom")
            {
                string url = String.Format("https://api.vk.com/method/users.getFollowers?user_id={0}&offset={1}&count={2}&name_case={3}&version=5.62",
                    id, offset, count, name_case);
                return VkGetFollowers(url);
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="id">идентификатор пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] GetFollowers(string id, int offset, int count,
                string[] fields, string name_case = "nom")
            {
                string url = String.Format("https://api.vk.com/method/users.getFollowers?user_id={0}&offset={1}&count={2}&fields={3}&name_case={4}&version=5.62",
                    id, offset, count, String.Join(",", fields), name_case);
                return VkGetFollowers(url);
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя
            /// </summary>
            /// <param name="user">экземпляр класса VkUser</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
            /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
            public static void GetFollowers(ref VkUser user, int offset, int count, string name_case = "nom")
            {
                string url = String.Format("https://api.vk.com/method/users.getFollowers?user_id={0}&offset={1}&count={2}",
                   user.id, offset, count);
                url += "&fields=first_name,last_name";
                url += "&name_case=" + name_case + "&version=5.62";
                Dictionary<string, string>[] data = VkGetFollowers(url);
                if (data[0].ContainsKey("error_code"))
                {
                    user.Error = new Dictionary<string, string>(data[0]);
                }
                else
                {
                    user.countFollowers = Convert.ToInt32(data[0]["count"]);
                    if (data.Length > 1)
                    {
                        user.Followers = new Dictionary<string, Dictionary<string, string>>();
                        string key;
                        for (int i = 1; i < data.Length; i++)
                        {
                            key = data[i].Keys.ToArray<string>()[0];
                            user.Followers.Add(data[i][key], new Dictionary<string, string>(data[i]));
                        }
                    }
                }
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя
            /// </summary>
            /// <param name="user">экземпляр класса VkUser</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
            public static void GetFollowers(ref VkUser user, int offset, int count, string[] fields, string name_case = "nom")
            {
                string url = String.Format("https://api.vk.com/method/users.getFollowers?user_id={0}&offset={1}&count={2}",
                   user.id, offset, count);
                if (fields.Length == 0)
                {
                    url += "&fields=first_name,last_name";
                }
                else
                {
                    url += "&fields=" + String.Join(",", fields);
                }
                url += "&name_case=" + name_case + "&version=5.62";
                Dictionary<string, string>[] data = VkGetFollowers(url);
                if (data[0].ContainsKey("error_code"))
                {
                    user.Error = new Dictionary<string, string>(data[0]);
                }
                else
                {
                    user.countFollowers = Convert.ToInt32(data[0]["count"]);
                    if (data.Length > 1)
                    {
                        user.Followers = new Dictionary<string, Dictionary<string, string>>();
                        string key;
                        for (int i = 1; i < data.Length; i++)
                        {
                            key = data[i].Keys.ToArray<string>()[0];
                            user.Followers.Add(data[i][key], new Dictionary<string, string>(data[i]));
                        }
                    }
                }
            }
        }
    }
}

