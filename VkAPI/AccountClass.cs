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
                filter = filter ?? new string[0];
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
                filter = filter ?? new string[0];
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
            //--------------------| Метод account.lookupContacts |-------------
            /// <summary>
            /// Позволяет искать пользователей ВКонтакте, используя телефонные номера, email-адреса, и идентификаторы пользователей в других сервисах
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="contacts">список контактов, разделенных через запятую</param>
            /// <param name="service">строковой идентификатор сервиса, по контактам которого производится поиск. Может принимать следующие значения:
            /// email; phone; twitter; facebook; odnoklassniki; instagram; google.</param>
            /// <param name="mycontact">контакт текущего пользователя в заданном сервисе</param>
            /// <param name="return_all">true — возвращать также контакты, найденные ранее с использованием этого сервиса,
            /// false — возвращать только контакты, найденные с использованием поля contacts</param>
            /// <param name="fields">список дополнительных полей, которые необходимо вернуть</param>
            public static Dictionary<string, Dictionary<string, string>[]> lookupContacts(string access_token,
                string[] contacts, string service, string mycontact, bool return_all, string[] fields)
            {
                string url = String.Format("https://api.vk.com/method/account.lookupContacts?access_token={0}" +
                    "&contacts={1}&service={2}", access_token, String.Join(",", contacts), service);
                if (mycontact != "") { url += "&mycontact=" + mycontact; }
                if (return_all) { url += "&return_all=1"; }
                else { url += "&return_all=0"; }
                fields = fields ?? new string[0];
                if (fields.Length != 0) { url += "&fields=" + String.Join(",", fields); }
                string dataJson = VkJson.getResponse(url + "&version=5.62");
                Dictionary<string, Dictionary<string, string>[]> data = new Dictionary<string, Dictionary<string, string>[]>();
                try
                {
                    if (VkJson.SearchKey("error", dataJson) == -1)
                    {
                        string json = VkJson.GetSubDictionary("response", dataJson);
                        if (VkJson.SearchKey("found", json) != -1) { data.Add("found", A_ListValues("found", json)); }
                        if (VkJson.SearchKey("other ", json) != -1) { data.Add("other ", A_ListValues("other", json)); }
                        return data;
                    }
                    else
                    {
                        data.Add("error", new Dictionary<string, string>[1] { VkJson.ResponseError(dataJson) });
                        return data;
                    }
                }
                catch
                {
                    data.Add("error", new Dictionary<string, string>[1] { new Dictionary<string, string>() });
                    data["error"][0].Add("error", "Failed to process response");
                    data["error"][0].Add("response", dataJson);
                    return data;
                }
            }
            /// <summary>
            /// Позволяет искать пользователей ВКонтакте, используя телефонные номера, email-адреса, и идентификаторы пользователей в других сервисах
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="contacts">список контактов, разделенных через запятую</param>
            /// <param name="service">строковой идентификатор сервиса, по контактам которого производится поиск. Может принимать следующие значения:
            /// email; phone; twitter; facebook; odnoklassniki; instagram; google.</param>
            /// <param name="mycontact">контакт текущего пользователя в заданном сервисе</param>
            /// <param name="return_all">true — возвращать также контакты, найденные ранее с использованием этого сервиса,
            /// false — возвращать только контакты, найденные с использованием поля contacts</param>
            public static Dictionary<string, Dictionary<string, string>[]> lookupContacts(string access_token,
                string[] contacts, string service, string mycontact, bool return_all)
            {
                return lookupContacts(access_token, contacts, service, mycontact, return_all, new string[0]);
            }
            /// <summary>
            /// Позволяет искать пользователей ВКонтакте, используя телефонные номера, email-адреса, и идентификаторы пользователей в других сервисах
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="contacts">список контактов, разделенных через запятую</param>
            /// <param name="service">строковой идентификатор сервиса, по контактам которого производится поиск. Может принимать следующие значения:
            /// email; phone; twitter; facebook; odnoklassniki; instagram; google.</param>
            /// <param name="mycontact">контакт текущего пользователя в заданном сервисе</param>
            /// false — возвращать только контакты, найденные с использованием поля contacts</param>
            public static Dictionary<string, Dictionary<string, string>[]> lookupContacts(string access_token,
                string[] contacts, string service, string mycontact)
            {
                return lookupContacts(access_token, contacts, service, mycontact, false, new string[0]);
            }
            /// <summary>
            /// Позволяет искать пользователей ВКонтакте, используя телефонные номера, email-адреса, и идентификаторы пользователей в других сервисах
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="contacts">список контактов, разделенных через запятую</param>
            /// <param name="service">строковой идентификатор сервиса, по контактам которого производится поиск. Может принимать следующие значения:
            /// email; phone; twitter; facebook; odnoklassniki; instagram; google.</param>
            /// false — возвращать только контакты, найденные с использованием поля contacts</param>
            public static Dictionary<string, Dictionary<string, string>[]> lookupContacts(string access_token,
                string[] contacts, string service)
            {
                return lookupContacts(access_token, contacts, service, "", false, new string[0]);
            }
            //--------------------| Метод account.account.registerDevice |-----
            /// <summary>
            /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
            /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
            /// <param name="device_model">cтроковое название модели устройства</param>
            /// <param name="device_year">год устройства. Установите отрицательное значение, если не хотите использовать его</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// <param name="system_version">строковая версия операционной системы устройства</param>
            /// <param name="settings">сериализованный JSON-объект, описывающий настройки уведомлений в специальном формате </param>
            /// <param name="sandbox">флаг предназначен для iOS устройств.
            /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
            /// <returns></returns>
            public static Dictionary<string, string> registerDevice(string access_token, string token, string device_id,
                string device_model, int device_year, string system_version, PushSettings settings, bool sandbox)
            {
                string url = String.Format("https://api.vk.com/method/account.registerDevice?access_token={0}&token={1}",
                    access_token, token);
                device_model = device_model ?? "";
                if (device_model != "") { url += "&device_model=" + device_model; }
                if (device_year >= 0) { url += "&device_year=" + device_year.ToString(); }
                url += "&device_id" + device_id;
                system_version = system_version ?? "";
                if (system_version != "") { url += "&system_version=" + system_version; }
                string set = settings.GetJson();
                if (set != "") { url += "&settings=" + set; }
                if (sandbox) { url += "&sandbox=1"; }
                string dataJson = VkJson.getResponse(url + "&version=5.62");
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
            /// <summary>
            /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
            /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
            /// <param name="device_model">cтроковое название модели устройства</param>
            /// <param name="device_year">год устройства. Установите отрицательное значение, если не хотите использовать его</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// <param name="system_version">строковая версия операционной системы устройства</param>
            /// <param name="settings">сериализованный JSON-объект, описывающий настройки уведомлений в специальном формате </param>
            /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
            /// <returns></returns>
            public static Dictionary<string, string> registerDevice(string access_token, string token, string device_id,
                string device_model, int device_year, string system_version, PushSettings settings)
            {
                return registerDevice(access_token, token, device_id, device_model, device_year, system_version,
                    settings, false);
            }
            /// <summary>
            /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
            /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
            /// <param name="device_model">cтроковое название модели устройства</param>
            /// <param name="device_year">год устройства. Установите отрицательное значение, если не хотите использовать его</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// <param name="system_version">строковая версия операционной системы устройства</param>
            /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
            /// <returns></returns>
            public static Dictionary<string, string> registerDevice(string access_token, string token, string device_id,
                string device_model, int device_year, string system_version)
            {
                return registerDevice(access_token, token, device_id, device_model, device_year, system_version,
                    new PushSettings(), false);
            }
            /// <summary>
            /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
            /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
            /// <param name="device_model">cтроковое название модели устройства</param>
            /// <param name="device_year">год устройства. Установите отрицательное значение, если не хотите использовать его</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
            /// <returns></returns>
            public static Dictionary<string, string> registerDevice(string access_token, string token, string device_id,
                string device_model, int device_year)
            {
                return registerDevice(access_token, token, device_id, device_model, device_year, "",
                    new PushSettings(), false);
            }
            /// <summary>
            /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
            /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
            /// <param name="device_model">cтроковое название модели устройства</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
            /// <returns></returns>
            public static Dictionary<string, string> registerDevice(string access_token, string token, string device_id,
                string device_model)
            {
                return registerDevice(access_token, token, device_id, device_model, -1, "",
                    new PushSettings(), false);
            }
            /// <summary>
            /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
            /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
            /// <returns></returns>
            public static Dictionary<string, string> registerDevice(string access_token, string token, string device_id)
            {
                return registerDevice(access_token, token, device_id, "", -1, "",
                    new PushSettings(), false);
            }
            //--------------------| Метод account.account.saveProfileInfo |----
            /// <summary>
            /// Редактирует информацию текущего профиля
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="settings">параметры метода</param>
            /// <returns></returns>
            public static Dictionary<string, string> saveProfileInfo(string access_token, SettingsProfileInfo settings)
            {
                string url = "https://api.vk.com/method/account.saveProfileInfo?access_token=" + access_token;
                string param = "";
                if (settings.first_name != "") { param += "&first_name" + settings.first_name; }
                if (settings.last_name != "") { param += "&last_name" + settings.last_name; }
                if (settings.maiden_name != "") { param += "&maiden_name" + settings.maiden_name; }
                if (settings.screen_name != "") { param += "&screen_name" + settings.screen_name; }
                if (settings.cancel_request_id != "") { param += "&cancel_request_id" + settings.cancel_request_id; }
                if (settings.sex != -1) { param += "&sex" + settings.sex.ToString(); }
                if (settings.relation != -1) { param += "&relation" + settings.relation.ToString(); }
                if (settings.relation_partner_id != "") { param += "&relation_partner_id" + settings.relation_partner_id; }
                if (settings.bdate != "") { param += "&bdate" + settings.bdate; }
                if (settings.bdate_visibility != -1) { param += "&bdate_visibility" + settings.bdate_visibility; }
                if (settings.home_town != "") { param += "&home_town" + settings.home_town; }
                if (settings.country_id != -1) { param += "&country_id" + settings.country_id; }
                if (settings.city_id != -1) { param += "&city_id" + settings.city_id; }
                if (settings.status != "") { param += "&status" + settings.status; }
                if (param.Length > 0)
                {
                    string dataJson = VkJson.getResponse(url + param + "&version=5.62");
                    // Попытка обработать и вернуть ответ
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
                else
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("error", "You do not pass any parameters");
                    return data;
                }
            }
            //--------------------| Метод account.account.setInfo |------------
            /// <summary>
            /// Позволяет редактировать информацию о текущем аккаунте.
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="name">имя настройки</param>
            /// <param name="value">значение настройки </param>
            /// <param name="intro">битовая маска, отвечающая за прохождение обучения в мобильных клиентах</param>
            /// <param name="own_posts_default">отображение записей на стене пользователя. Возможные значения:
            /// true — на стене пользователя по умолчанию должны отображаться только собственные записи; false — на стене пользователя должны отображаться все записи.</param>
            /// <param name="no_wall_replies">отключение комментирования записей на стене. Возможные значения: 
            /// true — отключить комментирование записей на стене; false — разрешить комментирование</param>
            public static Dictionary<string, string> setInfo(string access_token, string name, string value,
                string intro, bool own_posts_default, bool no_wall_replies)
            {
                string url = String.Format("https://api.vk.com/method/account.setInfo?access_token={0}&name={1}&value={2}",
                    access_token, name, value);
                intro = intro ?? "";
                if (intro != "") { url += "&intro=" + intro; }
                if (own_posts_default) { url += "&own_posts_default=1"; }
                if (no_wall_replies) { url += "&no_wall_replies=1"; }
                string dataJson = VkJson.getResponse(url + "&version=5.62");
                // Попытка обработать и вернуть ответ
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
            /// Позволяет редактировать информацию о текущем аккаунте.
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="name">имя настройки</param>
            /// <param name="value">значение настройки </param>
            /// <param name="intro">битовая маска, отвечающая за прохождение обучения в мобильных клиентах</param>
            /// <param name="own_posts_default">отображение записей на стене пользователя. Возможные значения:
            /// true — на стене пользователя по умолчанию должны отображаться только собственные записи; false — на стене пользователя должны отображаться все записи.</param>
            public static Dictionary<string, string> setInfo(string access_token, string name, string value,
                string intro, bool own_posts_default)
            {
                return setInfo(access_token, name, value, intro, own_posts_default, false);
            }
            /// <summary>
            /// Позволяет редактировать информацию о текущем аккаунте.
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="name">имя настройки</param>
            /// <param name="value">значение настройки </param>
            /// <param name="intro">битовая маска, отвечающая за прохождение обучения в мобильных клиентах</param>
            public static Dictionary<string, string> setInfo(string access_token, string name, string value,
                string intro)
            {
                return setInfo(access_token, name, value, intro, false, false);
            }
            /// <summary>
            /// Позволяет редактировать информацию о текущем аккаунте.
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="name">имя настройки</param>
            /// <param name="value">значение настройки </param>
            public static Dictionary<string, string> setInfo(string access_token, string name, string value)
            {
                return setInfo(access_token, name, value, "", false, false);
            }
            //--------------------| Метод account.account.setNameInMenu |------
            /// <summary>
            /// Устанавливает короткое название приложения (до 17 символов), которое выводится пользователю в левом меню.
            /// </summary>
            /// <param name="access_token">ключ доступа пользовател</param>
            /// <param name="user_id">идентификатор пользователя</param>
            /// <param name="name">короткое название приложения</param>
            public static Dictionary<string, string> setNameInMenu(string access_token, string user_id, string name)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.setNameInMenu?access_token={0}&user_id={1}&name={2}&version=5.62",
                    access_token, user_id, name));
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
            //--------------------| Метод account.account.setOffline |---------
            /// <summary>
            /// Помечает текущего пользователя как offline (только в текущем приложении)
            /// </summary>
            /// <param name="access_token">ключ доступа пользовател</param>
            public static Dictionary<string, string> setOffline(string access_token)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.setOffline?access_token={0}&version=5.62",
                    access_token));
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
            //--------------------| Метод account.account.setOnline |----------
            /// <summary>
            /// Помечает текущего пользователя как online на 15 минут
            /// </summary>
            /// <param name="access_token">ключ доступа пользовател</param>
            /// <param name="voip">возможны ли видеозвонки для данного устройства</param>
            public static Dictionary<string, string> setOnline(string access_token, bool voip)
            {
                string dataJson;
                if (voip)
                {
                    dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.setOnline?access_token={0}&voip=1&version=5.62",
                    access_token));
                }
                else
                {
                    dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.setOnline?access_token={0}&voip=0&version=5.62",
                    access_token));
                }
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
            /// <summary>
            /// Помечает текущего пользователя как online на 15 минут
            /// </summary>
            /// <param name="access_token">ключ доступа пользовател</param>
            public static Dictionary<string, string> setOnline(string access_token)
            {
                return setOnline(access_token, false);
            }
            //--------------------| Метод account.account.setPushSettings |----
            /// <summary>
            /// Изменяет настройку Push-уведомлений
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// <param name="settings">объект, описывающий настройки уведомлений</param>
            /// <param name="key">ключ уведомления</param>
            /// <param name="value">новое значение уведомления</param>
            public static Dictionary<string, string> setPushSettings(string access_token, string device_id,
                PushSettings settings, string key, PushSettings value)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.setOnline?access_token={0}&device_id={1}&settings={2}&key={3}&value={4}&version=5.62",
                    access_token, device_id, settings.GetJson(), key, value.GetJson()));
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
            //--------------------| Метод account.account.setSilenceMode |-----
            /// <summary>
            /// Отключает push-уведомления на заданный промежуток времени
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// <param name="time">время в секундах на которое требуется отключить уведомления, -1 отключить навсегда</param>
            /// <param name="peer_id">идентификатор назначения</param>
            /// <param name="sound">true — включить звук в этом диалоге, false — отключить звук 
            /// (параметр работает, только если в peer_id передан идентификатор групповой беседы или пользователя)</param>
            public static Dictionary<string, string> setSilenceMode(string access_token, string device_id, int time,
                string peer_id, bool sound)
            {
                string url = String.Format("https://api.vk.com/method/account.setOnline?access_token={0}&device_id={1}&time={2}",
                    access_token, device_id, time.ToString());
                peer_id = peer_id ?? "";
                if (peer_id != "")
                {
                    url += "&peer_id" + peer_id;
                    if (!sound)
                    {
                        url += "&sound=0";
                    }else { url += "&sound=1"; }
                }
                string dataJson = VkJson.getResponse(url + "&version=5.62");
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
            /// <summary>
            /// Отключает push-уведомления на заданный промежуток времени
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// <param name="time">время в секундах на которое требуется отключить уведомления, -1 отключить навсегда</param>
            /// <param name="peer_id">идентификатор назначения</param>
            public static Dictionary<string, string> setSilenceMode(string access_token, string device_id, int time,
                string peer_id)
            {
                return setSilenceMode(access_token, device_id, time, peer_id, true);
            }
            /// <summary>
            /// Отключает push-уведомления на заданный промежуток времени
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// <param name="time">время в секундах на которое требуется отключить уведомления, -1 отключить навсегда</param>
            public static Dictionary<string, string> setSilenceMode(string access_token, string device_id, int time)
            {
                return setSilenceMode(access_token, device_id, time, "", true);
            }
            //--------------------| Метод account.account.unbanUser |----------
            /// <summary>
            /// Убирает пользователя из черного списка
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="user_id">идентификатор пользователя, которого нужно убрать из черного списка</param>
            public static Dictionary<string, string> unbanUser(string access_token, string user_id)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/account.unbanUser?access_token={0}&user_id={1}&version=5.62",
                    access_token, user_id));
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
            //--------------------| Метод account.account.unregisterDevice |---
            /// <summary>
            /// Отписывает устройство от Push уведомлений
            /// </summary>
            /// <param name="access_token">ключ доступа</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            /// <param name="sandbox">флаг предназначен для iOS устройств.
            /// true — отписать устройство, использующего sandbox сервер для отправки push-уведомлений,
            /// false — отписать устройство, не использующее sandbox сервер</param>
            public static Dictionary<string, string> unregisterDevice(string access_token, string device_id, bool sandbox)
            {
                string url = String.Format("https://api.vk.com/method/account.registerDevice?access_token={0}&device_id={1}",
                    access_token, device_id);
                if (sandbox) { url += "&sandbox=1"; }
                string dataJson = VkJson.getResponse(url + "&version=5.62");
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
            /// <summary>
            /// Отписывает устройство от Push уведомлений
            /// </summary>
            /// <param name="access_token">ключ доступа</param>
            /// <param name="device_id">уникальный идентификатор устройства</param>
            public static Dictionary<string, string> unregisterDevice(string access_token, string device_id)
            {
                return unregisterDevice(access_token, device_id, false);
            }
        }
    }

    /// <summary>
    /// Класс для работы с настрой ками пользователя
    /// </summary>
    public class SettingsProfileInfo
    {
        /// <summary>
        /// имя пользователя
        /// </summary>
        public string first_name;
        /// <summary>
        /// фамилия пользователя
        /// </summary>
        public string last_name;
        /// <summary>
        /// девичья фамилия пользователя (только для женского пола)
        /// </summary>
        public string maiden_name;
        /// <summary>
        /// короткое имя страницы
        /// </summary>
        public string screen_name;
        /// <summary>
        /// идентификатор заявки на смену имени, которую необходимо отменить. 
        /// Если передан этот параметр, все остальные параметры игнорируются
        /// </summary>
        public string cancel_request_id;
        /// <summary>
        /// пол пользователя. Возможные значения: 1 — женский; 2 — мужской
        /// </summary>
        public int sex;
        /// <summary>
        /// семейное положение пользователя. Возможные значения: 1 — не женат/не замужем; 2 — есть друг/есть подруга;
        /// 3 — помолвлен/помолвлена; 4 — женат/замужем; 5 — всё сложно; 6 — в активном поиске; 7 — влюблён/влюблена;
        /// 8 — в гражданском браке; 0 — не указано
        /// </summary>
        public int relation;
        /// <summary>
        /// идентификатор пользователя, с которым связано семейное положение
        /// </summary>
        public string relation_partner_id;
        /// <summary>
        /// дата рождения пользователя в формате DD.MM.YYYY, например "15.11.1984"
        /// </summary>
        public string bdate;
        /// <summary>
        /// видимость даты рождения. Возможные значения: 1 — показывать дату рождения; 
        /// 2 — показывать только месяц и день; 0 — не показывать дату рождения.
        /// </summary>
        public int bdate_visibility;
        /// <summary>
        /// родной город пользователя
        /// </summary>
        public string home_town;
        /// <summary>
        /// идентификатор страны пользователя
        /// </summary>
        public int country_id;
        /// <summary>
        /// идентификатор города пользователя
        /// </summary>
        public int city_id;
        /// <summary>
        /// статус пользователя, который также может быть изменен методом status.set
        /// </summary>
        public string status;

        /// <summary>
        /// конструктор класса SettingsProfileInfo
        /// </summary>
        public SettingsProfileInfo()
        {
            first_name = "";
            last_name = "";
            maiden_name = "";
            screen_name = "";
            cancel_request_id = "";
            sex = -1;
            relation = -1;
            relation_partner_id = "";
            bdate = "";
            bdate_visibility = -1;
            home_town = "";
            country_id = -1;
            city_id = -1;
            status = "";
        }
    }
    /// <summary>
    /// Настройка Push уведомлений
    /// </summary>
    public class PushSettings
    {
        /// <summary>
        /// личные сообщения
        /// </summary>
        public string msg { get { return GetValue("msg"); } }
        /// <summary>
        /// групповые чаты
        /// </summary>
        public string chat { get { return GetValue("chat"); } }
        /// <summary>
        /// запрос на добавления в друзья
        /// </summary>
        public string friend { get { return GetValue("friend"); } }
        /// <summary>
        /// регистрация импортированного контакта
        /// </summary>
        public string friend_found { get { return GetValue("friend_found"); } }
        /// <summary>
        /// подтверждение заявки в друзья
        /// </summary>
        public string friend_accepted { get { return GetValue("friend_accepted"); } }
        /// <summary>
        /// ответы
        /// </summary>
        public string reply { get { return GetValue("reply"); } }
        /// <summary>
        ///  комментарии
        /// </summary>
        public string comment { get { return GetValue("comment"); } }
        /// <summary>
        ///  упоминания
        /// </summary>
        public string mention { get { return GetValue("mention"); } }
        /// <summary>
        ///  отметки "Мне нравится"
        /// </summary>
        public string like { get { return GetValue("like"); } }
        /// <summary>
        /// действия "Рассказать друзьям"
        /// </summary>
        public string repost { get { return GetValue("repost"); } }
        /// <summary>
        /// новая запись на стене пользователя
        /// </summary>
        public string wall_post { get { return GetValue("wall_post"); } }
        /// <summary>
        /// размещение предложенной новости
        /// </summary>
        public string wall_publish { get { return GetValue("wall_publish"); } }
        /// <summary>
        /// приглашение в сообщество
        /// </summary>
        public string group_invite { get { return GetValue("group_invite"); } }
        /// <summary>
        /// подтверждение заявки на вступление в группу
        /// </summary>
        public string group_accepted { get { return GetValue("group_accepted"); } }
        /// <summary>
        /// ближайшие мероприятия
        /// </summary>
        public string event_soon { get { return GetValue("event_soon"); } }
        /// <summary>
        /// отметки на фотографиях
        /// </summary>
        public string tag_photo { get { return GetValue("tag_photo"); } }
        /// <summary>
        /// запросы в приложениях
        /// </summary>
        public string app_request { get { return GetValue("app_request"); } }
        /// <summary>
        /// установка приложения
        /// </summary>
        public string sdk_open { get { return GetValue("sdk_open"); } }
        /// <summary>
        /// записи выбранных людей и сообществ
        /// </summary>
        public string new_post { get { return GetValue("new_post "); } }
        /// <summary>
        /// уведомления о днях рождениях на текущую дату
        /// </summary>
        public string birthday { get { return GetValue("birthday "); } }

        private string[] param;
        private Dictionary<string, string> data;
        /// <summary>
        /// Конструктор класса PushSettings
        /// </summary>
        public PushSettings()
        {
            data = new Dictionary<string, string>();
            param = new string[20] { "msg", "chat", "friend", "friend_found", "friend_accepted", "reply", "comment",
                "mention", "like", "repost", "wall_post", "wall_publish", "group_invite", "group_accepted", "event_soon",
                "tag_photo", "app_request", "sdk_open", "new_post", "birthday" };
        }
        // set методы 
        /// <summary>
        /// Устанавливает значение параметра уведомления
        /// </summary>
        /// <param name="key">имя параметра, которому надо установить значение</param>
        /// <param name="status">значение. true - off; false - on</param>
        public void SetValue(string key, bool status)
        {
            if (status) { SetValue(key, "on"); }else { SetValue(key, "off"); }
        }
        /// <summary>
        /// Устанавливает значение параметра уведомления
        /// </summary>
        /// <param name="key">имя параметра, которому надо установить значение</param>
        /// <param name="value">значение, которое нужно установить</param>
        public void SetValue(string key, string value)
        {
            key = key.ToLower();
            foreach (string k in param)
            {
                if (k == key)
                {
                    if (data.ContainsKey(key)) { data[key] = value; }
                    else { data.Add(key, value); }
                }
            }
        }
        /// <summary>
        /// Устанавливает значение параметра уведомления
        /// </summary>
        /// <param name="key">имя параметра, которому надо установить значение</param>
        /// <param name="value">список значений, которое нужно установить</param>
        public void SetValue(string key, string[] value)
        {
            SetValue(key, "[" + String.Join(",", value) + "]");
        }
        // get методы
        private string GetValue(string key)
        {
            if (data.ContainsKey(key)) { return data[key]; }
            else { return ""; }
        }
        /// <summary>
        /// Восвращает параметры класса в виде Json строки
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            string str = "";
            foreach (string key in data.Keys)
            {
                str += String.Format("\",{0}\":{1}");
            }
            if (str.Length > 0)
            {
                return "{" + str.Substring(1) + "}";
            }
            else { return "{}"; }
        }
    }
}