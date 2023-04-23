using Telegram.Bot;

namespace HabrPost
{
    public class Bot
    {
        private static TelegramBotClient client {get; set;}

        public static TelegramBotClient GetTelegramBot()
        {
            if(client != null)
            {
                return client;
            }

            client = new TelegramBotClient("TOKEN");
            return client;
        }
    }
}