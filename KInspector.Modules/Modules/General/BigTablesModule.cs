﻿using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.General
{
    public class BigTablesModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Size of the tables",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2") 
                },
                Comment = @"Selects top 25 biggest tables from the database",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var results = dbService.ExecuteAndGetTableFromFile("BigTablesModule.sql");

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
