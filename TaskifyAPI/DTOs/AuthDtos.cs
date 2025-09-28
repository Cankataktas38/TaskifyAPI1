namespace TaskifyAPI.DTOs
{
    
    
        public class RegisterDto
        {
            public string UserName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
        public class LoginDto
        {
            public string UserName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
        public class AuthResponseDto
        {
            public string? Token { get; set; } = string.Empty;
        }
    }

