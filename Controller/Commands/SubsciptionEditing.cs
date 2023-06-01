using System.Text.RegularExpressions;
using HabrPost.LogException;
using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;
using HabrPost.Controllers.Messages;
using Telegram.Bot.Types.ReplyMarkups;

namespace HabrPost.Controllers.Commands.Admin
{
    class SubsciptionEditing : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "SubscriptionEditing";

        public async Task Execute(Update update)
        {
            Regex titleSubscription = new Regex(@"^SubscriptionEditing(.*)");
            CallbackQuery callBack = update.CallbackQuery;
            DBRequest db = new DBRequest();
            string titleSub = "";
            SubList subscriptions = new SubList();
            bool hasSubscriptrion = false;
            //Пытаемся найти имя подписки
            try
            {
                Match match = titleSubscription.Match(callBack.Data);
                titleSub = match.Groups[1].ToString();
                subscriptions = db.GetSubscriptionTitle(titleSub);
            }  
            catch (Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
            }         
            MessageController mc = new MessageController();
            InlineKeyboardMarkup SubscribeAccept = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Подписаться", callbackData: $"SubscribeAccept{titleSub}"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "SubscriptionManager"),
                }
            });
            InlineKeyboardMarkup SubscribeCancel = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Отписаться", callbackData: $"SubscribeCancel{titleSub}")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "SubscriptionManager"),
                }
            });

            hasSubscriptrion = db.HasUserSubscription(update.CallbackQuery.Message.Chat.Id, subscriptions.id);
            
            if(hasSubscriptrion)
                await mc.ReplaceInlineKeyboardMessageForMarkup($"\"{titleSub}\" - Активна\n{subscriptions.description}\n{subscriptions.price}р.", SubscribeCancel, update);
            else
                await mc.ReplaceInlineKeyboardMessageForMarkup($"\"{titleSub}\" - Не активна\n{subscriptions.description}\n{subscriptions.price}р.", SubscribeAccept, update);
        }
    }
}