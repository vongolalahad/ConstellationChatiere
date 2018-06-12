using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConstellationMyBrain
{
    public class Program : PackageBase
    {
        //Capteur exterieur
        [StateObjectLink("*","Chatiere","CapteurExterieur")]
        public StateObjectNotifier CapteurExterieur { get; set; }

        //Capteur interieur
        [StateObjectLink("*", "Chatiere", "CapteurInterieur")]
        public StateObjectNotifier CapteurInterieur { get; set; }

        //Etat de la chatiere
        [StateObjectLink("*", "Chatiere", "Chatiere")]
        public StateObjectNotifier Chatiere { get; set; }

        //Presence de l'animale
        [StateObjectLink("*", "Chatiere", "PresenceAnimale")]
        public StateObjectNotifier PresenceAnimale { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            //events:
            this.CapteurInterieur.ValueChanged += CapteurInterieur_ValueChanged;
            this.CapteurExterieur.ValueChanged += CapteurExterieur_ValueChanged;

            PackageHost.WriteInfo("Valeur du stateObject CapteurInterieur : {0}", this.CapteurInterieur.DynamicValue.Etat);
            PackageHost.WriteInfo("Valeur du stateObject CapteurExterieur : {0}", this.CapteurExterieur.DynamicValue.Etat);
            PackageHost.WriteInfo("Valeur du stateObject PresenceAnimale : {0}", this.PresenceAnimale.DynamicValue.Etat);
            PackageHost.WriteInfo("Valeur du stateObject Chatier : {0}", this.Chatiere.DynamicValue.Etat);
            
            PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.CapteurInterieur.Value.Name,true);
            PackageHost.WriteInfo("Valeur du stateObject Chatier : {0}", this.Chatiere.DynamicValue.Etat);
        }

        private void CapteurExterieur_ValueChanged(object sender, StateObjectChangedEventArgs e)
        {

        }

        /// <summary>
        /// Handles the ValueChanged event of the CapteurInterieur control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StateObjectChangedEventArgs"/> instance containing the event data.</param>
        private void CapteurInterieur_ValueChanged(object sender, StateObjectChangedEventArgs e)
        {
            if (e.NewState.DynamicValue.Etat == "True" && this.Chatiere.DynamicValue.Etat == "False")
            {
                PackageHost.WriteInfo("Ouverture de la chatiere");
                PackageHost.CreateMessageProxy("Chatiere").OuvrirChatiere();
                PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.Chatiere.Value.Name,true);


                /*int ouverture = PackageHost.CreateMessageProxy("Chatiere").OuvrirChatiere();
                if (ouverture == 0)
                {
                    PackageHost.CreateMessageProxy("Chatiere").Chatiere_ValueChange(true);
                }
                else
                {
                    PackageHost.WriteWarn("Attention la chatiere n'a pas pu s'ouvrir");
                    //Envoyer aussi par pushbullet un message au telephone de l'utilisateur
                }*/

            }
            else if(e.NewState.DynamicValue.Etat == "True" && this.Chatiere.DynamicValue.Etat == "True")
            {
                /*PackageHost.WriteInfo("Fermeture de la chatiere");
                int ouverture = PackageHost.CreateMessageProxy("Chatiere").FermerChatiere();
                if (ouverture == 0) { }
                else
                {
                    PackageHost.WriteWarn("Attention la chatiere n'a pas pu se fermer");
                    //Envoyer aussi par pushbullet un message au telephone de l'utilisateur
                }*/
            }
        }
    }
}
