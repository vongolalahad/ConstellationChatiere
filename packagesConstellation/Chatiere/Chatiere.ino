#include <BaseDefinitions.h>
#include <BufferedPrint.h>
#include <Constellation.h>
#include <LinkedList.h>
#include <PackageDescriptor.h>

#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>


const char* ssid = "SSID"; // replace by WIFI's SSID
const char* password = "PASSWORD"; // replace with WIFI's password
ESP8266WebServer server(80); // instanciate a server on port 80
WiFiClient client;

/// <summary>
/// connect to constellation
/// </summary>
/// <param name = "IP_or_DNS_CONSTELLATION_SERVER">IP adress or DNS server of constellation</param>
/// <param name = "PORT">port which one the server listening</param>
/// <param name = "YOUR_SENTINEL_NAME">Sentinel where the package will be deploy</param>
/// <param name = "YOUR_PACKAGE_NAME">The name of the package</param>
/// <param name = "YOUR_ACCESS_KEY">access key of the credential create on constellation</param>
Constellation<WiFiClient> constellation("192.168.43.202", 8088, "MyvirtualSentinel", "Chatiere", "6bfda29f2fcfae3e4f13fc1867f4e0a8e7225974");

int led = 16;                           // arduino's pin D0
int magneto = 5;                        // arduino's pin D1

void setup(void) {
	Serial.begin(115200);  delay(10);
	
	WiFi.begin(ssid, password);
  
	Serial.println("");
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");
	}
	Serial.println("");
	Serial.print("IP address: ");
	Serial.println(WiFi.localIP());
	constellation.writeInfo("Connected!!");

	pinMode(led,OUTPUT);
	pinMode(magneto,OUTPUT);
  
	// Getting setting value of your virtual package
	JsonObject& settings = constellation.getSettings();

	// Initialisation of state objects
  
	constellation.pushStateObject("CapteurInterieur", stringFormat("{ \"Etat\":%s }", "false"), "StateObject.State");

	constellation.pushStateObject("CapteurExterieur", stringFormat("{ \"Etat\":%s }", "false"), "StateObject.State");

	constellation.pushStateObject("Lumiere", stringFormat("{ \"Etat\":%s }", "false"), "StateObject.State");

	constellation.pushStateObject("PresenceAnimale", stringFormat("{ \"Etat\":%s }", "true"), "StateObject.AnimalPresence");

	constellation.pushStateObject("VerouillerChatiere", stringFormat("{ \"Etat\":%s }", "false"), "StateObject.State");

	constellation.pushStateObject("Chatiere", stringFormat("{ \"Etat\":%s }", "false"), "StateObject.State");


	/// <summary>
	/// Callback message for opening the pet door
	/// </summary>
	constellation.registerMessageCallback("OpenFlap",
	MessageCallbackDescriptor().setDescription("Open the flap"),
	[](JsonObject & json) {
		constellation.pushStateObject("Chatiere", stringFormat("{'Etat':%s}", "true"), "StateObject.State");
		constellation.writeInfo("Ouverture de la chatière /!\\");
		digitalWrite(magneto, HIGH); //Fait passer du courant dans l'électro-aimant
	});
	
	/// <summary>
	/// Callback message for closing the pet door
	/// </summary>
	constellation.registerMessageCallback("CloseFlap",
	MessageCallbackDescriptor().setDescription("Close the flap"),
	[](JsonObject & json) {
		constellation.pushStateObject("Chatiere", stringFormat("{'Etat':%s}", "false"), "StateObject.State");
		constellation.writeInfo("Fermeture de la chatière /!\\");
		digitalWrite(magneto, LOW); //Coupe le courant circulant dans l'électro-aimant
	});
	
	/// <summary>
	/// change the value state object with type AnimalPresence
	/// </summary>
	/// <param name = "so type = string> The name of the state object </param>
	/// <param name = "change" type =bool> the new value of the state object </param>
	constellation.registerMessageCallback("StateObjectAnimalPresence_ChangeValue",
	MessageCallbackDescriptor().setDescription("Change the state of the state object with type StateObject.AnimalPresence").addParameter<const char*>("so").addParameter<bool>("change"),
	[](JsonObject & json) {
		constellation.pushStateObject(json["Data"][0].asString(), stringFormat("{'Etat':%s}", json["Data"][1].asString()), "StateObject.AnimalPresence");
	});
	
	/// <summary>
	/// change the value state object with type StateObject.State
	/// </summary>
	/// <param name = "so type = string> The name of the state object </param>
	/// <param name = "change" type =bool> the new value of the state object </param>
	constellation.registerMessageCallback("StateObjectState_ChangeValue",
	MessageCallbackDescriptor().setDescription("Change the state of state objects with type StateObject.State").addParameter<const char*>("so").addParameter<bool>("change"),
	[](JsonObject & json) {
		constellation.pushStateObject(json["Data"][0].asString(), stringFormat("{'Etat':%s}", json["Data"][1].asString()), "StateObject.State");
	});
	
	/// <summary>
	/// Callback message for switching on the light
	/// </summary>
	constellation.registerMessageCallback("SwitchOnLed",
	MessageCallbackDescriptor().setDescription("Switch on the led"),
	[](JsonObject & json) {
		constellation.pushStateObject("Lumiere", stringFormat("{'Etat':%s}", "true"), "StateObject.State");
		constellation.writeInfo("Allumage des lumières /!\\");
		digitalWrite(led, HIGH); // On fait passer le courant dans la led
	});
	
	/// <summary>
	/// Callback message for switching off the light
	/// </summary>
	constellation.registerMessageCallback("SwitchOffLed",
	MessageCallbackDescriptor().setDescription("Switch off the led"),
	[](JsonObject & json) {
		constellation.pushStateObject("Lumiere", stringFormat("{'Etat':%s}", "false"), "StateObject.State");
		constellation.writeInfo("Extinction des lumières /!\\");
		digitalWrite(led, LOW); // On fait passer le courant dans la led
	});
	
	/// <summary>
	/// Callback message for locking the pet door
	/// </summary>
	constellation.registerMessageCallback("LockFlap",
	MessageCallbackDescriptor().setDescription("Lock the flap"),
	[](JsonObject & json) {
		constellation.pushStateObject("VerouillerChatiere", stringFormat("{'Etat':%s}", "true"), "StateObject.State");
	});
	
	/// <summary>
	/// Callback message for unlocking the pet door
	/// </summary>
	constellation.registerMessageCallback("UnlockFlap",
	MessageCallbackDescriptor().setDescription("Unlock the flap"),
	[](JsonObject & json) {
		constellation.pushStateObject("VerouillerChatiere", stringFormat("{'Etat':%s}", "false"), "StateObject.State");
	});

	// Declare the package descriptor
	constellation.declarePackageDescriptor();
	
	// WriteLog info
	constellation.writeInfo("Virtual Package on '%s' is started !", constellation.getSentinelName());
}

void loop() {
	static int ts = millis();
	if(millis() - ts > 10000){
		ts = millis();
	}
	constellation.loop();

}
