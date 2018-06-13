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
        /// <summary>
        /// Gets or sets the outdoor sensor's state object.
        /// </summary>
        /// <value>
        /// The outdoor sensor.
        /// </value>
        [StateObjectLink("*","Chatiere","CapteurExterieur")]
        public StateObjectNotifier OutdoorSensor { get; set; }

        /// <summary>
        /// Gets or sets the indoor sensor's state object.
        /// </summary>
        /// <value>
        /// The indoor sensor.
        /// </value>
        [StateObjectLink("*", "Chatiere", "CapteurInterieur")]
        public StateObjectNotifier IndoorSensor { get; set; }

        /// <summary>
        /// Gets or sets the flap's state object.
        /// </summary>
        /// <value>
        /// The flap.
        /// </value>
        [StateObjectLink("*", "Chatiere", "Chatiere")]
        public StateObjectNotifier Flap { get; set; }

        //Presence de l'animale
        /// <summary>
        /// Gets or sets the animal presence's state object.
        /// </summary>
        /// <value>
        /// The animal presence.
        /// </value>
        [StateObjectLink("*", "Chatiere", "PresenceAnimale")]
        public StateObjectNotifier AnimalPresence { get; set; }

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
            this.IndoorSensor.ValueChanged += IndoorSensor_ValueChanged;
            this.OutdoorSensor.ValueChanged += OutdoorSensor_ValueChanged;

            //Debug : value of each state object
            PackageHost.WriteInfo("Valeur du stateObject CapteurInterieur : {0}", this.IndoorSensor.DynamicValue.Etat);
            PackageHost.WriteInfo("Valeur du stateObject CapteurExterieur : {0}", this.OutdoorSensor.DynamicValue.Etat);
            PackageHost.WriteInfo("Valeur du stateObject PresenceAnimale : {0}", this.AnimalPresence.DynamicValue.Etat);
            PackageHost.WriteInfo("Valeur du stateObject Chatier : {0}", this.Flap.DynamicValue.Etat);
            
            //Change the value of the state object CapteurInterieur to activate the event
            PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.IndoorSensor.Value.Name,true);
            PackageHost.WriteInfo("Valeur du stateObject Chatier : {0}", this.Flap.DynamicValue.Etat);
        }

        /// <summary>
        /// Handles the ValueChanged event of the IndoorSensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StateObjectChangedEventArgs"/> instance containing the event data.</param>
        private void IndoorSensor_ValueChanged(object sender, StateObjectChangedEventArgs e)
        {
            //If the indoor sensor detect the necklace and the pet was inside
            if (e.NewState.DynamicValue.Etat == "True" && this.AnimalPresence.DynamicValue.Etat == "True")
            {
                if (this.Flap.DynamicValue.Etat == "False")
                {
                    PackageHost.WriteInfo("Ouverture de la chatiere");
                    PackageHost.CreateMessageProxy("Chatiere").OpenFlap();
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.Flap.Value.Name, true);

                    /*int open = PackageHost.CreateMessageProxy("Chatiere").OpenFlap();
                    if (open == 0)
                    {
                        PackageHost.CreateMessageProxy("Chatiere").Chatiere_ValueChange(true);
                    }
                    else
                    {
                        PackageHost.WriteWarn("Attention la chatiere n'a pas pu s'ouvrir");
                        //send a message pushbullet to the user
                    }*/
                }
                else
                {
                    //Close the flap
                    PackageHost.WriteInfo("Fermeture de la chatiere");
                    PackageHost.CreateMessageProxy("Chatiere").CloseFlap();
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.Flap.Value.Name, false);
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectAnimalPresence_ChangeValue(this.AnimalPresence.Value.Name, true);
                }

            }
            //if the indoor sensor detect the necklace and the pet was outside
            else if (e.NewState.DynamicValue.Etat == "True" && this.AnimalPresence.DynamicValue.Etat == "False")
            {
                //If the flap is closed
                if(this.Flap.DynamicValue.Etat == "False")
                {
                    //Nothing to do
                }
                else
                {
                    PackageHost.WriteInfo("Fermeture de la chatiere");
                    PackageHost.CreateMessageProxy("Chatiere").CloseFlap();
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.Flap.Value.Name, false);
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectAnimalPresence_ChangeValue(this.AnimalPresence.Value.Name, true);
                }
            }
            else
            {
                //Nothing to do
            }
        }


        /// <summary>
        /// Handles the ValueChanged event of the OutdoorSensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StateObjectChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void OutdoorSensor_ValueChanged(object sender, StateObjectChangedEventArgs e)
        {
            
        }
    }
}
