using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ResoniteAccountDownloader.Models;
using ResoniteAccountDownloader.Services;
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

        public AboutWindowViewModel(IAssemblyInfoService assemblyInfoService, ContributionsService contributionsService)
        {
            _assemblyInfoService = assemblyInfoService;
            ContributionsService = contributionsService;
            AppVersion = _assemblyInfoService.Version;
            AppCompany = _assemblyInfoService.CompanyName;
            DotNetVersion = _assemblyInfoService.DotNetVersion;
        }
    }
}
