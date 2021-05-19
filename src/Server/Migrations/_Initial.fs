﻿// <auto-generated />
namespace Server.Migrations

open System
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Infrastructure
open Microsoft.EntityFrameworkCore.Metadata
open Microsoft.EntityFrameworkCore.Migrations
open Microsoft.EntityFrameworkCore.Storage.ValueConversion
open Server

[<DbContext(typeof<DataModels.SmthContext>)>]
[<Migration("_Initial")>]
type Initial() =
    inherit Migration()

    override this.Up(migrationBuilder:MigrationBuilder) =
        migrationBuilder.CreateTable(
            name = "Users"
            ,columns = (fun table -> 
            {|
                Id =
                    table.Column<int>(
                        nullable = false
                        ,``type`` = "INTEGER"
                    ).Annotation("Sqlite:Autoincrement", true)
                FirstName =
                    table.Column<string>(
                        nullable = true
                        ,``type`` = "TEXT"
                    )
                LastName =
                    table.Column<string>(
                        nullable = true
                        ,``type`` = "TEXT"
                    )
            |})
            ,constraints =
                (fun table -> 
                    table.PrimaryKey("PK_Users", (fun x -> (x.Id) :> obj)) |> ignore
                ) 
        ) |> ignore


    override this.Down(migrationBuilder:MigrationBuilder) =
        migrationBuilder.DropTable(
            name = "Users"
            ) |> ignore


    override this.BuildTargetModel(modelBuilder: ModelBuilder) =
        modelBuilder
            .HasAnnotation("ProductVersion", "5.0.5")
            |> ignore

        modelBuilder.Entity("Server.DataModels+DbUser", (fun b ->

            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER") |> ignore
            b.Property<string>("FirstName")
                .HasColumnType("TEXT") |> ignore
            b.Property<string>("LastName")
                .HasColumnType("TEXT") |> ignore

            b.HasKey("Id") |> ignore

            b.ToTable("Users") |> ignore

        )) |> ignore

