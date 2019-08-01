(function (app) {
    'use strict';

    app.factory('membershipService', membershipService);

    membershipService.$inject = ['apiService', 'notificationService', '$http', '$base64', '$cookieStore', '$rootScope'];

    function membershipService(apiService, notificationService, $http, $base64, $cookieStore, $rootScope) {

        var service = {
            login: login,
            register: register,
            saveJWTAuthToken: saveJWTAuthToken,
            removeJWTToken: removeJWTToken,           
            isUserLoggedIn: isUserLoggedIn
        }

        function login(user, completed) {
            apiService.post('/api/account/authenticate', user,
                completed,
                loginFailed);
        }

        function register(user, completed) {
            apiService.post('/api/account/register', user,
                completed,
                registrationFailed);
        }

        function saveJWTAuthToken(result) {
           
            $rootScope.repository = {
                loggedUser: {
                    username: result.userName,
                    authdata: result.token
                }
            };
          
            $http.defaults.headers.common['Authorization'] = 'Bearer ' + result.token;
            $cookieStore.put('repository', $rootScope.repository);
        }

        function removeJWTToken() {
            $rootScope.repository = {};
            $cookieStore.remove('repository')
            $http.defaults.headers.common.Authorization = '';           
        }
    

        function loginFailed(response) {
            notificationService.displayError(response.data);
        }

        function registrationFailed(response) {

            notificationService.displayError('Registration failed. Try again.');
        }

        function isUserLoggedIn() {      
           
            return $rootScope.repository.loggedUser != null;
        }

        return service;
    }



})(angular.module('common.core'));