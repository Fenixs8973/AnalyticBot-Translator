using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.Messages;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands
{
    class PublishNews : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "PublishNews";

        public async Task Execute(Update update)
        {
            string news = update.CallbackQuery.Message.Text;
            news = news.Remove(0, 13);
            news = "Вышел новый пост:\n\n" + news;
            DBRequest db = new DBRequest();
            MessageController mc = new MessageController();
            List<long> subs = db.GetListSubs();
            foreach(long chatId in subs)
                mc.SendSimpleMessage(news, chatId);
        }
    }
}