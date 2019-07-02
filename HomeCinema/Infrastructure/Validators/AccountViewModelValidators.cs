using FluentValidation;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Infrastructure.Validators
{
    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {

        public RegistrationViewModelValidator()
        {
            RuleFor(r => r.Email).NotEmpty().EmailAddress().WithMessage("Invalid Email Address");
            RuleFor(r => r.Username).NotEmpty().WithMessage("Invalid Username");
            RuleFor(r => r.Password).NotEmpty().WithMessage("Invalid Password");
        }

    }

    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(r => r.Username).NotEmpty().WithMessage("Invalid Username");
            RuleFor(r => r.Password).NotEmpty().WithMessage("Invalid Password");
        }
    }

}