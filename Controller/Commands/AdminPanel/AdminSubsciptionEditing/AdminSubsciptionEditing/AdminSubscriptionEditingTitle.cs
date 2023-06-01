using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.Messages;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;
using System.Text.RegularExpressions;
using HabrPost.Model;
using HabrPost.LogException;

namespace HabrPost.Controllers.Commands
{
    public class AdminSubscriptionEditingTitle : ICommand, IRedirection
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionEditingTitle";

        public CommandExecutor Executor { get; }

        public AdminSubscriptionEditingTitle(CommandExecutor executor)
        {
            Executor = executor;
        }

        MessageController mc = new MessageController();

        private string? subscriptionTitle;
        private string? newTitle;

        //Получил ли бот новое название
        private bool titleAccept = false;
        //Список подписок
        SubList[] subList = Subscriptions.subList;

        public async Task Execute(Update update)
        {
            MessageController mc = new MessageController();
            //Регулярка для получения название подписки
            Regex priceSubscription = new Regex(@"^AdminSubscriptionEditingTitle(.*)");
            //Выделяем свпадения
            Match match = priceSubscription.Match(update.CallbackQuery.Data);
            //Получаем название подписки
            subscriptionTitle = match.Groups[1].ToString();
            await mc.SendSimpleMessage("Пришлите новое название для подписки", update);
            Executor.StartListen(this);
        }

        public async Task Redirection(Update update)
        {
            DBRequest db = new DBRequest();
            MessageController mc = new MessageController();
            InlineKeyboardMarkup inlineKeyboardMarkup = new(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "AdminSubscriptionManager")
                    }
                });
            try
            {                
                //Получаем текст для описания
                newTitle = update.Message.Text;
                //Меняем текст описания
                db.SQLInstructionSet($"UPDATE subscriptions SET title = '{newTitle}' WHERE title = '{subscriptionTitle}'");
                //Отправляем сообщение об успешном редактировании описания
                await mc.SendInlineKeyboardMessage($"Название для {subscriptionTitle} успешно узменена на:\n\n {newTitle}", inlineKeyboardMarkup, update);
                //Останавливаем переадрессацию
                Executor.StopListen(update);
            }
            catch (Exception exception)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger();
                exceptionLogger.NewException(exception);
                Executor.StopListen(update);
            }
        }
    }
}