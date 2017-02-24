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
            // Метод account.banUser |-----------------------------------------
            /// <summary>
            /// Добавляет пользователя в черный список
            /// </summary>
            /// <param name="user_id">идентификатор пользователя, которого нужно добавить в черный список</param>
            /// <param name="access_token">Ключ доступа пользователя</param>
            public static string banUser(string user_id, string access_token)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.banUser?user_id={0}&access_token={1}&version=5.62",
                    user_id, access_token));
                // Попытка обработать и вернуть ответ
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1)
                    {
                        return VkJson.GetValueDictionary("response", dataJson).Substring(1, 1);
                    }
                    else
                    {
                        return "error_code " + VkJson.ResponseError(dataJson)["error_code"];
                    }
                }
                catch { return "Failed to process line: " + dataJson; }
            }
            // Метод account.changePassword |----------------------------------
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
            // Метод account.getActiveOffers |---------------------------------
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
        }
    }
}