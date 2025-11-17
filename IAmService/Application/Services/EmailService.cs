using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        #region Attributes
        private readonly string fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM") ?? throw new InvalidOperationException("EMAIL_FROM not found in enviroment variables.");
        private readonly string displayName = Environment.GetEnvironmentVariable("EMAIL_DISPLAY_NAME") ?? throw new InvalidOperationException("EMAIL_DISPLAY_NAME not found in enviroment variables.");
        private readonly string smtpHost = Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST") ?? throw new InvalidOperationException("EMAIL_SMTP_HOST not found in enviroment variables.");
        private readonly int smtpPort = GetSmtpPort();
        private readonly string username = Environment.GetEnvironmentVariable("EMAIL_USERNAME") ?? throw new InvalidOperationException("EMAIL_USERNAME not found in enviroment variables.");
        private readonly string password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? throw new InvalidOperationException("EMAIL_PASSWORD not found in enviroment variables.");
        private readonly bool enableSsl = GetEnableSsl();
        #endregion

        #region Properties
        #endregion

        #region Private method helper
        private static bool GetEnableSsl()
        {
            var value = Environment.GetEnvironmentVariable("EMAIL_ENABLE_SSL")
                        ?? throw new InvalidOperationException("EMAIL_ENABLE_SSL not found in environment variables.");
            if (!bool.TryParse(value, out bool enableSsl))
                throw new InvalidOperationException("EMAIL_ENABLE_SSL must be a valid boolean.");
            return enableSsl;
        }

        private static int GetSmtpPort()
        {
            var value = Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT")
                        ?? throw new InvalidOperationException("EMAIL_SMTP_PORT not found in environment variables.");
            if (!int.TryParse(value, out int port))
                throw new InvalidOperationException("EMAIL_SMTP_PORT must be a valid integer.");
            return port;
        }
        #endregion

        #region Methods
        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var message = new MailMessage();
            message.From = new MailAddress(fromEmail, displayName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = "Reset Your Password";
            message.IsBodyHtml = true;
            message.Body = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        background-color: #f4f4f7;
                        margin: 0; padding: 0;
                    }}
                    .container {{
                        width: 100%;
                        max-width: 600px;
                        margin: 40px auto;
                        background-color: #ffffff;
                        border-radius: 8px;
                        box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                        padding: 30px;
                    }}
                    h2 {{
                        color: #333333;
                    }}
                    p {{
                        color: #555555;
                        line-height: 1.6;
                    }}
                    .button {{
                        display: inline-block;
                        padding: 12px 24px;
                        background-color: #4a90e2;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 6px;
                        font-weight: bold;
                        margin-top: 20px;
                    }}
                    .footer {{
                        margin-top: 30px;
                        font-size: 12px;
                        color: #999999;
                        text-align: center;
                    }}
                    @media only screen and (max-width: 600px) {{
                        .container {{
                            padding: 20px;
                        }}
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>Password Reset Request</h2>
                    <p>Hello,</p>
                    <p>We received a request to reset your password for your account.</p>
                    <p>Click the button below to set a new password. This link will expire in 1 hour.</p>
                    <a href='{resetLink}' class='button'>Reset Password</a>
                    <p class='footer'>If you did not request this, you can safely ignore this email.</p>
                </div>
            </body>
            </html>";

            using var smtp = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl,
            };

            await smtp.SendMailAsync(message);
        }
        #endregion
    }
}
