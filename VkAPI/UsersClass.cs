using System;
using System.Collections.Generic;

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
                return VkJson.JsonToListDictionary(url);
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
                return VkJson.JsonToListDictionary(url);
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
                Dictionary<string, string> data = VkJson.JsonToListDictionary(url)[0];
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
                Dictionary<string, string> data = VkJson.JsonToListDictionary(url)[0];
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
                return VkJson.JsonToListDictionary(url, "items");
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
                return VkJson.JsonToListDictionary(url, "items");
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
                Dictionary<string, string>[] data = VkJson.JsonToListDictionary(url, "items");
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
                        for (int i = 1; i < data.Length; i++)
                        {
                            foreach (string key in data[i].Keys)
                            {
                                user.Followers.Add(data[i][key], new Dictionary<string, string>(data[i]));
                                break;
                            }
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
                Dictionary<string, string>[] data = VkJson.JsonToListDictionary(url, "items");
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
                        for (int i = 1; i < data.Length; i++)
                        {
                            foreach (string key in data[i].Keys)
                            {
                                user.Followers.Add(data[i][key], new Dictionary<string, string>(data[i]));
                                break;
                            }
                        }
                    }
                }
            }

            // Метод users.getNearby |-----------------------------------------
            /// <summary>
            /// Индексирует текущее местоположение пользователя и возвращает список пользователей, которые находятся вблизи
            /// </summary>
            /// <param name="access_token">Ключ доступа</param>
            /// <param name="latitude">географическая широта точки, в которой в данный момент находится пользователь, заданная в градусах (от -90 до 90)</param>
            /// <param name="longitude">географическая долгота точки, в которой в данный момент находится пользователь, заданная в градусах (от -180 до 180)</param>
            /// <param name="accuracy">точность текущего местоположения пользователя в метрах</param>
            /// <param name="timeout">время в секундах через которое пользователь должен перестать находиться через поиск по местоположению</param>
            /// <param name="radius">тип радиуса зоны поиска (от 1 до 4) </param>
            /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] GetNearby(string access_token, float latitude, float longitude, int accuracy = 1,
                int timeout = 7200, int radius = 1, string name_case = "nom")
            {
                string url = String.Format("https://api.vk.com/method/users.getNearby?access_token={0}&latitude={1}&longitude={2}",
                   access_token, latitude.ToString().Replace(",", "."), longitude.ToString().Replace(",", "."));
                url += "&accuracy=" + accuracy;
                url += "&timeout=" + timeout;
                url += "&radius=" + radius;
                url += "&name_case=" + name_case + "&version=5.62";
                return VkJson.JsonToListDictionary(url, "items");
            }
            /// <summary>
            /// Индексирует текущее местоположение пользователя и возвращает список пользователей, которые находятся вблизи
            /// </summary>
            /// <param name="access_token">Ключ доступа</param>
            /// <param name="latitude">географическая широта точки, в которой в данный момент находится пользователь, заданная в градусах (от -90 до 90)</param>
            /// <param name="longitude">географическая долгота точки, в которой в данный момент находится пользователь, заданная в градусах (от -180 до 180)</param>
            /// <param name="accuracy">точность текущего местоположения пользователя в метрах</param>
            /// <param name="timeout">время в секундах через которое пользователь должен перестать находиться через поиск по местоположению</param>
            /// <param name="radius">тип радиуса зоны поиска (от 1 до 4) </param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] GetNearby(string access_token, float latitude, float longitude,
                string[] fields, int accuracy = 1, int timeout = 7200, int radius = 1, string name_case = "nom")
            {
                string url = String.Format("https://api.vk.com/method/users.getNearby?access_token={0}&latitude={1}&longitude={2}",
                   access_token, latitude.ToString().Replace(",", "."), longitude.ToString().Replace(",", "."));
                url += "&accuracy=" + accuracy;
                url += "&timeout=" + timeout;
                url += "&radius=" + radius;
                url += "&fields=" + String.Join(",", fields);
                url += "&name_case=" + name_case + "&version=5.62";
                return VkJson.JsonToListDictionary(url, "items");
            }

            // Метод users.getSubscriptions |----------------------------------

            public static void GetSubscriptions(string id)
            {

            }
        }
    }
}

