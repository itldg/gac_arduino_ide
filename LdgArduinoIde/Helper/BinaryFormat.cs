using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LdgArduinoIde
{
    public class BinaryFormat
    {

        public static byte[] Serialize(Object Urobject) //序列化 返回byte[]类型
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memory = new MemoryStream();
            bf.Serialize(memory, Urobject);
            byte[] bytes = memory.GetBuffer();
            memory.Close();
            return bytes;
        }

        public static object Deserialize(byte[] bytes) //反序列化，返回object类型的
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memory = new MemoryStream(bytes);
            object ss = bf.Deserialize(memory);
            memory.Close();
            return ss;
        }
        public static object Deserialize(string FileName) //反序列化，返回object类型的
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            object ss = bf.Deserialize(fileStream);
            fileStream.Close();
            return ss;
        }
    }

}
