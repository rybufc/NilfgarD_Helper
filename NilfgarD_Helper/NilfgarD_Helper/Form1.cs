using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MyAddonsForVkNet;
using VkNet;
using VkNet.Enums.Filters;

namespace NilfgarD_Helper
{
    public partial class Form1 : Form
    {
        //ID приложения
        private readonly ulong AppId = 5225863;

        //ID обсуждения с чёрным списком
        private readonly int BlackListId = 31587447;

        //Словарь символов, которые возможно встретить в нике
        private readonly string Dict = "abcdefghijklmnopqrstuvwxyz1234567890_";

        //ID группы игрового сообщества
        private readonly int GroupId = 83974957;

        //Используется для быстрого форматирования текста элемента informationTextBox
        private readonly StringBuilder Information = new StringBuilder();

        //Список сообщений сообщений с никами для добавления в сообщество
        private readonly List<VkBoardMessage> ListOfApprovedMessages = new List<VkBoardMessage>();

        //Список пользователей, доступных к добавлению в сообщество
        private readonly List<string> ListOfApprovedNicknames = new List<string>();

        //Список пользователей в чёрном списке
        private readonly List<string> ListOfBannedNicknames = new List<string>();

        //ID обсуждения с заявками на вступление в сообщество
        private readonly int RequestThreadId = 31296485;

        //Класс для работы с api вконтакте
        private readonly VkApi Vk = new VkApi();

        //Расширение класса работы с api вконтакте
        private readonly VkApiExtensions VkExtensions = new VkApiExtensions();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            informationTextBox.Text = "Загрузка завершена.";
        }

        /// <summary>
        ///     Получить список забаненных пользователей
        /// </summary>
        private void getListOfBannedMembersButton_Click(object sender, EventArgs e)
        {
            if (!Vk.IsAuthorized)
            {
                informationTextBox.Text = "Авторизация не была произведена!";
                return;
            }

            //Оповещение пользователя о начале парсинга обсуждения
            Information.Clear();
            Information.AppendLine("Идёт получение списка сообщений...");
            informationTextBox.Text = Information.ToString();

            //Список сообщений из обсуждения
            var messages = new List<VkBoardMessage>();

            //Парсинг сообщений из обсуждения
            try
            {
                messages = VkExtensions.GetMessageList(GroupId, BlackListId, Vk);
            }
            catch
            {
                Information.AppendLine("Что-то пошло не так. Проверьте подключение к интернету.");
                informationTextBox.Text = Information.ToString();
                return;
            }

            //Очистка чёрного списка. Может понадобиться при повторном парсинге обсуждения.
            ListOfBannedNicknames.Clear();

            //Оповещение пользователя о начале парсинга ников из полученных сообщений
            Information.AppendLine("Идёт получение ников из сообщений...");
            informationTextBox.Text = Information.ToString();

            //Парсинг ников из сообщений
            foreach (var message in messages)
            {
                var nickname = GetNickname(message.Text, Dict);
                while (nickname != "")
                {
                    ListOfBannedNicknames.Add(nickname);
                    message.Text = message.Text.Remove(message.Text.IndexOf(nickname), nickname.Length);
                    nickname = GetNickname(message.Text, Dict);
                }
            }

            getListOfBannedMembersButton.Text = "Обновить список забаненных пользователей...";

            Information.AppendLine("Готово!");
            informationTextBox.Text = Information.ToString();
        }

        /// <summary>
        ///     Парсинг ника из строки
        /// </summary>
        /// <param сообщение из которого парсится ник="message"></param>
        /// <param словарь символов, которые можно встретить в нике="dict"></param>
        /// <param минимальная длинна ника="minLength"></param>
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (!Vk.IsAuthorized)
            {
                informationTextBox.Text = "Авторизация не была произведена!";
                return;
            }

            if (ListOfBannedNicknames.Count <= 0)
            {
                informationTextBox.Text = "Не был получен список забаненных пользователей!";
                return;
            }

            Information.Clear();
            Information.AppendLine("Получение списка заявок...");
            informationTextBox.Text = Information.ToString();

            var messagesToCheck = new List<VkBoardMessage>();

            try
            {
                messagesToCheck = VkExtensions.GetMessageList(GroupId, RequestThreadId, Vk);
            }
            catch
            {
                Information.AppendLine("Что-то пошло не так. Проверьте подключение к интернету.");
                informationTextBox.Text = Information.ToString();
                return;
            }

            Information.AppendLine("Чистка темы...");
            informationTextBox.Text = Information.ToString();

            ListOfApprovedMessages.Clear();

