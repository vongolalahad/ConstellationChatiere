using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chatiere
{
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("LINK START!! - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            //Initialisation des stateObject
            var OutdoorSensor = new StateObject.State(); OutdoorSensor.Etat = false;
            var IndoorSensor = new StateObject.State(); IndoorSensor.Etat = false;
            var Flap = new StateObject.State(); Flap.Etat = false;
            var AnimalPresence = new StateObject.AnimalPresence(); AnimalPresence.Etat = true;
            //Envoi des valeur sur constellation
            PackageHost.PushStateObject("CapteurInterieur",IndoorSensor,lifetime: 0);
            PackageHost.PushStateObject("CapteurExterieur", OutdoorSensor, lifetime: 0);
            PackageHost.PushStateObject("Chatiere", Flap, lifetime: 0);
            PackageHost.PushStateObject("PresenceAnimale", AnimalPresence, lifetime: 0);
        }

        /// <summary>
        /// Open the pet door.
        /// </summary>
        /// <returns></returns>
        [MessageCallback]
        public int OpenFlap()
        {
            return 0;
        }

        /// <summary>
        /// Close the pet door.
        /// </summary>
        /// <returns></returns>
        [MessageCallback]
        public int CloseFlap()
        {
            return 0;
        }


        /// <summary>
        /// Change the value of the stateObject of type StateObject.State
        /// </summary>
        /// <param name="so">The so.</param>
        /// <param name="change">if set to <c>true</c> [change].</param>
        [MessageCallback]
        public void StateObjectState_ChangeValue(string so, bool change)
        {
            var StateObject = new StateObject.State();
            StateObject.Etat = change;
            PackageHost.PushStateObject(so, StateObject, lifetime: 0);
        }

        /// <summary>
        /// Change the value of the stateObject of type StateObject.AnimalPresence
        /// </summary>
        /// <param name="so">The so.</param>
        /// <param name="change">if set to <c>true</c> [change].</param>
        [MessageCallback]
        public void StateObjectAnimalPresence_ChangeValue(string so, bool change)
        {
            var StateObject = new StateObject.AnimalPresence();
            StateObject.Etat = change;
            PackageHost.PushStateObject(so, StateObject, lifetime: 0);
        }
    }
}
