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

        Subscriptions subscriptions = new Subscriptions();

        public async Task Execute(Update update)
        {
            string msg = "Управление списком подписок";
            foreach(SubList i in Subscriptions.subList)
            {
                msg += $"\n\n{i.title}:\nОписание: {i.description}\nЦена: {i.price}р.";
            }
            MessageController mc = new MessageController();
            //Формируем inlineKeyboard для всех подписок
            InlineKeyboardMarkup inlineKeyboard = mc.GetInlineKeyboardForAdminSubscriptions();
            await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboard, update);
        }
    }
}