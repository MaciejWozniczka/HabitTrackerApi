﻿namespace HabitTrackerApi.Users;

public class TokenOption
{
    [Required]
    public string SecretKey { get; set; }
    [Required]
    public string Issuer { get; set; }
    [Required]
    public string Audience { get; set; }
    [Required]
    public int ExpiryMinutes { get; set; }
}