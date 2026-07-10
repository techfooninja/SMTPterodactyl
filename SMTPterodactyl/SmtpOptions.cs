namespace SMTPterodactyl;

public class SmtpOptions
{
    public int[] Ports { get; set; } = new int[] { 25 };

    public string ServerName { get; set; } = "localhost";
}
