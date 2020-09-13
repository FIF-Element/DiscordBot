Stop-Service -Name "FIFElementDiscordBot"

Remove-Service -Name "FIFElementDiscordBot"

sc.exe delete "FIFElementDiscordBot"