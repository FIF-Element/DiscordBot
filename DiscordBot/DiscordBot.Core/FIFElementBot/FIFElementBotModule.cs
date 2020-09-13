namespace DiscordBot.Core.FIFElementBot
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Discord;
    using Discord.Commands;

    using DiscordBot.Core.Attributes;
    using DiscordBot.Core.Interfaces;

    using Microsoft.Extensions.Logging;

    internal class FIFElementBotModule : ModuleBase<SocketCommandContext>
    {
        private static readonly object asyncLock = new object();

        private static bool isRunningAsyncMethod;

        private readonly Emoji hourglass = new Emoji("\u23F3");

        private readonly ILogger<FIFElementBotModule> logger;

        private readonly IDiscordBotModuleService service;

        public FIFElementBotModule(IDiscordBotModuleService service, ILogger<FIFElementBotModule> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [Hidden]
        [Command("bad bot")]
        [Summary("Tell the bot it's being bad")]
        public Task AcknowledgeBadBot()
        {
            return Reply("D:");
        }

        [Hidden]
        [Command("good bot")]
        [Summary("Tell the bot it's being good")]
        public Task AcknowledgeGoodBot()
        {
            return Reply(":D");
        }

        [Command("help")]
        [Summary("Show the list of available commands")]
        public async Task Help()
        {
            await Reply(service.Help());
        }

        [Default]
        [Hidden]
        [Command("{default}")]
        [Summary("Show the list of available commands")]
        public async Task NoCommand()
        {
            await Reply(service.Help());
        }

        private async Task Reply(string message, [CallerMemberName] string methodName = "")
        {
            await Reply(() => Task.FromResult(message), methodName);
        }

        private async Task Reply(Func<Task<string>> getMessage, [CallerMemberName] string methodName = "")
        {
            try
            {
                logger.LogInformation("Method Invoked: {methodName}", methodName);

                await Context.Message.AddReactionAsync(hourglass);

                await ReplyAsync(await getMessage.Invoke());

                await Context.Message.RemoveReactionAsync(hourglass, Context.Client.CurrentUser,
                    RequestOptions.Default);

                logger.LogInformation("Method Finished: {methodName}", methodName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "There was an unhandled exception");
            }
        }

        private async Task ReplyAsyncMode(Func<Task<string>> getMessage, [CallerMemberName] string methodName = "")
        {
            var runAsync = true;

            lock (asyncLock)
            {
                if (isRunningAsyncMethod)
                {
                    runAsync = false;
                }
                else
                {
                    isRunningAsyncMethod = true;
                }
            }

            if (runAsync)
            {
                await Reply(getMessage, methodName);

                isRunningAsyncMethod = false;
            }
            else
            {
                await Reply("Wait until the bot has finished responding to another user's long running request.");
            }
        }
    }
}