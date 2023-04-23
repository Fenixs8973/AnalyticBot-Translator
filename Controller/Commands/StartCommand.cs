using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;

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
            string msg = "Здравствуйте! Вы находителсь в меню.";
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Управление подпиской", callbackData: "SubscriptionManagement"),
                }
            });
            try
            {
                chatId = update.Message.Chat.Id;
                mc.SendInlineKeyboardMessage(msg, inlineKeyboard, chatId);
            }
            catch
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
                mc.ReplaceInlineKeyboardMessage(msg, inlineKeyboard, update);
            }
        }
    }
}