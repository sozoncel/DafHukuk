using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DafHukuk.Service
{
    public interface IEmailService
    {
        Task<bool> SendContactFormEmail(string name, string surname, string email,
            string phone, string company, string position, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendContactFormEmail(string name, string surname, string email,
            string phone, string company, string position, string message)
        {
            try
            {
                // Mail sunucu ayarları - appsettings.json'dan okunuyor
                var smtpServer = _configuration["Email:SmtpServer"] ?? "svrm16.trwww.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "465");
                var senderEmail = _configuration["Email:SenderEmail"];
                var senderPassword = _configuration["Email:SenderPassword"];
                var recipientEmail = _configuration["Email:RecipientEmail"];
                var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");

                // Gerekli bilgilerin kontrolü
                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                    Console.WriteLine("HATA: Email ayarları appsettings.json'da eksik!");
                    return false;
                }

                using var smtpClient = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail, senderPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "DAF Hukuk İletişim Formu"),
                    Subject = $"İletişim Formu - {name} {surname}",
                    Body = CreateEmailBody(name, surname, email, phone, company, position, message),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                // Form dolduran kişinin emailini de CC olarak ekle (geçerliyse)
                if (!string.IsNullOrEmpty(email) && IsValidEmail(email))
                {
                    mailMessage.CC.Add(email);
                }

                Console.WriteLine($"Mail gönderiliyor: {smtpServer}:{smtpPort} (SSL: {enableSsl})");
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Mail başarıyla gönderildi!");
                return true;
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Hatası: {smtpEx.StatusCode} - {smtpEx.Message}");
                Console.WriteLine($"Inner Exception: {smtpEx.InnerException?.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email gönderme hatası: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return false;
            }
        }

        private string CreateEmailBody(string name, string surname, string email,
            string phone, string company, string position, string message)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #1F3363; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f8f9fa; padding: 30px; margin-top: 20px; }}
                        .field {{ margin-bottom: 15px; }}
                        .label {{ font-weight: bold; color: #1F3363; }}
                        .value {{ margin-left: 10px; }}
                        .footer {{ text-align: center; margin-top: 20px; padding: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>DAF Hukuk İletişim Formu</h2>
                        </div>
                        <div class='content'>
                            <div class='field'>
                                <span class='label'>Ad Soyad:</span>
                                <span class='value'>{name} {surname}</span>
                            </div>
                            <div class='field'>
                                <span class='label'>E-posta:</span>
                                <span class='value'>{email}</span>
                            </div>
                            <div class='field'>
                                <span class='label'>Telefon:</span>
                                <span class='value'>{phone ?? "Belirtilmemiş"}</span>
                            </div>
                            <div class='field'>
                                <span class='label'>Şirket:</span>
                                <span class='value'>{company ?? "Belirtilmemiş"}</span>
                            </div>
                            <div class='field'>
                                <span class='label'>Pozisyon:</span>
                                <span class='value'>{position ?? "Belirtilmemiş"}</span>
                            </div>
                            <div class='field'>
                                <span class='label'>Mesaj:</span>
                                <div class='value' style='margin-top: 10px; white-space: pre-wrap;'>{message}</div>
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Bu mesaj DAF Hukuk web sitesi iletişim formundan gönderilmiştir.</p>
                            <p>Gönderim Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}