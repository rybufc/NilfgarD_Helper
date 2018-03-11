using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyAddonsForVkNet;
using VkNet;
using VkNet.Enums.Filters;

namespace NilfgarD_Helper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            if (File.Exists("ExclusionMessages.txt"))
                ListOfExcludeMessages = File.ReadAllLines("ExclusionMessages.txt").ToList();

            IsListOfBannedNicknamesGot = false;
            InitializeComponent();
        }

        //ID приложения
        private static ulong AppId => 5225863;

        //ID группы игрового сообщества
        private static int GroupId => 83974957;

        //ID обсуждения с чёрным списком
        private static int BlackListId => 31587447;

        //ID обсуждения с заявками на вступление в сообщество
        private static int RequestThreadId => 31296485;

        //Словарь символов, которые возможно встретить в нике
        private static string Dict => "abcdefghijklmnopqrstuvwxyz1234567890_";

        //Используется для быстрого форматирования текста элемента informationTextBox
        public StringBuilder Information { get; } = new StringBuilder();

        //Список сообщений сообщений с никами для добавления в сообщество
        public List<VkBoardMessage> ListOfApprovedMessages { get; } = new List<VkBoardMessage>();

        //Список сообщений, исключённых из проверки
        public List<string> ListOfExcludeMessages { get; } = new List<string>();

        //Список пользователей, доступных к добавлению в сообщество
        public List<string> ListOfApprovedNicknames { get; } = new List<string>();

        //Список пользователей в чёрном списке
        public List<string> ListOfBannedNicknames { get; } = new List<string>();

        //Класс для работы с api вконтакте
        public VkApi Vk { get; } = new VkApi();

        //Расширение класса работы с api вконтакте
        public VkApiExtensions VkExtensions { get; } = new VkApiExtensions();

        //Флаг, указывающий на то, был-ли получен чёрный список
        public bool IsListOfBannedNicknamesGot { get; set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            informationTextBox.Text = @"Загрузка завершена.";
        }

        /// <summary>
        ///     Получить список забаненных пользователей
        /// </summary>
        private void getListOfBannedMembersButton_Click(object sender, EventArgs e)
        {
            if (!Vk.IsAuthorized)
            {
                informationTextBox.Text = @"Авторизация не была произведена!";
                return;
            }

            //Оповещение пользователя о начале парсинга обсуждения
            Information.Clear();
            Information.AppendLine("Идёт получение списка сообщений...");
            informationTextBox.Text = Information.ToString();

            //Список сообщений из обсуждения
            List<VkBoardMessage> messages;

            //Парсинг сообщений из обсуждения
            try
            {
                messages = VkExtensions.GetMessageList(GroupId, BlackListId, Vk);
            }
            catch
            {
                //Если не получилосьдостать список сообщений из обсуждения, сообщаем об этом пользователю
                Information.AppendLine("Что-то пошло не так. Проверьте подключение к интернету.");
                informationTextBox.Text = Information.ToString();
                return;
            }

            //Очистка чёрного списка. Может понадобиться при повторном парсинге обсуждения.
            ListOfBannedNicknames.Clear();

            //Оповещение пользователя о начале парсинга ников из полученных сообщений
            Information.AppendLine("Идёт получение ников из сообщений...");
            informationTextBox.Text = Information.ToString();

            //парсинг ников из сообщений
            foreach (var message in messages)
            {
                //получение первого ника из сообщения
                var nickname = GetNickname(message.Text, Dict);
                //если ник найден, поиск других ников в сообщении
                while (nickname != null)
                {
                    //добавление ника в список забаненных ников
                    ListOfBannedNicknames.Add(nickname);
                    //обрезаем сообщение до окончания найденного ника
                    message.Text = message.Text.Remove(message.Text.IndexOf(nickname), nickname.Length);
                    //пытаемся найти новый ник
                    nickname = GetNickname(message.Text, Dict);
                }
            }

            //Меняем название кнопки для отображения изменений в её функционале
            getListOfBannedMembersButton.Text = @"Обновить список забаненных пользователей...";

            //т.к. список забаненных получен, меняем значение соответствующего флага
            IsListOfBannedNicknamesGot = true;

            //Оповещаем пользователя о том, что процес получения ников из чёрного списка завершён
            Information.AppendLine("Готово!");
            informationTextBox.Text = Information.ToString();
        }

        /// <summary>
        ///     Парсинг ника из строки
        /// </summary>
        /// <param name="message">сообщение из которого парсится ник</param>
        /// <param name="dict">словарь символов, которые можно встретить в нике</param>
        /// <param name="minLength">минимальная длинна ника</param>
        /// <returns></returns>
        private static string GetNickname(string message, string dict, int minLength = 1)
        {
            //флаг, отображающий, нашёлся - ли ник
            var isNicknameFound = false;
            //ник
            var nickname = new StringBuilder();

            //процесс парсинга ника
            for (var i = 0; i < message.Length; i++)
                //Проверка символа на присутствие в словаре
                if (dict.IndexOf(message.ToLower()[i]) != -1)
                {
                    //добавление нового символа из сообщения в ник
                    nickname.Append(message[i]);

                    //проверка ника на длинну, требуется для того, чтобы отбросить лишние артикли в сообщении
                    if (nickname.Length >= minLength)
                        isNicknameFound = true;
                }
                //проверка на то, был-ли найден ник
                else if (isNicknameFound)
                {
                    //если найден - выходим из цикла
                    break;
                }

            //проверка на то, был ли найден ник
            if (!isNicknameFound)
                //если ник не был найден, возвращаем null
                return null;

            //возвращаем найденный ник
            return nickname.ToString();
        }

        /// <summary>
        ///     Чистка сообщений и получение ников из обсуждения
        /// </summary>
        private void ClearRequestThreadButton_Click(object sender, EventArgs e)
        {
            if (!Vk.IsAuthorized)
            {
                informationTextBox.Text = @"Авторизация не была произведена!";
                return;
            }

            //Если не был получен список забаненных пользователей, оповещаем об этом пользователя
            //и выходим из процедуры
            if (!IsListOfBannedNicknamesGot)
            {
                informationTextBox.Text = @"Не был получен список забаненных пользователей!";
                return;
            }

            //Оповещение пользователя о начале парсинга обсуждения
            Information.Clear();
            Information.AppendLine("Получение списка заявок...");
            informationTextBox.Text = Information.ToString();

            //Список сообщений из обсуждения, из которых надо достать и проверить ники
            List<VkBoardMessage> messagesToCheck;

            //Получение сообщений из обсуждения
            try
            {
                messagesToCheck = VkExtensions.GetMessageList(GroupId, RequestThreadId, Vk);
            }
            catch
            {
                //если получить сообщения не удалось, оповещаем об этом пользователя
                Information.AppendLine("Что-то пошло не так. Проверьте подключение к интернету.");
                informationTextBox.Text = Information.ToString();
                return;
            }

            //Оповещение пользователя о начале чистки обсуждения с заявками
            Information.AppendLine("Чистка темы...");
            informationTextBox.Text = Information.ToString();

            ListOfApprovedMessages.Clear();

            foreach (var itemToCheck in messagesToCheck)
            {
                //Проверка сообщений на доверенные
                var isMessageExcluded = false;
                foreach (var excludeMessage in ListOfExcludeMessages)
                {
                    if (itemToCheck.Text.ToLower().IndexOf(excludeMessage.ToLower()) != -1)
                    {
                        isMessageExcluded = true;
                        break;
                    }
                }
                if (isMessageExcluded)
                    continue;

                //Получение ника из сообщения
                var nickname = GetNickname(itemToCheck.Text, Dict);

                //Проверка на наличие ника в сообщении
                if (nickname == "" || nickname.Length < 3)
                {
                    //Оповещение об очистке от спам-сообщений
                    Information.AppendLine("Ника не обнаружено|обнаружен неправильный ник. Сообщение будет удалено.");
                    informationTextBox.Text = Information.ToString();

                    //Попытка удаления сообщения
                    try
                    {
                        VkExtensions.DeleteBoardComment(GroupId, RequestThreadId, itemToCheck.MessageId, Vk);
                    }
                    catch
                    {
                        //Если не получается удалить - оповещаем пользователя
                        Information.AppendLine("Ошибка доступа!");
                        informationTextBox.Text = Information.ToString();
                    }

                    continue;
                }

                //Проверка ника на наличие его в чёрном списке сообщества
                var isNicknameBanned = false;
                foreach (var bannedNickname in ListOfBannedNicknames)
                    if (bannedNickname.ToLower() == nickname.ToLower())
                    {
                        isNicknameBanned = true;

                        //Если ник найден в чс, оповещаем пользователя и пытаемся удалить сообщение с ником
                        Information.AppendLine(nickname + " найден в чёрном списке! Сообщение будет удалено!");
                        informationTextBox.Text = Information.ToString();

                        try
                        {
                            VkExtensions.DeleteBoardComment(GroupId, RequestThreadId, itemToCheck.MessageId, Vk);
                        }
                        catch
                        {
                            Information.AppendLine("Ошибка доступа!");
                            informationTextBox.Text = Information.ToString();
                        }

                        break;
                    }

                if (isNicknameBanned) continue;

                //Если ник проходит все проверки, оповещаем о найдённом нике пользователя
                Information.Append(nickname);
                informationTextBox.Text = Information.ToString();

                var indToDelete = -1;

                //Проверяем тему на наличие сообщения с найденным ником. Если находим - пытаемся удалить старое сообщение.
                foreach (var message in ListOfApprovedMessages)
                    if (nickname.ToLower() == GetNickname(message.Text.ToLower(), Dict))
                    {
                        Information.Append(" Найдено дублирование! Старое сообщение будет удалено!");
                        informationTextBox.Text = Information.ToString();
                        try
                        {
                            VkExtensions.DeleteBoardComment(GroupId, RequestThreadId, message.MessageId, Vk);
                        }
                        catch
                        {
                            Information.AppendLine("Ошибка доступа!");
                            informationTextBox.Text = Information.ToString();
                        }

                        indToDelete = ListOfApprovedMessages.FindIndex(x => x.MessageId == message.MessageId);

                        break;
                    }

                //Если находили дублирование - удаляем лишнюю запись из списка сообщений с никами, иначе - добавляем в список ников новую запись.
                if (indToDelete != -1)
                    ListOfApprovedMessages.RemoveAt(indToDelete);
                else ListOfApprovedNicknames.Add(nickname);

                Information.AppendLine();

                //Добавляем найденное сообщение с ником в список сообщений с никами.
                ListOfApprovedMessages.Add(itemToCheck);
            }

            //Оповещаем пользователя о начале записи ников в файл
            Information.AppendLine();
            Information.AppendLine("Запись списка ников в файл ApprovedNicknames.txt...");
            informationTextBox.Text = Information.ToString();

            //Запись ников в файл
            File.Delete("ApprovedNicknames.txt");
            File.AppendAllLines("ApprovedNicknames.txt", ListOfApprovedNicknames);

            //Оповещение о готовности
            Information.AppendLine();
            Information.AppendLine("Готово!");
            informationTextBox.Text = Information.ToString();
        }

        /// <summary>
        /// Авторизация Вконтакте
        /// </summary>
        private void vkAuthButton_Click(object sender, EventArgs eventArgs)
        {
            //Оповещаем пользователя о начале процесса авторизации
            Information.Clear();
            Information.AppendLine("Идёт авторизация...");
            informationTextBox.Text = Information.ToString();

            //Инициализируем параметры авторизации
            var authorizer = new ApiAuthParams();

            var scope = Settings.All;

            authorizer.Login = loginTextBox.Text;
            authorizer.Password = passwordTextBox.Text;
            authorizer.ApplicationId = AppId;
            authorizer.Settings = scope;

            //Попытка авторизации
            try
            {
                Vk.Authorize(authorizer);
            }
            catch
            {
                //Если авторизация не удалась, сообщаем об этом пользователю.
                Information.AppendLine(
                    "Что-то пошло не так. Проверьте правильность введённых данных и подключение к интернету!");
                informationTextBox.Text = Information.ToString();
                return;
            }

            //Сообщаем пользователю об успешной авторизации.
            Information.AppendLine("Авторизация прошла успешно!");
            informationTextBox.Text = Information.ToString();
        }

        /// <summary>
        /// Проверяет ник на наличие его в чёрном списке сообщества.
        /// </summary>
        private void checkNicknameButton_Click(object sender, EventArgs e)
        {
            //Проверка на авторизацию вконтакте
            if (!Vk.IsAuthorized)
            {
                informationTextBox.Text = "Авторизация не была произведена!";
                return;
            }

            //Проверка на то, был-ли получен чёрный список сообщества.
            if (!IsListOfBannedNicknamesGot)
            {
                informationTextBox.Text = "Не был получен список забаненных пользователей!";
                return;
            }

            //Проверка на наличие введённого в nicknameBox ника в чс
            var nickname = GetNickname(nicknameBox.Text, Dict);
            foreach (var bannedNickname in ListOfBannedNicknames)
                if (bannedNickname.ToLower() == nickname.ToLower())
                {
                    informationTextBox.Text = nickname + @" обнаружен в чёрном списке!";
                    return;
                }

            informationTextBox.Text = nickname + @" чист!";
        }

        /// <summary>
        /// Открывает список ников для добавления в стандартном приложении Windows
        /// (блокнот и т.д.)
        /// </summary>
        private void openRequestFileButton_Click(object sender, EventArgs e)
        {
            //Проверка на наличие записанного файла с никами
            if (!File.Exists("ApprovedNicknames.txt"))
            {
                informationTextBox.Text = @"Файл не найден!";
                return;
            }

            Process.Start("ApprovedNicknames.txt");
        }

        /// <summary>
        /// Удаляет сообщение с указанным в deleteBox ником
        /// </summary>
        private void deleteMessageButton_Click(object sender, EventArgs e)
        {
            //получаем ник из deleteBox
            var nicknameToDelete = deleteBox.Text;

            //Оповещаем пользователя о том, что идёт поиск сообщения с введённым ником
            Information.Clear();
            Information.AppendLine("Идёт поиск сообщения...");
            informationTextBox.Text = Information.ToString();

            foreach (var item in ListOfApprovedMessages)
                //Поиск сообщения с ником в списке найденных сообщений с никами
                if (GetNickname(nicknameToDelete, Dict).ToLower() == GetNickname(item.Text, Dict).ToLower())
                    //Удаление сообщения с ником из обсуждения
                    try
                    {
                        VkExtensions.DeleteBoardComment(GroupId, RequestThreadId, item.MessageId, Vk);
                        ListOfApprovedMessages.Remove(item);

                        //Оповещение пользователя об успешном удалении сообщения
                        Information.AppendLine("Сообщение успешно удалено!");
                        informationTextBox.Text = Information.ToString();

                        return;
                    }
                    catch
                    {
                        //Если удаление не удалось, оповещаем об этом пользователя
                        Information.AppendLine(
                            "Что-то пошло не так! Проверьте подключение к интернету. Возможно, у вас нет прав на администрирование группы!");
                        informationTextBox.Text = Information.ToString();
                        return;
                    }

            //Если не удалось найти сообщение с введённым ником, оповещаем об этом пользователя.
            Information.AppendLine("Не найдёно сообщений с таким ником!");
            informationTextBox.Text = Information.ToString();
        }
    }
}