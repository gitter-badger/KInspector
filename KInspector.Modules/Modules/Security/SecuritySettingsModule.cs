﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using KInspector.Core;

namespace KInspector.Modules.Modules.Security
{
    public class SecuritySettingsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Security settings",
                Comment = "Some security settings need to be verified. If REST is enabled, verify all REST settings as there are known security flaws.",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2") 
                },
                Category = "Security",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var results = dbService.ExecuteAndGetTableFromFile("SecuritySettingsModule.sql");
            List<DataRow> rowsToDelete = new List<DataRow>();

            // Iterate through and check int/double settings 
            foreach (DataRow row in results.Rows)
            {
                if (!String.IsNullOrEmpty(row["Key value"].ToString()))
                {
                    switch (row["Key name"].ToString()) 
                    {
                        case "CMSResetPasswordInterval":
                            if (GetValueAndCheckInterval(row, 1, 12))
                            {
                                rowsToDelete.Add(row);
                            }
                            break;
                        case "CMSPolicyMinimalLength":
                            if (GetValueAndCheckInterval(row, 8))
                            {
                                rowsToDelete.Add(row);
                            }
                            break;
                        case "CMSPolicyNumberOfNonAlphaNumChars":
                            if (GetValueAndCheckInterval(row, 2))
                            {
                                rowsToDelete.Add(row);
                            }
                            break;
                        case "CMSMaximumInvalidLogonAttempts":
                            if (GetValueAndCheckInterval(row, 0, 5))
                            {
                                rowsToDelete.Add(row); 
                            }
                            break;
                        default: break;
                    }
                }
            }

            foreach(DataRow row in rowsToDelete)
            {
                results.Rows.Remove(row);
            }

            return new ModuleResults
            {
                Result = results,
                ResultComment = "",
                Status = Status.Warning,
            };
        }


        /// <summary>
        /// Checks if given value falls in given interval.
        /// </summary>
        /// <param name="row">Row witch contains value to test (Key value).</param>
        /// <param name="lower">Minimum value.</param>
        /// <param name="upper">Optional maximum value.</param>
        /// <returns>True if value falls into interval, false otherwise.</returns>
        private bool GetValueAndCheckInterval(DataRow row, int lower, int upper = Int32.MaxValue)
        {
            double value = 0;
            if (Double.TryParse(row["Key value"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return (value >= lower) && (value <= upper);
            }
  
            return false;
        }
    }
}
