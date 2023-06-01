using Telegram.Bot.Types;
using Telegram.Bot;

namespace HabrPost.Controllers.Commands
{
    public interface ICommand
    {
        public TelegramBotClient botClient { get; }

        public string Name { get; } 

        public async Task Execute(Update update) { }
    }
}