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

            client = new TelegramBotClient("6266923002:AAEVzH7kTGIB1-Eajgdycs-Su9EQ2ttZDAA");
            return client;
        }
    }
}