using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace VkAPI
{
    public partial class VK
    {
        /// <summary>
        /// ID пользователя или группы
        /// </summary>
        public string id;
        /// <summary>
        /// Ключ доступа, разрешаюший проводить действия от имени поользователя
        /// </summary>
        public string token;
        public string Name;
        public string FirsName;

        public VK() { }
        /// <summary>
        /// Инициализирует класс с информацией о пользователе
        /// </summary>
        /// <param name="id">id пользователя</param>
        public VK(string id)
        {
            this.id = id;
        }
        /// <summary>
        /// Инициализирует класс с информацией о пользователе
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <param name="token">ключ доступа, для совершения действий от имени пользователя</param>
        public VK(string id, string token)
        {
            this.id = id;
            this.token = token;
        }

        public class User : VK
        {
            protected static Dictionary<string, string>[] VkGet(string url)
            {
                string dataJson = getResponse(url);
                Dictionary<string, string>[] data = new Dictionary<string, string>[1];
                if (dataJson.Substring(2, 5) == "error")
                {
                    data = new Dictionary<string, string>[1] { ResponseError(dataJson) };
                }
                else
                {
                    string[] response = ListDictionary(dataJson.Substring(12, dataJson.Length - 13));
                    data = new Dictionary<string, string>[response.Length];
                    for (int i = 0; i < response.Length; i++)
                    {
                        FillDictionary(ref data[i], response[i], "");
                    }
                }
                return data;
            }
        }
    }
}

