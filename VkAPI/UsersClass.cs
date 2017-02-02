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
                Dictionary<string, string>[] data = new Dictionary<string, string>[1];
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
            /// <param name="ids">id пользователей</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] Get(string[] ids)
            {
                string url = String.Format("https://api.vk.com/method/users.get?user_ids={0}",
                    String.Join(",", ids));
                return VkGet(url);
            }
            /// <summary>
            /// Возвращает расширеную информацию о пользователях
            /// </summary>
            /// <param name="ids">id пользователей</param>
            /// <param name="param">параметры, которые вы хотите получить</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] Get(string[] ids, string[] param)
            {
                string url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields={1}",
                    String.Join(",", ids), String.Join(",", param));
                return VkGet(url);
            }
            /// <summary>
            /// Заносит в класс VkUser общую информацию о пользователе
            /// </summary>
            /// <param name="user"></param>
            public static void Get(ref VkUser user)
            {
                string url = "https://api.vk.com/method/users.get?user_ids=" + user.id;
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
            /// <param name="user"></param>
            /// <param name="param">параметры запроса</param>
            public static void Get(ref VkUser user, string[] param)
            {
                string url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields={1}",
                    user.id, String.Join(",", param));
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
        }
    }
}

