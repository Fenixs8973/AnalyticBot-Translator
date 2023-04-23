using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.DB;
using HabrPost.Controllers.Messages;

namespace HabrPost.Controllers.Commands
{
    class QueryUnSubscribe : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "QueryUnSubscribe";

        private bool subscribed { get; set; }
        public void SetSubscribed(bool i) { subscribed = i; }
        private bool hasUser { get; set; }
        public void SetHasUser(bool i) { hasUser = i; }

        public async Task Execute(Update update)
        {
            DBRequest db = new DBRequest();
            MessageController mc = new MessageController();
            long chatId = update.CallbackQuery.From.Id;
            
            db.HasUserInDB(chatId);
            hasUser = DBRequest.hasUser;
            subscribed = DBRequest.subscribed;
            if(hasUser)
            {
                if(!subscribed)
                {
                    mc.ReplaceInlineKeyboardMessage("У вас нету активных подписок!", MessageController.OnlyStart, update);
                }
                else
                {
                    db.SQLInstructionSet($"UPDATE users SET subscribed = FALSE WHERE tgid = {chatId}");
                    mc.ReplaceInlineKeyboardMessage("Вы успешно отписаны!", MessageController.OnlyStart, update);
                }
            }
            else
            {
                db.SQLInstructionSet($"INSERT INTO users (tgid, subscribed) VALUES ({chatId} , false)");
                mc.ReplaceInlineKeyboardMessage("У вас нету активных подписок, вы доавлены в бд!", MessageController.OnlyStart, update);
            }
        }
    }
}