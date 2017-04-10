using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ServiceFramework.Serialization
{
    public class JsonSerializer : ISerializer
    {
        Encoding _encoding;
        public JsonSerializer(Encoding encode = null)
        {
            _encoding = encode == null ? Encoding.UTF8 : encode;
        }
        public object Deserialize(Type type, byte[] data)
        {
            string str = _encoding.GetString(data);
            return JsonConvert.DeserializeObject(str, type);
        }

        public byte[] Serialize(object obj)
        {
            string str = JsonConvert.SerializeObject(obj);
            return _encoding.GetBytes(str);
        }
    }
}
