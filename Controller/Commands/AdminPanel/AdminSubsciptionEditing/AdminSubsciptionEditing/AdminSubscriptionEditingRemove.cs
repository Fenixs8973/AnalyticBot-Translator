using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Model;
using HabrPost.Model.Struct;
using System.Text.RegularExpressions;
using HabrPost.LogException;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminSubscriptionEditingRemove : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionEditingRemove";

        Subscriptions subscriptions = new Subscriptions();

        public async Task Execute(Update update)
        {
            Regex titleSubscription = new Regex(@"^AdminSubscriptionEditingRemove(.*)");
            CallbackQuery callBack = update.CallbackQuery;
            string titleSub = "";
            string msg = $"Вы уверены, что хотите удалить подписку {titleSub}?";
            try
            {
                Match match = titleSubscription.Match(callBack.Data);
                titleSub = match.Groups[1].ToString();
            }
            catch (Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
            } 
            MessageController mc = new MessageController();
            InlineKeyboardMarkup inlineKeyboardMarkup = new(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Да, удалить", callbackData: "AdminSubscriptionEditingRemoveYes" + titleSub)
                    },
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "НЕТ, НЕ удалить", callbackData: $"AdminSubscriptionEditing{titleSub}")
                    }
                });
            await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboardMarkup, update);
        }
    }
}