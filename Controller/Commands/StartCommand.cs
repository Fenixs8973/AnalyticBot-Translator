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
            long chatId = CommandExecutor.GetChatId(update);

            //Уникальный идентификатор пользователя
            string userName = null;
            //Имя
            string firstName = null;
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

            if(update.Message != null)
            {
                userName = update.Message.From.Username;
                firstName = update.Message.From.FirstName;
                msg = $"{firstName}, здравствуйте! Вы находитесь в меню.";
                await MessageController.SendInlineKeyboardMessage(msg, inlineKeyboard, update);
            }
            else if(update.CallbackQuery != null)
            {
                userName = update.CallbackQuery.Message.From.Username;
                firstName = update.CallbackQuery.Message.From.FirstName;
                msg = $"{firstName}, здравствуйте! Вы находитесь в меню.";
                await MessageController.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboard, update);
            }
            
            if(!await DBRequest.HasUserInDB(chatId))
            {
                await DBRequest.SQLInstructionSet($"INSERT INTO users (tg_id, first_name, username, is_admin) VALUES ('{chatId}', '{firstName}', '{userName}', FALSE)");
            }
        }
    }
}