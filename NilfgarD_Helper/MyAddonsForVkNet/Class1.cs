using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using VkNet;
using VkNet.Utils;

namespace MyAddonsForVkNet
{
    [JsonObject(MemberSerialization.OptIn)]
    struct JsonComment
    {
        [JsonProperty("from_id")]
        public int Sender { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("date")]
        public int Date { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }


    public class MyVkAddons
    {
        public void DeleteBoardComment(int groupId, int topicId, int messageId, VkApi vk)
        {
            Thread.Sleep(200);
            var parameters = new VkParameters
                {
                    { "group_id", groupId },
                    { "topic_id", topicId },
                    { "comment_id", messageId },
                    { "access_token", vk.AccessToken }
                };
            vk.Invoke("board.deleteComment", parameters);
        }

        private int GetNumberOfComments(int groupId, int topicId, VkApi vk)
        {
            var parameters = new VkParameters
                {
                    { "group_id", groupId },
                    { "topic_id", topicId }
                };

            string token = vk.Invoke("board.getComments", parameters);

            token = token.Split('[')[1];

            Thread.Sleep(200);

            return int.Parse(token.Split(',')[0]);
        }

        public List<VkBoardMessage> GetMessageList(int groupId, int topicId, VkApi vk)
        {
            var tokens = new List<string>();
            List<JsonComment> messages = new List<JsonComment>();

            int counter = GetNumberOfComments(groupId, topicId, vk);

            int offset = 0;

            while (counter > 0)
            {
                int counterToAdd = 0;

                if (counter < 100)
                    counterToAdd = counter % 100;
                else counterToAdd = 100;

                var parameters = new VkParameters
                {
                    { "group_id", groupId },
                    { "topic_id", topicId },
                    { "count", counterToAdd },
                    { "offset", offset }
                };
                counter -= 100;
                offset += 100;
                tokens.Add(vk.Invoke("board.getComments", parameters));

                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(tokens.Last());

                for (int i = 0; i < counterToAdd; i++)
                {
                    JsonComment objToAdd = JsonConvert.DeserializeObject<JsonComment>(obj["response"]["comments"][i+1].ToString());
                    messages.Add(objToAdd);
                }

                Thread.Sleep(200);
            }

            List<VkBoardMessage> comments = new List<VkBoardMessage>();

            foreach (var comment in messages)
            {
                comments.Add(new VkBoardMessage
                {
                    Date = comment.Date,
                    MessageId = comment.Id,
                    Sender = comment.Sender,
                    Text = comment.Text
                });
            }

            return comments;
        }

        private List<VkBoardMessage> ParseCommentsTokenList(List<string> token)
        {
            List<VkBoardMessage> result = new List<VkBoardMessage>();
            List<string> parsedToken = new List<string>();
            var input = new List<string>();

            for (int i = 0; i < token.Count; i++)
            {
                int startPos = token[i].IndexOf('[') + 1;
                int endPos = token[i].LastIndexOf(']');

                input.Add(token[i].Substring(startPos, endPos - startPos));
            }

            for (int i = 0; i < input.Count; i++)
            {
                parsedToken.AddRange(ParseTokenByBraces(input[i]));
            }

            string[] delims = new string[] { ",\"" };

            for (int i = 0; i < parsedToken.Count; i++)
            {
                string[] messageInfo = parsedToken[i].Split(delims, StringSplitOptions.RemoveEmptyEntries);
                result.Add(parseMessageToken(messageInfo));
            }

            return result;
        }

        private VkBoardMessage parseMessageToken(string[] tokens)
        {

            int messageId = 0;
            int sender = 0;
            string text = "";
            for (int i = 0; i < tokens.Length; i++)
            {
                string buff = "";

                int typeOfToken = checkMessageToken(tokens[i].Split(':')[0]);
                if (typeOfToken == -1) continue;

                string[] delims = new string[] { "\":" };

                switch (typeOfToken)
                {
                    case 0:
                        buff = tokens[i].Split(delims, StringSplitOptions.RemoveEmptyEntries)[1];
                        messageId = int.Parse(buff);
                        break;
                    case 1:
                        buff = tokens[i].Split(delims, StringSplitOptions.RemoveEmptyEntries)[1];
                        sender = int.Parse(buff);
                        break;
                    case 2:
                        buff = tokens[i].Split(delims, StringSplitOptions.RemoveEmptyEntries)[1];
                        buff.Replace("<br>", " ");
                        text = buff;
                        break;
                }
            }

            return new VkBoardMessage() { MessageId = messageId, Sender = sender, Text = text };
        }

        private int checkMessageToken(string token)
        {
            if (token.IndexOf("from_id") != -1) return 1;
            if (token.IndexOf("id") != -1) return 0;
            if (token.IndexOf("text") != -1) return 2;
            return -1;
        }

        public List<VkBoardTopic> GetTopicList(int groupId, VkApi vk)
        {
            var parameters = new VkParameters
            {
                { "group_id", groupId }
            };

            var token = vk.Invoke("board.getTopics", parameters);

            return (ParseTopicListToken(token));
        }

        private List<string> ParseTokenByBraces(string token)
        {
            List<string> parsedInput = new List<string>();

            int startPos = 0;
            int endPos = 0;

            while (token.IndexOf('{') != -1)
            {
                startPos = token.IndexOf('{') + 1;
                endPos = token.IndexOf("},");
                int attachment = token.IndexOf("\"attachments\"");

                if (((attachment != -1)) && (attachment < endPos))
                    endPos = attachment;

                if (endPos == -1)
                    endPos = token.LastIndexOf('}');

                parsedInput.Add(token.Substring(startPos, endPos - startPos));

                token = token.Substring(endPos + 1);
            }

            return parsedInput;
        }

        private List<VkBoardTopic> ParseTopicListToken(string token)
        {
            List<VkBoardTopic> topics = new List<VkBoardTopic>();

            int startPos = token.IndexOf('[') + 1;
            int endPos = token.IndexOf(']');

            token = token.Substring(startPos, endPos - startPos);

            List<string> parsedInput = ParseTokenByBraces(token);

            string[] delims = new string[] { ",\"" };

            for (int i = 0; i < parsedInput.Count; i++)
            {
                string[] topicInfo = parsedInput[i].Split(delims, StringSplitOptions.RemoveEmptyEntries);

                topics.Add(convertInformationToTopic(topicInfo));
            }

            return topics;
        }

        private VkBoardTopic convertInformationToTopic(string[] tokens)
        {
            int id = 0;
            int comments = 0;
            string title = "";
            for (int i = 0; i < tokens.Length; i++)
            {
                string buff = "";

                int typeOfToken = checkTopicProperty(tokens[i].Split(':')[0]);
                if (typeOfToken == -1) continue;

                string[] delims = new string[] { "\":" };

                switch (typeOfToken)
                {
                    case 0:
                        buff = tokens[i].Split(delims, StringSplitOptions.RemoveEmptyEntries)[1];
                        id = int.Parse(buff);
                        break;
                    case 1:
                        buff = tokens[i].Split(delims, StringSplitOptions.RemoveEmptyEntries)[1];
                        comments = int.Parse(buff);
                        break;
                    case 2:
                        buff = tokens[i].Split(delims, StringSplitOptions.RemoveEmptyEntries)[1];
                        title = buff;
                        break;
                }
            }

            return new VkBoardTopic() { Id = id, Title = title, Comments = comments };
        }

        private int checkTopicProperty(string token)
        {
            if (token.IndexOf("id") != -1) return 0;
            if (token.IndexOf("title") != -1) return 2;
            if (token.IndexOf("comments") != -1) return 1;
            return -1;
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