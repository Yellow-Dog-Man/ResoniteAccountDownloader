using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ResoniteAccountDownloader.Models;
using ResoniteAccountDownloader.Services;
using Splat;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ResoniteAccountDownloader.ViewModels
{
    public class AboutWindowViewModel: ReactiveObject
    {
        [Reactive]
        public string AppVersion { get; set; }

        [Reactive]
        public string AppCompany { get; set; }

        [Reactive]
        public string DotNetVersion { get; set; }

        private ContributionsService ContributionsService { get; set; }

        public List<Contributor>? Contributors => ContributionsService.Contributors;

        private readonly IAssemblyInfoService _assemblyInfoService;

        public AboutWindowViewModel()
        {
            _assemblyInfoService = Locator.Current.GetService<IAssemblyInfoService>() ?? throw new NullReferenceException("No version info");
            ContributionsService =  Locator.Current.GetService<ContributionsService>() ?? throw new NullReferenceException("No contributor information available");
            AppVersion = _assemblyInfoService.Version;
            AppCompany = _assemblyInfoService.CompanyName;
            DotNetVersion = _assemblyInfoService.DotNetVersion;
        }
    }
}
