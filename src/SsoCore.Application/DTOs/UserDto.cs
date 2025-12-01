using OnnexNotification;
using SsoCore.Application.Constants;
using SsoCore.Application.Helpers;

namespace SsoCore.Application.DTOs
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public bool IsDisabled { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string? Email { get; set; }

        public List<UserRoleDto>? UserRoles { get; set; } = [];
        
        public async Task<EmailMessage> RegistrationOtpEmailModel(string otpCode)
        {
            var template = await EmailTemplateHelper.GetEmailTemplate(EmailTemplates.LoginOtp);
            return new EmailMessage
            {
                Email = Email!,
                Subject = "OTP Verification",
                FullName = $"{FirstName} {LastName}",
                Title = "OTP Verification",
                Body = (template?.Data ?? string.Empty)
                    .Replace("[User]", $"{FirstName} {LastName}")
                    .Replace("[OTP Code]", otpCode),
                BannerMessage = "OTP Verification"
            };
        }

        public async Task<EmailMessage> ConfirmAccountEmailModel(string url)
        {
            var template = await EmailTemplateHelper.GetEmailTemplate(EmailTemplates.ConfirmAccount);
            return new EmailMessage
            {
                Email = Email!,
                Subject = "Confirm Account",
                FullName = $"{FirstName} {LastName}",
                Title = "Confirm Account",
                Body = (template?.Data ?? string.Empty)
                            .Replace("[User]", $"{FirstName} {LastName}")
                            .Replace("[Confirmation Link]", url),
                 BannerMessage = "Confirm Account"
            };
        }

        public async Task<EmailMessage> ConfirmAccountSuccessfulEmailModel(string loginUrl)
        {
            var template = await EmailTemplateHelper.GetEmailTemplate(EmailTemplates.ConfirmAccountSuccessful);
            return new EmailMessage
            {
                Email = Email!,
                Subject = "Account Confirmation Successful",
                FullName = $"{FirstName} {LastName}",
                Title = "Account Confirmation Successful",
                Body = (template?.Data ?? string.Empty)
                            .Replace("[User]", $"{FirstName} {LastName}")
                            .Replace("[Login Link]", loginUrl),
                BannerMessage = "Account Confirmation Successful"
            };
        }

        public async Task<EmailMessage> LoginOtpEmailModel(string otpCode)
        {
            var template = await EmailTemplateHelper.GetEmailTemplate(EmailTemplates.LoginOtp);
            return new EmailMessage
            {
                Email = Email!,
                Subject = "OTP Verification",
                FullName = $"{FirstName} {LastName}",
                Title = "OTP Verification",
                Body = (template?.Data ?? string.Empty)
                            .Replace("[User]", $"{FirstName} {LastName}")
                            .Replace("[OTP Code]", otpCode),
                BannerMessage = "OTP Verification"
            };
        }

        public async Task<EmailMessage> ResetPasswordEmailModel(string resetOTP)
        {
            var template = await EmailTemplateHelper.GetEmailTemplate(EmailTemplates.ResetPassword);
            return new EmailMessage
            {
                Email = Email!,
                Subject = "Password Reset Request",
                FullName = $"{FirstName} {LastName}",
                Title = "Reset Password",
                Body = (template?.Data ?? string.Empty)
                            .Replace("[User]", $"{FirstName} {LastName}")
                            .Replace("[OTP Code]", resetOTP),
                BannerMessage = "Reset Password"
            };
        }

        public async Task<EmailMessage> ResetPasswordSuccessfulEmailModel(string loginUrl)
        {
            var template = await EmailTemplateHelper.GetEmailTemplate(EmailTemplates.ResetPasswordSuccessful);
            return new EmailMessage
            {
                Email = Email!,
                Subject = "Password Reset Successful",
                FullName = $"{FirstName} {LastName}",
                Title = "Password Reset Successful",
                Body = (template?.Data ?? string.Empty)
                            .Replace("[User]", $"{FirstName} {LastName}")
                            .Replace("[Login Link]", loginUrl),
                BannerMessage = "Password Reset Successful"
            };
        }
    }
}
