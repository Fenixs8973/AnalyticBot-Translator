using HabrPost.Controllers.DB;
using HabrPost.Controllers.Messages;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.LogException;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminSubscriptionNew : ICommand, IRedirection
    {
        MessageController mc = new MessageController();

        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionNew";

        public CommandExecutor Executor { get; }
        
        public AdminSubscriptionNew(CommandExecutor executor)
        {
            Executor = executor;
        }

        string? title = null;
        string? description = null;
        int? price = null;

        public async Task Execute(Update update)
        {
            await mc.SendSimpleMessage("Введите название подписки", update);
            Executor.StartListen(this);
        }
        public async Task Redirection(Update update)
        {
            if(title == null)
            {
                try
                {
                    title = update.Message.Text;
                    await mc.SendSimpleMessage("Введите описание для подписки", update);
                }
                catch
                {
                    UpdateDistributor<CommandExecutor> updateDistributor = new UpdateDistributor<CommandExecutor>();
                    updateDistributor.UpdateProcessing(update);
                    Executor.StopListen(update);
                    return;
                }
            }
            else if(description == null)
            {
                try
                {
                    description = update.Message.Text;
                    await mc.SendSimpleMessage("Введите цену для подписки", update);
                }
                catch
                {
                    UpdateDistributor<CommandExecutor> updateDistributor = new UpdateDistributor<CommandExecutor>();
                    updateDistributor.UpdateProcessing(update);
                    Executor.StopListen(update);
                    return;
                }
            }
            else
            {
                try
                {
                    price = Convert.ToInt32(update.Message.Text);
                }
                catch
                {
                    await mc.SendSimpleMessage("Не верная цена. Введите цену для подписки", update);
                    return;
                }
                
                DBRequest db = new DBRequest();
                try
                {
                    db.SQLInstructionSet($"INSERT INTO subscriptions (title, description, price) VALUES ('{title}', '{description}', '{price}')");
                    
                    InlineKeyboardMarkup inlineKeyboardMarkup = new(new[]
                    {
                        // first row
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                        }
                    });
                    await mc.SendInlineKeyboardMessage("Подписка успешно добавлена", inlineKeyboardMarkup, update);
                    title = null;
                    description = null;
                    price = null;
                    Executor.StopListen(update);
                }
                catch (Exception exception)
                {
                    ExceptionLogger exceptionLogger = new ExceptionLogger();
                    exceptionLogger.NewException(exception);
                }
            }
        }
    }
}