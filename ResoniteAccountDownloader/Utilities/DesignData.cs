using AccountOperationUtilities.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ResoniteAccountDownloader.Services;
using ResoniteAccountDownloader.Utilities;
using ResoniteAccountDownloader.ViewModels;
using System;
using System.Collections.Generic;
using SkyFrost.Base;
using IRecord = AccountOperationUtilities.Interfaces.IRecord;

//TODO: strip this file from the final build somehow
namespace ResoniteAccountDownloader
{
    // Design based view models that are intended just for the view designer to work, I'm not sure how to handle these so I'll leave them here and deal with them later.
    public class DesignUserProfile : ReactiveObject, IUserProfile
    {
        [Reactive]
        public string UserName { get; set; }

        [Reactive]
        public Uri? PictureURI { get; set; }

        public void UpdateUser(IUser obj)
        {
            // No-op
        }

        public DesignUserProfile()
        {
            UserName = "User";
            PictureURI = AssetHelper.GetUri("AnonymousHeadset.png");
        }
    }
    public class DesignStorageRecord : IStorageRecord
    {
        public DesignStorageRecord()
        {
            Random rand = new Random();
            UsedBytes = rand.Next(1000, 10000);
        }
        public long UsedBytes { get; set; }
        public long TotalBytes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OwnerType OwnerType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => throw new NotImplementedException();

        public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => throw new NotImplementedException();

        public IDisposable SuppressChangeNotifications()
        {
            throw new NotImplementedException();
        }

        public void Update(long used, long total)
        {
            throw new NotImplementedException();
        }
    }
    public class DesignData
    {
        public static List<IRecordDownloadFailure> GenerateRecordFailList()
        {
            var list = new List<IRecordDownloadFailure>();
            for (var i = 0; i < 10; i++)
            {
                // TODO
                list.Add(new DesignRecordDownloadFailure
                {
                    FailureReason = "Download Failed osdijfoisjdofjsdoifjosdifj",
                    OwnerId = "U-User sdfsdfsdfsdf",
                    RecordName = "HomeWorldsdfsfffffffffffffffffff",
                    RecordPath = "U-User/Worlds/HomeWorld/sdf/sd/f/sdf/sdf///ffff/ffffffffff",
                    RecordId = "R-12342342342344532523523562361166116161616"
                });
            }
            return list;
        }
        public static readonly FailedRecordsViewModel DesignFailedRecordsViewModel = new(GenerateRecordFailList(), GenerateAssetFailList());

        private static List<IAssetFailure> GenerateAssetFailList()
        {
            var list = new List<IAssetFailure>();
            for (var i = 0; i < 10; i++)
            {
                //TODO
                list.Add(new DesignAssetFailure("1234566sodijosdijfoisdjofijsdf9ij", "Asset failed to download23523523532523523523523525", null));
            }
            return list;
        }

        public static readonly UserProfileViewModel DesignProfileViewModel = new(new DesignUserProfile());
        private static readonly List<GroupRecord> _groups = new() {
            new GroupRecord("G-Group0", "Group 1 with Long Name", false, new DesignStorageRecord()),
            new GroupRecord("G-Group1", "Group 2", false, new DesignStorageRecord()),
            new GroupRecord("G-Group2", "Group 3", true, new DesignStorageRecord()),
            new GroupRecord("G-Group3", "Group 4", false, new DesignStorageRecord()),
            new GroupRecord("G-Group4", "Group 5", false, new DesignStorageRecord()),
            new GroupRecord("G-Group5", "Group 6", false, new DesignStorageRecord()),
            new GroupRecord("G-Group6", "Group 7", true, new DesignStorageRecord()),
            new GroupRecord("G-Group7", "Group 8", false, new DesignStorageRecord()),
            new GroupRecord("G-Group8", "Group 9", false, new DesignStorageRecord()),
            new GroupRecord("G-Group9", "Group 10", false, new DesignStorageRecord()),
            new GroupRecord("G-Group10", "Group 11", false, new DesignStorageRecord()),
            new GroupRecord("G-Group11", "Group 12", true, new DesignStorageRecord()),
            new GroupRecord("G-Group12", "Group 13", false, new DesignStorageRecord()),
            new GroupRecord("G-Group13", "Group 14", false, new DesignStorageRecord()),
            new GroupRecord("G-Group14", "Group 15", false, new DesignStorageRecord()),
            new GroupRecord("G-Group15", "Group 16", false, new DesignStorageRecord()),
            new GroupRecord("G-Group16", "Group 17", false, new DesignStorageRecord()),
            new GroupRecord("G-Group17", "Group 18 with longer name that's just silly.", false, new DesignStorageRecord()),
            new GroupRecord("G-Group18", "Group 19", false, new DesignStorageRecord()),
            new GroupRecord("G-Group19", "Group 20", true, new DesignStorageRecord()),
            new GroupRecord("G-Group20", "Group 21", false, new DesignStorageRecord())
        };
        public static readonly GroupsListViewModel DesignGroupsListViewModel = new(_groups);

        
    }
    public class DesignAssetFailure : IAssetFailure
    {
        public AccountOperationUtilities.Interfaces.IRecord? ForRecord { get; set; } = null;

        public string Hash { get; set; } = "";

        public string OwnerId { get; set; } = "";

        public string Reason { get; set; } = "";

        public string RecordName { get; set; } = "";

        public string RecordPath { get; set; } = "";

        public DesignAssetFailure(string hash, string reason, IRecord? forRecord)
        {
            if (forRecord != null)
            {
                RecordName = forRecord.Name;
                RecordPath = forRecord.Path;
            }
            Hash = hash;
            Reason = reason;
        }
    }

    public class DesignRecordDownloadFailure : IRecordDownloadFailure
    {
        public string FailureReason { get; set; } = "";
        public string OwnerId { get; set; } = "";
        public string RecordName { get; set; } = "";
        public string RecordPath { get; set; } = "";
        public string RecordId { get; set; } = "";
    }
}
