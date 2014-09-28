﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;

namespace SSRSMigrate.Exporter
{
    public interface IItemExporter<T> where T : ReportServerItem
    {
        ExportStatus SaveItem(T item, string fileName, bool overwrite = true);
    }
}
