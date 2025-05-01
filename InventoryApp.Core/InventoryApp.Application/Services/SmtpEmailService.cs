using System.Net;
using System.Net.Mail;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Results;
using InventoryApp.Application.Results.Raw;


namespace InventoryApp.Application.Services;
public class SmtpConfiguration
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; }
}

public class SmtpEmailService : IEmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";  
    private readonly int _smtpPort = 587;  
    private readonly string _username = "simoshstoreco@gmail.com";  
    private readonly string _password = "lnqr khna jkbx ffyq";  

    public async Task<IServiceResult> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_username),
                Subject = subject,
                Body = body,
                IsBodyHtml = false 
            };

            mailMessage.To.Add(to);

            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.Credentials = new NetworkCredential(_username, _password);
                smtpClient.EnableSsl = true;
                smtpClient.Timeout = 60000;  
                await smtpClient.SendMailAsync(mailMessage);
                return new SuccessResult("E-posta başarıyla gönderildi.");
            }
        }
        catch (Exception ex)
        {
            return new ErrorResult($"{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}");
        }
    }
    public async Task<IServiceResult> ResetPasswordAsync(string email, string token)
    {
        try
        {
            var resetLink = $"http://localhost:3000/reset-password?token={token}";

            string subject = "Şifre Sıfırlama Bağlantısı";

            string body = $"Merhaba, <br> Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın: <br> <a href='{resetLink}'>Şifre Sıfırlama Bağlantısı</a>";

            var result = await SendEmailAsync(email, subject, body);

            if (!result.Success)
                return new ErrorResult("E-posta gönderilemedi.");

            return new SuccessResult("E-posta gönderildi.");
        }catch (Exception ex)
        {
            return new ErrorResult($"{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}");
        }
    }
    }