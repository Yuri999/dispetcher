﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dispetcher.Common.IoC;

namespace Dispetcher.Common.Database
{
    public class StructureCreator
    {
        public static void Create()
        {
            var manager = Locator.Resolve<IDbManager>();

            var tables = manager.ExecQuery<MasterTableItem>(@"SELECT type,name,sql,tbl_name FROM sqlite_master").ToList();

            if (!tables.Any(t => t.tbl_name == "Journal" && t.type == "table"))
            {
                var transaction = manager.BeginTransaction();
                try
                {
                    manager.ExecNonQuery(String.Format(@"CREATE TABLE `Journal` (
	`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`Date`	TEXT NOT NULL,
	`SideNumberPlan`	TEXT,
    `SideNumberFact`	TEXT,
	`Schedule`	TEXT,
	`Route`	TEXT,
	`VehicleType`	INTEGER,
    `Protected` INTEGER,
    `ModifyDate`	TEXT NOT NULL
);"));
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // TODO log
                    transaction.Rollback();
                    throw;
                }
            }

            if (!tables.Any(t => t.tbl_name == "UserLog" && t.type == "table"))
            {
                var transaction = manager.BeginTransaction();
                try
                {
                    manager.ExecNonQuery(String.Format(@"CREATE TABLE `UserLog` (
	`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`Date`	TEXT NOT NULL,
	`ActionType`	INTEGER NOT NULL,
    `RecId` INTEGER,
	`OldValue`	TEXT,
	`NewValue`	TEXT
);"));
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // TODO log
                    transaction.Rollback();
                    throw;
                }
            }
        }

        class MasterTableItem
        {
            public string type { get; set; }
            public string name { get; set; }
            public string sql { get; set; }
            public string tbl_name { get; set; }
        }
    }
}

