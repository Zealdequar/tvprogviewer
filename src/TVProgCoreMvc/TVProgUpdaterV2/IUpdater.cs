using System.Threading.Tasks;

namespace TVProgViewer.TVProgUpdaterV2
{
    public interface IUpdater
    {
        public Task UpdateTvProgrammes();
    }
}