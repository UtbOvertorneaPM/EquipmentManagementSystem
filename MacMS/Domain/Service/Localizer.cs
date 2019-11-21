using Microsoft.Extensions.Localization;
using System.Reflection;


namespace EquipmentManagementSystem.Domain.Service {

    public class Localizer {

        private readonly IStringLocalizer _Localizer;

        public Localizer(IStringLocalizerFactory factory) { 

            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _Localizer = factory.Create("SharedResource", assemblyName.Name);
        }

        public LocalizedString this[string key] {
            get => _Localizer[key];
        }
    }


}

namespace EquipmentManagementSystem {

    /// <summary>
    /// Dummy class to group shared resources
    /// </summary>
    public class SharedResource {
    }
}