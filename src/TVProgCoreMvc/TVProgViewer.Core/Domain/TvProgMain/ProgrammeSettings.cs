using TVProgViewer.Core.Configuration;

namespace TVProgViewer.Core.Domain.TvProgMain
{
    public class ProgrammeSettings: ISettings
    {
        public int PrimarySystemProviderId { get; set; }

        public int PrimarySystemTypeProgId { get; set; }

        public string PrimarySystemCategory { get; set; }
    }
}
