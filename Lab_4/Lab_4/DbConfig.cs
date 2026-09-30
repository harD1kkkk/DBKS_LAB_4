using System;
using System.Collections.Generic;
using System.Text;

namespace Lab_4
{
    internal static class DbConfig
    {
        public const string Server = @"(localdb)\ProjectModels";
        public const string Database = "BooksDB";

        public static string ConnectionString =>
            $"Server={Server};Database={Database};Trusted_Connection=True;TrustServerCertificate=True;";
    }
}
