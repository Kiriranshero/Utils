    public static class MSGConvert
    {
        private static ConcurrentDictionary<Type, IMessagePackSingleObjectSerializer> Serializer = new ConcurrentDictionary<Type, IMessagePackSingleObjectSerializer>();

        public static byte[] SerializeObject(object inObject)
        {
            IMessagePackSingleObjectSerializer serializer = null;
            var inType = inObject.GetType();
            if (Serializer.TryGetValue(inType, out serializer) == false)
            {
                serializer = MessagePackSerializer.Get(inType);
                Serializer.TryAdd(inType, serializer);
            }

            return serializer.PackSingleObject(inObject);
        }
        public static T DeserializeObject<T>(byte[] indata)
        {
            IMessagePackSingleObjectSerializer serializer = null;
            var inType = typeof(T);
            if (Serializer.TryGetValue(inType, out serializer) == false)
                throw new Exception("????");

            return (T)serializer.UnpackSingleObject(indata);
        }
    }
