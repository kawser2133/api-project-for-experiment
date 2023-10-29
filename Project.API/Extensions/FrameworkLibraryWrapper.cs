using B_SDK;

namespace Project.API.Extensions
{
    public class FrameworkLibraryWrapper
    {
        private BService frameworkLibrary;

        public FrameworkLibraryWrapper()
        {
            frameworkLibrary = new BService();
        }

        public void CallFrameworkLibraryMethod()
        {
            frameworkLibrary.IsDeviceConnected();
        }
    }
}
