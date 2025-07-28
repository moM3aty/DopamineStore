using System.Threading.Tasks;

namespace DopamineStore.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
