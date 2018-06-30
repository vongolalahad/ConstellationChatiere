using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        [StateObjectLink("*", "Chatiere", "CapteurExterieur")]
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

        /// <summary>
        /// Gets or sets the animal presence's state object.
        /// </summary>
        /// <value>
        /// The animal presence.
        /// </value>
        [StateObjectLink("*", "Chatiere", "PresenceAnimale")]
        public StateObjectNotifier AnimalPresence { get; set; }

        /// <summary>
        /// Gets or sets the light's state object.
        /// </summary>
        /// <value>
        /// The light.
        /// </value>
        [StateObjectLink("*", "Chatiere", "Lumiere")]
        public StateObjectNotifier Light { get; set; }

        /// <summary>
        /// Gets or sets the state object LockFlap.
        /// </summary>
        /// <value>
        /// The lock flap.
        /// </value>
        [StateObjectLink("*", "Chatiere", "VerouillerChatiere")]
        public StateObjectNotifier LockFlap { get; set; }

        [StateObjectLink("*", "DayInfo", "SunInfo")]
        public StateObjectNotifier SunInfo { get; set; }

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
            this.Flap.ValueChanged += Flap_ValueChanged;

            //Push state object with an interval (default value : 1 seconde)
            Task.Run(async () =>
            {
                while (PackageHost.IsRunning)
                {
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.IndoorSensor.Value.Name, this.IndoorSensor.DynamicValue.Etat);
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.OutdoorSensor.Value.Name, this.OutdoorSensor.DynamicValue.Etat);
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.Flap.Value.Name, this.Flap.DynamicValue.Etat);
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.Light.Value.Name, this.Light.DynamicValue.Etat);
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.LockFlap.Value.Name, this.LockFlap.DynamicValue.Etat);
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectAnimalPresence_ChangeValue(this.AnimalPresence.Value.Name, this.AnimalPresence.DynamicValue.Etat);
                    await Task.Delay(PackageHost.GetSettingValue<int>("interval"));
                }
            });

            Task.Run(async () =>
            {
                while (PackageHost.IsRunning)
                {
                    while (true)
                    {
                        if (DateTime.Now.Hour >= PackageHost.GetSettingValue<int>("LockIntervalHigh"))
                        {
                            //change the state object to on
                            PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.LockFlap.Value.Name, true);
                            break;
                        }
                        await Task.Delay(15000);
                    }
                    while (DateTime.Now.Hour < PackageHost.GetSettingValue<int>("LockIntervalLow")
                        || DateTime.Now.Hour >= PackageHost.GetSettingValue<int>("LockIntervalHigh"))
                    {
                        await Task.Delay(15000);
                    }
                    //change the state object to off
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectState_ChangeValue(this.LockFlap.Value.Name, false);
                }
            });
        }

        /// <summary>
        /// Handles the ValueChanged event of the Flap control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StateObjectChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private async void Flap_ValueChanged(object sender, StateObjectChangedEventArgs e)
        {
            //If the flap is opened
            if ((bool)e.NewState.DynamicValue.Etat)
            {
                await Task.Delay(10000);
                if ((bool)this.Flap.DynamicValue.Etat)
                {
                    PackageHost.CreateMessageProxy("Chatiere").CloseFlap();
                }
            }
            //Else, nothing to do
        }

        /// <summary>
        /// Handles the ValueChanged event of the IndoorSensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StateObjectChangedEventArgs"/> instance containing the event data.</param>
        private void IndoorSensor_ValueChanged(object sender, StateObjectChangedEventArgs e)
        {
            //If the indoor sensor detect the collar and the pet was inside
            if ((bool)e.NewState.DynamicValue.Etat && (bool)this.AnimalPresence.DynamicValue.Etat)
            {
                if ((bool)this.LockFlap.DynamicValue.Etat)
                {
                        //Do not open the flap
                }
                else
                {
                    if (!(bool)this.Flap.DynamicValue.Etat)
                    {
                        PackageHost.CreateMessageProxy("Chatiere").OpenFlap();
                        if (DayPeriod())
                        {
                            PackageHost.CreateMessageProxy("Chatiere").SwitchOnLed();
                        }
                    }
                    else
                    {
                        //Nothing to do
                    }
                }

            }
            //if the indoor sensor detect the collar and the pet was outside
            else if ((bool)e.NewState.DynamicValue.Etat && !(bool)this.AnimalPresence.DynamicValue.Etat)
            {
                //If the flap is closed
                if (!(bool)this.Flap.DynamicValue.Etat)
                {
                    //Nothing to do
                }
                else
                {
                    PackageHost.CreateMessageProxy("Chatiere").CloseFlap();
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
            //If the outdoor sensor detect the necklace and the pet was inside
            if ((bool)e.NewState.DynamicValue.Etat && (bool)this.AnimalPresence.DynamicValue.Etat)
            {
                //If the flap is closed
                if (!(bool)this.Flap.DynamicValue.Etat)
                {
                    //Nothing to do
                }
                else
                {
                    //Close the flap
                    PackageHost.CreateMessageProxy("Chatiere").CloseFlap();
                    PackageHost.CreateMessageProxy("Chatiere").StateObjectAnimalPresence_ChangeValue(this.AnimalPresence.Value.Name, false);
                }
            }
            //If the outdoor sensor detect the necklace and the pet was outside
            else if ((bool)e.NewState.DynamicValue.Etat && !(bool)this.AnimalPresence.DynamicValue.Etat)
            {
                if (!(bool)this.Flap.DynamicValue.Etat)
                {
                    if ((bool)this.LockFlap.DynamicValue.Etat)
                    {
                        //Do not open the flap
                    }
                    else
                    {
                        if (!(bool)this.Flap.DynamicValue.Etat)
                        {
                            PackageHost.CreateMessageProxy("Chatiere").OpenFlap();
                            if (DayPeriod())
                            {
                                PackageHost.CreateMessageProxy("Chatiere").SwitchOnLed();
                            }
                            else
                            {

                                //Nothing to do
                            }
                        }
                        else
                        {
                            //Nothing to do
                        }
                    }
                }
                else
                {
                    //Nothing to do
                }
            }
        }

        private bool DayPeriod()
        {
            PackageHost.WriteInfo("{0}", this.SunInfo.DynamicValue.Sunset);
            String[] ArraySunrise = ((string)this.SunInfo.DynamicValue.Sunrise).Split(':');
            String[] ArraySunset = ((string)this.SunInfo.DynamicValue.Sunset).Split(':');
            if (DateTime.Now.Hour >= Int32.Parse(ArraySunset[0])
                && DateTime.Now.Hour <= Int32.Parse(ArraySunrise[0])
                && DateTime.Now.Minute >= Int32.Parse(ArraySunset[1])
                && DateTime.Now.Minute <= Int32.Parse(ArraySunrise[1]))
            {
                return true;
            }
            return false;
        }
    }
}
