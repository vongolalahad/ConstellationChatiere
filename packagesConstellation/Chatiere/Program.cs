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

        public override void OnStart()
        {
            PackageHost.WriteInfo("LINK START!! - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            //Initialisation des stateObject
            var CapteurExterieur = new StateObject.State(); CapteurExterieur.Etat = false;
            var CapteurInterieur = new StateObject.State(); CapteurInterieur.Etat = false;
            var Chatiere = new StateObject.State(); Chatiere.Etat = false;
            var PresenceAnimal = new StateObject.PresenceAnimale(); PresenceAnimal.Etat = true;
            //Envoi des valeur sur constellation
            PackageHost.PushStateObject("CapteurInterieur",CapteurInterieur,lifetime: 0);
            PackageHost.PushStateObject("CapteurExterieur", CapteurExterieur, lifetime: 0);
            PackageHost.PushStateObject("Chatiere", Chatiere, lifetime: 0);
            PackageHost.PushStateObject("PresenceAnimale", PresenceAnimal, lifetime: 0);
        }

        /// <summary>
        /// Open the pet door.
        /// </summary>
        /// <returns></returns>
        [MessageCallback]
        public int OuvrirChatiere()
        {
            return 0;
        }

        /// <summary>
        /// Close the pet door.
        /// </summary>
        /// <returns></returns>
        [MessageCallback]
        public int FermerChatier()
        {
            return 0;
        }


        /// <summary>
        /// Change the value of the stateObject of type StateObject.State
        /// </summary>
        /// <param name="so">The so.</param>
        /// <param name="change">if set to <c>true</c> [change].</param>
        [MessageCallback]
        public void StateObjectState_ChangeValue(string so,bool change)
        {
            var StateObject = new StateObject.State();
            StateObject.Etat = change;
            PackageHost.PushStateObject(so, StateObject, lifetime: 0);
        }

        /// <summary>
        /// Change the value of the stateObject of type StateObject.PresenceAnimale
        /// </summary>
        /// <param name="so">The so.</param>
        /// <param name="change">if set to <c>true</c> [change].</param>
        [MessageCallback]
        public void StateObjectPresenceAnimale_ChangeValue(string so, bool change)
        {
            var StateObject = new StateObject.PresenceAnimale();
            StateObject.Etat = change;
            PackageHost.PushStateObject(so, StateObject, lifetime: 0);
        }
    }
}
