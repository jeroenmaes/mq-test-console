using IBM.WMQ;
using System.Collections;

namespace MqTestConsole.Transport
{
    public static class MqQueueManager
    {
        public static MQQueueManager InitQueueManager(string channelName, string connectionName, string queueManagerName)
        {
            MQQueueManager mqQMgr = null;
            
            try
            {
                var properties = new Hashtable
                {
                    {MQC.CHANNEL_PROPERTY, channelName},
                    {MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED},
                    {MQC.CONNECT_OPTIONS_PROPERTY, MQC.MQCNO_RECONNECT | MQC.MQCNO_HANDLE_SHARE_BLOCK},
                    {MQC.CONNECTION_NAME_PROPERTY, connectionName}
                };
                mqQMgr = new MQQueueManager(queueManagerName, properties);
            }
            catch (MQException mqe)
            {
                Logger.LogMessage("create of MQQueueManager ended with " + mqe);
            }

            return mqQMgr;
        }

    }
}
