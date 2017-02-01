using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace VkAPI
{
    /// <summary>
    /// Класс для работы c VK API
    /// </summary>
    public partial class VK
    {
        /// <summary>
        /// Возвращает ответ сервера в json виде
        /// </summary>
        /// <param name="url">адрес по которму происходит запрос</param>
        /// <returns></returns>
        protected static string getResponse(string url)
        {
            WebRequest reqGET = WebRequest.Create(url);
            WebResponse resp = reqGET.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string s = sr.ReadToEnd();
            sr.Dispose();
            return s;
        }

        /// <summary>
        /// Заполняет словарь в соответствии с ответом сервера
        /// </summary>
        /// <param name="data">словар для заполнения</param>
        /// <param name="json">ответ сервера в json формате</param>
        /// <param name="key">значение, которое добавляется перед ключем</param>
        protected static void FillDictionary(ref Dictionary<string, string> data, string json, string key)
        {
            string[] jsonLines = SerializeJson(json).Split('\n');
            data = new Dictionary<string, string>();
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
                    data.Add(key + keyValeu[0], keyValeu[1]);
                }
            }
        }

        protected static string SerializeJson(string str)
        // Преобразует строку в Json формат <string, string>
        {
            int basket = -1;
            int quotes = 0;
            string json = "";
            char s;
            for (int i = 0; i < str.Length; i++)
            {
                json += str[i];

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

                if (((s == ',') & (basket == 0) & (quotes == 0)) | (i == str.Length))
                {
                    json += "\n";
                }
            }
            return json.Substring(1, json.Length - 2);
        }

        /// <summary>
        /// Возвращает список из словарей, содержащихся в строке
        /// </summary>
        /// <param name="dataJson">список из словарей в строке</param>
        /// <returns></returns>
        protected static string[] ListDictionary(string dataJson)
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
        protected static Dictionary<string, string> ResponseError(string errorJson)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            FillDictionary(ref data, errorJson.Substring(9, errorJson.Length - 10), "");
            return data;
        }

    }

    public class Users : VK.User
    {
        /// <summary>
        /// Возвращает расширеную информацию о пользователях
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
        /// Возвращает расширеную информацию о пользователях
        /// </summary>
        /// <param name="ids">id пользователей</param>
        /// <param name="tokenes">ключи безопасности пользователей</param>
        /// <param name="param">параметры, которые вы хотите получить</param>
        /// <returns></returns>
        public static Dictionary<string, string>[] Get(string[] ids, string[] tokenes, string[] param)
        {
            string url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&access_token={1}&fields={2}",
                String.Join(",", ids), String.Join(",", tokenes), String.Join(",", param));
            return VkGet(url);
        }
    }
}