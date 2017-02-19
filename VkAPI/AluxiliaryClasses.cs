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
        private Dictionary<string, Dictionary<string, string>[]> data;

        /// <summary>
        /// идентификатор пользователя
        /// </summary>
        public string ID { get { return data["data"][0]["id"]; } }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string FirstName { get { return data["data"][0]["first_name"]; } }
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string LastName { get { return data["data"][0]["last_name"]; } }
        /// <summary>
        /// Полная информация о пользователе
        /// </summary>
        public Dictionary<string, string> Info { get { return data["get"][0]; } }
        /// <summary>
        /// Содержит последний ответ об ошибке сервера
        /// </summary>
        public Dictionary<string, string> Error { get { return data["error"][0]; } }
        /// <summary>
        /// Список фоловеров, где 1-й ключ это id фоловера, а дальше информация о нем. 
        /// Используйте метод GetFollowers, чтобы заполнить его
        /// </summary>
        public Dictionary<string, string>[] Followers { get { return data["getFollowers"]; } }
        /// <summary>
        /// Количество подписчиков у данного пользователя
        /// Используйте метод GetFollowers, чтобы заполнить его
        /// </summary>
        public int countFollowers { get { return Convert.ToInt32(data["countFollowers"][0]["count"]); } }

        // Конструкторы класса |-----------------------------------------------
        /// <summary>
        /// Конструктор класса User 
        /// </summary>
        /// <param name="id">индетификатор пользователя</param>
        /// <param name="firstName">имя пользователя</param>
        /// <param name="lastName">фамилия пользователя</param>
        public User(string id, string firstName, string lastName)
        {
            data = new Dictionary<string, Dictionary<string, string>[]>();
            data.Add("data", new Dictionary<string, string>[1] { new Dictionary<string, string>() } );
            data["data"][0].Add("id", id);
            data["data"][0].Add("first_name", firstName);
            data["data"][0].Add("last_name", lastName);
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
        /// <param name="firstName">имя пользователя</param>
        public User(string id, string firstName) : this(id, firstName, "") { }
        /// <summary>
        /// Конструктор класса User, в котором выясняются параметры FirstName и LastName
        /// </summary>
        /// <param name="id">id пользователя</param>
        public User(string id): this(id, "", "") { }

        // Методы для работы с информацией |-----------------------------------
        /// <summary>
        /// Возвращает информацию из переменной data
        /// </summary>
        /// <param name="key">ключ, в котором содержится информация</param>
        /// <returns></returns>
        private Dictionary<string, string>[] GetData(string key)
        {
            if (data.ContainsKey(key))
            {
                return data[key];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Заносит информацию в переменную data
        /// </summary>
        /// <param name="key">ключ, по которума надо сохранить информацию</param>
        /// <param name="dicList">Информация, в виде массива словарей</param>
        private void SetData(string key, Dictionary<string, string>[] dicList)
        {
            if (data.ContainsKey(key))
            {
                data[key] = dicList;
            }
            else
            {
                data.Add(key, dicList);
            }
        }

        // Метод user.get |----------------------------------------------------
        /// <summary>
        /// Возвращает общую информацию о пользователe
        /// </summary>
        public void Get()
        {
            Get(null, "nom");
        }
        /// <summary>
        /// Возвращает общую информацию о пользователe
        /// </summary>
        /// <param name="fields">параметры запроса</param>
        public void Get(string[] fields)
        {
            Get(fields, "nom");
        }
        /// <summary>
        /// Возвращает общую информацию о пользователe
        /// </summary>
        /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
        /// <param name="fields">параметры запроса</param>
        public void Get( string[] fields, string name_case)
        {
            Dictionary<string, string>[] d = VK.Users.Get(new string[] { ID }, fields, name_case);
            if (d[0].ContainsKey("error_code"))
            {
                SetData("error", d);
            }
            else
            {
                SetData("get", d);
                foreach (string key in d[0].Keys)
                {
                    if ((key == "first_name") | (key == "last_name"))
                    {
                        data["data"][0][key] = d[0][key];
                    }
                }
            }
            d[0].Clear();
            d = null;
        }

        // Метод users.getFollowers |------------------------------------------
        /// <summary>
        /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        public void GetFollowers()
        {
            GetFollowers(0, 5, null, "nom");
        }
        /// <summary>
        /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        public void GetFollowers(int offset)
        {
            GetFollowers(offset, 5, null, "nom");
        }
        /// <summary>
        /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        public void GetFollowers(int offset, int count)
        {
            GetFollowers(offset, count, null, "nom");
        }
        /// <summary>
        /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
        public void GetFollowers(int offset, int count, string[] fields)
        {
            GetFollowers(offset, count, fields, "nom");
        }
        /// <summary>
        /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
        /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
        public void GetFollowers(int offset, int count, string[] fields, string name_case)
        {
            Dictionary<string, string>[] d = VK.Users.GetFollowers(ID, offset, count, fields, name_case);

            if (d[0].ContainsKey("error_code"))
            {
                SetData("error", d);
            }
            else
            {
                SetData("countFollowers", new Dictionary<string, string>[1] { d[0] });
                if (d.Length > 1)
                {
                    Dictionary<string, string>[] d1 = new Dictionary<string, string>[d.Length - 1];
                    for (int i = 0; i < d1.Length; i++)
                    {
                        d1[i] = new Dictionary<string, string>(d[i + 1]);
                    }
                    SetData("getFollowers", d1);
                }
            }
        }
    }
}