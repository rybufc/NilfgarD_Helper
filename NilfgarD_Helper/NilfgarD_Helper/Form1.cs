using MyAddonsForVkNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;

namespace NilfgarD_Helper
{
    public partial class Form1 : Form
    {
        VkApi Vk = new VkApi();
        MyVkAddons VkExtensions = new MyVkAddons();

        List<string> ListOfBannedNicknames = new List<string>();
        List<string> ListOfApprovedNicknames = new List<string>();

        List<VkBoardMessage> ListOfApprovedMessages = new List<VkBoardMessage>();

        StringBuilder Information = new StringBuilder();

        string Dict = "abcdefghijklmnopqrstuvwxyz1234567890_";

        int GroupId = 83974957;
        int BlackListId = 31587447;
        //        int RequestThreadId = 34351484;31296485
        int RequestThreadId = 31296485;
        ulong AppId = 5225863;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox4.Text = "Загрузка завершена.";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Vk.IsAuthorized) { textBox4.Text = "Авторизация не была произведена!"; return; }

            Information.Clear();
            Information.AppendLine("Идёт получение списка сообщений...");
            textBox4.Text = Information.ToString();

            var messageList = new List<VkBoardMessage>();

            try
            {
                messageList = VkExtensions.GetMessageList(GroupId, BlackListId, Vk);
            }
            catch
            {
                Information.AppendLine("Что-то пошло не так. Проверьте подключение к интернету.");
                textBox4.Text = Information.ToString();
                return;
            }

            ListOfBannedNicknames.Clear();

            Information.AppendLine("Идёт получение ников из сообщений...");
            textBox4.Text = Information.ToString();

            foreach (var item in messageList)
            {
                string nickname = GetNickName(item.Text, Dict);
                while (nickname != "")
                {
                    ListOfBannedNicknames.Add(nickname);
                    item.Text = item.Text.Remove(item.Text.IndexOf(nickname), nickname.Length);
                    nickname = GetNickName(item.Text, Dict);
                }
            }

            button2.Text = "Обновить список забаненных пользователей...";

