using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Application.Utility
{
    public class ObjectFormatter
    {
        /// <summary>
        /// Coverts instance object to its bytes representation.
        /// </summary>
        /// <param name="instanceObject"></param>
        /// <returns></returns>
        public static byte[] GetInstanceBytes(object instanceObject)
        {
            if (instanceObject == null)
            {
                return null;
            }

            MemoryStream memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, instanceObject);
            return memoryStream.GetBuffer();
        }

        /// <summary>
        /// Coverts instance bytes into instance object.
        /// </summary>
        /// <param name="instanceObject"></param>
        /// <returns></returns>
        public static object GetInstanceObject(byte[] instanceBytes)
        {
            if (instanceBytes == null)
            {
                return null;
            }

            MemoryStream memoryStream = new MemoryStream(instanceBytes);
            object instanceObject = new BinaryFormatter().Deserialize(memoryStream);
            return instanceObject;
        }
    }
}
