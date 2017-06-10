﻿// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Util
{
    using System;
    using System.Diagnostics;
    using System.Reflection;


    [Serializable]
    public class BusHostInfo :
        HostInfo
    {
        public BusHostInfo()
        {
        }

        public BusHostInfo(bool initialize)
        {
            MachineName = Environment.MachineName;

            MassTransitVersion = GetAssemblyInformationalVersion(typeof(IBus).GetTypeInfo().Assembly);
            FrameworkVersion = Environment.Version.ToString();
            OperatingSystemVersion = Environment.OSVersion.ToString();
            var currentProcess = Process.GetCurrentProcess();
            ProcessId = currentProcess.Id;
            ProcessName = currentProcess.ProcessName;

            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();
            var assemblyName = entryAssembly.GetName();
            Assembly = assemblyName.Name;
            AssemblyVersion = GetAssemblyFileVersion(entryAssembly);
        }

        public string MachineName { get; private set; }
        public string ProcessName { get; private set; }
        public int ProcessId { get; private set; }
        public string Assembly { get; private set; }
        public string AssemblyVersion { get; private set; }
        public string FrameworkVersion { get; private set; }
        public string MassTransitVersion { get; private set; }
        public string OperatingSystemVersion { get; private set; }

        static string GetAssemblyFileVersion(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (attribute != null)
            {
                return attribute.Version;
            }

            var assemblyLocation = assembly.Location;
            if (assemblyLocation != null)
                return FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;

            return "Unknown";
        }

        static string GetAssemblyInformationalVersion(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute != null)
            {
                return attribute.InformationalVersion;
            }

            return GetAssemblyFileVersion(assembly);
        }
    }
}