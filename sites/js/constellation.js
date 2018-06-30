

var myApp = angular.module('MyApp',['ngConstellation'])
myApp.controller('MyController', ['$scope', 'constellationConsumer',function($scope, constellation){

    $scope.IsConnected = false
    $scope.Chatiere = {}
	
	//// <summary>
	//// connect to constellation
	//// </summary>
	//// <param name = "IP_or_DNS_CONSTELLATION_SERVER">IP adress or DNS server of constellation</param>
	//// <param name = "PORT">port which one the server listening</param>
	//// <param name = "YOUR_ACCESS_KEY">access key of the credential create on constellation</param>
	//// <param name = "APPLICATION_NAME">Name of the site</param>
    constellation.initializeClient("http://localhost:8088", "1234567890", "Ma Page Web")//IP adress of the server constellation
    
    //creating a subscription
    constellation.onConnectionStateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.connected) {
            console.log("Je suis connecté !")
            //Recover all state objects of the package Chatiere
            constellation.registerStateObjectLink("*","Chatiere","*","*",function (so) {
                $scope.Chatiere[so.Name] = so
                $scope.$apply()
            })
        }
        $scope.IsConnected = (change.newState === $.signalR.connectionState.connected)
        $scope.$apply()
    })
	
	//// <summary>
	//// toggle the class checked on span tag with class switch_span and containing an input tag checked
	//// and call the function setStateObject
	//// </summary>
	//// <param name = "so">The state object corresponding to the input tag</param>
    $scope.toggleClassChecked = function(so){
        $("p.state_object_name:contains('"+so.Name+"')")[0].parentNode.parentNode.classList.toggle("checked")
        $scope.setStateObject(!($("p.state_object_name:contains('"+so.Name+"')")[0].nextElementSibling.children[0].children[0].children[0].checked),so)
    }
	
	//// <summary>
	//// Call the callback message StateObjectState_ChangeValue or StateObjectAnimalPresence_ChangeValue in package Chatiere
	//// for changing the value the state object
	//// </summary>
	//// <param name = "state" type = bool> The new value of the state object </param>
	//// <param name = "so"> The state object to modify </param>
    $scope.setStateObject = function(state, so){
        if(so.Type == "StateObject.State"){
            constellation.sendMessage({ Scope: 'Package', Args: ['Chatiere'] }, 'StateObjectState_ChangeValue', so.Name, state)
        }
        else if(so.Type == "StateObject.AnimalPresence"){
            constellation.sendMessage({ Scope: 'Package', Args: ['Chatiere'] }, 'StateObjectAnimalPresence_ChangeValue', so.Name, state)
        }
    }

    constellation.connect()// Start the connection
}])