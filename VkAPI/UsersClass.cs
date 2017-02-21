using System;
using System.Collections.Generic;

namespace VkAPI
{
    /// <summary>
    /// Клас для работы с VK API
    /// </summary>
    public static partial class VK
    {
        /// <summary>
        /// Класс для работы с Users методами
        /// </summary>
        public static class Users
        {
            // Метод user.get |------------------------------------------------
            /// <summary>
            /// Возвращает общую информацию о пользователях
            /// </summary>
            /// <fields name="ids">id пользователей</fields>
            /// <returns></returns>
            public static Dictionary<string, string>[] get(string[] ids)
            {
                return get(ids, null, "nom");
            }
            /// <summary>
            /// Возвращает расширеную информацию о пользователях
            /// </summary>
            /// <fields name="ids">id пользователей</fields>
            /// <fields name="fields">параметры, которые вы хотите получить</fields>
            /// <returns></returns>
            public static Dictionary<string, string>[] get(string[] ids, string[] fields)
            {
                return get(ids, fields, "nom");
            }
            /// <summary>
            /// Возвращает расширеную информацию о пользователях
            /// </summary>
            /// <fields name="ids">id пользователей</fields>
            /// <fields name="fields">параметры, которые вы хотите получить</fields>
            /// <fields name="name_case">падеж для склонения имени и фамилии пользователя</fields>
            /// <returns></returns>
            public static Dictionary<string, string>[] get(string[] ids, string[] fields, string name_case)
            {
                string url;
                if (fields == null)
                {
                    url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields=last_name&name_case={1}&version=5.62",
                    String.Join(",", ids), name_case);
                }
                else
                {
                    url = String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields={1}&name_case={2}&version=5.62",
                    String.Join(",", ids), String.Join(",", fields), name_case);
                }
                return A_ListValues(VkJson.getResponse(url));
            }

