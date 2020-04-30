﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using rkbc.core.repository;

namespace rkbc.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200423142702_editPost")]
    partial class editPost
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("rkbc.core.models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationRoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("address1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("countryCode")
                        .HasColumnType("int");

                    b.Property<DateTime?>("createdDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("department")
                        .HasColumnType("int");

                    b.Property<string>("firstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("gender")
                        .HasColumnType("int");

                    b.Property<string>("lastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("officeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("updatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationUserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationUserLogin", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationUserRole", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationUserToken", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("rkbc.core.models.Attachment", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("attachmentSectionEnum")
                        .HasColumnType("int");

                    b.Property<string>("caption")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("createDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("createUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isOn")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("lastUpdDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("lastUpdUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("originalFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("pageEnum")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("rkbc.core.models.Blog", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("authorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("createDt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("lastUpdDt")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.HasIndex("authorId");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("rkbc.core.models.Comment", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("authorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isUser")
                        .HasColumnType("bit");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("postId")
                        .HasColumnType("int");

                    b.Property<DateTime>("pubDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.HasIndex("authorId");

                    b.HasIndex("postId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("rkbc.core.models.Contact", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("createDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("createUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("subject")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("rkbc.core.models.HomeContentItem", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("homePageId")
                        .HasColumnType("int");

                    b.Property<bool>("isOn")
                        .HasColumnType("bit");

                    b.Property<int>("sectionId")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("homePageId");

                    b.ToTable("HomeContentItems");
                });

            modelBuilder.Entity("rkbc.core.models.HomePage", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("bannerFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("bannerUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("churchAnnounceTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("createDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("createUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("lastUpdDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("lastUpdUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("memberAnnounceTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("originalFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("schoolAnnounceTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("sundayServiceVideoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("titleContent")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("HomePages");
                });

            modelBuilder.Entity("rkbc.core.models.PastorPage", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("author")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("column")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("columnTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("createDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("createUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("lastUpdDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("lastUpdUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pageTitle")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("PastorPages");
                });

            modelBuilder.Entity("rkbc.core.models.Post", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("blogId")
                        .HasColumnType("int");

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("excerpt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("imageFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isPublished")
                        .HasColumnType("bit");

                    b.Property<DateTime>("lastModified")
                        .HasColumnType("datetime2");

                    b.Property<int>("postType")
                        .HasColumnType("int");

                    b.Property<DateTime>("pubDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("slug")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("videoURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("views")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("blogId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationRoleClaim", b =>
                {
                    b.HasOne("rkbc.core.models.ApplicationRole", "Role")
                        .WithMany("RoleClaims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationUserClaim", b =>
                {
                    b.HasOne("rkbc.core.models.ApplicationUser", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationUserLogin", b =>
                {
                    b.HasOne("rkbc.core.models.ApplicationUser", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationUserRole", b =>
                {
                    b.HasOne("rkbc.core.models.ApplicationRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("rkbc.core.models.ApplicationUser", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("rkbc.core.models.ApplicationUserToken", b =>
                {
                    b.HasOne("rkbc.core.models.ApplicationUser", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("rkbc.core.models.Blog", b =>
                {
                    b.HasOne("rkbc.core.models.ApplicationUser", "author")
                        .WithMany()
                        .HasForeignKey("authorId");
                });

            modelBuilder.Entity("rkbc.core.models.Comment", b =>
                {
                    b.HasOne("rkbc.core.models.ApplicationUser", "author")
                        .WithMany()
                        .HasForeignKey("authorId");

                    b.HasOne("rkbc.core.models.Post", "post")
                        .WithMany("comments")
                        .HasForeignKey("postId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("rkbc.core.models.HomeContentItem", b =>
                {
                    b.HasOne("rkbc.core.models.HomePage", "homePage")
                        .WithMany("announcements")
                        .HasForeignKey("homePageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("rkbc.core.models.Post", b =>
                {
                    b.HasOne("rkbc.core.models.Blog", "blog")
                        .WithMany("posts")
                        .HasForeignKey("blogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
