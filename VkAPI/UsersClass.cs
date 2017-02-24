using System;
using System.Collections.Generic;

namespace VkNet
{
    /// <summary>
    /// Клас для работы с VK API
    /// </summary>
    public static partial class VkAPI
    {
        /// <summary>
        /// Класс для работы с Users методами
        /// </summary>
        public static class Users
        {
            //--------------------| Метод user.get |---------------------------
            /// <summary>
            /// Возвращает расширеную информацию о пользователях
            /// </summary>
            /// <fields name="ids">id пользователей</fields>
            /// <fields name="fields">параметры, которые вы хотите получить</fields>
            /// <fields name="name_case">падеж для склонения имени и фамилии пользователя</fields>
            /// <returns></returns>
            public static Dictionary<string, string>[] get(string[] user_ids, string[] fields, string name_case)
            {
                string url;
                if (fields == null)
                {
                    url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields=last_name&name_case={1}&version=5.62",
                    String.Join(",", user_ids), name_case);
                }
                else
                {
                    url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields={1}&name_case={2}&version=5.62",
                    String.Join(",", user_ids), String.Join(",", fields), name_case);
                }
                string dataJson = VkJson.getResponse(url);
                // попытка обработать и вернуть ответ
                try
                {
                    return A_ListValues(dataJson);
                }
                catch
                {
                    Dictionary<string, string>[] data = new Dictionary<string, string>[1] { new Dictionary<string, string>() };
                    data[0].Add("error", "Failed to process line");
                    data[0].Add("response", dataJson);
                    return data;
                }
            }
            /// <summary>
            /// Возвращает общую информацию о пользователях
            /// </summary>
            /// <fields name="ids">id пользователей</fields>
            /// <returns></returns>
            public static Dictionary<string, string>[] get(string[] user_ids)
            {
                return get(user_ids, null, "nom");
            }
            /// <summary>
            /// Возвращает расширеную информацию о пользователях
            /// </summary>
            /// <fields name="ids">id пользователей</fields>
            /// <fields name="fields">параметры, которые вы хотите получить</fields>
            /// <returns></returns>
            public static Dictionary<string, string>[] get(string[] user_ids, string[] fields)
            {
                return get(user_ids, fields, "nom");
            }
            //--------------------| Метод users.getFollowers |-----------------
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="user_id">идентификатор пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <param name="count">количество подписчиков, информацию о которых нужно получить (Не больше 100)</param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getFollowers(string user_id, int offset, int count,
                string[] fields, string name_case)
            {
                if (count > 1000)
                {
                    count = 1000;
                }
                string url;
                if (fields == null)
                {
                    url = String.Format("https://api.vk.com/method/users.getFollowers?user_id={0}&offset={1}&count={2}&name_case={3}&version=5.62",
                    user_id, offset, count, name_case);
                }
                else
                {
                    url = String.Format("https://api.vk.com/method/users.getFollowers?user_id={0}&offset={1}&count={2}&fields={3}&name_case={4}&version=5.62",
                    user_id, offset, count, String.Join(",", fields), name_case);
                }

                string dataJson = VkJson.getResponse(url);
                // попытка обработать и вернуть ответ
                try
                {
                    return A_ListInKey("items", VkJson.GetSubDictionary("response", dataJson));
                }
                catch
                {
                    Dictionary<string, string>[] data = new Dictionary<string, string>[1] { new Dictionary<string, string>() };
                    data[0].Add("error", "Failed to process line");
                    data[0].Add("response", dataJson);
                    return data;
                }
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="user_id">идентификатор пользователя</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getFollowers(string user_id)
            {
                return getFollowers(user_id, 0, 5, null,"nom");
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="user_id">идентификатор пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getFollowers(string user_id, int offset)
            {
                return getFollowers(user_id, offset, 5, null, "nom");
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="user_id">идентификатор пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <param name="count">количество подписчиков, информацию о которых нужно получить (Не больше 100)</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getFollowers(string user_id, int offset, int count)
            {
                return getFollowers(user_id, offset, count, null, "nom");
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="user_id">идентификатор пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <param name="count">количество подписчиков, информацию о которых нужно получить (Не больше 100)</param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getFollowers(string user_id, int offset, int count,
                string[] fields)
            {
                return getFollowers(user_id, offset, count, fields, "nom");
            }
            //--------------------| Метод users.getNearby |--------------------
            /// <summary>
            /// Индексирует текущее местоположение пользователя и возвращает список пользователей, которые находятся вблизи
            /// </summary>
            /// <param name="access_token">Ключ доступа</param>
            /// <param name="latitude">географическая широта точки, в которой в данный момент находится пользователь, заданная в градусах (от -90 до 90)</param>
            /// <param name="longitude">географическая долгота точки, в которой в данный момент находится пользователь, заданная в градусах (от -180 до 180)</param>
            /// <param name="accuracy">точность текущего местоположения пользователя в метрах</param>
            /// <param name="timeout">время в секундах через которое пользователь должен перестать находиться через поиск по местоположению</param>
            /// <param name="radius">тип радиуса зоны поиска (от 1 до 4) </param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getNearby(string access_token, float latitude, float longitude,
                string[] fields, int accuracy, int timeout, int radius, string name_case)
            {
                string url;
                if (fields == null)
                {
                    url = String.Format("https://api.vk.com/method/users.getNearby?access_token={0}&latitude={1}&longitude={2}" +
                    "&accuracy={3}&timeout={4}&radius={5}&name_case={6}&version=5.62",
                   access_token, latitude.ToString().Replace(",", "."), longitude.ToString().Replace(",", "."),
                   accuracy, timeout, radius, name_case);
                }
                else
                {
                    url = String.Format("https://api.vk.com/method/users.getNearby?access_token={0}&latitude={1}&longitude={2}" +
                    "&accuracy={3}&timeout={4}&radius={5}&fields={6}&name_case={7}&version=5.62",
                   access_token, latitude.ToString().Replace(",", "."), longitude.ToString().Replace(",", "."),
                   accuracy, timeout, radius, String.Join(",", fields), name_case);
                }
                string dataJson = VkJson.getResponse(url);
                // попытка обработать и вернуть ответ
                try
                {
                    return A_ListInKey("items", dataJson);
                }
                catch
                {
                    Dictionary<string, string>[] data = new Dictionary<string, string>[1] { new Dictionary<string, string>() };
                    data[0].Add("error", "Failed to process line");
                    data[0].Add("response", dataJson);
                    return data;
                }
            }
            /// <summary>
            /// Индексирует текущее местоположение пользователя и возвращает список пользователей, которые находятся вблизи
            /// </summary>
            /// <param name="access_token">Ключ доступа</param>
            /// <param name="latitude">географическая широта точки, в которой в данный момент находится пользователь, заданная в градусах (от -90 до 90)</param>
            /// <param name="longitude">географическая долгота точки, в которой в данный момент находится пользователь, заданная в градусах (от -180 до 180)</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getNearby(string access_token, float latitude, float longitude)
            {
                return getNearby(access_token, latitude, longitude, null, 1, 7200, 1, "nom");
            }
            /// <summary>
            /// Индексирует текущее местоположение пользователя и возвращает список пользователей, которые находятся вблизи
            /// </summary>
            /// <param name="access_token">Ключ доступа</param>
            /// <param name="latitude">географическая широта точки, в которой в данный момент находится пользователь, заданная в градусах (от -90 до 90)</param>
            /// <param name="longitude">географическая долгота точки, в которой в данный момент находится пользователь, заданная в градусах (от -180 до 180)</param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getNearby(string access_token, float latitude, float longitude,
                string[] fields)
            {
                return getNearby(access_token, latitude, longitude, fields, 1, 7200, 1, "nom");
            }
            /// <summary>
            /// Индексирует текущее местоположение пользователя и возвращает список пользователей, которые находятся вблизи
            /// </summary>
            /// <param name="access_token">Ключ доступа</param>
            /// <param name="latitude">географическая широта точки, в которой в данный момент находится пользователь, заданная в градусах (от -90 до 90)</param>
            /// <param name="longitude">географическая долгота точки, в которой в данный момент находится пользователь, заданная в градусах (от -180 до 180)</param>
            /// <param name="accuracy">точность текущего местоположения пользователя в метрах</param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getNearby(string access_token, float latitude, float longitude,
                string[] fields, int accuracy)
            {
                return getNearby(access_token, latitude, longitude, fields, accuracy, 7200, 1, "nom");
            }
            /// <summary>
            /// Индексирует текущее местоположение пользователя и возвращает список пользователей, которые находятся вблизи
            /// </summary>
            /// <param name="access_token">Ключ доступа</param>
            /// <param name="latitude">географическая широта точки, в которой в данный момент находится пользователь, заданная в градусах (от -90 до 90)</param>
            /// <param name="longitude">географическая долгота точки, в которой в данный момент находится пользователь, заданная в градусах (от -180 до 180)</param>
            /// <param name="accuracy">точность текущего местоположения пользователя в метрах</param>
            /// <param name="timeout">время в секундах через которое пользователь должен перестать находиться через поиск по местоположению</param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getNearby(string access_token, float latitude, float longitude,
                string[] fields, int accuracy, int timeout)
            {
                return getNearby(access_token, latitude, longitude, fields, accuracy, timeout, 1, "nom");
            }
            /// <summary>
            /// Индексирует текущее местоположение пользователя и возвращает список пользователей, которые находятся вблизи
            /// </summary>
            /// <param name="access_token">Ключ доступа</param>
            /// <param name="latitude">географическая широта точки, в которой в данный момент находится пользователь, заданная в градусах (от -90 до 90)</param>
            /// <param name="longitude">географическая долгота точки, в которой в данный момент находится пользователь, заданная в градусах (от -180 до 180)</param>
            /// <param name="accuracy">точность текущего местоположения пользователя в метрах</param>
            /// <param name="timeout">время в секундах через которое пользователь должен перестать находиться через поиск по местоположению</param>
            /// <param name="radius">тип радиуса зоны поиска (от 1 до 4) </param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getNearby(string access_token, float latitude, float longitude,
                string[] fields, int accuracy, int timeout, int radius)
            {
                return getNearby(access_token, latitude, longitude, fields, accuracy, timeout, radius, "nom");
            }
            //--------------------| Метод users.getSubscriptions |-------------
            /// <summary>
            /// Возвращает список идентификаторов пользователей и публичных страниц, которые входят в список подписок пользователя.
            /// </summary>
            /// <param name="user_id">идентификатор пользователя, подписки которого необходимо получить</param>
            /// <param name="extended">true - возвращает объединенный список, содержащий объекты group и user вместе
            /// false - возвращает список идентификаторов групп и пользователей отдельно</param>
            /// <param name="offset">смещение необходимое для выборки определенного подмножества подписок. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <param name="count">количество подписок, которые необходимо вернуть. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <param name="fields">список дополнительных полей для объектов user и group, которые необходимо вернуть. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getSubscriptions(string user_id, bool extended, int offset, int count,
                string[] fields)
            {
                if (count > 1000)
                {
                    count = 1000;
                }
                string url;
                if (extended)
                {
                    if (fields == null)
                    {
                        url = String.Format("https://api.vk.com/method/users.getSubscriptions?user_id={0}&extended=1" +
                        "&offset={1}&count={2}&version=5.62", user_id, offset, count);
                    }
                    else
                    {
                        url = String.Format("https://api.vk.com/method/users.getSubscriptions?user_id={0}&extended=1" +
                        "&offset={1}&count={2}&fields=last_name&version=5.62", user_id, offset, count, fields);
                    }

                    string dataJson = VkJson.getResponse(url);
                    char s = dataJson[12];
                    // попытка обработать и вернуть ответ
                    try
                    {
                        if (s == '{')
                        {
                            return A_ListInKey("items", VkJson.GetSubDictionary("response", dataJson));
                        }
                        else if (s == '[')
                        {
                            return A_ListValues(dataJson);
                        }
                        else
                        {
                            Dictionary<string, string> data = new Dictionary<string, string>();
                            data.Add("error", "not fuond");
                            return new Dictionary<string, string>[1] { data };
                        }
                    }
                    catch
                    {
                        Dictionary<string, string>[] d = new Dictionary<string, string>[1] { new Dictionary<string, string>() };
                        d[0].Add("error", "Failed to process line");
                        d[0].Add("response", dataJson);
                        return d;
                    }
                }
                else
                {
                    url = String.Format("https://api.vk.com/method/users.getSubscriptions?user_id={0}&extended=0&version=5.62", user_id);

                    string dataJson = VkJson.GetSubDictionary("response", VkJson.getResponse(url));
                    Dictionary<string, string>[] data;
                    // попытка обработать ответ
                    try
                    {
                        if (VkJson.SearchKey("error", dataJson) == -1)
                        {
                            data = new Dictionary<string, string>[2];

                            string dataSubjson = VkJson.GetSubDictionary("users", dataJson);
                            data[0] = new Dictionary<string, string>();
                            data[0].Add("count", VkJson.GetValueDictionary("count", dataSubjson));
                            data[0].Add("items", VkJson.GetListDictionary("items", dataSubjson));

                            dataSubjson = VkJson.GetSubDictionary("groups", dataJson);
                            data[1] = new Dictionary<string, string>();
                            data[1].Add("count", VkJson.GetValueDictionary("count", dataSubjson));
                            data[1].Add("items", VkJson.GetListDictionary("items", dataSubjson));
                        }
                        else
                        {
                            data = new Dictionary<string, string>[1] { VkJson.ResponseError(dataJson) };
                        }
                    }
                    catch
                    {
                        data = new Dictionary<string, string>[1] { new Dictionary<string, string>() };
                        data[0].Add("error", "Failed to process line");
                        data[0].Add("response", dataJson);
                    }

                    return data;
                }
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей и публичных страниц, которые входят в список подписок пользователя.
            /// </summary>
            /// <param name="user_id">идентификатор пользователя, подписки которого необходимо получить</param>
            /// <param name="extended">true - возвращает объединенный список, содержащий объекты group и user вместе
            /// false - возвращает список идентификаторов групп и пользователей отдельно</param>
            /// <param name="offset">смещение необходимое для выборки определенного подмножества подписок. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <param name="count">количество подписок, которые необходимо вернуть. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getSubscriptions(string user_id, bool extended, int offset, int count)
            {
                return getSubscriptions(user_id, extended, offset, count, null);
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей и публичных страниц, которые входят в список подписок пользователя.
            /// </summary>
            /// <param name="user_id">идентификатор пользователя, подписки которого необходимо получить</param>
            /// <param name="extended">true - возвращает объединенный список, содержащий объекты group и user вместе
            /// false - возвращает список идентификаторов групп и пользователей отдельно</param>
            /// <param name="offset">смещение необходимое для выборки определенного подмножества подписок. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getSubscriptions(string user_id, bool extended, int offset)
            {
                return getSubscriptions(user_id, extended, offset, 5, null);
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей и публичных страниц, которые входят в список подписок пользователя.
            /// </summary>
            /// <param name="user_id">идентификатор пользователя, подписки которого необходимо получить</param>
            /// <param name="extended">true - возвращает объединенный список, содержащий объекты group и user вместе
            /// false - возвращает список идентификаторов групп и пользователей отдельно</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getSubscriptions(string user_id, bool extended)
            {
                return getSubscriptions(user_id, extended, 0, 5, null);
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей и публичных страниц, 
            /// которые входят в список подписок пользователя.
            /// </summary>
            /// <param name="user_id">идентификатор пользователя, подписки которого необходимо получить</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getSubscriptions(string user_id)
            {
                return getSubscriptions(user_id, false, 0, 5, null);
            }
            //--------------------| Метод users.isAppUser |--------------------
            /// <summary>
            /// Возвращает информацию о том, установил ли пользователь приложение
            /// </summary>
            /// <param name="user_id">идентификатор пользователя</param>
            /// <param name="access_token">Ключ доступа</param>
            /// <returns></returns>
            public static Dictionary<string, string> isAppUser(string user_id, string access_token)
            {
                string dataJson = VkJson.getResponse("https://api.vk.com/method/users.isAppUser?user_id=" +
                    user_id + "&access_token=" + access_token);
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

            //--------------------| Метод users.report |-----------------------
            /// <summary>
            /// Позволяет пожаловаться на пользователя.
            /// </summary>
            /// <param name="user_id">идентификатор пользователя, на которого нужно подать жалобу</param>
            /// <param name="type">тип жалобы, может принимать следующие значения:
            /// porn — порнографияж;
            /// spam — рассылка спама;
            /// insult — оскорбительное поведение;
            /// advertisment — рекламная страница, засоряющая поиск</param>
            /// <param name="comment">комментарий к жалобе на пользователя</param>
            /// <returns>После успешного выполнения возвращает 1.</returns>
            public static Dictionary<string, string> report(string user_id, string type, string comment)
            {
                string dataJson = VkJson.getResponse(String.Format("https://api.vk.com/method/users.report?user_id={0}" +
                    "&type{1}&comment{2}", user_id, type, comment));
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
            /// </summary>
            /// <param name="id">идентификатор пользователя, на которого нужно подать жалобу</param>
            /// <param name="type">тип жалобы, может принимать следующие значения:
            /// porn — порнографияж;
            /// spam — рассылка спама;
            /// insult — оскорбительное поведение;
            /// advertisment — рекламная страница, засоряющая поиск</param>
            /// <returns>После успешного выполнения возвращает 1</returns>
            public static Dictionary<string, string> report(string user_id, string type)
            {
                return report(user_id, type, "");
            }

            //--------------------| Метод users.search |-----------------------
            /// <summary>
            /// Возвращает список пользователей в соответствии с заданным критерием поиска
            /// </summary>
            /// <param name="access_token">ключ доступа пользователя</param>
            /// <param name="settings">параметры поиска</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] search(string access_token, SettingsSearchUsers settings)
            {
                string str = "";
                if (settings.q != null) { str += "&q=" + settings.q; }
                if (settings.sort >= 0) { str += "&sort=" + settings.sort.ToString(); }
                if (settings.offset >= 0) { str += "&offset=" + settings.offset.ToString(); }
                if (settings.count > 1000) { settings.count = 1000; }
                if (settings.count >= 0) { str += "&count=" + settings.count.ToString(); }
                if (settings.fields != null) { str += "&fields=" + String.Join(",", settings.fields); }
                if (settings.city >= 0) { str += "&city=" + settings.city.ToString(); }
                if (settings.country >= 0) { str += "&country=" + settings.country.ToString(); }
                if (settings.hometown != null) { str += "&hometown=" + settings.hometown; }
                if (settings.university_country >= 0) { str += "&university_country=" + settings.university_country.ToString(); }
                if (settings.university >= 0) { str += "&university=" + settings.university.ToString(); }
                if (settings.university_year >= 0) { str += "&university_year=" + settings.university_year.ToString(); }
                if (settings.university_faculty >= 0) { str += "&university_faculty=" + settings.university_faculty.ToString(); }
                if (settings.university_chair >= 0) { str += "&university_chair=" + settings.university_chair.ToString(); }
                if (settings.sex >= 0) { str += "&sex=" + settings.sex.ToString(); }
                if (settings.status >= 0) { str += "&status=" + settings.status.ToString(); }
                if (settings.age_from >= 0) { str += "&age_from=" + settings.age_from.ToString(); }
                if (settings.age_to >= 0) { str += "&age_to=" + settings.age_to.ToString(); }
                if (settings.birth_day >= 0) { str += "&birth_day=" + settings.birth_day.ToString(); }
                if (settings.birth_month >= 0) { str += "&birth_month=" + settings.birth_month.ToString(); }
                if (settings.birth_year >= 0) { str += "&birth_year=" + settings.birth_year.ToString(); }
                if (settings.online) { str += "&online=1"; } else { str += "&online=0"; }
                if (settings.has_photo) { str += "&has_photo=1"; } else { str += "&has_photo=0"; }
                if (settings.school_country >= 0) { str += "&school_country=" + settings.school_country.ToString(); }
                if (settings.school_city >= 0) { str += "&school_city=" + settings.school_city.ToString(); }
                if (settings.school_class >= 0) { str += "&school_class=" + settings.school_class.ToString(); }
                if (settings.school >= 0) { str += "&school=" + settings.school.ToString(); }
                if (settings.school_year >= 0) { str += "&school_year=" + settings.school_year.ToString(); }
                if (settings.religion != null) { str += "&religion=" + settings.religion; }
                if (settings.interests != null) { str += "&interests=" + settings.interests; }
                if (settings.company != null) { str += "&company=" + settings.company; }
                if (settings.position != null) { str += "&position=" + settings.position; }
                if (settings.group_id != null) { str += "&group_id=" + settings.group_id; }
                if (settings.from_list != null) { str += "&from_list=" +String.Join(",",settings.from_list); }

                string url = "https://api.vk.com/method/users.search?access_token=" + access_token + str + "&version=5.62";
                string dataJson = VkJson.getResponse(url);
                try { return A_ListInKey("items", VkJson.GetValueDictionary("response", dataJson)); }
                catch
                {
                    try { return A_ListInKey("response", dataJson); }
                    catch
                    {
                        Dictionary<string, string> d = new Dictionary<string, string>();
                        d.Add("error", "Failed to process response");
                        d.Add("response", dataJson);
                        return new Dictionary<string, string>[1] { d };
                    }
                }
            }
        }
    }

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
        public string FirstName { get { return data.GetData("data", 0, "first_name"); } }
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
        public Dictionary<string, string> Error { get { return data.GetData("data", 1); } }
        /// <summary>
        /// Список фоловеров, где 1-й ключ это id фоловера, а дальше информация о нем. 
        /// Используйте метод getFollowers, чтобы заполнить его
        /// </summary>
        public Dictionary<string, string>[] Followers { get { return data.GetData("getFollowers"); } }
        /// <summary>
        /// Количество подписчиков у данного пользователя
        /// Используйте метод getFollowers, чтобы заполнить его
        /// </summary>
        public int countFollowers { get { return Convert.ToInt32(data.GetData("data", 0, "countFollowers")); } }
        /// <summary>
        /// Указывает установил ли пользователь приложение. 
        /// Используйте метод isAppUser, чтобы заполнить его
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
            data.SetData("data", new Dictionary<string, string>[2] { new Dictionary<string, string>(), new Dictionary<string, string>() });
            data.SetData("data", 0, "id", id);
            data.SetData("data", 0, "access_token", access_token);
            data.SetData("data", 0, "first_name", firstName);
            data.SetData("data", 0, "last_name", lastName);
            try
            {
                get();
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
        public User(string id) : this(id, "", "", "") { }

        //--------------------| Метод user.get |---------------------------
        /// <summary>
        /// Устанавливает общую информацию о пользователe
        /// </summary>
        /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
        /// <param name="fields">параметры запроса</param>
        public void get(string[] fields, string name_case)
        {
            Dictionary<string, string>[] d = VkAPI.Users.get(new string[] { ID }, fields, name_case);
            if (d[0].ContainsKey("error_code"))
            {
                data.SetData("error", 1, d[0]);
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
        /// <summary>
        /// Устанавливает общую информацию о пользователe
        /// </summary>
        public void get()
        {
            get(null, "nom");
        }
        /// <summary>
        /// Устанавливает общую информацию о пользователe
        /// </summary>
        /// <param name="fields">параметры запроса</param>
        public void get(string[] fields)
        {
            get(fields, "nom");
        }
        //--------------------| Метод users.getFollowers |-----------------
        /// <summary>
        /// Устанавливает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
        /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
        public void getFollowers(int offset, int count, string[] fields, string name_case)
        {
            Dictionary<string, string>[] d = VkAPI.Users.getFollowers(ID, offset, count, fields, name_case);

            if (d[0].ContainsKey("error_code"))
            {
                data.SetData("data",1, d[0]);
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
        /// Устанавливает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        public void getFollowers()
        {
            getFollowers(0, 5, null, "nom");
        }
        /// <summary>
        /// Устанавливает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        public void getFollowers(int offset)
        {
            getFollowers(offset, 5, null, "nom");
        }
        /// <summary>
        /// Устанавливает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        public void getFollowers(int offset, int count)
        {
            getFollowers(offset, count, null, "nom");
        }
        /// <summary>
        /// Устанавливает список идентификаторов пользователей, которые являются подписчиками пользователя
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
        /// <param name="count">количество подписчиков, информацию о которых нужно получить</param>
        /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
        public void getFollowers(int offset, int count, string[] fields)
        {
            getFollowers(offset, count, fields, "nom");
        }
        //--------------------| Метод users.isAppUser |--------------------
        /// <summary>
        /// Устанавливает установил ли пользователь приложение
        /// </summary>
        public void isAppUser()
        {
            Dictionary<string, string> d = VkAPI.Users.isAppUser(ID, AccessToken);
            if (d.Keys.Count == 1)
            {
                if (d["response"] == "1")
                {
                    data.SetData("data", 0, "isAppUser", "true");
                }
                else
                {
                    data.SetData("data", 0, "isAppUser", "false");
                }
            }
            else
            {
                data.SetData("data", 1, d);
            }
        }
        //--------------------| Метод users.report |-----------------------
        /// <summary>
        /// Позволяет пожаловаться на пользователя.
        /// Взвращает true, если жалоба отправлена
        /// </summary>
        /// <param name="type">тип жалобы, может принимать следующие значения:
        /// porn — порнографияж;
        /// spam — рассылка спама;
        /// insult — оскорбительное поведение;
        /// advertisment — рекламная страница, засоряющая поиск</param>
        /// <param name="comment">комментарий к жалобе на пользователя</param>
        public bool report(string type, string comment)
        {
            Dictionary<string, string> d = VkAPI.Users.report(ID, type, comment);
            if (d.Keys.Count == 1) { return true; }
            else
            {
                data.SetData("data", 1, d);
                return false;
            }
        }
        /// <summary>
        /// Позволяет пожаловаться на пользователя.
        /// Взвращает true, если жалоба отправлена
        /// </summary>
        /// <param name="type">тип жалобы, может принимать следующие значения:
        /// porn — порнографияж;
        /// spam — рассылка спама;
        /// insult — оскорбительное поведение;
        /// advertisment — рекламная страница, засоряющая поиск</param>
        public bool report(string type)
        {
            return report(type, "");
        }
    }

    /// <summary>
    /// Класс сдержащий параметры поиска
    /// </summary>
    public class SettingsSearchUsers
    {
        /// <summary>
        /// строка поискового запроса. Например, Вася Бабич.
        /// </summary>
        public string q;
        /// <summary>
        /// сортировка результатов. Возможные значения:
        /// 1 — по дате регистрации;
        /// 0 — по популярности.
        /// </summary>
        public int sort;
        /// <summary>
        /// смещение относительно первого найденного пользователя для выборки 
        /// определенного подмножествa (положительное число)
        /// </summary>
        public int offset;
        /// <summary>
        /// количество возвращаемых пользователей.
        /// </summary>
        public int count;
        /// <summary>
        /// список дополнительных полей профилей, которые необходимо вернуть. 
        /// </summary>
        public string[] fields;
        /// <summary>
        /// идентификатор города (положительное число)
        /// </summary>
        public int city;
        /// <summary>
        /// идентификатор страны (положительное число)
        /// </summary>
        public int country;
        /// <summary>
        /// название города строкой
        /// </summary>
        public string hometown;
        /// <summary>
        /// идентификатор страны, в которой пользователи закончили ВУЗ (положительное число)
        /// </summary>
        public int university_country;
        /// <summary>
        /// идентификатор ВУЗа (положительное число)
        /// </summary>
        public int university;
        /// <summary>
        /// год окончания ВУЗа (положительное число)
        /// </summary>
        public int university_year;
        /// <summary>
        /// идентификатор факультета (положительное число)
        /// </summary>
        public int university_faculty;
        /// <summary>
        /// идентификатор кафедры (положительное число)
        /// </summary>
        public int university_chair;
        /// <summary>
        /// пол. Возможные значения:
        /// 1 — женщина;
        /// 2 — мужчина;
        /// 0 — любой(по умолчанию)
        /// </summary>
        public int sex;
        /// <summary>
        /// семейное положение. Возможные значения:
        /// 1 — не женат(не замужем);
        /// 2 — встречается;
        /// 3 — помолвлен(-а);
        /// 4 — женат(замужем);
        /// 5 — всё сложно;
        /// 6 — в активном поиске;
        /// 7 — влюблен(-а);
        /// 8 — в гражданском браке.
        /// </summary>
        public int status;
        /// <summary>
        /// возраст, от (положительное число)
        /// </summary>
        public int age_from;
        /// <summary>
        /// возраст, до (положительное число)
        /// </summary>
        public int age_to;
        /// <summary>
        /// день рождения (положительное число)
        /// </summary>
        public int birth_day;
        /// <summary>
        /// месяц рождения (положительное число)
        /// </summary>
        public int birth_month;
        /// <summary>
        /// год рождения (положительное число)
        /// </summary>
        public int birth_year;
        /// <summary>
        /// учитывать ли статус «онлайн»
        /// </summary>
        public bool online;
        /// <summary>
        /// учитывать ли наличие фото
        /// </summary>
        public bool has_photo;
        /// <summary>
        /// идентификатор страны, в которой пользователи закончили школу (положительное число)
        /// </summary>
        public int school_country;
        /// <summary>
        /// идентификатор города, в котором пользователи закончили школу (положительное число)
        /// </summary>
        public int school_city;
        /// <summary>
        /// буква класса (положительное число)
        /// </summary>
        public int school_class;
        /// <summary>
        /// идентификатор школы, которую закончили пользователи (положительное число)
        /// </summary>
        public int school;
        /// <summary>
        /// год окончания школы (положительное число)
        /// </summary>
        public int school_year;
        /// <summary>
        /// религиозные взгляды
        /// </summary>
        public string religion;
        /// <summary>
        /// интересы
        /// </summary>
        public string interests;
        /// <summary>
        /// название компании, в которой работают пользователи
        /// </summary>
        public string company;
        /// <summary>
        /// название должности
        /// </summary>
        public string position;
        /// <summary>
        /// идентификатор группы, среди пользователей которой необходимо проводить поиск
        /// </summary>
        public string group_id;
        /// <summary>
        /// Разделы среди которых нужно осуществить поиск, перечисленные через запятую. Возможные значения:
        /// friends — искать среди друзей;
        /// subscriptions — искать среди друзей и подписок пользователя
        /// </summary>
        public string[] from_list;

        /// <summary>
        /// Конструктор класса SettingsSearchUsers
        /// </summary>
        public SettingsSearchUsers()
        {
            sort = -1;
            offset = -1;
            count = -1;
            country = -1;
            city = -1;
            university_country = -1;
            university = -1;
            university_year = -1;
            university_faculty = -1;
            university_chair = -1;
            sex = -1;
            status = -1;
            age_from = -1;
            age_to = -1;
            birth_day = -1;
            birth_month = -1;
            birth_year = -1;
            online = false;
            has_photo = false;
            school_country = -1;
            school_city = -1;
            school_class = -1;
            school = -1;
            school_year = -1;
        }
    }
}