            // Метод users.getFollowers |--------------------------------------
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="id">идентификатор пользователя</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getFollowers(string id)
            {
                return getFollowers(id, 0, 5, null,"nom");
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="id">идентификатор пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getFollowers(string id, int offset)
            {
                return getFollowers(id, offset, 5, null, "nom");
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="id">идентификатор пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <param name="count">количество подписчиков, информацию о которых нужно получить (Не больше 100)</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getFollowers(string id, int offset, int count)
            {
                return getFollowers(id, offset, count, null, "nom");
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="id">идентификатор пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <param name="count">количество подписчиков, информацию о которых нужно получить (Не больше 100)</param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getFollowers(string id, int offset, int count,
                string[] fields)
            {
                return getFollowers(id, offset, count, fields, "nom");
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя. 
            /// </summary>
            /// <param name="id">идентификатор пользователя</param>
            /// <param name="offset">смещение, необходимое для выборки определенного подмножества подписчиков (положительное число)</param>
            /// <param name="count">количество подписчиков, информацию о которых нужно получить (Не больше 100)</param>
            /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть</param>
            /// <param name="name_case">падеж для склонения имени и фамилии пользователя</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getFollowers(string id, int offset, int count,
                string[] fields, string name_case)
            {
                string url;
                if (fields == null)
                {
                    url = String.Format("https://api.vk.com/method/users.getFollowers?user_id={0}&offset={1}&count={2}&name_case={3}&version=5.62",
                    id, offset, count, name_case);
                }
                else
                {
                    url = String.Format("https://api.vk.com/method/users.getFollowers?user_id={0}&offset={1}&count={2}&fields={3}&name_case={4}&version=5.62",
                    id, offset, count, String.Join(",", fields), name_case);
                }
                return A_ListInKey("items", VkJson.GetSubDictionary("response", VkJson.getResponse(url)));
            }

            // Метод users.getNearby |-----------------------------------------
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
                return A_ListInKey("items", VkJson.getResponse(url));
            }

            // Метод users.getSubscriptions |----------------------------------
            /// <summary>
            /// Возвращает список идентификаторов пользователей и публичных страниц, которые входят в список подписок пользователя.
            /// </summary>
            /// <param name="id">идентификатор пользователя, подписки которого необходимо получить</param>
            /// <param name="extended">true - возвращает объединенный список, содержащий объекты group и user вместе
            /// false - возвращает список идентификаторов групп и пользователей отдельно</param>
            /// <param name="offset">смещение необходимое для выборки определенного подмножества подписок. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <param name="count">количество подписок, которые необходимо вернуть. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <param name="fields">список дополнительных полей для объектов user и group, которые необходимо вернуть. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getSubscriptions(string id, bool extended, int offset, int count,
                string[] fields)
            {
                string url;
                if (extended)
                {
                    if (fields == null)
                    {
                        url = String.Format("https://api.vk.com/method/users.getSubscriptions?user_id={0}&extended=1" +
                        "&offset={1}&count={2}&version=5.62", id, offset, count);
                    }
                    else
                    {
                        url = String.Format("https://api.vk.com/method/users.getSubscriptions?user_id={0}&extended=1" +
                        "&offset={1}&count={2}&fields=last_name&version=5.62", id, offset, count, fields);
                    }

                    string dataJson = VkJson.getResponse(url);
                    char s = dataJson[12];
                    if (s == '{')
                    {
                        return A_ListInKey("items", VkJson.GetSubDictionary("response", dataJson));
                    } else if (s == '[')
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
                else
                {
                    url = String.Format("https://api.vk.com/method/users.getSubscriptions?user_id={0}&extended=0&version=5.62", id);

                    string datajson = VkJson.GetSubDictionary("response", VkJson.getResponse(url));
                    Dictionary<string, string>[] data = new Dictionary<string, string>[2];

                    string dataSubjson = VkJson.GetSubDictionary("users", datajson);
                    data[0] = new Dictionary<string, string>();
                    data[0].Add("count", VkJson.GetValueDictionary("count", dataSubjson));
                    data[0].Add("items", VkJson.GetListDictionary("items", dataSubjson));

                    dataSubjson = VkJson.GetSubDictionary("groups", datajson);
                    data[1] = new Dictionary<string, string>();
                    data[1].Add("count", VkJson.GetValueDictionary("count", dataSubjson));
                    data[1].Add("items", VkJson.GetListDictionary("items", dataSubjson));

                    return data;
                }
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей и публичных страниц, которые входят в список подписок пользователя.
            /// </summary>
            /// <param name="id">идентификатор пользователя, подписки которого необходимо получить</param>
            /// <param name="extended">true - возвращает объединенный список, содержащий объекты group и user вместе
            /// false - возвращает список идентификаторов групп и пользователей отдельно</param>
            /// <param name="offset">смещение необходимое для выборки определенного подмножества подписок. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <param name="count">количество подписок, которые необходимо вернуть. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getSubscriptions(string id, bool extended, int offset, int count)
            {
                return getSubscriptions(id, extended, offset, count, null);
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей и публичных страниц, которые входят в список подписок пользователя.
            /// </summary>
            /// <param name="id">идентификатор пользователя, подписки которого необходимо получить</param>
            /// <param name="extended">true - возвращает объединенный список, содержащий объекты group и user вместе
            /// false - возвращает список идентификаторов групп и пользователей отдельно</param>
            /// <param name="offset">смещение необходимое для выборки определенного подмножества подписок. 
            /// Этот параметр используется только если передан extended=true</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getSubscriptions(string id, bool extended, int offset)
            {
                return getSubscriptions(id, extended, offset, 5, null);
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей и публичных страниц, которые входят в список подписок пользователя.
            /// </summary>
            /// <param name="id">идентификатор пользователя, подписки которого необходимо получить</param>
            /// <param name="extended">true - возвращает объединенный список, содержащий объекты group и user вместе
            /// false - возвращает список идентификаторов групп и пользователей отдельно</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getSubscriptions(string id, bool extended)
            {
                return getSubscriptions(id, extended, 0, 5, null);
            }
            /// <summary>
            /// Возвращает список идентификаторов пользователей и публичных страниц, 
            /// которые входят в список подписок пользователя.
            /// </summary>
            /// <param name="id">идентификатор пользователя, подписки которого необходимо получить</param>
            /// <returns></returns>
            public static Dictionary<string, string>[] getSubscriptions(string id)
            {
                return getSubscriptions(id, false, 0, 5, null);
            }

            // Метод users.isAppUser |-----------------------------------------
            /// <summary>
            /// Возвращает информацию о том, установил ли пользователь приложение
            /// </summary>
            /// <param name="id">идентификатор пользователя</param>
            /// <param name="access_token">Ключ доступа</param>
            /// <returns></returns>
            public static string isAppUser(string id, string access_token)
            {
                string dataJson = VkJson.getResponse("https://api.vk.com/method/users.isAppUser?user_id=" +
                    id + "&access_token=" + access_token);
                if (VkJson.SearchKey("error", dataJson) == -1)
                {
                    return VkJson.GetValueDictionary("response", dataJson).Substring(1, 1);
                }
                else
                {
                    return "error_code " + VkJson.ResponseError(dataJson)["error_code"];
                }
            }
        }
    }
}

