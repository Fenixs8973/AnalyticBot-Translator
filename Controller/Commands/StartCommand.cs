using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands
{
    public class StartCommand : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "/start";

        public async Task Execute(Update update)
        {
            long chatId;
            MessageController mc = new MessageController();
            DBRequest db = new DBRequest();

            //Уникальный идентификатор пользователя
            string userName;
            //Имя
            string firstName;
            //Текст сообщения
            string msg;
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Управление подписками", callbackData: "SubscriptionManager"),
                }
            });
            try
            {
                chatId = update.Message.Chat.Id;
                userName = update.Message.From.Username;
                firstName = update.Message.From.FirstName;
                msg = $"{firstName}, здравствуйте! Вы находитесь в меню.";
                await mc.SendInlineKeyboardMessage(msg, inlineKeyboard, update);
            }
            catch
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
                userName = update.CallbackQuery.Message.From.Username;
                firstName = update.CallbackQuery.Message.From.FirstName;
                msg = $"{firstName}, здравствуйте! Вы находитесь в меню.";
                await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboard, update);
            }
            
            if(!db.HasUserInDB(chatId))
            {
                db.SQLInstructionSet($"INSERT INTO users (tg_id, first_name, username, is_admin) VALUES ('{chatId}', '{firstName}', '{userName}', FALSE)");
            }
        }
    }
}