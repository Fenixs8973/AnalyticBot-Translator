using Telegram.Bot.Types;

namespace HabrPost.Controllers
{
    public interface IRedirection
    {
        public async Task Redirection(Update update) { }

        public CommandExecutor Executor { get; }
    }
}