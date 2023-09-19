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
    public class AdminNewPublication : ICommand, IRedirection
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminNewPublication";

        public CommandExecutor Executor { get; }

        public AdminNewPublication(CommandExecutor executor)
        {
            Executor = executor;
        }

        private string? news;

        //Получил ли бот текст новости
        private bool newsAccept = false;
        //Список подписок
        Subscription[] Subscription = SubscriptionsArray.subArray;
        //Получает ли та или иная подписка этот пост
        Dictionary<string, bool> subscriptionsStatus;

        public async Task Execute(Update update)
        {
            await MessageController.SendSimpleMessage("Пришлите текст новости", update);
            Executor.StartListen(this);
        }

        public async Task Redirection(Update update)
        {
            //Получаем кнопки для подписок
            InlineKeyboardMarkup inlineKeyboardMarkup = await MessageController.GetInlineKeyboardForNewPublication();
            if(!newsAccept)
            {
                //Получаем текст новости, если еще не получали
                news = update.Message.Text;
                await MessageController.SendInlineKeyboardMessage($"Новый пост:\n\n{news}\n\nПолучатели:\nПолучатели не выбраны", inlineKeyboardMarkup, update); 
                subscriptionsStatus = new Dictionary<string, bool>();
                //Заполняем словарь названиями подписок
                foreach(Subscription i in Subscription)
                {
                    subscriptionsStatus.Add(i.title, false);
                }
                Subscription = await DBRequest.GetSubscriptions();
                //Получили ли мы уже текст новости
                newsAccept = true;
            }
            else if(newsAccept)
            {
                string subscriptionTitle;
                Regex regex = new Regex("^AdminNewPublication(.*)");
                MatchCollection regexMatches = regex.Matches(update.CallbackQuery.Data);
                if(regexMatches.Count > 0)
                {
                    Match match = regex.Match(update.CallbackQuery.Data);
                    //Получаем название подписки
                    subscriptionTitle = match.Groups[1].ToString();
                    //Инвертируем значение
                    subscriptionsStatus[subscriptionTitle] = !subscriptionsStatus[subscriptionTitle];
                    string message = $"Новый пост:\n\n{news}\n\nПолучатели:";
                    //Если словарь не содержит подписки с true
                    if(!subscriptionsStatus.ContainsValue(true))
                    {
                        message += $"\nПолучатели не выбраны";
                    }
                    else
                    {
                        foreach(var i in subscriptionsStatus)
                        {
                            if(i.Value)
                            message += $"\n-{i.Key};";
                        }
                    }
                    await MessageController.ReplaceInlineKeyboardMessageForMarkup(message, inlineKeyboardMarkup, update);
                }
                else
                {
                    Executor.StopListen(update);
                    UpdateDistributor<CommandExecutor> updateDistributor = new UpdateDistributor<CommandExecutor>();
                    await updateDistributor.UpdateProcessing(update);
                }
            }
        }
    }
}