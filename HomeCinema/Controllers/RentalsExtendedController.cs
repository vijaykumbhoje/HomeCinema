﻿using AutoMapper;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HomeCinema.Controllers
{
    public class RentalsExtendedController : ApiControllerBaseExtended
    {
        public RentalsExtendedController(IDataRepositoryFactory dataRepositoryFactory, IUnitOfWork _unitOfWork)
            :base (dataRepositoryFactory, _unitOfWork)
        { }

        private List<RentalHistoryViewModel> GetMovieRentalHistory(int movieId)
        {
            List<RentalHistoryViewModel> _rentalHistory = new List<RentalHistoryViewModel>();
            List<Rental> rentals = new List<Rental>();

            var movie = _moviesRepository.GetSingle(movieId);

            foreach (var stock in movie.Stocks)
            {
                rentals.AddRange(stock.Rentals);
            }

            foreach (var rental in rentals)
            {
                var customer = _customersRepository.GetSingle(rental.CustomerId);

                RentalHistoryViewModel _historyItem = new RentalHistoryViewModel()
                {
                    Id = rental.Id,
                    StockId = rental.StockId,
                    RentalDate = rental.RentalDate,
                    ReturnDate = rental.ReturnDate,
                    Status = rental.Status,
                    Customer = customer.FirstName + ' ' + customer.LastName
                };
                _rentalHistory.Add(_historyItem);
            }
            _rentalHistory.Sort((r1, r2) => r2.RentalDate.CompareTo(r1.RentalDate));
            return _rentalHistory;
        }

        private List<RentalHistoryPerDate> GetRentalHistoryPerDates(int movieId)
        {
            List<RentalHistoryPerDate> listHistory = new List<RentalHistoryPerDate>();
            List<RentalHistoryViewModel> _rentalHistory = GetMovieRentalHistory(movieId);
            if (_rentalHistory.Count > 0)
            {
                List<DateTime> _distinctDates = new List<DateTime>();
                _distinctDates = _rentalHistory.Select(h => h.RentalDate).Distinct().ToList();
                foreach (var distinctDate in _distinctDates)
                {
                    var totalDateRentals = _rentalHistory.Count(r => r.RentalDate.Date == distinctDate);
                    RentalHistoryPerDate movieRentalHistoryPerDate = new RentalHistoryPerDate()
                    {
                        Date = distinctDate,
                        TotalRentals = totalDateRentals
                    };
                    listHistory.Add(movieRentalHistoryPerDate);
                }
                listHistory.Sort((r1, r2) => r1.Date.CompareTo(r2.Date));
            }
            return listHistory;
        }

        [HttpPost]
        [Route("{id:int}/rentalhistory")]
        public HttpResponseMessage RentalHistory(HttpRequestMessage request, int Id)
        {
            _requiredRepositories = new List<Type> { typeof(Customer), typeof(Stock), typeof(Rental) };
       
            return CreateHttpResponse(request,_requiredRepositories , () => {
                HttpResponseMessage response = null;
                List<RentalHistoryViewModel> _rentalHistory = GetMovieRentalHistory(Id);
                response = request.CreateResponse(HttpStatusCode.OK, _rentalHistory);
                return response;
            });
        }

        [HttpGet]
        [Route("rentalhistory")]
        public HttpResponseMessage TotalRentalHistory(HttpRequestMessage request)
        {
            _requiredRepositories = new List<Type> { typeof(Customer), typeof(Stock), typeof(Rental) };

            return CreateHttpResponse(request, _requiredRepositories, () => {
                HttpResponseMessage response = null;
                List<TotalRentalHistoryViewModel> _totalMoviesRentalHistory = new List<TotalRentalHistoryViewModel>();
                var movies = _moviesRepository.GetAll();
                foreach (var movie in movies)
                {
                    TotalRentalHistoryViewModel _totalRentalHistory = new TotalRentalHistoryViewModel()
                    {
                        Id = movie.Id,
                        Title = movie.Title,
                        Image = movie.Image,
                        Rentals = GetRentalHistoryPerDates(movie.Id)
                    };
                    if (_totalRentalHistory.TotalRentals > 0)
                        _totalMoviesRentalHistory.Add(_totalRentalHistory);
                }
                response = request.CreateResponse(HttpStatusCode.OK, _totalMoviesRentalHistory);
                return response;
            });
        }

        [HttpPost]
        [Route("return/{rentalId:int}")]
        public HttpResponseMessage Return(HttpRequestMessage request, int Id)
        {
            _requiredRepositories = new List<Type> { typeof(Rental) };

            return CreateHttpResponse(request, _requiredRepositories,  () => {
                HttpResponseMessage response = null;
                var rental = _rentalsRepository.GetSingle(Id);
                if (rental != null)
                {
                    rental.Status = "Returned";
                    rental.Stock.isAvailble = true;
                    rental.ReturnDate = DateTime.Now;
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "No Rental Found");
                }
                return response;
            });
        }

        [HttpPost]
        [Route("rent/{customerId:int}/{stockId:int}")]
        public HttpResponseMessage Rent(HttpRequestMessage request, int customerId, int stockId)
        {
            _requiredRepositories = new List<Type> { typeof(Customer), typeof(Stock), typeof(Rental) };

            return CreateHttpResponse(request, _requiredRepositories , () => {

                HttpResponseMessage response = null;

                var customer = _customersRepository.GetSingle(customerId);
                var stock = _stocksRepository.GetSingle(stockId);
                if (customer == null || stock == null)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.NoContent, "Invalid Customer or Stock");
                }
                else
                {
                    if (stock.isAvailble)
                    {
                        Rental _rental = new Rental()
                        {
                            CustomerId = customerId,
                            StockId = stockId,
                            RentalDate = DateTime.Now,
                            Status = "Borrowed"
                        };
                        _rentalsRepository.Add(_rental);
                        stock.isAvailble = false;
                        _unitOfWork.Commit();
                        RentalViewModel rentalvm = Mapper.Map<Rental, RentalViewModel>(_rental);
                        response = request.CreateResponse(HttpStatusCode.OK, rentalvm);

                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Selected Stock not available anymore");
                    }
                }
                return response;
            });
        }
    }
}