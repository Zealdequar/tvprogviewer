using System.Threading.Tasks;

namespace TvProgViewer.TvProgUpdaterV3
{
    public interface IUpdater
    {
        public Task UpdateTvProgrammes();
    }
}