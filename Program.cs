namespace BatteryNotificator
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    // Program class with main entry point to display an example.
    public class Program
    {
        public static void Main()
        {
            PowerState state;

            while (true)
            {
                state = PowerState.GetPowerState();

                if (state.BatteryLifePercent <= 30 && state.ACLineStatus.Equals(ACLineStatus.Offline))
                {
                    MessageBox.Show("Battery is low!",
                                    "Battery Power State",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button1,
                                    MessageBoxOptions.DefaultDesktopOnly);
                }
                else if (state.BatteryLifePercent >= 100 && state.ACLineStatus.Equals(ACLineStatus.Online))
                {
                    MessageBox.Show("Battery is fully charged!", 
                                    "Battery Power State", 
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Warning, 
                                    MessageBoxDefaultButton.Button1, 
                                    MessageBoxOptions.DefaultDesktopOnly);
                }

                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
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