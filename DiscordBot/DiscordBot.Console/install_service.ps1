$params = @{
  Name = "FIFElementDiscordBot"
  DisplayName = "FIF Element's Discord Bot"
  Description = "This is a bot for FIF Element's Discord server."
  BinaryPathName = "dotnet C:\DiscordBot\DiscordBot.Console.dll --environment=production"
  StartupType = "Automatic"
}
New-Service @params

Start-Service -Name "FIFElementDiscordBot"