using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.Utils;

namespace MyAddonsForVkNet
{
    [JsonObject(MemberSerialization.OptIn)]
    internal struct JsonComment
    {
        [JsonProperty("from_id")] public int Sender { get; set; }

        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("date")] public int Date { get; set; }

        [JsonProperty("text")] public string Text { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal struct JsonTopic
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("comments")] public int Comments { get; set; }

        [JsonProperty("title")] public string Title { get; set; }
    }

    public class VkApiExtensions
    {
        public void DeleteBoardComment(int groupId, int topicId, int messageId, VkApi vk)
        {
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

        private int GetNumberOfComments(int groupId, int topicId, VkApi vk)
        {
            var parameters = new VkParameters
            {
                {"group_id", groupId},
                {"topic_id", topicId}
            };

            var token = vk.Invoke("board.getComments", parameters);

            token = token.Split('[')[1];

            Thread.Sleep(200);

            return int.Parse(token.Split(',')[0]);
        }

        public List<VkBoardMessage> GetMessageList(int groupId, int topicId, VkApi vk)
        {
            var tokens = new List<string>();
            var messages = new List<JsonComment>();

            var counter = GetNumberOfComments(groupId, topicId, vk);

            var offset = 0;

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
                tokens.Add(vk.Invoke("board.getComments", parameters));

                var obj = JObject.Parse(tokens.Last());

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

        public List<VkBoardTopic> GetTopicList(int groupId, VkApi vk)
        {
            var parameters = new VkParameters
            {
                {"group_id", groupId}
            };

            var token = vk.Invoke("board.getTopics", parameters);

            var topics = new List<VkBoardTopic>();
            var obj = JObject.Parse(token);
            var count = JsonConvert.DeserializeObject<int>(obj["response"]["topics"][0].ToString());

            for (var i = 0; i < count; i++)
            {
                var objToAdd = JsonConvert.DeserializeObject<JsonTopic>(obj["response"]["topics"][i + 1].ToString());

                topics.Add(new VkBoardTopic {Id = objToAdd.Id, Title = objToAdd.Title, Comments = objToAdd.Comments});
            }

            return topics;
        }
    }

    public class VkBoardTopic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Comments { get; set; }
    }

    public class VkBoardMessage
    {
        public int Date { get; set; }
        public int Sender { get; set; }
        public string Text { get; set; }
        public int MessageId { get; set; }
    }
}