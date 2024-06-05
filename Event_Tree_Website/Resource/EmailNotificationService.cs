using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Event_Tree_Website.Models;
using Microsoft.EntityFrameworkCore;

public class EmailNotificationService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly TimeSpan CheckInterval = TimeSpan.FromHours(24); // Kiểm tra mỗi 24 giờ

    public EmailNotificationService(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration, ILogger<EmailNotificationService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAndSendEmailNotifications();
            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    private async Task CheckAndSendEmailNotifications()
    {
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Event_TreeContext>();

                var targetDate = DateTime.Today.AddDays(7);

                var upcomingEvents = await context.PersonalEvents
                    .Where(e => e.DateTime.Date == targetDate)
                    .ToListAsync();

                foreach (var personalEvent in upcomingEvents)
                {
                    var user = await context.Users.FirstOrDefaultAsync(u => u.Id == personalEvent.IdUser);

                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        string subject = "Thông báo sự kiện sắp diễn ra";
                        string body = $"Sự kiện '{personalEvent.Name}' sẽ diễn ra vào ngày {personalEvent.DateTime.ToShortDateString()}.";

                        SendEmail(user.Email, subject, body);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending email notifications.");
        }
    }

    private void SendEmail(string toEmail, string subject, string body)
    {
        var fromEmail = _configuration["EmailSettings:FromEmail"];
        var emailPassword = _configuration["EmailSettings:EmailPassword"];
        var smtpHost = _configuration["EmailSettings:SmtpHost"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

        MailMessage mail = new MailMessage(fromEmail, toEmail)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            UseDefaultCredentials = false,
            Credentials = new System.Net.NetworkCredential(fromEmail, emailPassword),
            EnableSsl = true
        };

        smtpClient.Send(mail);
    }
}
