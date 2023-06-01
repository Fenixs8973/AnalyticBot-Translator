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
    class SubscribeCancel : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "SubscribeCancel";

        public async Task Execute(Update update)
        {
            MessageController mc = new MessageController();
            //Сюда будем записывать данные о подписке
            SubList subscription = new SubList();
            DBRequest db = new DBRequest();
            //Регулярка для выделения названия подписки
            Regex titleSubscription = new Regex("^SubscribeCancel(.*)");
            //Сюда будем сохранять название подписки
            string titleSub = "";
            //Имеет ли пользователь подписку
            bool hasSubscriptrion = false;
            bool hasUser = false;
            //Пытаемся найти имя подписки
            try
            {
                Match match = titleSubscription.Match(update.CallbackQuery.Data);
                //Получаем имя подписки и callback'a
                titleSub = match.Groups[1].ToString();
                //Получаем данные подписки по названию
                subscription = db.GetSubscriptionTitle(titleSub);
                //Проверяем имеет ся ли у пользователя эта подписка
                hasSubscriptrion = db.HasUserSubscription(update.CallbackQuery.Message.Chat.Id, subscription.id);
                hasUser = db.HasUserInDB(update.CallbackQuery.Message.Chat.Id);
                if(!hasUser)
                {
                    db.SQLInstructionSet($"INSERT INTO users (tg_id) VALUES ('{update.CallbackQuery.Message.Chat.Id}')");
                    mc.ReplaceInlineKeyboardMessageForMarkup($"У вас нету подписки {titleSub}", MessageController.Start, update);
                    return;
                }
                
                if(!hasSubscriptrion)
                {
                    await mc.ReplaceInlineKeyboardMessageForMarkup($"У вас нету подписки {titleSub}", MessageController.Start, update);
                }
                else
                {
                    db.SQLInstructionSet($"DELETE FROM user_subscriptions WHERE tg_id='{update.CallbackQuery.Message.Chat.Id}' AND subscriptions = '{subscription.id}'");
                    await mc.ReplaceInlineKeyboardMessageForMarkup($"Вы успешно отменили подписку {titleSub}", MessageController.Start, update);
                }
            }  
            catch (Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
            }         
        }
    }
}