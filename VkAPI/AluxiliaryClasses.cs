using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace VkNet
{
    /// <summary>
    /// Класс с частными методами для работы с аккаунтом
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Контейнер с информацией
        /// </summary>
        private VkData data;

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string ID { get { return data.GetData("data", 0, "user_id"); } }
        /// <summary>
        /// Ключ доступа пользователя (будет использоватся для всех внутрених методв).
        /// Исползуйте метод SaveAccessToken чтобы изменить его
        /// </summary>
        public string AccessToken { get { return data.GetData("data", 0, "access_token"); } }
        /// <summary>
        /// Дата и время создания access_token
        /// </summary>
        public DateTime TokenCreation { get { return GetDate(data.GetData("data", 0, "token_creation")); } }
        /// <summary>
        /// Содержит последнюю, вершувшуюся от сервера, ошибку
        /// </summary>
        public Dictionary<string, string> Error { get { return data.GetData("data", 1); } }
        /// <summary>
        /// Словарь из пользователей, которых пытались забанить в текущей сессии и результаты попыток
        /// </summary>
        public Dictionary<string, string> BanUsers { get { return data.GetData("banUser", 0); } }
        /// <summary>
        /// Список активных рекламных предложений (офферов)
        /// </summary>
        public Dictionary<string, string>[] ActiveOffers { get { return data.GetData("getActiveOffers"); } }
        /// <summary>
        /// Количество активных рекламных предложений (офферов)
        /// </summary>
        public int CountActiveOffers { get { return Convert.ToInt32(data.GetData("data", 0, "countgetActiveOffers")); } }

        /// <summary>
        /// Конструктор класса Account
        /// </summary>
        /// <param name="user_id">идентификатор пользователя</param>
        /// <param name="access_token">ключ доступа пользователя</param>
        /// <param name="date">дата создания ключа доступа</param>
        public Account(string user_id, string access_token, DateTime date)
        {
            data = new VkData();
            data.SetData("data", new Dictionary<string, string>[2] { new Dictionary<string, string>(), new Dictionary<string, string>() });
            data.SetData("data", 0, "user_id", user_id);
            SaveAccessToken(access_token, date);
        }
        /// <summary>
        /// Конструктор класса Account. Если использовать эту перегрузку, то будет использовано системное время
        /// для опредиления даты
        /// </summary>
        /// <param name="user_id">идентификатор пользователя</param>
        /// <param name="access_token">ключ доступа пользователя</param>
        public Account(string user_id, string access_token): this(user_id, access_token, DateTime.Now) { }

        /// <summary>
        /// Сохранить новый токен
        /// </summary>
        /// <param name="access_token">ключ доступа пользователя</param>
        /// <param name="date">дата создания ключа доступа</param>
        public void SaveAccessToken(string access_token, DateTime date)
        {
            data.SetData("data", 0, "access_token", access_token);
            data.SetData("data", 0, "token_creation", date.ToString());
            date.AddDays(1);
            data.SetData("data", 0, "token_expires", date.ToString());
        }
        /// <summary>
        /// Сохранить новый токен. Если использовать эту перегрузку, то будет использовано системное время
        /// для опредиления даты
        /// </summary>
        /// <param name="access_token"></param>
        public void SaveAccessToken(string access_token)
        {
            SaveAccessToken(access_token, DateTime.Now);
        }
        /// <summary>
        /// Возвращает DateTime обьект с данными и переменной date
        /// </summary>
        /// <param name="date">строковое представление даты</param>
        /// <returns>DateTime обьект</returns>
        private DateTime GetDate(string date)
        {
            string[] s = date.Split(new char[] { ' ', '.', ':' });
            int[] f = new int[s.Length];
            for (int i = 0; i < f.Length; i++) { f[i] = Convert.ToInt32(s[i]); }
            return new DateTime(f[2], f[1], f[0], f[3], f[4], f[5]);
        }
        //--------------------| Метод account.banUser |--------------------
        /// <summary>
        /// Добавляет пользователя в черный список
        /// </summary>
        /// <param name="user_id">идентификатор пользователя, которого нужно добавить в черный список</param>
        public void banUser(string user_id)
        {
            Dictionary<string, string> d1 = VkAPI.Account.banUser(user_id, AccessToken);
            if (d1.Keys.Count == 1)
            {
                Dictionary<string, string> d = data.GetData("banUser", 0);
                if (d != null) { data.SetData("banUser", 0, user_id, d1["response"]); }
                else
                {
                    d = new Dictionary<string, string>();
                    d.Add(user_id, d1["response"]);
                    data.SetData("banUser", 0, d);
                }
            }
        }
        //--------------------| Метод account.changePassword |-------------
        /// <summary>
        /// Позволяет сменить пароль пользователя после успешного восстановления доступа к аккаунту через СМС,
        /// используя метод auth.restore.
        /// </summary>
        /// <param name="restore_sid">идентификатор сессии, полученный при восстановлении доступа используя метод auth.restore. (В случае если пароль меняется сразу после восстановления доступа)</param>
        /// <param name="change_password_hash">хэш, полученный при успешной OAuth авторизации по коду полученному по СМС (В случае если пароль меняется сразу после восстановления доступа)</param>
        /// <param name="old_password">текущий пароль пользователя</param>
        /// <param name="new_password">Новый пароль, который будет установлен в качестве текущего</param>
        /// <returns></returns>
        public void changePassword(string restore_sid, string change_password_hash, string old_password,
            string new_password)
        {
            Dictionary<string, string> d = VkAPI.Account.changePassword(restore_sid, change_password_hash,
                old_password, new_password, AccessToken);
            if (d.ContainsKey("error_code"))
            {
                data.SetData("data", 1, d);
            }
            else
            {
                if (d.ContainsKey("token")) { SaveAccessToken(d["token"]); }
            }
        }
        //--------------------| Метод account.getActiveOffers |------------
        /// <summary>
        /// Устанавливает список активных рекламных предложений (офферов),
        /// выполнив которые пользователь сможет получить соответствующее количество голосов на свой счёт внутри приложения
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества офферов</param>
        /// <param name="count">количество офферов, которое необходимо получить</param>
        public void getActiveOffers(int offset, int count)
        {
            Dictionary<string, string>[] d = VkAPI.Account.getActiveOffers(AccessToken, offset, count);
            if (d[0].ContainsKey("error_code"))
            {
                data.SetData("data", 1, d[0]);
            }
            else
            {
                data.SetData("data", 0, "countgetActiveOffers", d[0]["count"]);
                Dictionary<string, string>[] d1 = new Dictionary<string, string>[d.Length - 1];
                for (int i = 0; i < d1.Length; i++)
                {
                    d1[i] = new Dictionary<string, string>(d[i + 1]);
                }
                data.SetData("getActiveOffers", d1);
            }
        }
        /// <summary>
        /// Устанавливает список активных рекламных предложений (офферов),
        /// выполнив которые пользователь сможет получить соответствующее количество голосов на свой счёт внутри приложения
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества офферов</param>
        public void getActiveOffers(int offset)
        {
            getActiveOffers(offset, 100);
        }
        /// <summary>
        /// Устанавливает список активных рекламных предложений (офферов),
        /// выполнив которые пользователь сможет получить соответствующее количество голосов на свой счёт внутри приложения
        /// </summary>
        public void getActiveOffers()
        {
            getActiveOffers(0, 100);
        }
    }
}