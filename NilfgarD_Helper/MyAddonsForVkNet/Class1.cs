using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.Utils;

namespace MyAddonsForVkNet
{
    /// <summary>
    ///     Формат ответа - комментария из обсуждения в формате JSon
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal struct JsonComment
    {
        [JsonProperty("from_id")] public int Sender { get; set; }

        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("date")] public int Date { get; set; }

        [JsonProperty("text")] public string Text { get; set; }
    }

    /// <summary>
    ///     Формат ответа-обсуждения в JSon
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal struct JsonTopic
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("comments")] public int Comments { get; set; }

        [JsonProperty("title")] public string Title { get; set; }
    }

    /// <summary>
    ///     Класс, дополняющий функционал библиотеки VkNet
    /// </summary>
    public class VkApiExtensions
    {
        /// <summary>
        ///     Удаление комментария из обсуждения
        /// </summary>
        /// <param name="groupId">ID группы</param>
        /// <param name="topicId">ID обсуждения</param>
        /// <param name="messageId">ID сообщения</param>
        /// <param name="vk">Класс для работы с api vkontakte</param>
        public void DeleteBoardComment(int groupId, int topicId, int messageId, VkApi vk)
        {
            //отправка запроса на удаление комментария
            Thread.Sleep(200);
            var parameters = new VkParameters
            {
                {"group_id", groupId},
                {"topic_id", topicId},
                {"comment_id", messageId},
                {"access_token", vk.AccessToken}
            };
            vk.Invoke("board.deleteComment", parameters);
        }

        /// <summary>
        ///     Получение количества коммантариев в обсуждении
        /// </summary>
        /// <param name="groupId">ID группы</param>
        /// <param name="topicId">ID обсуждения</param>
        /// <param name="vk">Класс для работы с api Vkontakte</param>
        /// <returns></returns>
        private int GetNumberOfComments(int groupId, int topicId, VkApi vk)
        {
            //Отправка запроса на получение списка комментариев
            var parameters = new VkParameters
            {
                {"group_id", groupId},
                {"topic_id", topicId}
            };

            var response = vk.Invoke("board.getComments", parameters);

            //Парсим количество участников из ответа сервера
            response = response?.Split('[')[1];

            Thread.Sleep(200);

            return int.Parse(response.Split(',')[0]);
        }

        /// <summary>
        ///     Получение списка комментариев из обсуждения
        /// </summary>
        /// <param name="groupId">ID группы</param>
        /// <param name="topicId">ID обсуждения</param>
        /// <param name="vk">Класс для работы с api vkontakte</param>
        /// <returns></returns>
        public List<VkBoardMessage> GetMessageList(int groupId, int topicId, VkApi vk)
        {
            //инициализируем список ответов(vkontakte может отослать не более 100 комментариев за запрос) на запросы
            var responses = new List<string>();
            //инициализируем список сообщений
            var messages = new List<JsonComment>();

            var counter = GetNumberOfComments(groupId, topicId, vk);

            var offset = 0;

            //получение списка сообщений
            while (counter > 0)
            {
                var counterToAdd = 0;

                if (counter < 100)
                    counterToAdd = counter % 100;
                else counterToAdd = 100;

                var parameters = new VkParameters
                {
                    {"group_id", groupId},
                    {"topic_id", topicId},
                    {"count", counterToAdd},
                    {"offset", offset}
                };
                counter -= 100;
                offset += 100;
                responses.Add(vk.Invoke("board.getComments", parameters));

                var obj = JObject.Parse(responses.Last());

                for (var i = 0; i < counterToAdd; i++)
                {
                    var objToAdd =
                        JsonConvert.DeserializeObject<JsonComment>(obj["response"]["comments"][i + 1].ToString());
                    messages.Add(objToAdd);
                }

                Thread.Sleep(200);
            }

            var comments = new List<VkBoardMessage>();

            foreach (var comment in messages)
                comments.Add(new VkBoardMessage
                {
                    Date = comment.Date,
                    MessageId = comment.Id,
                    Sender = comment.Sender,
                    Text = comment.Text
                });

            return comments;
        }

        /// <summary>
        ///     Получение списка обсуждений группы
        /// </summary>
        /// <param name="groupId">ID группы вконтакте</param>
        /// <param name="vk">Класс для работы с api vkontakte</param>
        /// <returns></returns>
        public List<VkBoardTopic> GetTopicList(int groupId, VkApi vk)
        {
            //Создаём параметр для запроса к api vkontakte
            var parameters = new VkParameters
            {
                {"group_id", groupId}
            };

            //Отравка запроса к api vkontakte
            var response = vk.Invoke("board.getTopics", parameters);

            //Получение списка групп из ответа
            var topics = new List<VkBoardTopic>();
            var obj = JObject.Parse(response);
            var count = JsonConvert.DeserializeObject<int>(obj["response"]["topics"][0].ToString());

            for (var i = 0; i < count; i++)
            {
                var objToAdd = JsonConvert.DeserializeObject<JsonTopic>(obj["response"]["topics"][i + 1].ToString());

                topics.Add(new VkBoardTopic {Id = objToAdd.Id, Title = objToAdd.Title, Comments = objToAdd.Comments});
            }

            return topics;
        }
    }

    //Класс обсуждения
    public class VkBoardTopic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Comments { get; set; }
    }

    //Класс сообщения из обсуждения
    public class VkBoardMessage
    {
        public int Date { get; set; }
        public int Sender { get; set; }
        public string Text { get; set; }
        public int MessageId { get; set; }
    }
}