            foreach (var itemToCheck in messagesToCheck)
            {
                if (itemToCheck.Text.IndexOf(
                        "Данное обсуждение создано временно для теста специальной программы, которая упростит добавление народа в клуб. На момент написания п") !=
                    -1) continue;
                if (itemToCheck.Text.IndexOf(
                        "Здесь вы можете оставить свой ник, для вступления в клуб и в игре Trove") != -1) continue;
                if (itemToCheck.Text.IndexOf(
                        "Убедительная просьба ко всем ДОБАВЛЕННЫМ. Удаляйте свои заявки после добавления(админы иногда могут не удалить по невнимател") !=
                    -1) continue;

                var nickname = GetNickname(itemToCheck.Text, Dict);

                if (nickname == "" || nickname.Length < 3)
                {
                    Information.AppendLine("Ника не обнаружено|обнаружен неправильный ник. Сообщение будет удалено.");
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

                    continue;
                }

                var isNicknameBanned = false;

                foreach (var bannedNickname in ListOfBannedNicknames)
                    if (bannedNickname.ToLower() == nickname.ToLower())
                    {
                        isNicknameBanned = true;

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

                Information.Append(nickname);
                informationTextBox.Text = Information.ToString();

                var indToDelete = -1;

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

                if (indToDelete != -1)
                    ListOfApprovedMessages.RemoveAt(indToDelete);
                else ListOfApprovedNicknames.Add(nickname);

                Information.AppendLine();

                ListOfApprovedMessages.Add(itemToCheck);
            }

            Information.AppendLine();
            Information.AppendLine("Запись списка ников в файл ApprovedNicknames.txt...");
            informationTextBox.Text = Information.ToString();

            File.Delete("ApprovedNicknames.txt");
            File.AppendAllLines("ApprovedNicknames.txt", ListOfApprovedNicknames);

            Information.AppendLine();
            Information.AppendLine("Готово!");
            informationTextBox.Text = Information.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Information.Clear();
            Information.AppendLine("Идёт авторизация...");
            informationTextBox.Text = Information.ToString();

            var authorize = new ApiAuthParams();

            var scope = Settings.All;

            authorize.Login = loginTextBox.Text;
            authorize.Password = passwordTextBox.Text;
            authorize.ApplicationId = AppId;
            authorize.Settings = scope;

            try
            {
                Vk.Authorize(authorize);
            }
            catch
            {
                Information.AppendLine(
                    "Что-то пошло не так. Проверьте правильность введённых данных и подключение к интернету!");
                informationTextBox.Text = Information.ToString();
                return;
            }

            Information.AppendLine("Авторизация прошла успешно!");
            informationTextBox.Text = Information.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Vk.IsAuthorized)
            {
                informationTextBox.Text = "Авторизация не была произведена!";
                return;
            }

            if (ListOfBannedNicknames.Count <= 0)
            {
                informationTextBox.Text = "Не был получен список забаненных пользователей!";
                return;
            }

            var nickname = GetNickname(nicknameBox.Text, Dict);

            var isBanned = false;

            foreach (var bannedNickname in ListOfBannedNicknames)
                if (bannedNickname.ToLower() == nickname.ToLower())
                {
                    informationTextBox.Text = nickname + " обнаружен в чёрном списке!";
                    isBanned = true;
                    break;
                }

            if (!isBanned) informationTextBox.Text = nickname + " чист!";
        }

        private void openRequestFileButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists("ApprovedNicknames.txt"))
            {
                informationTextBox.Text = "Файл не найден!";
                return;
            }

            Process.Start("ApprovedNicknames.txt");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var nicknameToDelete = deleteBox.Text;

            Information.Clear();
            Information.AppendLine("Идёт поиск сообщения...");
            informationTextBox.Text = Information.ToString();

            foreach (var item in ListOfApprovedMessages)
                if (GetNickname(nicknameToDelete, Dict).ToLower() == GetNickname(item.Text, Dict).ToLower())
                    try
                    {
                        VkExtensions.DeleteBoardComment(GroupId, RequestThreadId, item.MessageId, Vk);
                        ListOfApprovedMessages.Remove(item);

                        Information.AppendLine("Сообщение успешно удалено!");
                        informationTextBox.Text = Information.ToString();

                        return;
                    }
                    catch
                    {
                        Information.AppendLine(
                            "Что-то пошло не так! Проверьте подключение к интернету. Возможно, у вас нет прав на администрирование группы!");
                        informationTextBox.Text = Information.ToString();
                        return;
                    }

            Information.AppendLine("Не найдёно сообщений с таким ником!");
            informationTextBox.Text = Information.ToString();
        }
    }
}