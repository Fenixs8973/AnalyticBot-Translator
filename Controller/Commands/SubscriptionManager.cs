using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.Messages;
using HabrPost.Model;
using HabrPost.Model.Struct;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands
{
    class SubscriptionManager : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "SubscriptionManager";

        SubscriptionsArray subscriptions = new SubscriptionsArray();

        public async Task Execute(Update update)
        {
            long chatId = CommandExecutor.GetChatId(update);
            string msg = "Управление подписками. Вы можете выбрать определенную подписку.";
            bool hasSubscriptrion = false;
            foreach(Subscription i in SubscriptionsArray.subArray)
            {
                hasSubscriptrion = await DBRequest.HasUserSubscription(chatId, i.id);
                if(hasSubscriptrion)
                    msg += $"\n\n{i.title} - Активна\n*Описание: {i.description}\n*Цена: {i.price}р.";
                else
                    msg += $"\n\n{i.title} - Не Активна\n*Описание: {i.description}\n*Цена: {i.price}р.";
            }
            InlineKeyboardMarkup inlineKeyboard = await MessageController.GetInlineKeyboardForSubscriptions();
            await MessageController.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboard, update);
        }
    }
}