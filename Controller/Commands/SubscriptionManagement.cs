using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;

namespace HabrPost.Controllers.Commands
{
    class SubscriptionManagement : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "SubscriptionManagement";

        public async Task Execute(Update update)
        {
            long chatId = update.CallbackQuery.Message.Chat.Id;

            //update.CallbackQuery.Message.ReplyMarkup
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Подписаться", callbackData: "QuerySubscribe"),
                    InlineKeyboardButton.WithCallbackData(text: "Отписаться", callbackData: "QueryUnSubscribe"),
                },
                // second row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/start")
                }
            });
            string msg = "Управление подпиской. Вы можете подписаться или отписаться.";
            MessageController mc = new MessageController();
            mc.ReplaceInlineKeyboardMessage(msg, inlineKeyboard, update);
        }
    }
}