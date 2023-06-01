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
    class SubscribeAccept : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "SubscribeAccept";

        public async Task Execute(Update update)
        {
            MessageController mc = new MessageController();
            //Сюда будем записывать данные о подписке
            SubList subscription = new SubList();
            DBRequest db = new DBRequest();
            //Регулярка для выделения названия подписки
            Regex titleSubscription = new Regex("^SubscribeAccept(.*)");
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
                    db.SQLInstructionSet($"INSERT INTO user_subscriptions VALUES ('{update.CallbackQuery.Message.Chat.Id}', '{subscription.id}')");
                }
                
                if(!hasSubscriptrion)//INSERT INTO user_subscriptions (tg_id, user_subscriptions) VALUES ('583028387', '15') 
                {
                    db.SQLInstructionSet($"INSERT INTO user_subscriptions VALUES ('{update.CallbackQuery.Message.Chat.Id}', '{subscription.id}')");
                    await mc.ReplaceInlineKeyboardMessageForMarkup($"Подписка успешно оформлена", MessageController.Start, update);
                }
                else
                {
                    await mc.ReplaceInlineKeyboardMessageForMarkup($"Подписка уже оформлена", MessageController.Start, update);
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