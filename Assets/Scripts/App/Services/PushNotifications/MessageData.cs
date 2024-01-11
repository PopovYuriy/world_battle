using System.Collections.Generic;

namespace App.Services.PushNotifications
{
    public class MessageData
    {
        public MessageType Type { get; }
        public IDictionary<string, string> Data { get; }
        public bool IsOpened { get; private set; }

        public MessageData(MessageType type, IDictionary<string, string> data, bool opened)
        {
            Type = type;
            Data = data;
            IsOpened = opened;
        }

        public override string ToString()
        {
            var result = $"{nameof(MessageData)}:\n";
            result += $"Message type = {Type}";
            result += "\nData:";

            foreach (var kv in Data)
                result += $"\n  {kv.Key} : {kv.Value}";
            
            return result;
        }
    }
}