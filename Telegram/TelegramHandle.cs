using AMXWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telepawn.Telegram
{
    public class TelegramHandle
    {
        public TelegramBotClient m_TelegramBotClient;
        private CancellationTokenSource m_Cts;
        private CancellationToken m_CancellationToken;
        private ReceiverOptions m_rcvOptions;


        //User data?
        public User m_User = null;

        public TelegramHandle()
        {
            try
            {
                m_TelegramBotClient = new TelegramBotClient(Program.m_Settings._botToken);

                var me = m_TelegramBotClient.GetMeAsync();
                m_User = me.Result;

                m_Cts = new CancellationTokenSource();
                m_CancellationToken = m_Cts.Token;
                m_rcvOptions = new ReceiverOptions
                {
                    AllowedUpdates = { } // receive all update types
                };
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex);
                Utils.Log.Info("[HINT] Above exception may be due to a wrong Telegram Bot Token! Set a Telegram Bot up!");
                Thread.Sleep(5000);
            }
        }

        public bool StartReceiving()
        {
            this.m_TelegramBotClient.StartReceiving(
               this.HandleUpdateAsync,
               this.HandleErrorAsync,
               m_rcvOptions,
               m_CancellationToken
           );
            return true;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await Task.Run(() => {

                if (update.Message is Message message)
                {
                    Utils.Log.Debug("Received message ( " + message.Type.ToString() + " ) : " + message.Text);
                    switch (message.Type)
                    {
                        case MessageType.Text:
                            //await botClient.SendTextMessageAsync(message.Chat, "Hello");
                            AMXPublic p = Program.m_Scripts[0].m_Amx.FindPublic("OnTelegramMessage");
                            if(p != null)
                            {
                                var tmp = p.AMX.Push(message.Text);
                                var tmp2 = p.AMX.Push(message.From.Username);
                                var tmp3 = p.AMX.Push(message.MessageId.ToString());
                                var tmp4 = p.AMX.Push(message.Chat.Id.ToString());
                                p.Execute();
                                p.AMX.Release(tmp);
                                p.AMX.Release(tmp2);
                                p.AMX.Release(tmp3);
                                p.AMX.Release(tmp4);
                            }
                           
                            break;
                    }
                }
            });
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            //Untested but should work somehow.
            if (exception is ApiRequestException apiRequestException)
            {

                await Task.Run(() =>
                {
                    Utils.Log.Debug("Telegram exception " + apiRequestException.ToString() + ": " + apiRequestException.Message + "  at  " + apiRequestException.Source);
                    
                    AMXPublic p = Program.m_Scripts[0].m_Amx.FindPublic("OnTelegramError");

                    if(p != null)
                    {
                        var tmp1 = p.AMX.Push(apiRequestException.Message);
                        var tmp2 = p.AMX.Push(apiRequestException.Source);
                        p.AMX.Push(apiRequestException.ErrorCode);
                        p.Execute();
                        p.AMX.Release(tmp1);
                        p.AMX.Release(tmp2);
                    }
                    
                });
            }
        }
    }
}
