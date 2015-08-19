using System;
using Devq.Bids.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Devq.Bids {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("BidPartRecord", table => table
                .ContentPartRecord()
                .Column<decimal>("BidPrice")
                .Column<int>("BidedOn")
                .Column<int>("BidsPartRecord_id"));

            SchemaBuilder.CreateTable("BidsPartRecord", table => table
                .ContentPartRecord()
                .Column<bool>("BidsActive")
                .Column<decimal>("MinimumBidPrice")
                .Column<string>("BidType"));

            ContentDefinitionManager.AlterPartDefinition("BidPart", part => part
                .WithDescription("Used by the Bid content type."));

            ContentDefinitionManager.AlterTypeDefinition("Bid",
               cfg => cfg
                   .WithPart("BidPart")
                   .WithPart("CommonPart", 
                        p => p
                            .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false")
                            .WithSetting("DateEditorSettings.ShowDateEditor", "false")));

            ContentDefinitionManager.AlterPartDefinition("BidsPart", builder => builder
                .Attachable()
                .WithDescription("Allows content items to be Bided on."));

            SchemaBuilder.AlterTable("BidPartRecord",
               table => table
                   .CreateIndex("IDX_BidedOn", "BidedOn"));

            return 1;
        }

        public int UpdateFrom1() {

            SchemaBuilder.AlterTable(typeof (BidPartRecord).Name,
                table => table
                    .AddColumn<string>("Bider"));

            return 2;
        }

        public int UpdateFrom2() {

            SchemaBuilder.AlterTable(typeof (BidPartRecord).Name,
                table => table
                    .AddColumn<DateTime>("BidDateUtc"));

            return 3;
        }

        public int UpdateFrom3() {
            
            // Email notification
            SchemaBuilder.AlterTable(typeof (BidsPartRecord).Name,
                table => table
                    .AddColumn<bool>("NotificationEmail"));

            return 4;
        }
    }
}