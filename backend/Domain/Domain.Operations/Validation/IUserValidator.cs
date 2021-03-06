﻿using WhiteRaven.Domain.Models.Authentication;

namespace WhiteRaven.Domain.Operations.Validation
{
    public interface IUserValidator : IValidator<User>
    {
        void ValidateEmailAddress(string email);
        void ValidatePassword(string password);
        void ValidateSearchTerm(string term);
    }
}