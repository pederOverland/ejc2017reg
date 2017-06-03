using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ecreg.Data;

namespace ecreg.Migrations
{
    [DbContext(typeof(EcRegDb))]
    [Migration("20170528211607_nationColumn")]
    partial class nationColumn
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("ecreg.Models.Contestant", b =>
                {
                    b.Property<int>("ContestantId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("Name");

                    b.Property<string>("Nation");

                    b.Property<string>("PassportNumber");

                    b.Property<string>("Role");

                    b.HasKey("ContestantId");

                    b.ToTable("Contestants");
                });
        }
    }
}
