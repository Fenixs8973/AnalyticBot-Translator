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
    public class AdminSubscriptionEditingDescription : ICommand, IRedirection
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionEditingDescription";

        public CommandExecutor Executor { get; }

        public AdminSubscriptionEditingDescription(CommandExecutor executor)
        {
            Executor = executor;
        }

        private string? subscriptionTitle;
        private string? newDescription;

        //Получил ли бот новое название
        private bool descriptionAccept = false;
        //Список подписок
        Subscription[] Subscription = SubscriptionsArray.subArray;

        public async Task Execute(Update update)
        {
            MessageController mc = new MessageController();
            //Регулярка для получения название подписки
            Regex priceSubscription = new Regex(@"^AdminSubscriptionEditingDescription(.*)");
            //Выделяем свпадения
            Match match = priceSubscription.Match(update.CallbackQuery.Data);
            //Получаем название подписки
            subscriptionTitle = match.Groups[1].ToString();
            await MessageController.SendSimpleMessage("Пришлите новое описание для подписки", update);
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
            //Получаем текст для описания
            if(update.Message.Text != null)
            {
                newDescription = update.Message.Text;
            }
            else
            {
                return;
            }
            //Меняем текст описания
            await DBRequest.SQLInstructionSet($"UPDATE subscriptions SET description = '{newDescription}' WHERE title = '{subscriptionTitle}'");
            //Отправляем сообщение об успешном редактировании описания
            await MessageController.SendInlineKeyboardMessage($"Описание для {subscriptionTitle} успешно узменена на:\n\n {newDescription}", inlineKeyboardMarkup, update);
            //Останавливаем переадрессацию
            Executor.StopListen(update);
        }
    }
}