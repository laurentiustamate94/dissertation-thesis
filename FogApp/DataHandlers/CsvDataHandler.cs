﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Communication.Common;
using Communication.Common.Models;
using FogApp.Interfaces;

namespace FogApp.DataHandlers
{
    public class CsvDataHandler : IDataHandler
    {
        public Task Execute(DecryptedData data)
        {
            if (data.DataSource != DataSourceType.Battery)
            {
                return Task.CompletedTask;
            }

            if (data.DataType != DataType.Number)
            {
                return Task.CompletedTask;
            }

            var actualData = BitConverter.ToDouble(Convert.FromBase64String(data.Base64Data));
            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var inputFilePath = Path.Combine(directory, "battery_data.csv");
            var delimitator = File.Exists(inputFilePath) ? ";" : "";

            using (var stream = File.AppendText(inputFilePath))
            {
                stream.Write($"{delimitator}{(int)(actualData * 100)}");
            }

            return Task.CompletedTask;
        }
    }
}
