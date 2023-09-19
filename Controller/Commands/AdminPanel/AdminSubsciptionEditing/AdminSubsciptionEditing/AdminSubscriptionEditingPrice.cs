using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.Messages;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.DB;
using HabrPost.Model.Struct;
using System.Text.RegularExpressions;
using HabrPost.Model;

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

        private string? subscriptionTitle;
        private int newPrice;

        //Получил ли бот новое название
        private bool priceAccept = false;
        //Список подписок
        Subscription[] Subscription = SubscriptionsArray.subArray;

        public async Task Execute(Update update)
        {
            //Регулярка для получения название подписки
            Regex priceSubscription = new Regex(@"^AdminSubscriptionEditingPrice(.*)");
            //Выделяем свпадения
            Match match = priceSubscription.Match(update.CallbackQuery.Data);
            //Получаем название подписки
            subscriptionTitle = match.Groups[1].ToString();
            await MessageController.SendSimpleMessage("Пришлите новую цену для подписки", update);
            Executor.StartListen(this);
        }

        public async Task Redirection(Update update)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "AdminSubscriptionManager")
                }
            });             
            //Если новую цену еще не получали
            if(!priceAccept)
            {
                //Получаем новую цену подписки
                if(Int32.TryParse(update.Message.Text, out newPrice))
                {
                    newPrice = Convert.ToInt32(update.Message.Text);
                    //Меняем цену подписки
                    await DBRequest.SQLInstructionSet($"UPDATE subscriptions SET price = {newPrice} WHERE title = '{subscriptionTitle}'");
                    //Отправляем сообщение об успешном редактировании цены
                    await MessageController.SendInlineKeyboardMessage($"Цена для {subscriptionTitle} успешно узменена на {newPrice}", inlineKeyboardMarkup, update);
                    //Останавливаем переадрессацию
                    Executor.StopListen(update);
                }
                //Если не получилось конвертировать
                else
                {
                    await MessageController.SendInlineKeyboardMessage("Вы ввели не правильную цену. Пожалуйста введите цену новой подписки", inlineKeyboardMarkup, update);
                    //Цену еще не получили
                    priceAccept = false;
                    return;
                }
            }
            Executor.StopListen(update);
        }
    }
}