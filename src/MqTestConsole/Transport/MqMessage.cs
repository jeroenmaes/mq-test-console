using IBM.WMQ;

namespace MqTestConsole.Transport
{
    public static class MqMessage
    {
        public static MQMessage InitMqMessage()
        {
            var mqMsg = new MQMessage
            {
                CharacterSet = MQC.CODESET_UTF,
                Encoding = MQC.MQENC_NATIVE,
                Format = MQC.MQFMT_STRING,
                MessageType = MQC.MQMT_DATAGRAM,
                Persistence = MQC.MQPER_PERSISTENT,
                PutApplicationName = "MqTestConsole",
                PutApplicationType = MQC.MQAT_WINDOWS
            };
            
            return mqMsg;
        }
    }
}
