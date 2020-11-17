namespace BatteryNotificator
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    // Program class with main entry point to display an example.
    public class Program
    {
        public static void Main()
        {
            PowerState state;

            while (true)
            {
                state = PowerState.GetPowerState();

                if (state.BatteryLifePercent <= 35 && state.ACLineStatus.Equals(ACLineStatus.Offline))
                    for (int i = 0; i < 5; i++)
                    {
                        state = PowerState.GetPowerState();

                        if (state.ACLineStatus.Equals(ACLineStatus.Online))
                            break;

                        PlaySoundAndSleep();
                    }
                else if (state.BatteryLifePercent >= 95 && state.ACLineStatus.Equals(ACLineStatus.Online))
                    for (int i = 0; i < 5; i++)
                    {
                        state = PowerState.GetPowerState();

                        if (state.ACLineStatus.Equals(ACLineStatus.Offline))
                            break;

                        PlaySoundAndSleep();
                    }

                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        private static void PlaySoundAndSleep()
        {
            System.Media.SystemSounds.Beep.Play();
            Thread.Sleep(TimeSpan.FromSeconds(3.5));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class PowerState
    {
        public ACLineStatus ACLineStatus;
        public BatteryFlag BatteryFlag;
        public Byte BatteryLifePercent;
        public Byte Reserved1;
        public Int32 BatteryLifeTime;
        public Int32 BatteryFullLifeTime;

        // direct instantation not intended, use GetPowerState.
        private PowerState() { }

        public static PowerState GetPowerState()
        {
            PowerState state = new PowerState();
            if (GetSystemPowerStatusRef(state))
                return state;

            throw new ApplicationException("Unable to get power state");
        }

        [DllImport("Kernel32", EntryPoint = "GetSystemPowerStatus")]
        private static extern bool GetSystemPowerStatusRef(PowerState sps);
    }

    // Note: Underlying type of byte to match Win32 header
    public enum ACLineStatus : byte
    {
        Offline = 0, Online = 1, Unknown = 255
    }

    public enum BatteryFlag : byte
    {
        High = 1, Low = 2, Critical = 4, Charging = 9,
        NoSystemBattery = 128, Unknown = 255
    }

}