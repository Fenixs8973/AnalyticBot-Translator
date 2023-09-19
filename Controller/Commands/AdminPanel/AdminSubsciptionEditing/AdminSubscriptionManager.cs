using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Model;
using HabrPost.Model.Struct;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminSubscriptionManager : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionManager";

        SubscriptionsArray subscriptions = new SubscriptionsArray();

        public async Task Execute(Update update)
        {
            string msg = "Управление списком подписок";
            foreach(Subscription i in SubscriptionsArray.subArray)
            {
                msg += $"\n\n{i.title}:\nОписание: {i.description}\nЦена: {i.price}р.";
            }
            //Формируем inlineKeyboard для всех подписок
            InlineKeyboardMarkup inlineKeyboard = await MessageController.GetInlineKeyboardForAdminSubscriptions();
            await MessageController.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboard, update);
        }
    }
}