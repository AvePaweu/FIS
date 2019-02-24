var app = angular.module('app', ['ngRoute', 'angular-table']);

app.config(['$routeProvider', function($routeProvider) {
	$routeProvider.when('/start', { templateUrl: 'partials/start.html', controller: 'appCTRL'})
				  .when('/jumper/:id', { templateUrl: 'partials/jumper.html', controller: 'jumperCTRL'})
				  .when('/competition/:id', { templateUrl: 'partials/competition.html', controller: 'competitionCTRL'})
				  .when('/jumpers', { templateUrl: 'partials/jumpers.html', controller: 'jumpersCTRL'})
				  .when('/competitions', { templateUrl: 'partials/competitions.html', controller: 'competitionsCTRL'})
				  .otherwise({ redirectTo: '/start' });
}]);

app.factory('w', function() {
	this.date = new Date();
	return this.date;
});

function appCTRL($scope, $http) {
	if ($scope.date === undefined) {
		$scope.date = moment().format('YYYY-MM-DD');
	}
	
	$scope.config = { itemsPerPage: 100, maxPages: 5, fillLastPage: false };
	
	$scope.CalendarClick = function() {
		$("#dp1").fdatepicker('show');
	};
	
	$scope.GetRanking = function(date) {
		console.log(date);
		$http.get('./api.php/ranking/' + date).success(function (d) {
			$scope.ranking = d;
			$http.get('./api.php/quotas').success(function(f) {
			$scope.quotas = f;
			});
		});		
	};	
	
	$scope.GetRanking($scope.date);
}

function jumperCTRL ($scope, $http, $routeParams) {
	$scope.id = $routeParams.id;
	$http.get('./api.php/jumper/' + $scope.id)
		.success(function(d) {
			$scope.jumper = d;
		});
		
	$scope.config = { itemsPerPage: 50, maxPages: 5, fillLastPage: false };
}
		
function competitionCTRL ($scope, $http, $routeParams) {
		$scope.id = $routeParams.id;
		$http.get('./api.php/competition/' + $scope.id)
			.success(function(d) {
				$scope.competition = d;
			});
}

function jumpersCTRL ($scope, $http, $routeParams) {
	$scope.id = $routeParams.id;
	$http.get('./api.php/jumpers')
		.success(function(d) {
			$scope.jumpers = d;
		});
		
	$scope.config = { itemsPerPage: 50, maxPages: 10, fillLastPage: false };
}
		
function competitionsCTRL ($scope, $http, $routeParams) {
		$scope.id = $routeParams.id;
		$http.get('./api.php/competitions')
			.success(function(d) {
				$scope.competitions = d;
			});
		$scope.config = { itemsPerPage: 50, maxPages: 10, fillLastPage: false };
}