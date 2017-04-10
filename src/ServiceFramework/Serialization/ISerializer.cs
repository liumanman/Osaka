using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceFramework.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize(object obj);
        object Deserialize(Type type, byte[] data);
    }
}
