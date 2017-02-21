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
    public class User
    {
        /// <summary>
        /// Переменная для хранения всей информации 
        /// </summary>
        private VkData data;

        /// <summary>
        /// идентификатор пользователя
        /// </summary>
        public string ID { get { return data.GetData("data", 0, "id"); } }
        /// <summary>
        /// Ключ доступа
        /// </summary>
        public string AccessToken { get { return data.GetData("data", 0, "access_token"); } set { data.SetData("data", 0, "access_token", value); } }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string FirstName { get { return data.GetData("data",0, "first_name"); } }
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string LastName { get { return data.GetData("data", 0, "last_name"); } }
        /// <summary>
        /// Полная информация о пользователе
        /// </summary>
        public Dictionary<string, string> Info { get { return data.GetData("get", 0); } }
        /// <summary>
        /// Содержит последний ответ об ошибке сервера
        /// </summary>
        public Dictionary<string, string> Error { get { return data.GetData("error", 0); } }
        /// <summary>
        /// Список фоловеров, где 1-й ключ это id фоловера, а дальше информация о нем. 
        /// Используйте метод GetFollowers, чтобы заполнить его
        /// </summary>
        public Dictionary<string, string>[] Followers { get { return data.GetData("getFollowers"); } }
        /// <summary>
        /// Количество подписчиков у данного пользователя
        /// Используйте метод GetFollowers, чтобы заполнить его
        /// </summary>
        public int countFollowers { get { return Convert.ToInt32(data.GetData("data", 0, "countFollowers")); } }
        /// <summary>
        /// Указывает установил ли пользователь приложение
        /// </summary>
        public bool isApp { get { return Convert.ToBoolean(data.GetData("data", 0, "isAppUser")); } }

        // Конструкторы класса |-----------------------------------------------
        /// <summary>
        /// Конструктор класса User 
        /// </summary>
        /// <param name="id">индетификатор пользователя</param>
        /// <param name="access_token">Ключ доступа</param>
        /// <param name="firstName">имя пользователя</param>
        /// <param name="lastName">фамилия пользователя</param>
        public User(string id, string access_token, string firstName, string lastName)
        {
            data = new VkData();
            data.SetData("data", new Dictionary<string, string>[1] { new Dictionary<string, string>() });
            data.SetData("data", 0, "id", id);
            data.SetData("data", 0, "access_token", access_token);
            data.SetData("data", 0, "first_name", firstName);
            data.SetData("data", 0, "last_name", lastName);
            try
            {
                Get();
            }
            catch { }
        }
        /// <summary>
        /// Конструктор класса User 
        /// </summary>
        /// <param name="id">индетификатор пользователя</param>
        /// <param name="access_token">Ключ доступа</param>
        /// <param name="firstName">имя пользователя</param>
        public User(string id, string access_token, string firstName) : this(id, access_token, firstName, "") { }
        /// <summary>
        /// Конструктор класса User 
        /// </summary>
        /// <param name="id">индетификатор пользователя</param>
        /// <param name="access_token">ключ доступа</param>
        public User(string id, string access_token) : this(id, access_token, "", "") { }
        /// <summary>
        /// Конструктор класса User, в котором выясняются параметры FirstName и LastName
        /// </summary>
        /// <param name="id">id пользователя</param>
        public User(string id): this(id, "", "", "") { }

        // Метод user.get |----------------------------------------------------
        /// <summary>
        /// Устанавливает общую информацию о пользователe
        /// </summary>
        public void Get()
        {
            Get(null, "nom");
        }
        /// <summary>
        /// Устанавливает общую информацию о пользователe
        /// </summary>
        /// <param name="fields">параметры запроса</param>
        public void Get(string[] fields)
        {
            Get(fields, "nom");
        }
        /// <summary>
        /// Устанавливает общую информацию о пользователe
        /// </summary>
        /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
        /// <param name="fields">параметры запроса</param>
        public void Get( string[] fields, string name_case)
        {
            Dictionary<string, string>[] d = VK.Users.get(new string[] { ID }, fields, name_case);
            if (d[0].ContainsKey("error_code"))
            {
                data.SetData("error", d);
            }
            else
            {
                data.SetData("get", d);
                foreach (string key in d[0].Keys)
                {
                    if ((key == "first_name") | (key == "last_name"))
                    {
                        data.SetData("data", 0, key, d[0][key]);
                    }
                }
            }
            d[0].Clear();
            d = null;
        }

        // Метод users.getFollowers |------------------------------------------
        /// <summary>
        /// Устанавливает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        public void GetFollowers()
        {
            GetFollowers(0, 5, null, "nom");
        }
        /// <summary>
        /// Устанавливает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        public void GetFollowers(int offset)
        {
            GetFollowers(offset, 5, null, "nom");
        }
        /// <summary>
        /// Устанавливает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        public void GetFollowers(int offset, int count)
        {
            GetFollowers(offset, count, null, "nom");
        }
        /// <summary>
        /// Устанавливает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
        public void GetFollowers(int offset, int count, string[] fields)
        {
            GetFollowers(offset, count, fields, "nom");
        }
        /// <summary>
        /// Устанавливает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
        /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
        public void GetFollowers(int offset, int count, string[] fields, string name_case)
        {
            Dictionary<string, string>[] d = VK.Users.getFollowers(ID, offset, count, fields, name_case);

            if (d[0].ContainsKey("error_code"))
            {
                data.SetData("error", d);
            }
            else
            {
                data.SetData("data", 0, "countFollowers", d[0]["count"]);
                if (d.Length > 1)
                {
                    Dictionary<string, string>[] d1 = new Dictionary<string, string>[d.Length - 1];
                    for (int i = 0; i < d1.Length; i++)
                    {
                        d1[i] = new Dictionary<string, string>(d[i + 1]);
                    }
                    data.SetData("getFollowers", d1);
                }
            }
        }
        /// <summary>
        /// Устанавливает установил ли пользователь приложение
        /// </summary>
        public void isAppUser()
        {
            string s = VK.Users.isAppUser(ID, AccessToken);
            if (s == "1")
            {
                data.SetData("data", 0, "isAppUser", "true");
            } else if(s == "0")
            {
                data.SetData("data", 0, "isAppUser", "false");
            }
        }
    }
}