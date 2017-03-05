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
        /// Используйте метод getActiveOffers, чтобы заполнить его
        /// </summary>
        public int CountActiveOffers { get { return Convert.ToInt32(data.GetData("data", 0, "countgetActiveOffers")); } }
        /// <summary>
        /// Cписок пользователей, находящихся в черном списке.
        /// Используйте метод getBanned, чтобы заполнить его
        /// </summary>
        public Dictionary<string, string>[] Banned { get { return data.GetData("getBanned"); } }
        /// <summary>
        /// Количество пользователей, находящихся в черном списке.
        /// Используйте метод getBanned, чтобы заполнить его
        /// </summary>
        public int CountBanned { get { return Convert.ToInt32(data.GetData("data", 0, "countgetBanned")); } }
        /// <summary>
        /// Информацию о текущем аккаунте
        /// </summary>
        public Dictionary<string, string> Info { get { return data.GetData("data", 2); } }
        /// <summary>
        /// Информацию о текущем профиле
        /// </summary>
        public Dictionary<string, string> ProfileInfo { get { return data.GetData("data", 3); } }

        /// <summary>
        /// Конструктор класса Account
        /// </summary>
        /// <param name="user_id">идентификатор пользователя</param>
        /// <param name="access_token">ключ доступа пользователя</param>
        /// <param name="date">дата создания ключа доступа</param>
        public Account(string user_id, string access_token, DateTime date)
        {
            data = new VkData();
            data.SetData("data", new Dictionary<string, string>[2] { new Dictionary<string, string>(),
                new Dictionary<string, string>() });
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
        /// Определяет список активных рекламных предложений (офферов),
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
        /// Определяет список активных рекламных предложений (офферов),
        /// выполнив которые пользователь сможет получить соответствующее количество голосов на свой счёт внутри приложения
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества офферов</param>
        public void getActiveOffers(int offset)
        {
            getActiveOffers(offset, 100);
        }
        /// <summary>
        /// Определяет список активных рекламных предложений (офферов),
        /// выполнив которые пользователь сможет получить соответствующее количество голосов на свой счёт внутри приложения
        /// </summary>
        public void getActiveOffers()
        {
            getActiveOffers(0, 100);
        }
        //--------------------| Метод account.getBanned |------------------
        /// <summary>
        /// Определяет список пользователей, находящихся в черном списке
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества черного списка</param>
        /// <param name="count">количество объектов, информацию о которых необходимо вернуть</param>
        public void getBanned(int offset, int count)
        {
            Dictionary<string, string>[] d = VkAPI.Account.getBanned(AccessToken, offset, count);
            if (d[0].ContainsKey("error_code"))
            {
                data.SetData("data", 1, d[0]);
            }
            else
            {
                data.SetData("data", 0, "countgetBanned", d[0]["count"]);
                Dictionary<string, string>[] d1 = new Dictionary<string, string>[d.Length - 1];
                for (int i = 0; i < d1.Length; i++)
                {
                    d1[i] = new Dictionary<string, string>(d[i + 1]);
                }
                data.SetData("getBanned", d1);
            }
        }
        /// <summary>
        /// Определяет список пользователей, находящихся в черном списке
        /// </summary>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества черного списка</param>
        public void getBanned(int offset) { getBanned(offset, 20); }
        /// <summary>
        /// Определяет список пользователей, находящихся в черном списке
        /// </summary>
        public void getBanned() { getBanned(0, 20); }
        //--------------------| Метод account.getCounters |----------------
        /// <summary>
        /// Возвращает ненулевые значения счетчиков пользователя
        /// </summary>
        /// <param name="filter">счетчики, информацию о которых нужно вернуть</param>
        public Dictionary<string, string> getCounters(string[] filter)
        {
            return VkAPI.Account.getCounters(AccessToken, filter);
        }
        //--------------------| Метод account.getInfo |--------------------
        /// <summary>
        /// Возвращает информацию о текущем аккаунте
        /// </summary>
        /// <param name="filter">список полей, которые необходимо вернуть</param>
        public void getInfo(string[] filter) { data.SetData("data", 2, VkAPI.Account.getInfo(AccessToken, filter)); }
        /// <summary>
        /// Возвращает информацию о текущем аккаунте
        /// </summary>
        public void getInfo() { data.SetData("data", 2, VkAPI.Account.getInfo(AccessToken, new string[0])); }
        //--------------------| Метод account.getProfileInfo |-------------
        /// <summary>
        /// Определяет информацию о текущем профиле
        /// </summary>
        public void getProfileInfo() { data.SetData("data", 3, VkAPI.Account.getProfileInfo(AccessToken)); }
        //--------------------| Метод account.getPushSettings |------------
        /// <summary>
        /// Позволяет получать настройки Push-уведомлений
        /// </summary>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        public Dictionary<string, string> getPushSettings(string device_id)
        {
            Dictionary<string, string> d = VkAPI.Account.getPushSettings(AccessToken, device_id);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
            return d;
        }
        //--------------------| Метод account.lookupContacts |-------------
        /// <summary>
        /// Позволяет искать пользователей ВКонтакте, используя телефонные номера, email-адреса, и идентификаторы пользователей в других сервисах
        /// </summary>
        /// <param name="contacts">список контактов, разделенных через запятую</param>
        /// <param name="service">строковой идентификатор сервиса, по контактам которого производится поиск. Может принимать следующие значения:
        /// email; phone; twitter; facebook; odnoklassniki; instagram; google.</param>
        /// <param name="mycontact">контакт текущего пользователя в заданном сервисе</param>
        /// <param name="return_all">true — возвращать также контакты, найденные ранее с использованием этого сервиса,
        /// false — возвращать только контакты, найденные с использованием поля contacts</param>
        /// <param name="fields">список дополнительных полей, которые необходимо вернуть</param>
        public Dictionary<string, Dictionary<string, string>[]> lookupContacts(string[] contacts,
            string service, string mycontact, bool return_all, string[] fields)
        {
            return VkAPI.Account.lookupContacts(AccessToken, contacts, service, mycontact, return_all, fields);
        }
        /// <summary>
        /// Позволяет искать пользователей ВКонтакте, используя телефонные номера, email-адреса, и идентификаторы пользователей в других сервисах
        /// </summary>
        /// <param name="contacts">список контактов, разделенных через запятую</param>
        /// <param name="service">строковой идентификатор сервиса, по контактам которого производится поиск. Может принимать следующие значения:
        /// email; phone; twitter; facebook; odnoklassniki; instagram; google.</param>
        /// <param name="mycontact">контакт текущего пользователя в заданном сервисе</param>
        /// <param name="return_all">true — возвращать также контакты, найденные ранее с использованием этого сервиса,
        /// false — возвращать только контакты, найденные с использованием поля contacts</param>
        public Dictionary<string, Dictionary<string, string>[]> lookupContacts(string[] contacts, 
            string service, string mycontact, bool return_all)
        {
            return VkAPI.Account.lookupContacts(AccessToken, contacts, service, mycontact, return_all, new string[0]);
        }
        /// <summary>
        /// Позволяет искать пользователей ВКонтакте, используя телефонные номера, email-адреса, и идентификаторы пользователей в других сервисах
        /// </summary>
        /// <param name="contacts">список контактов, разделенных через запятую</param>
        /// <param name="service">строковой идентификатор сервиса, по контактам которого производится поиск. Может принимать следующие значения:
        /// email; phone; twitter; facebook; odnoklassniki; instagram; google.</param>
        /// <param name="mycontact">контакт текущего пользователя в заданном сервисе</param>
        /// false — возвращать только контакты, найденные с использованием поля contacts</param>
        public Dictionary<string, Dictionary<string, string>[]> lookupContacts(string[] contacts, 
            string service, string mycontact)
        {
            return VkAPI.Account.lookupContacts(AccessToken, contacts, service, mycontact, false, new string[0]);
        }
        /// <summary>
        /// Позволяет искать пользователей ВКонтакте, используя телефонные номера, email-адреса, и идентификаторы пользователей в других сервисах
        /// </summary>
        /// <param name="contacts">список контактов, разделенных через запятую</param>
        /// <param name="service">строковой идентификатор сервиса, по контактам которого производится поиск. Может принимать следующие значения:
        /// email; phone; twitter; facebook; odnoklassniki; instagram; google.</param>
        /// false — возвращать только контакты, найденные с использованием поля contacts</param>
        public Dictionary<string, Dictionary<string, string>[]> lookupContacts(string[] contacts, string service)
        {
            return VkAPI.Account.lookupContacts(AccessToken, contacts, service, "", false, new string[0]);
        }
        //--------------------| Метод account.account.registerDevice |-----
        /// <summary>
        /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
        /// </summary>
        /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
        /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
        /// <param name="device_model">cтроковое название модели устройства</param>
        /// <param name="device_year">год устройства. Установите отрицательное значение, если не хотите использовать его</param>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// <param name="system_version">строковая версия операционной системы устройства</param>
        /// <param name="settings">сериализованный JSON-объект, описывающий настройки уведомлений в специальном формате </param>
        /// <param name="sandbox">флаг предназначен для iOS устройств.
        /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
        public Dictionary<string, string> registerDevice(string token, string device_id, string device_model,
                int device_year, string system_version, PushSettings settings, bool sandbox)
        {
            Dictionary<string, string> d = VkAPI.Account.registerDevice(AccessToken, token, device_id, device_model, device_year,
                system_version, settings, sandbox);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
            return d;
        }
        /// <summary>
        /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
        /// </summary>
        /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
        /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
        /// <param name="device_model">cтроковое название модели устройства</param>
        /// <param name="device_year">год устройства. Установите отрицательное значение, если не хотите использовать его</param>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// <param name="system_version">строковая версия операционной системы устройства</param>
        /// <param name="settings">сериализованный JSON-объект, описывающий настройки уведомлений в специальном формате </param>
        /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
        public Dictionary<string, string> registerDevice(string token, string device_id, string device_model,
                int device_year, string system_version, PushSettings settings)
        {
            return registerDevice(token, device_id, device_model, device_year,
                system_version, settings, false);
        }
        /// <summary>
        /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
        /// </summary>
        /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
        /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
        /// <param name="device_model">cтроковое название модели устройства</param>
        /// <param name="device_year">год устройства. Установите отрицательное значение, если не хотите использовать его</param>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// <param name="system_version">строковая версия операционной системы устройства</param>
        /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
        public Dictionary<string, string> registerDevice(string token, string device_id, string device_model,
                int device_year, string system_version)
        {
            return registerDevice(token, device_id, device_model, device_year,
                system_version, new PushSettings(), false);
        }
        /// <summary>
        /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
        /// </summary>
        /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
        /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
        /// <param name="device_model">cтроковое название модели устройства</param>
        /// <param name="device_year">год устройства. Установите отрицательное значение, если не хотите использовать его</param>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
        public Dictionary<string, string> registerDevice(string token, string device_id, string device_model,
                int device_year)
        {
            return registerDevice(token, device_id, device_model, device_year,
                "", new PushSettings(), false);
        }
        /// <summary>
        /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
        /// </summary>
        /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
        /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
        /// <param name="device_model">cтроковое название модели устройства</param>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
        public Dictionary<string, string> registerDevice(string token, string device_id, string device_model)
        {
            return registerDevice(token, device_id, device_model, -1,
                "", new PushSettings(), false);
        }
        /// <summary>
        /// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений
        /// </summary>
        /// <param name="token">идентификатор устройства, используемый для отправки уведомлений.
        /// (для mpns идентификатор должен представлять из себя URL для отправки уведомлений)</param>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// true — использовать sandbox сервер для отправки push-уведомлений, false — не использовать </param>
        public Dictionary<string, string> registerDevice(string token, string device_id)
        {
            return registerDevice(token, device_id, "", -1,
                "", new PushSettings(), false);
        }
        //--------------------| Метод account.account.saveProfileInfo |----
        /// <summary>
        /// Редактирует информацию текущего профиля
        /// </summary>
        /// <param name="settings">параметры метода</param>
        public Dictionary<string, string> saveProfileInfo(SettingsProfileInfo settings)
        {
            return VkAPI.Account.saveProfileInfo(AccessToken, settings);
            
        }
        //--------------------| Метод account.account.setInfo |------------
        /// <summary>
        /// Позволяет редактировать информацию о текущем аккаунте.
        /// </summary>
        /// <param name="name">имя настройки</param>
        /// <param name="value">значение настройки </param>
        /// <param name="intro">битовая маска, отвечающая за прохождение обучения в мобильных клиентах</param>
        /// <param name="own_posts_default">отображение записей на стене пользователя. Возможные значения:
        /// true — на стене пользователя по умолчанию должны отображаться только собственные записи; false — на стене пользователя должны отображаться все записи.</param>
        /// <param name="no_wall_replies">отключение комментирования записей на стене. Возможные значения: 
        /// true — отключить комментирование записей на стене; false — разрешить комментирование</param>
        public Dictionary<string, string> setInfo(string name, string value, string intro, bool own_posts_default,
            bool no_wall_replies)
        {
            Dictionary<string, string> d = VkAPI.Account.setInfo(AccessToken, name, value,
                intro, own_posts_default, no_wall_replies);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
            return d;
        }
        /// <summary>
        /// Позволяет редактировать информацию о текущем аккаунте.
        /// </summary>
        /// <param name="name">имя настройки</param>
        /// <param name="value">значение настройки </param>
        /// <param name="intro">битовая маска, отвечающая за прохождение обучения в мобильных клиентах</param>
        /// <param name="own_posts_default">отображение записей на стене пользователя. Возможные значения:
        /// true — на стене пользователя по умолчанию должны отображаться только собственные записи; false — на стене пользователя должны отображаться все записи.</param>
        public Dictionary<string, string> setInfo(string name, string value, string intro, bool own_posts_default)
        {
            return setInfo(name, value, intro, own_posts_default, false);
        }
        /// <summary>
        /// Позволяет редактировать информацию о текущем аккаунте.
        /// </summary>
        /// <param name="name">имя настройки</param>
        /// <param name="value">значение настройки </param>
        /// <param name="intro">битовая маска, отвечающая за прохождение обучения в мобильных клиентах</param>
        public Dictionary<string, string> setInfo(string name, string value, string intro)
        {
            return setInfo(name, value, intro, false, false);
        }
        /// <summary>
        /// Позволяет редактировать информацию о текущем аккаунте.
        /// </summary>
        /// <param name="name">имя настройки</param>
        /// <param name="value">значение настройки </param>
        public Dictionary<string, string> setInfo(string name, string value)
        {
            return setInfo(name, value, "", false, false);
        }
        //--------------------| Метод account.account.setNameInMenu |------
        /// <summary>
        /// Устанавливает короткое название приложения (до 17 символов), которое выводится пользователю в левом меню.
        /// </summary>
        /// <param name="user_id">идентификатор пользователя</param>
        /// <param name="name">короткое название приложения</param>
        public void setNameInMenu(string user_id, string name)
        {
            Dictionary<string,string> d = VkAPI.Account.setNameInMenu(AccessToken, user_id, name);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
        }
        //--------------------| Метод account.account.setOffline |---------
        /// <summary>
        /// Помечает текущего пользователя как offline (только в текущем приложении)
        /// </summary>
        public void setOffline()
        {
            Dictionary<string, string> d = VkAPI.Account.setOffline(AccessToken);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
        }
        //--------------------| Метод account.account.setOnline |----------
        /// <summary>
        /// Помечает текущего пользователя как online на 15 минут
        /// </summary>
        /// <param name="voip">возможны ли видеозвонки для данного устройства</param>
        public void setOnline(bool voip)
        {
            Dictionary<string, string> d = VkAPI.Account.setOnline(AccessToken, voip);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
        }
        /// <summary>
        /// Помечает текущего пользователя как online на 15 минут
        /// </summary>
        public void setOnline() { setOnline(false); }
        //--------------------| Метод account.account.setPushSettings |----
        /// <summary>
        /// Изменяет настройку Push-уведомлений
        /// </summary>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// <param name="settings">объект, описывающий настройки уведомлений</param>
        /// <param name="key">ключ уведомления</param>
        /// <param name="value">новое значение уведомления</param>
        public void setPushSettings(string device_id,
                PushSettings settings, string key, PushSettings value)
        {
            Dictionary<string, string> d = VkAPI.Account.setPushSettings(AccessToken, device_id, settings, key, value);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
        }
        //--------------------| Метод account.account.setSilenceMode |-----
        /// <summary>
        /// Отключает push-уведомления на заданный промежуток времени
        /// </summary>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// <param name="time">время в секундах на которое требуется отключить уведомления, -1 отключить навсегда</param>
        /// <param name="peer_id">идентификатор назначения</param>
        /// <param name="sound">true — включить звук в этом диалоге, false — отключить звук 
        /// (параметр работает, только если в peer_id передан идентификатор групповой беседы или пользователя)</param>
        public void setSilenceMode(string device_id, int time,
            string peer_id, bool sound)
        {
            Dictionary<string, string> d = VkAPI.Account.setSilenceMode(AccessToken, device_id, time, peer_id, sound);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
        }
        /// <summary>
        /// Отключает push-уведомления на заданный промежуток времени
        /// </summary>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// <param name="time">время в секундах на которое требуется отключить уведомления, -1 отключить навсегда</param>
        /// <param name="peer_id">идентификатор назначения</param>
        public void setSilenceMode(string device_id, int time,
            string peer_id)
        {
            Dictionary<string, string> d = VkAPI.Account.setSilenceMode(AccessToken, device_id, time, peer_id, true);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
        }
        /// <summary>
        /// Отключает push-уведомления на заданный промежуток времени
        /// </summary>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// <param name="time">время в секундах на которое требуется отключить уведомления, -1 отключить навсегда</param>
        public void setSilenceMode(string device_id, int time)
        {
            Dictionary<string, string> d = VkAPI.Account.setSilenceMode(AccessToken, device_id, time, "", true);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
        }
        //--------------------| Метод account.account.unbanUser |----------
        /// <summary>
        /// Убирает пользователя из черного списка
        /// </summary>
        /// <param name="user_id">идентификатор пользователя, которого нужно убрать из черного списка</param>
        public void unbanUser(string user_id)
        {
            Dictionary<string, string> d = VkAPI.Account.unbanUser(AccessToken, user_id);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
        }
        //--------------------| Метод account.account.unregisterDevice |---
        /// <summary>
        /// Отписывает устройство от Push уведомлений
        /// </summary>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        /// <param name="sandbox">флаг предназначен для iOS устройств.
        /// true — отписать устройство, использующего sandbox сервер для отправки push-уведомлений,
        /// false — отписать устройство, не использующее sandbox сервер</param>
        public void unregisterDevice(string device_id, bool sandbox)
        {
            Dictionary<string, string> d = VkAPI.Account.unregisterDevice(AccessToken, device_id, sandbox);
            if (d.ContainsKey("error_code")) { data.SetData("data", 1, d); }
        }
        /// <summary>
        /// Отписывает устройство от Push уведомлений
        /// </summary>
        /// <param name="device_id">уникальный идентификатор устройства</param>
        public void unregisterDevice(string device_id)
        {
            unregisterDevice(device_id, false);
        }
    }
}