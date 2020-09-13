namespace DiscordBot.Core.FIFElementBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Discord.Commands;

    using DiscordBot.Core.Attributes;
    using DiscordBot.Core.Extensions;
    using DiscordBot.Core.Interfaces;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class FIFElementBotModuleProvider : IDiscordBotModuleService
    {
        private readonly ICommandService commandService;

        private readonly IConfiguration configuration;

        private readonly ILogger<FIFElementBotModuleProvider> logger;

        public FIFElementBotModuleProvider(ILogger<FIFElementBotModuleProvider> logger, IConfiguration configuration,
                                           ICommandService commandService)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.commandService = commandService;
        }

        public string Help()
        {
            IEnumerable<CommandInfo> commandList = GetCommands();

            var builder = new StringBuilder();

            builder.AppendLine(
                "Are you trying to use me? Tag me, tell me a command, and provide additional information when needed.");
            builder.AppendLine();
            builder.Append($"Usage: @{configuration.GetAppSetting("BotName")} ");
            builder.AppendLine("{command} {data}");
            builder.AppendLine();
            builder.AppendLine("Commands -");

            foreach (CommandInfo command in commandList)
            {
                var usageAttribute =
                    command.Attributes.FirstOrDefault(attribute => attribute is UsageAttribute) as UsageAttribute;

                if (usageAttribute is default(UsageAttribute))
                {
                    builder.AppendLine($"\t{command.Name} - {command.Summary}");
                }
                else
                {
                    builder.AppendLine($"\t{command.Name} {usageAttribute.Usage} - {command.Summary}");
                }
            }

            return builder.ToString();
        }

        public string SocialMedia()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Vibe with a dood, visit our social media pages:");
            builder.AppendLine($"\tInstagram: {configuration.GetAppSetting("Instagram")}");

            return builder.ToString();
        }

        private IEnumerable<CommandInfo> GetCommands()
        {
            List<CommandInfo> commands = commandService.GetCommands().ToList();
            commands.Sort((command1, command2) =>
                string.Compare(command1.Name, command2.Name, StringComparison.CurrentCulture));
            return commands;
        }
    }
}