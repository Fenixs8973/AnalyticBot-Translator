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
    public class AdminSubscriptionEditingPrice : ICommand, IRedirection
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionEditingPrice";

        public CommandExecutor Executor { get; }

        public AdminSubscriptionEditingPrice(CommandExecutor executor)
        {
            Executor = executor;
        }

        MessageController mc = new MessageController();

        private string? subscriptionTitle;
        private string? newPriceStr;
        private int? newPrice;

        //Получил ли бот новое название
        private bool priceAccept = false;
        //Список подписок
        SubList[] subList = Subscriptions.subList;

        public async Task Execute(Update update)
        {
            MessageController mc = new MessageController();
            //Регулярка для получения название подписки
            Regex priceSubscription = new Regex(@"^AdminSubscriptionEditingPrice(.*)");
            //Выделяем свпадения
            Match match = priceSubscription.Match(update.CallbackQuery.Data);
            //Получаем название подписки
            subscriptionTitle = match.Groups[1].ToString();
            await mc.SendSimpleMessage("Пришлите новую цену для подписки", update);
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
                //Если новую цену еще не получали
                if(!priceAccept)
                {
                    //Получаем новую цену подписки
                    try
                    {
                        //Пробуем конвертировать сообщение в int
                        newPrice = Convert.ToInt32(update.Message.Text);
                        //Меняем цену подписки
                        db.SQLInstructionSet($"UPDATE subscriptions SET price = {newPrice} WHERE title = '{subscriptionTitle}'");
                        //Отправляем сообщение об успешном редактировании цены
                        await mc.SendInlineKeyboardMessage($"Цена для {subscriptionTitle} успешно узменена на {newPrice}", inlineKeyboardMarkup, update);
                        //Останавливаем переадрессацию
                        Executor.StopListen(update);
                    }
                    //Если не получилось конвертировать
                    catch
                    {
                        await mc.SendInlineKeyboardMessage("Вы ввели не правильную цену. Пожалуйста введите цену новой подписки", inlineKeyboardMarkup, update);
                        //Цену еще не получили
                        priceAccept = false;
                        return;
                    }
                }
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