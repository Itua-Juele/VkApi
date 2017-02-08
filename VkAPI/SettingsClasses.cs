using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Используйте метод users.GetFollowers, чтобы заполнить его
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Followers;
        /// <summary>
        /// Количество подписчиков у данного пользователя
        /// Используйте метод users.GetFollowers, чтобы заполнить его
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
    }

    /// <summary>
    /// Класс для обработки ответов от сервера VK
    /// </summary>
    static class VkJson
    {
        /// <summary>
        /// Преобразует json строку в список из словарей
        /// </summary>
        /// <param name="url"></param>
        /// <param name="region">переменная, которой находится словарь</param>
        /// <returns></returns>
        internal static Dictionary<string, string>[] JsonToListDictionary(string url, string region = "response")
        {
            string dataJson = VkJson.getResponse(url);
            if (region == "response")
            {
                return response(dataJson);
            }
            else // items
            {
                return items(dataJson);
            }
        }

        private static Dictionary<string, string>[] response(string dataJson)
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

        private static Dictionary<string, string>[] items(string dataJson)
        {
            char s = dataJson[12];
            Dictionary<string, string>[] data;
            if (s == '[')
            {
                dataJson = dataJson.Substring(13, dataJson.Length - 14);
                string count = "";
                foreach (char x in dataJson)
                {
                    if (x == ',')
                    {
                        break;
                    }
                    count += x;
                }
                string[] users = VkJson.ListDictionary("[" + dataJson.Substring(count.Length + 1));
                data = new Dictionary<string, string>[1 + users.Length];
                data[0] = new Dictionary<string, string>();
                data[0].Add("count", count);
                if (users.Length > 0)
                {
                    for (int i = 0; i < users.Length; i++)
                    {
                        VkJson.FillDictionary(ref data[i + 1], users[i]);
                    }
                }
                return data;
            }
            else
            {
                if (dataJson.Substring(2, 5) == "error")
                {
                    data = new Dictionary<string, string>[1] { VkJson.ResponseError(dataJson) };
                }
                else
                {
                    string count = "0";
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
        }

        // Oбщие методы формирования словаря |---------------------------------
        /// <summary>
        /// Возвращает список из словарей, содержащихся в строке
        /// </summary>
        /// <param name="dataJson">строка вида [словарь, словарь..]</param>
        /// <returns></returns>
        public static string[] ListDictionary(string dataJson)
        {
            int basket = 0;
            string str = "";
            char s;
            for (int i = 1; i < dataJson.Length - 1; i++)
            {
                s = dataJson[i];
                if (s == '{')
                {
                    basket++;
                }
                else if (s == '}')
                {
                    basket--;
                }

                if ((s == ',') & (basket == 0))
                {
                    str += "\n";
                }
                else
                {
                    str += s;
                }
            }
            return str.Split('\n');
        }

        /// <summary>
        /// Преобразует ответ сервера в словарь, если ответом сервера была ошибка
        /// </summary>
        /// <param name="errorJson">ответ сервера</param>
        /// <returns></returns>
        public static Dictionary<string, string> ResponseError(string errorJson)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            FillDictionary(ref data, errorJson.Substring(9, errorJson.Length - 10), "");
            return data;
        }

        /// <summary>
        /// Заполняет словарь в соответствии с ответом сервера
        /// </summary>
        /// <param name="data">словар для заполнения</param>
        /// <param name="json">ответ сервера в json формате, без внешнего словаря response или error</param>
        /// <param name="key">значение, которое добавляется перед ключем</param>
        public static void FillDictionary(ref Dictionary<string, string> data, string json, string key = "")
        {
            string[] jsonLines = SerializeJson(json).Split('\n');
            if (data == null)
            {
                data = new Dictionary<string, string>();
            }
            // Удаляет в строке ненужные символы
            string[] jsonLine;
            char s;
            for (int i = 0; i < jsonLines.Length; i++)
            {
                if (i < jsonLines.Length - 1)
                // Удаляем запятые
                {
                    jsonLines[i] = jsonLines[i].Substring(0, jsonLines[i].Length - 1);
                }

                jsonLine = jsonLines[i].Split(new char[] { ':' }, 2);
                s = jsonLine[1][0];
                if (s == '"')
                {
                    if (jsonLine[1].Length == 2)
                    {
                        jsonLine[1] = "information is absent";
                    }
                    // Удаляем кавычки в значении valeu
                    jsonLines[i] = jsonLine[0] + ":" + jsonLine[1].Substring(1, jsonLine[1].Length - 2);
                }
            }

            // Заполняем словарь
            string[] keyValeu;
            foreach (string line in jsonLines)
            {
                keyValeu = line.Split(new char[] { ':' }, 2);

                if (keyValeu[1].Substring(0, 1) == "{")
                // Если в значении value находится словарь
                {
                    FillDictionary(ref data, keyValeu[1], key + "*");
                }
                else
                {
                    data.Add(key + keyValeu[0].Substring(1, keyValeu[0].Length - 2), keyValeu[1]);
                }
            }
        }

        /// <summary>
        /// Преобразует строку Json в читабельный вид
        /// </summary>
        /// <param name="str">строка json-формата</param>
        /// <returns></returns>
        private static string SerializeJson(string str)
        // Преобразует строку в Json формат <string, string>
        {
            int basket = 0;
            int quotes = 0;
            string json = "";
            char s;
            for (int i = 1; i < str.Length - 1; i++)
            {
                // Отслеживаем уровень, чтобы не попасть во внутрений словарь
                s = str[i];
                if ((s == '{') | (s == '['))
                {
                    basket++;
                }
                else if ((s == '}') | (s == ']'))
                {
                    basket--;
                }

                // Отслеживаем закрытость ковычек
                if ((quotes == 0) & (s == '"'))
                {
                    quotes++;
                }
                else if ((quotes == 1) & (s == '"'))
                {
                    quotes--;
                }

                json += s;
                if ((s == ',') & (basket == 0) & (quotes == 0))
                {
                    json += "\n";
                }
            }
            return json;
        }

        /// <summary>
        /// Возвращает ответ сервера в json виде
        /// </summary>
        /// <param name="url">адрес по которму происходит запрос</param>
        /// <returns></returns>
        public static string getResponse(string url)
        {
            WebRequest reqGET = WebRequest.Create(url);
            WebResponse resp = reqGET.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string s = sr.ReadToEnd();
            sr.Dispose();
            return s.Replace("\\/", "/");
        }
    }
}