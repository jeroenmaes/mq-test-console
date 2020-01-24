using IBM.WMQ;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqTestConsole.Transport
{
    public class MqReceiver : IDisposable
    {
        private readonly MqSettings _settings;
        private readonly MQQueueManager _mqQmgr;

        public MqReceiver(MqSettings settings)
        {
            _settings = settings;
            _mqQmgr = MqQueueManager.InitQueueManager(_settings.Channel, _settings.Connection, _settings.QueueManager);
        }
        
        public void GetMessages(string queueName, Func<Message, Task> messageHandler, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                Logger.LogMessage($"Start receive messages on queue '{queueName}'...");

                while (!ct.IsCancellationRequested)
                {
                    GetMessage(queueName, messageHandler, ct);

                    ct.ThrowIfCancellationRequested();

                    Thread.Sleep(1000);
                }
            }
            catch (OperationCanceledException e)
            {
                
            }
        }

        private void GetMessage(string queueName, Func<Message, Task> messageHandler, CancellationToken ct = default(CancellationToken))
        {

            var mqGetMsgOpts = new MQGetMessageOptions
            {
                WaitInterval = 1000,
                Options = MQC.MQGMO_WAIT | MQC.MQGMO_SYNCPOINT | MQC.MQGMO_FAIL_IF_QUIESCING
            };
            
            MQQueue mqQueue;
            try
            {
                mqQueue = _mqQmgr.AccessQueue(queueName, MQC.MQOO_OUTPUT | MQC.MQOO_INPUT_SHARED | MQC.MQOO_INQUIRE);
            }
            catch (MQException mqe)
            {
                Logger.LogMessage("MQQueueManager::AccessQueue ended with " + mqe);
                return;
            }

            try
            {
                bool error = false;
                while (!error && !ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();

                    try
                    {
                        var mqMsg = new MQMessage();
                        mqQueue.Get(mqMsg, mqGetMsgOpts);

                        var msgBody = mqMsg.ReadString(mqMsg.MessageLength);
                        var messageId = mqMsg.GetStringProperty("MessageId");
                        var msg = new Message {Body = msgBody, MessageId = messageId};

                        messageHandler(msg);

                        _mqQmgr.Commit();
                    }
                    catch (MQException mqEx1)
                    {
                        if (mqEx1.Reason == 2033) // No message available
                        {
                            error = true;
                        }
                    }

                }
            }
            catch (MQException mqe)
            {
                // report reason, if any
                if (mqe.Reason == MQC.MQRC_NO_MSG_AVAILABLE)
                {
                    // special report for normal end
                    Logger.LogMessage("Wait timeout happened");
                }
                else
                {
                    // general report for other reasons
                    Logger.LogMessage("MQQueue::Get ended with " + mqe);

                    // treat truncated message as a failure for this sample
                    if (mqe.Reason == MQC.MQRC_TRUNCATED_MSG_FAILED)
                    {
                        // TODO Handle connection error here
                    }
                }
            }

            try
            {
                //Close the Queue
                mqQueue.Close();
            }
            catch (MQException mqe)
            {
                Logger.LogMessage("Completion code " + mqe.CompletionCode + "Reason code " + mqe.ReasonCode);
            }
        }

        public void Dispose()
        {
            _mqQmgr.Disconnect();
            ((IDisposable) _mqQmgr)?.Dispose();
        }
    }
}
