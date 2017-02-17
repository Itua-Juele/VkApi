using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace VkAPI
{
    /// <summary>
    /// Класс содержащий информацию о пользователе
    /// </summary>
    public class VkUser
    {
        /// <summary>
        /// id пользователя
        /// </summary>
        public string id;
        /// <summary>
        /// Уникальный ключ пользователя
        /// </summary>
        public string token;
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string FirstName;
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string LastName;
        /// <summary>
        /// Полная информация о пользователе
        /// </summary>
        public Dictionary<string, string> Info;
        /// <summary>
        /// Содержит последний ответ об ошибке сервера
        /// </summary>
        public Dictionary<string, string> Error;
        /// <summary>
        /// Список фоловеров, где 1-й ключ это id фоловера, а дальше информация о нем. 
        /// Используйте метод GetFollowers, чтобы заполнить его
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Followers;
        /// <summary>
        /// Количество подписчиков у данного пользователя
        /// Используйте метод GetFollowers, чтобы заполнить его
        /// </summary>
        public int countFollowers;

        /// <summary>
        /// Конструктор класса VkUser, в котором сразу выясняются параметры FirstName и LastName
        /// </summary>
        /// <param name="id">id пользователя</param>
        public VkUser(string id)
        {
            this.id = id;
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                Dictionary<string, string> data = VK.Users.Get(new string[] { id })[0];
                if (!data.ContainsKey("error_code"))
                {
                    FirstName = data["first_name"];
                    LastName = data["last_name"];
                    Error = new Dictionary<string, string>();
                    Info = new Dictionary<string, string>(data);
                }
                else
                {
                    Error = new Dictionary<string, string>(data);
                    Info = new Dictionary<string, string>();
                }
                data.Clear();
            }
            catch
            {
                Info = new Dictionary<string, string>();
                Error = new Dictionary<string, string>();
            }
        }

        // Метод user.get |------------------------------------------------
        /// <summary>
        /// Возвращает общую информацию о пользователe
        /// </summary>
        /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
        public void Get(string name_case = "nom")
        {
            string url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&name_case={1}",
                    id, name_case);
            Dictionary<string, string> data = VkJson.JsonToListDictionary(url)[0];
            if (data.ContainsKey("error_code"))
            {
                Error = new Dictionary<string, string>(data);
            }
            else
            {
                Info = new Dictionary<string, string>(data);
                foreach (string key in Info.Keys)
                {
                    if (key == "first_name")
                    {
                        FirstName = Info[key];
                    } else if (key == "last_name")
                    {
                        LastName = Info[key];
                    }
                }
            }
            data.Clear();
            data = null;
        }
        /// <summary>
        /// Возвращает расширеную информацию о пользователe
        /// </summary>
        /// <param name="fields">параметры запроса</param>
        /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
        public void Get(string[] fields, string name_case = "nom")
        {
            string url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields={1}&name_case={2}",
                    id, String.Join(",", fields), name_case);
            Dictionary<string, string> data = VkJson.JsonToListDictionary(url)[0];
            if (data.ContainsKey("error_code"))
            {
                Error = new Dictionary<string, string>(data);
            }
            else
            {
                Info = new Dictionary<string, string>(data);
                foreach (string key in Info.Keys)
                {
                    if (key == "first_name")
                    {
                        FirstName = Info[key];
                    }
                    else if (key == "last_name")
                    {
                        LastName = Info[key];
                    }
                }
            }
            data.Clear();
            data = null;
        }

        // Метод users.getFollowers |--------------------------------------
        /// <summary>
        /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
        public void GetFollowers(int offset, int count, string name_case = "nom")
        {
            string url = String.Format("https://api.vk.com/method/users.getFollowers?user_id={0}&offset={1}&count={2}",
                   id, offset, count);
            url += "&fields=first_name,last_name";
            url += "&name_case=" + name_case + "&version=5.62";
            Dictionary<string, string>[] data = VkJson.JsonToListDictionary(url, "items");
            if (data[0].ContainsKey("error_code"))
            {
                Error = new Dictionary<string, string>(data[0]);
            }
            else
            {
                countFollowers = Convert.ToInt32(data[0]["count"]);
                if (data.Length > 1)
                {
                    Followers = new Dictionary<string, Dictionary<string, string>>();
                    for (int i = 1; i < data.Length; i++)
                    {
                        foreach (string key in data[i].Keys)
                        {
                            Followers.Add(data[i][key], new Dictionary<string, string>(data[i]));
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
        /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
        public void GetFollowers(int offset, int count, string[] fields, string name_case = "nom")
        {
            string url = String.Format("https://api.vk.com/method/users.getFollowers?user_id={0}&offset={1}&count={2}",
               id, offset, count);
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
                Error = new Dictionary<string, string>(data[0]);
            }
            else
            {
                countFollowers = Convert.ToInt32(data[0]["count"]);
                if (data.Length > 1)
                {
                    Followers = new Dictionary<string, Dictionary<string, string>>();
                    for (int i = 1; i < data.Length; i++)
                    {
                        foreach (string key in data[i].Keys)
                        {
                            Followers.Add(data[i][key], new Dictionary<string, string>(data[i]));
                            break;
                        }
                    }
                }
            }
        }
    }
}