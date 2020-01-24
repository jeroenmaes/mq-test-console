using System;
using System.Text;
using IBM.WMQ;

namespace MqTestConsole.Transport
{
    public class MqSender
    {
        private readonly MqSettings _settings;
        
        public MqSender(MqSettings settings)
        {
            _settings = settings;
        }

        public void PutMessage(string queueName, string messageBody, string messageId)
        {
            Logger.LogMessage($"Connect to QueueManager '{_settings.QueueManager}'...");

            var mqQMgr = MqQueueManager.InitQueueManager(_settings .Channel, _settings.Connection, _settings.QueueManager);

            MQQueue mqQueue;
            try
            {
                mqQueue = mqQMgr.AccessQueue(queueName, MQC.MQOO_OUTPUT | MQC.MQOO_INPUT_SHARED | MQC.MQOO_INQUIRE);
            }
            catch (MQException mqe)
            {
                Logger.LogMessage("MQQueueManager::AccessQueue ended with " + mqe);
                return;
            }
            
            var mqMsg = MqMessage.InitMqMessage();
            mqMsg.SetStringProperty("MessageId", messageId);
            Logger.LogMessage($"Send message to Queue '{queueName}'...");

            mqMsg.WriteString(messageBody);

            var mqPutMsgOpts = new MQPutMessageOptions {Options = MQC.MQPMO_SYNCPOINT};

            try
            {
                mqQueue.Put(mqMsg, mqPutMsgOpts);
                mqQMgr.Commit();

                mqQueue.Close();
                mqQMgr.Close();
            }
            catch (MQException mqe)
            {
                Logger.LogMessage("MQQueue::Put ended with " + mqe);
            }
        }
    }
}
