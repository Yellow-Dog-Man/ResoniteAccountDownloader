using SkyFrost.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using AccountOperationUtilities.Interfaces;
using ResoniteAccountDownloader.Models.Adapters;

namespace ResoniteAccountDownloader.Services
{
    public class ReactiveStorageRecord : ReactiveObject, IStorageRecord
    {
        [Reactive]
        public long UsedBytes { get; set; }

        [Reactive]
        public long TotalBytes { get; set; }

        public OwnerType OwnerType { get; set; }
        public string Id { get; set; }

        public void Update(long used, long total)
        {
            UsedBytes = used;
            TotalBytes = total;
        }

        public void Update(IUser? u)
        {
            if (OwnerType != OwnerType.User)
                return;
            if (u == null)
                return;
            if (u.Id != Id)
                return;
            Update(u.UsedBytes, u.QuotaBytes);
        }

        public void Update(IGroup g)
        {
            if (OwnerType != OwnerType.Group)
                return;
            if (g.Id != Id)
                return;
            Update(g.UsedBytes, g.QuotaBytes);
        }

        public ReactiveStorageRecord(string id, OwnerType type)
        {
            Id = id;
            OwnerType = type;
        }
    }
    internal class CloudStorageService : IStorageService
    {
        private SkyFrostInterface Interface { get; }
        private ILogger Logger { get; }
        public CloudStorageService(SkyFrostInterface? _interface, ILogger? logger)
        {
            if (_interface == null)
                throw new ArgumentNullException(nameof(_interface));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            Logger = logger;
            Interface = _interface;
        }

        public IStorageRecord GetUserStorage()
        {
            var storage = new ReactiveStorageRecord(Interface.CurrentUser.Id, OwnerType.User);
            IUser user = ResoniteUserAdapter.FromResoniteUser(Interface.CurrentUser);
            storage.Update(user.UsedBytes, user.QuotaBytes);
            Interface.Session.UserUpdated += (User user) =>
            {
                if (user == null)
                    return;
                storage.Update(ResoniteUserAdapter.FromResoniteUser(user));
            };

            return storage;
        }

        private async void UpdateStorage(ReactiveStorageRecord storage)
        {
            var storageResult = await Interface.Storage.GetStorage(storage.Id);
            if (storageResult.IsOK)
            {
                var storageInfo = storageResult.Entity;
                storage.Update(storageInfo?.UsedBytes ?? 0, storageInfo?.QuotaBytes ?? 0);
            }
        }

        public IStorageRecord GetGroupStorage(string groupId)
        {
            ReactiveStorageRecord storage;
            storage = new ReactiveStorageRecord(groupId, OwnerType.Group);
            UpdateStorage(storage);

            Interface.Groups.GroupUpdated += (group) =>
            {
                if (group.GroupId != storage.Id) return;
                UpdateStorage(storage);
            };

            return storage;
        }
    }
}
