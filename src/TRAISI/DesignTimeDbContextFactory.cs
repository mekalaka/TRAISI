// ====================================================
// More Templates: https://www.ebenmonney.com/templates
// Email: support@ebenmonney.com
// ====================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using DAL;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AutoMapper;


namespace TRAISI
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			Mapper.Reset();

			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddJsonFile("appsettings.Development.json", optional: true)
				.Build();

			var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

			bool development = false;
			Boolean.TryParse(configuration.GetSection("DevelopmentSettings").
			GetSection("UseSqliteDatabaseProvider").Value, out development);

			if (development)
			{
				builder.UseSqlite("Data Source=dev.db;");
			}
			else
			{
				builder.UseNpgsql(configuration["ConnectionStrings:DefaultConnection"], b => b.MigrationsAssembly("TRAISI"));
			}
			builder.UseOpenIddict();

			return new ApplicationDbContext(builder.Options);
		}
	}
}
