using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace VkAPI
{
    /// <summary>
    /// Класс для обработки ответов от сервера VK
    /// </summary>
    static class VkJson
    {
        // Методы поддержки |--------------------------------------------------
        /// <summary>
        /// Возвращает значение, содержащееся в данном ключе
        /// </summary>
        /// <param name="key">имя ключа</param>
        /// <param name="json">словарь, в котором ключ содержится</param>
        /// <returns></returns>
        public static string GetValueDictionary(string key, string json)
        {
            int pos = SearchKey(key, json);
            if (pos != -1)
            {
                char s = json[pos + key.Length + 2];
                if (s == '{')
                {
                    json = GetSubDictionary(key, json);
                }
                else if (s == '[')
                {
                    json = GetListDictionary(key, json);
                }
                else
                {
                    json = json.Substring(pos + key.Length + 2).Split(',')[0];
                    s = json[json.Length - 1];
                    if (s == '}')
                    {
                        json = json.Substring(0, json.Length - 1);
                    }
                }
            }
            else
            {
                json = "not found";
            }
            return json;
        }

        /// <summary>
        /// Возваращает позицию данного ключа
        /// </summary>
        /// <param name="key">ключ, позицию которого нужно найти</param>
        /// <param name="json">словарь, в котором ключ содержится</param>
        /// <returns>Позиция данного ключа</returns>
        internal static int SearchKey(string key, string json)
        {
            int position = -1;
            int len = json.Length - key.Length - 1;
            char s;
            char s1 = key[0];
            for (int i = 1; i < len; i++)
            {
                s = json[i];
                if (s == s1)
                {
                    s = json[i - 1];
                    if (s == '"')
                    {
                        s = json[i + key.Length];
                        if (s == '"')
                        {
                            s = json[i + key.Length + 1];
                            if ((s == ':') & (json.Substring(i, key.Length) == key))
                            {
                                position = i;
                                break;
                            }
                        }
                    }
                }
            }
            return position;
        }

        /// <summary>
        /// Возвращает начало позиции и длину первого найденного списка начиная с данной позиции
        /// </summary>
        /// <param name="begin">позиция, от которой идет начало поиска</param>
        /// <param name="str">словарь, в котором ключ содержитсяа</param>
        /// <returns>[начало, длина]</returns>
        internal static int[] GetSettingsList(int begin, string str)
        {
            int braskets = 0;
            int[] settings_list = new int[2] { -1, -1 };
            bool quotes = false;
            char s;
            for (int i = begin; i < str.Length; i++)
            {
                s = str[i];
                if ((s == '"') & !quotes)
                {
                    quotes = true;
                }
                else if (s == '"')
                {
                    quotes = false;
                }
                if (!quotes)
                {
                    if (s == '[')
                    {
                        braskets++;
                        if (braskets == 1)
                        {
                            settings_list[0] = i;
                        }
                    }
                    else if (s == ']')
                    {
                        braskets--;
                        if ((braskets == 0) & (settings_list[0] != -1))
                        {
                            settings_list[1] = i - settings_list[0] + 1;
                            break;
                        }
                    }
                }
            }
            return settings_list;
        }

        /// <summary>
        /// возвращает массив содержащийся по данному ключу
        /// </summary>
        /// <param name="key">ключ, в котором содержится список</param>
        /// <param name="json">словарь, в котором ключ содержится</param>
        /// <returns>Список содержащийся в ключе</returns>
        internal static string GetListDictionary(string key, string json)
        {
            int pos = SearchKey(key, json);
            int[] setList;
            if (pos != -1)
            {
                setList = GetSettingsList(pos - 1, json);
                if ((setList[0] != -1) & (setList[1] != -1))
                {
                    json = json.Substring(setList[0], setList[1]);
                }
            }
            return json;
        }

        /// <summary>
        /// возвращает словарь содержащийся по данному ключу
        /// </summary>
        /// <param name="key">имя ключа</param>
        /// <param name="dictionary">словарь,  в котором содержится ключ</param>
        /// <returns></returns>
        internal static string GetSubDictionary(string key, string dictionary)
        {
            int braskets = 0;
            bool quotes = true;
            int begin = 0;
            int pos = SearchKey(key, dictionary);
            char s;
            if (pos != -1)
            {
                for (int i = pos; i < dictionary.Length; i++)
                {
                    s = dictionary[i];
                    if ((s == '"') & quotes)
                    {
                        quotes = false;
                    }else if ((s == '"') & !quotes)
                    {
                        quotes = true;
                    }

                    if (!quotes)
                    {
                        if (s == '{')
                        {
                            if (braskets == 0)
                            {
                                begin = i;
                            }
                            braskets++;
                        }
                        else if (s == '}')
                        {
                            braskets--;
                            if (braskets == 0)
                            {
                                dictionary = dictionary.Substring(begin, i - begin + 1);
                                break;
                            }
                        }
                    }
                }
            }
            return dictionary;
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
            int quotes = 0;
            for (int i = 1; i < dataJson.Length - 1; i++)
            {
                s = dataJson[i];
                if ((s == '"') & (quotes == 0))
                {
                    quotes++;
                }
                else if (s == '"')
                {
                    quotes--;
                }
                if (quotes == 0)
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
            if (SearchKey("response",errorJson) != -1)
            {
                data.Add("response", errorJson);
            }
            else
            {
                FillDictionary(ref data, errorJson.Substring(9, errorJson.Length - 10), "");
            }
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