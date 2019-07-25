using Microsoft.EntityFrameworkCore;
using offset_my_carbon_dal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace offset_my_carbon_dal.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Donation> Donations { get; set; }
    }
}
