using Microsoft.AspNetCore.Identity;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public static class IdentityErrorTranslator
    {
        public static string ToVietnameseMessage(IdentityResult result)
        {
            var messages = result.Errors
                .Select(Translate)
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .Distinct();

            return string.Join(" ", messages);
        }

        private static string Translate(IdentityError error)
        {
            return error.Code switch
            {
                nameof(IdentityErrorDescriber.DuplicateEmail) => "Email đã tồn tại.",
                nameof(IdentityErrorDescriber.DuplicateUserName) => "Email đã tồn tại.",
                nameof(IdentityErrorDescriber.InvalidEmail) => "Email không đúng định dạng.",
                nameof(IdentityErrorDescriber.PasswordTooShort) => "Mật khẩu phải có ít nhất 6 ký tự.",
                nameof(IdentityErrorDescriber.PasswordRequiresDigit) => "Mật khẩu phải có ít nhất 1 chữ số.",
                nameof(IdentityErrorDescriber.PasswordRequiresLower) => "Mật khẩu phải có ít nhất 1 chữ thường.",
                nameof(IdentityErrorDescriber.PasswordRequiresUpper) => "Mật khẩu phải có ít nhất 1 chữ in hoa.",
                nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric) => "Mật khẩu phải có ít nhất 1 ký tự đặc biệt.",
                nameof(IdentityErrorDescriber.InvalidUserName) => "Tên đăng nhập không hợp lệ.",
                _ => error.Description
            };
        }
    }
}
