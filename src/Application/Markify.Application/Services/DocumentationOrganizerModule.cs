using Markify.Domain.Document;
using Markify.Services.Document;
using Ninject.Modules;

namespace Markify.Application.Services
{ 
    public class DocumentationOrganizerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDocumentOrganizer>().To<SimpleDocumentOrganizer>();
        }
    }
}