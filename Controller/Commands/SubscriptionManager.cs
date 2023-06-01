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

        Subscriptions subscriptions = new Subscriptions();

        public async Task Execute(Update update)
        {
            long chatId = update.CallbackQuery.Message.Chat.Id;
            DBRequest db = new DBRequest();
            string msg = "Управление подписками. Вы можете выбрать определенную подписку.";
            bool hasSubscriptrion = false;
            foreach(SubList i in Subscriptions.subList)
            {
                hasSubscriptrion = db.HasUserSubscription(chatId, i.id);
                if(hasSubscriptrion)
                    msg += $"\n\n{i.title} - Активна\n*Описание: {i.description}\n*Цена: {i.price}р.";
                else
                    msg += $"\n\n{i.title} - Не Активна\n*Описание: {i.description}\n*Цена: {i.price}р.";
            }
            MessageController mc = new MessageController();
            InlineKeyboardMarkup inlineKeyboard = mc.GetInlineKeyboardForSubscriptions();
            await mc.ReplaceInlineKeyboardMessageForMarkup(msg, inlineKeyboard, update);
        }
    }
}