using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Core.Domain.TvProgMain
{
    public class ProgrammeSettings: BaseEntity, ISettings 
    {
        public int PrimarySystemProviderId { get; set; }

        public int PrimarySystemTypeProgId { get; set; }

        public string PrimarySystemCategory { get; set; }
    }
}
