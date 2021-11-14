using System.Windows.Input;
using WpfTools;

namespace PhotoMaster.Models
{
    internal class MainViewModel : WpfTools.PropertyNotifier
    {
        private DelegateCommand testAuthCommand;
        public ICommand TestAuthCommand => this.testAuthCommand ??= new DelegateCommand(this.TestAuth);

        private void TestAuth(object commandParameter)
        {
            // Authorize with Google Photos
        }
    }
}
