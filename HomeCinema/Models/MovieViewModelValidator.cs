﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Models
{
    public class MovieViewModelValidator :AbstractValidator<MovieViewModel>
    {
        public MovieViewModelValidator()
        {
            RuleFor(movie => movie.GenreId).GreaterThan(0).WithMessage("Select a Genre");
            RuleFor(movie => movie.Director).NotEmpty().Length(1, 100).WithMessage("Select a Director");
            RuleFor(movie => movie.Writer).NotEmpty().Length(1, 100).WithMessage("Select a Writer");
            RuleFor(movie => movie.Producer).NotEmpty().Length(1, 100).WithMessage("Select a Producer");
            RuleFor(movie => movie.Description).NotEmpty().WithMessage("Select a Description");
            RuleFor(movie => movie.Rating).InclusiveBetween((byte)0, (byte)5).WithMessage("Rating must be equal or less than 5");
            RuleFor(movie => movie.TrailerURI).NotEmpty().Must(ValidTrailerURI).WithMessage("Only Youtube Trailers are supported");
           
        }

        private bool ValidTrailerURI(string trailerURI)
        {
            return (!string.IsNullOrEmpty(trailerURI) && trailerURI.ToLower().StartsWith("https://www.youtube.com/watch?"));
        }
    }
}