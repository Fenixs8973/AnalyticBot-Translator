using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace HabrPost.Controllers.Messages
{
    class MessageController
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public async void SendSimpleMessage(string msg, long chatId)
        {
            try
            {
                Message message = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: msg
                    );
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        
        public async void SendInlineKeyboardMessage(string msg, InlineKeyboardMarkup inlineKeyboardMarkup, long chatId)
        {
            try
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: msg,
                    replyMarkup: inlineKeyboardMarkup);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
            }
            
        }

        public async void ReplaceInlineKeyboardMessage(string msg, InlineKeyboardMarkup inlineKeyboardMarkup, Update update)
        {
            long chatId = update.CallbackQuery.Message.Chat.Id;
            int messageId = update.CallbackQuery.Message.MessageId;
            Message EditText = await botClient.EditMessageTextAsync(
                chatId: update.CallbackQuery.Message.Chat.Id,
                messageId: messageId,
                text: msg,
                replyMarkup: inlineKeyboardMarkup
            );
        }
        static public readonly InlineKeyboardMarkup OnlyStart = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/start")
                }
            });
    }
}