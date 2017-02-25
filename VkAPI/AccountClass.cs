using System;
using System.Collections.Generic;

namespace VkNet
{
    public static partial class VkAPI
    {
        /// <summary>
        /// Класс с всеми методами для работы с аккаунтом 
        /// </summary>
        public static class Account
        {
            //--------------------| Метод account.banUser |--------------------
            /// <summary>
            /// Добавляет пользователя в черный список
            /// </summary>
            /// <param name="user_id">идентификатор пользователя, которого нужно добавить в черный список</param>
            /// <param name="access_token">Ключ доступа пользователя</param>
            public static Dictionary<string, string> banUser(string user_id, string access_token)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.banUser?user_id={0}&access_token={1}&version=5.62",
                    user_id, access_token));
                // Попытка обработать и вернуть ответ
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1)
                    {
                        Dictionary<string, string> data = new Dictionary<string, string>();
                        data.Add("response", VkJson.GetValueDictionary("response", dataJson));
                        return data;
                    }
                    else
                    {
                        return VkJson.ResponseError(dataJson);
                    }
                }
                catch
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("error", "Failed to process response");
                    data.Add("response", dataJson);
                    return data;
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
            /// <param name="access_token">Ключ доступа пользователя</param>
            /// <returns></returns>
            public static Dictionary<string, string> changePassword(string restore_sid, string change_password_hash, string old_password,
                string new_password, string access_token)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.banUser?" +
                    "access_token={0}&restore_sid={1}&change_password_hash={2}&old_password={3}&new_password={4}&version=5.62",
                    access_token, restore_sid, change_password_hash, old_password, new_password));
                // Попытка обработать и вернуть ответ
                dataJson = VkJson.GetValueDictionary("response", dataJson);
                Dictionary<string, string> data = new Dictionary<string, string>();
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1)
                    {
                        VkJson.FillDictionary(ref data, dataJson);
                        return data;
                    }
                    else
                    {
                        return VkJson.ResponseError(dataJson);
                    }
                }
                catch
                {
                    data.Add("error", "Failed to process response");
                    data.Add("response", dataJson);
                    return data;
                } 
            }
            //--------------------| Метод account.getActiveOffers |------------
            /// <summary>
            /// Возвращает список активных рекламных предложений (офферов),
            /// выполнив которые пользователь сможет получить соответствующее количество голосов на свой счёт внутри приложения
            /// </summary>
            /// <param name="access_token">Ключ доступа пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества офферов</param>
            /// <param name="count">количество офферов, которое необходимо получить</param>
            public static Dictionary<string, string>[] getActiveOffers(string access_token, int offset, int count)
            {
                if (count > 100) { count = 100; }
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.getActiveOffers?" +
                    "access_token={0}&offset={1}&count={2}&version=5.62", access_token, offset, count));
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1) { return A_ListInKey("response", dataJson); }
                    else
                    {
                        return new Dictionary<string, string>[1] { VkJson.ResponseError(dataJson) };
                    }
                }
                catch
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    d.Add("error", "Failed to process the response");
                    d.Add("response", dataJson);
                    return new Dictionary<string, string>[1] { d };
                }
            }
            /// <summary>
            /// Возвращает список активных рекламных предложений (офферов),
            /// выполнив которые пользователь сможет получить соответствующее количество голосов на свой счёт внутри приложения
            /// </summary>
            /// <param name="access_token">Ключ доступа пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества офферов</param>
            public static Dictionary<string, string>[] getActiveOffers(string access_token, int offset)
            {
                return getActiveOffers(access_token, offset, 100);
            }
            /// <summary>
            /// Возвращает список активных рекламных предложений (офферов),
            /// выполнив которые пользователь сможет получить соответствующее количество голосов на свой счёт внутри приложения
            /// </summary>
            /// <param name="access_token">Ключ доступа пользователя</param>
            public static Dictionary<string, string>[] getActiveOffers(string access_token)
            {
                return getActiveOffers(access_token, 0, 100);
            }
            //--------------------| Метод account.getAppPermissions |----------
            /// <summary>
            /// Получает настройки текущего пользователя в данном приложении.
            /// </summary>
            /// <param name="user_id">идентификатор пользователя, информацию о настройках которого необходимо получить</param>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <returns>После успешного выполнения возвращает битовую маску настроек текущего пользователя в данном приложении</returns>
            public static Dictionary<string, string> getAppPermissions(string user_id, string access_token)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.getAppPermissions?" +
                    "user_id={0}&access_token={1}&version=5.62", user_id, access_token));
                // Попытка обработать и вернуть ответ
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1)
                    {
                        Dictionary<string, string> data = new Dictionary<string, string>();
                        data.Add("response", VkJson.GetValueDictionary("response", dataJson));
                        return data;
                    }
                    else
                    {
                        return VkJson.ResponseError(dataJson);
                    }
                }
                catch
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("error", "Failed to process response");
                    data.Add("response", dataJson);
                    return data;
                }
            }
            //--------------------| Метод account.getBanned |------------------
            /// <summary>
            /// Возвращает список пользователей, находящихся в черном списке
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества черного списка</param>
            /// <param name="count">количество объектов, информацию о которых необходимо вернуть</param>
            /// <returns>Список пользователей, находящихся в черном списке</returns>
            public static Dictionary<string, string>[] getBanned(string access_token, int offset, int count)
            {
                if (count > 200) { count = 200; }
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.getBanned?" +
                    "access_token={0}&offset={1}&count={2}&version=5.62", access_token, offset, count));
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1)
                    {
                        return A_ListInKey("response",dataJson);
                    }
                    else { return new Dictionary<string, string>[1] { VkJson.ResponseError(dataJson) }; }
                }
                catch
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    d.Add("error", "Failed to process the response");
                    d.Add("response", dataJson);
                    return new Dictionary<string, string>[1] { d };
                }
            }
            /// <summary>
            /// Возвращает список пользователей, находящихся в черном списке
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества черного списка</param>
            public static Dictionary<string, string>[] getBanned(string access_token, int offset)
            {
                return getBanned(access_token, offset, 20);
            }
            /// <summary>
            /// Возвращает список пользователей, находящихся в черном списке
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            public static Dictionary<string, string>[] getBanned(string access_token)
            {
                return getBanned(access_token, 0, 20);
            }
            //--------------------| Метод account.getCounters |----------------
            /// <summary>
            /// Возвращает ненулевые значения счетчиков пользователя
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="filter">счетчики, информацию о которых нужно вернуть</param>
            public static Dictionary<string, string> getCounters(string access_token, string[] filter)
            {
                if (filter.Length == 0) { filter = new string[1] { "friends,messages,photos,videos,notes,gifts,events,groups,notifications,sdk,app_requests" }; }
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.getCounters?" +
                    "access_token={0}&filter={1}&version=5.62", access_token, String.Join(",", filter)));
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1)
                    {
                        Dictionary<string, string> data = new Dictionary<string, string>();
                        VkJson.FillDictionary(ref data, VkJson.GetSubDictionary("response", dataJson));
                        return data;
                    }
                    else { return VkJson.ResponseError(dataJson); }
                }
                catch
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("error", "Failed to process response");
                    data.Add("response", dataJson);
                    return data;
                }
            }
            //--------------------| Метод account.getInfo |--------------------
            /// <summary>
            /// Возвращает информацию о текущем аккаунте
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="filter">счетчики, информацию о которых нужно вернуть</param>
            public static Dictionary<string, string> getInfo(string access_token, string[] filter)
            {
                string url = "https://api.vk.com/method/account.getInfo?access_token=" + access_token;
                if (filter.Length != 0) { url += "&filter=" + String.Join(",", filter); }
                url += "&version=5.62";
                string dataJson = VkJson.getResponse(String.Format(url));
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1)
                    {
                        Dictionary<string, string> data = new Dictionary<string, string>();
                        VkJson.FillDictionary(ref data, VkJson.GetSubDictionary("response", dataJson));
                        return data;
                    }
                    else { return VkJson.ResponseError(dataJson); }
                }
                catch
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("error", "Failed to process response");
                    data.Add("response", dataJson);
                    return data;
                }
            }
            /// <summary>
            /// Возвращает информацию о текущем аккаунте
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            public static Dictionary<string, string> getInfo(string access_token)
            {
                return getInfo(access_token, new string[0]);
            }
            //--------------------| Метод account.getProfileInfo |-------------
            /// <summary>
            /// Возвращает информацию о текущем профиле
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            public static Dictionary<string, string> getProfileInfo(string access_token)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.getProfileInfo?" +
                    "access_token={0}&version=5.62", access_token));
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1)
                    {
                        Dictionary<string, string> data = new Dictionary<string, string>();
                        VkJson.FillDictionary(ref data, VkJson.GetSubDictionary("response", dataJson));
                        return data;
                    }
                    else { return VkJson.ResponseError(dataJson); }
                }
                catch
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("error", "Failed to process response");
                    data.Add("response", dataJson);
                    return data;
                }
            }
            //--------------------| Метод account.getPushSettings |------------
            /// <summary>
            /// Позволяет получать настройки Push-уведомлений
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            public static Dictionary<string, string> getPushSettings(string access_token, string device_id)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.getPushSettings?" +
                    "access_token={0}&device_id={1}&version=5.62", access_token, device_id));
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1)
                    {
                        Dictionary<string, string> data = new Dictionary<string, string>();
                        VkJson.FillDictionary(ref data, VkJson.GetSubDictionary("response", dataJson));
                        return data;
                    }
                    else { return VkJson.ResponseError(dataJson); }
                }
                catch
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("error", "Failed to process response");
                    data.Add("response", dataJson);
                    return data;
                }
            }
        }
    }
}