            Information.AppendLine("Готово!");
            textBox4.Text = Information.ToString();
        }

        private static string GetNickName(string message, string dict)
        {
            bool isNicknameHere = false;
            var nickname = new StringBuilder();
            for (int i = 0; i < message.Length; i++)
            {
                if (dict.IndexOf(message.ToLower()[i]) != -1)
                {
                    isNicknameHere = true;
                    nickname.Append(message[i]);
                }
                else if (isNicknameHere) break;
            }
            return (nickname.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!Vk.IsAuthorized) { textBox4.Text = "Авторизация не была произведена!"; return ; }
            if (ListOfBannedNicknames.Count <= 0) { textBox4.Text = "Не был получен список забаненных пользователей!"; return ; }

            Information.Clear();
            Information.AppendLine("Получение списка заявок...");
            textBox4.Text = Information.ToString();

            var messagesToCheck = new List<VkBoardMessage>();

            try
            {
                messagesToCheck = VkExtensions.GetMessageList(GroupId, RequestThreadId, Vk);
            }
            catch
            {
                Information.AppendLine("Что-то пошло не так. Проверьте подключение к интернету.");
                textBox4.Text = Information.ToString();
                return;
            }

            Information.AppendLine("Чистка темы...");
            textBox4.Text = Information.ToString();

            ListOfApprovedMessages.Clear();

            foreach (var itemToCheck in messagesToCheck)
            {
                if (itemToCheck.Text.IndexOf("Данное обсуждение создано временно для теста специальной программы, которая упростит добавление народа в клуб. На момент написания п") != -1) continue;
                if (itemToCheck.Text.IndexOf("Здесь вы можете оставить свой ник, для вступления в клуб и в игре Trove") != -1) continue;
                if (itemToCheck.Text.IndexOf("Убедительная просьба ко всем ДОБАВЛЕННЫМ. Удаляйте свои заявки после добавления(админы иногда могут не удалить по невнимател") != -1) continue;

                string nickname = GetNickName(itemToCheck.Text, Dict);

                if ((nickname == "") || (nickname.Length < 3))
                {
                    Information.AppendLine("Ника не обнаружено|обнаружен неправильный ник. Сообщение будет удалено.");
                    textBox4.Text = Information.ToString();

                    try
                    {
                        VkExtensions.DeleteBoardComment(GroupId, RequestThreadId, itemToCheck.MessageId, Vk);
                    }
                    catch
                    {
                        Information.AppendLine("Ошибка доступа!");
                        textBox4.Text = Information.ToString();
                    }
                    continue;
                }

                bool isNicknameBanned = false;

                foreach (var bannedNickname in ListOfBannedNicknames)
                {
                    if (bannedNickname.ToLower() == nickname.ToLower())
                    {
                        isNicknameBanned = true;

                        Information.AppendLine(nickname.ToString() + " найден в чёрном списке! Сообщение будет удалено!");
                        textBox4.Text = Information.ToString();

                        try
                        {
                            VkExtensions.DeleteBoardComment(GroupId, RequestThreadId, itemToCheck.MessageId, Vk);
                        }
                        catch
                        {
                            Information.AppendLine("Ошибка доступа!");
                            textBox4.Text = Information.ToString();
                        }
                        break;
                    }
                }

                if (isNicknameBanned) continue;

                Information.Append(nickname.ToString());
                textBox4.Text = Information.ToString();

                int indToDelete = -1;

                foreach (var message in ListOfApprovedMessages)
                    if (nickname.ToLower() == GetNickName(message.Text.ToLower(), Dict))
                    {
                        Information.Append(" Найдено дублирование! Старое сообщение будет удалено!");
                        textBox4.Text = Information.ToString();
                        try
                        {
                            VkExtensions.DeleteBoardComment(GroupId, RequestThreadId, message.MessageId, Vk);
                        }
                        catch
                        {
                            Information.AppendLine("Ошибка доступа!");
                            textBox4.Text = Information.ToString();
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
            textBox4.Text = Information.ToString();

            File.Delete("ApprovedNicknames.txt");
            File.AppendAllLines("ApprovedNicknames.txt", ListOfApprovedNicknames);

            Information.AppendLine();
            Information.AppendLine("Готово!");
            textBox4.Text = Information.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Information.Clear();
            Information.AppendLine("Идёт авторизация...");
            textBox4.Text = Information.ToString();

            var authorize = new ApiAuthParams();

            Settings scope = Settings.All;

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
                Information.AppendLine("Что-то пошло не так. Проверьте правильность введённых данных и подключение к интернету!");
                textBox4.Text = Information.ToString();
                return;
            }

            Information.AppendLine("Авторизация прошла успешно!");
            textBox4.Text = Information.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Vk.IsAuthorized) { textBox4.Text = "Авторизация не была произведена!"; return; }
            if (ListOfBannedNicknames.Count <= 0) { textBox4.Text = "Не был получен список забаненных пользователей!"; return; }

            string nickname = GetNickName(nicknameBox.Text, Dict);

            bool isBanned = false;

            foreach (var bannedNickname in ListOfBannedNicknames)
            {
                if (bannedNickname.ToLower() == nickname.ToLower())
                {
                    textBox4.Text = nickname + " обнаружен в чёрном списке!";
                    isBanned = true;
                    break;
                }
            }

            if (!isBanned) textBox4.Text = nickname + " чист!";
        }

        private void openRequestFileButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists("ApprovedNicknames.txt")) { textBox4.Text = "Файл не найден!"; return; }

            System.Diagnostics.Process.Start("ApprovedNicknames.txt");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string nicknameToDelete = deleteBox.Text;

            Information.Clear();
            Information.AppendLine("Идёт поиск сообщения...");
            textBox4.Text = Information.ToString();

            foreach (var item in ListOfApprovedMessages)
                if (GetNickName(nicknameToDelete,Dict).ToLower()==GetNickName(item.Text,Dict).ToLower())
                {
                    try
                    {
                        VkExtensions.DeleteBoardComment(GroupId, RequestThreadId, item.MessageId, Vk);
                        ListOfApprovedMessages.Remove(item);

                        Information.AppendLine("Сообщение успешно удалено!");
                        textBox4.Text = Information.ToString();

                        return;
                    }
                    catch
                    {
                        Information.AppendLine("Что-то пошло не так! Проверьте подключение к интернету. Возможно, у вас нет прав на администрирование группы!");
                        textBox4.Text = Information.ToString();
                        return;
                    }
                }
            Information.AppendLine("Не найдёно сообщений с таким ником!");
            textBox4.Text = Information.ToString();
        }
    }
}